using Microsoft.Z3;

public class Call:Head {

  // TODO: Construct this with the actual function to call?

  public readonly string name;
  public readonly IList<Actual> actuals;

  internal Match? match = null;
  internal Function function => (Function)match!.node;

  public Call(Place place, string name, List<Actual> actuals) : base(place) {
    this.name = name;
    this.actuals = set(actuals);
  }

  protected override Type resolve(Verifier v) {
    // TODO: Support function pointers
    // If name is a local variable, and it's a function pointer,
    // just call that immediately (its type will have the formals)
    var actuality = new Actuality(this, name, actuals);
    var matcher = new Matcher(actuality, true);
    var match = matcher.run(v);
    if (match == null) return Fail.FAIL;
    if (!(match.node is Function)) return Fail.FAIL;
    this.match = match;
    var function = (Function)match.node;
    function.callers.Add(this);
    return function.returnType ?? Type.VOID;
  }

  internal override ZZZ mk(Solva solva) {
    // TODO, make sure function is verified first...
    // ...but not if it's a recursive terminus
    // TODO, if this is a recursive terminus just return a ZZZ.var
    var f = this.function;
    var ledger = solva.ledger;
    var varMap = new Dictionary<Symbol.Part,Symbol.Part>();
    var actuals = match!.actuals;
    var formals = match!.formals;
    for (var i = 0; i < actuals.Count(); i++) {
      var formal = formals[i];
      var actual = actuals[i];
      var fpart = new Symbol.Part(formal.name, 0);
      if (actual.type.aggregate) { // TODO embed
        var av = (zzz.Var)actual.zzz(solva);
        varMap[fpart] = av.symbol.firstPart;
      } else {
        var n = ledger.newVar;
        var s = ZZZ.var(actual.type, n);
        solva.assert(new zzz.Eq(s, actual.zzz(solva)));
        varMap[fpart] = n.firstPart;
      }
    }
    var retVar = rightSymbol ?? ledger.newVar;
    if (function.terminus) {
      // Still need to handle cataclysms here
      return ZZZ.var(f.returnType!, retVar);
    }
    var rewriter = new Rewriter(ledger, varMap, retVar);
    foreach (var x in function.facts) {
      var xlat = x.rewriteBool(rewriter);
      solva.assert(xlat);
    }
    var err = solva.solve(ZZZ.var(Type.BOOL, Symbol.THROW));
    if (err == Solved.TRUE) {
      solva.report(this, "Called function will always throw an error.");
    }
    if (f.returnType == null) {
      return new zzz.Bool(true);
    } else {
      return ZZZ.var(f.returnType, rewriter.retVar);
    }
  }

  protected override Pair toPair(LLVM llvm) {
    // TODO: Support %out for brand-new functions
    foreach (var x in match!.actuals) {
      x.emit(llvm);
    }
    var tuple = llvm.nextVar();
    var f = function;
    llvm.print($"  {tuple} = call {f.returnLLVM} @{f.fullName}(");
    var first = true;
    foreach (var x in match!.actuals) {
      if (first) {
        first = false;
      } else {
        llvm.print(", ");
      }
      llvm.print(x.pair.ToString());
    }
    llvm.println(")");
    return llvm.handleError(function, tuple);
  }

  /////

  public override void format(Formatter fmt) {
    fmt.print(name);
    fmt.print("(");
    fmt.print(actuals);
    fmt.print(")");
  }

  static string partial() => "(";
}

public partial class In {

  public Call call { get {
    var place = skip();
    var name = id();
    var actuals = this.actuals('(', ')');
    return new Call(place, name, actuals);
  }}

}

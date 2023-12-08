public class Return:Stmt {

  public readonly Expr? expr;

  public Return(Place place, Expr? expr) : base(place) {
    this.expr = setNull(expr);
  }

  public override bool terminates => true;

  public override void verify(Verifier v) {
    var f = ancestor<Function>()!;
    if (f.returnType == null && expr == null) {
      return;
    }
    if (f.returnType == null) {
      v.report(this, "Can't return a value from this function.");
      return;
    }
    if (expr == null) {
      v.report(this, "Need to return a value.");
      return;
    }
    var formal = f.returnType;
    if (formal is Fail) return;
    expr.expect(formal);
    expr.reposition(Position.RIGHT);
    expr.verify(v);
    var actual = expr.type!;
    if (actual is Fail) return;
    foreach (var x in actual.returnTo(formal)) {
      v.report(this, x);
    }
  }

  public override void solve(Solva solva) {
    if (expr == null) return;
    var ez = expr.zzz(solva);
    var r = ZZZ.var(expr.type!, Symbol.RETURN);
    var eq = new zzz.Eq(r, ez);
    solva.assert(eq);
  }

  public override void emit(LLVM llvm) {
    expr?.emit(llvm);
    if (expr == null) llvm.voidReturn();
    else llvm.returnValue(expr);
  }

  public override void format(Formatter fmt) {
    fmt.print("return");
    if (expr == null) return;
    fmt.print(" ");
    expr.format(fmt);
  }

  static string parserName() => "returnStmt";
  static string full() => "return";

}

public partial class In {

  public Return? returnStmt { get {
    var place = skip();
    if (token() != "return") return null;
    expect("return", Flavor.KEYWORD);
    var nextPlace = skip();
    if (nextPlace.line != place.line) {
      return new Return(place, null);
    }
    if (token() == "if") {
      return new Return(place, null);
    }
    if (token() == "//") {
      return new Return(place, null);
    }
    return new Return(place, expr);
  }}

}

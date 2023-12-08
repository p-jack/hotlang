public class Loop:Formality {

  public readonly Function function;

  While whileStmt => (While)function.block.stmts.Last();
  Give give => (Give)whileStmt.block.stmts[0];
  public Type type => failed ? Fail.FAIL : give.expr.type;

  public Loop(Function f) : base(f.place, f.access) {
    this.function = set(f);
    this.name = f.name;
  }

  internal override IList<string> check(Type formal, Type actual) {
    return actual.passTo(formal);
  }

  /////

  public override bool keepWithNext(Top top) => false;

  protected override void index2(Out oot) {
    index(true, name);
    // function.fullName = fullName;
  }

  protected override void setSupers2(Out oot) {
    function.setSupers(oot);
  }

  protected override void prepare2(Out oot) {
    function.prepare(oot);
    this.formals = function.formals;
  }

  protected override void analyze2(Out oot) {
    function.analyze(oot);
    var last = function.block.stmts.Last();
    if (!(last is While)) {
      oot.report(this, "Last statement in a custom loop must be a while statement.");
      return;
    }
    var w = (While)last;
    var stmts = w.block.stmts;
    if (stmts.Count() == 0) {
      oot.report(this, "The first statement in a custom loop's while block must be a ---> statement. Any subsequent statements must advance the loop (eg, i++), so the while block needs at least two statements (the ---> statement, and at least one advacement).");
      return;
    }
    if (!(stmts[0] is Give)) {
      oot.report(this, "The first statement in a custom loop's while block must be a ---> statement.");
      return;
    }
    if (stmts.Count() < 2) {
      oot.report(this, "A custom loop's while block requires at least one statement after the ---> statement. That statement or statements must advance the loop.");
    }
  }

  public Source? newSource(Place place, IList<Actual> actuals) {
    if (failed) return null;
    var renamer = new Renamer();
    foreach (var x in function.locals) {
      var rn = x.rawName;
      var nn = $"{rn}{function.nextLocal}_";
      renamer.add(rn, nn);
    }
    var setup = new List<Stmt>();
    foreach (var x in function.block.stmts) {
      if (x is While) break;
      setup.Add(x.copy(renamer.xlat));
    }
    foreach (var x in actuals) {
      var n = renamer.get(x.name);
      var input = new In(syntax, place, $"let {n} = {x.expr}");
      var stmt = input.mustStmt;
      setup.Add(stmt);
    }
    var advance = new List<Stmt>();
    var stmts = whileStmt.block.stmts;
    for (int i = 1; i < stmts.Count(); i++) {
      advance.Add(stmts[i].copy(renamer.xlat));
    }
    var give = (Give)stmts[0];
    var cond = whileStmt.condition.copy(renamer.xlat);
    var yield = give.expr.copy(renamer.xlat);
    return new Source(place, setup, cond, yield, advance);
  }

  protected override void solve2(Out oot) {
    function.solve(oot);
  }

  public override void emit(LLVM llvm) {
    // nop
  }

  /////

  public override string ToString() {
    return $"{name} loop";
  }

  protected override void format2(Formatter fmt) {
    access.format(fmt);
    fmt.print(name);
    fmt.print(" loop");
    if (function.declaredParams.Count() > 0) {
      function.declaredParams.format(fmt);
    }
    fmt.print(" ");
    function.block.format(fmt);
    fmt.print("\n");
  }

  static string full() => "loop";

}

public partial class In {

  public Loop loop { get {
    var place = skip();
    var access = this.access;
    var name = id();
    skip();
    expect("loop", Flavor.KEYWORD);
    List<Param> paramz;
    if (peek == '(') {
      paramz = this.paramz;
    } else {
      paramz = new List<Param>();
    }
    var block = this.block;
    var f = new Function(place, access, Kind.FUNCTION, name, paramz, null, block);
    return new Loop(f);
  }}

}

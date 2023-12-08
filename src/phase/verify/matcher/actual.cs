public class Actual:Expr {

  public readonly string name;
  public readonly Expr expr;

  bool preverified = false;
  public IList<string> preIssues = new List<string>();
  public Type? castTo = null;

  public Actual(Place place, string name, Expr expr) : base(place) {
    this.name = name;
    this.expr = set(expr);
  }

  public Actual withName(string name) => new Actual(place, name, expr);
  public override Actual copy() => (Actual)base.copy();

  internal override ZZZ mk(Solva solva) {
    return expr.zzz(solva);
  }

  public void preverify(Formal formal, Out oot) {
    var unit = formal.source.ancestor<Unit>()!;
    Out out2 = new Out(oot.conf);
    Verifier v = new Verifier(out2, unit.symbols);
    preIssues = reverify2(v, formal.type);
    preverified = true;
  }

  public IList<string> reverify(Verifier v, Type expected) {
    if (preverified) return preIssues;
    var unit = ancestor<Unit>()!;
    var out2 = new Out(v.oot.conf);
    var v2 = new Verifier(out2, v.symbols);
    return reverify2(v2, type);
  }

  private IList<string> reverify2(Verifier v, Type expected) {
    reset();
    expect(expected);
    verify(v);
    return v.oot.errors;
  }

  protected override Type resolve(Verifier v) {
    expr.expect(expected);
    expr.verify(v);
    return expr.type;
  }

  protected override Pair toPair(LLVM llvm) {
    expr.emit(llvm);
    if (castTo == null) return expr.pair;
    return llvm.bitcast(expr.pair, castTo.llvm);
  }

  /////

  public override void format(Formatter fmt) {
    if (name != "") {
      fmt.print($"{name}:");
    }
    expr.format(fmt);
  }

  public string debug() {
    return $"(Actual name:{name}, expr:{expr})";
  }

}

public partial class In {

  public Actual actual { get {
    var place = this.skip();
    var e = this.mustExpr;
    skip();
    if (e is Find && peek == ':') {
      expect(":", Flavor.OPERATOR);
      var f = (Find)e;
      return new Actual(place, f.name, mustExpr);
    }
    return new Actual(place, "", e);
  }}

  public List<Actual> actuals(char start, char end) {
    this.skip();
    this.expect(start, Flavor.BRACE);
    this.skip();
    var actuals = new List<Actual>();
    while (this.peek != end && !this.eof) {
      actuals.Add(this.actual);
      this.skip();
      if (this.peek == ',') {
        this.expect(",", Flavor.OPERATOR);
        this.skip();
      }
    }
    this.expect(end, Flavor.BRACE);
    return actuals;
  }

}

public static class Actuals {

  public static int nameless(this IList<Actual> actuals) {
    var start = actuals.firstNamed();
    if (start < 0) return actuals.Count();
    for (var i = start; i < actuals.Count(); i++) {
      if (actuals[i].name == "") return -1;
    }
    return start;
  }

  static int firstNamed(this IList<Actual> actuals) {
    for (var i = 0; i < actuals.Count(); i++) {
      if (actuals[i].name != "") {
        return i;
      }
    }
    return -1;
  }

  public static void format(this IList<Actual> actuals, Formatter fmt) {
    if (actuals.Count() == 0) return;
    actuals[0].format(fmt);
    for (int i = 1; i < actuals.Count(); i++) {
      fmt.print(", ");
      actuals[i].format(fmt);
    }
  }

  public static List<Actual> copy(this IList<Actual> actuals) {
    var result = new List<Actual>(actuals.Count());
    foreach (var x in actuals) {
      result.Add(x);
    }
    return result;
  }

}

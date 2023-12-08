public class Negate:Head {

  public readonly Expr expr;

  public Negate(Place place, Expr expr) : base(place) {
    this.expr = set(expr);
  }

  protected override Type resolve(Verifier v) {
    expr.verify(v);
    if (expr.type! is types.Int) {
      var it = (types.Int)expr.type!;
      if (!it.signed) {
        v.report(this, "Can't negate an unsigned value!");
      }
      return expr.type;
    }
    // TODO, float
    v.report(this, $"Can't negate type: {expr.type!}");
    return Fail.FAIL;
  }

  internal override ZZZ mk(Solva solva) {
    if (expr is Number) {
      // TODO, overflow
      var n = (Number)expr;
      return new zzz.Int((types.Int)type, -n.value);
    } else {
      return new zzz.Negate(expr.zzz(solva));
    }
  }

  protected override Pair toPair(LLVM llvm) {
    expr.emit(llvm);
    var xored = llvm.xor(expr.pair!, expr.pair!);
    return llvm.add(xored, new Pair("1", expr.pair!.type));
  }

  public override void format(Formatter fmt) {
    fmt.print("-");
    expr.format(fmt);
  }

  static string partial() => "-";

}

public partial class In {

  public Negate? negate { get {
    var place = skip();
    if (peek != '-') return null;
    expect("-", Flavor.OPERATOR);
    return new Negate(place, mustExpr);
  }}

}

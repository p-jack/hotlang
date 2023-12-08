using Microsoft.Z3;

public class Not:Head {

  public readonly Expr expr;

  public Not(Place place, Expr expr) : base(place) {
    this.expr = set(expr);
  }

  protected override Type resolve(Verifier v) {
    expr.verify(v);
    if (!(expr.type is types.Bool)) {
      v.report(this, "Must apply to bool expression, not {}.");
    }
    return Type.BOOL;
  }

  internal override ZZZ mk(Solva solva) {
    var ez = (BoolZZZ)expr.zzz(solva);
    var n = ez.not;
    if (n == null) return new zzz.Not(ez);
    return n;
  }

  protected override Pair toPair(LLVM llvm) {
    expr.emit(llvm);
    var r = llvm.nextVar();
    llvm.println($"  ${r} = xor ${pair}, 1");
    return new Pair(r, LLVM.BOOL);
  }

  /////

  public override void format(Formatter fmt) {
    fmt.print("!");
    if (!expr.singleTerm) fmt.print("(");
    expr.format(fmt);
    if (!expr.singleTerm) fmt.print(")");
  }

  static string partial() => "!";

}

public partial class In {

  public Not? not { get {
    var place = skip();
    if (peek != '!') return null;
    expect("!", Flavor.OPERATOR);
    var expr = mustExpr;
    return new Not(place, expr);
  }}

}

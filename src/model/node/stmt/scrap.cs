public class Scrap:Stmt {

  public readonly Expr expr;
  public readonly bool synthesized;

  public Scrap(Place place, Expr expr, bool synthesized) : base(place) {
    this.expr = set(expr);
    this.synthesized = synthesized;
  }

  public override void verify(Verifier v) {
    expr.verify(v);
    if (synthesized) return;
    if (!(expr.type is types.Void)) {
      v.report(this, "Unused expression.");
    }
    // TODO: If the expression is anything other than a void type, then it's an
    // "unused value" error, UNLESS it's synthesized (which we need for verifying
    // initial values.)
  }

  public override void format(Formatter fmt) {
    expr.format(fmt);
  }

  public override void emit(LLVM llvm) {
    expr.emit(llvm);
  }

}

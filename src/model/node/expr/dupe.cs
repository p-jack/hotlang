public class Dupe:Expr {

  private Expr expr;

  public Dupe(Expr expr) : base(expr.place) {
    this.expr = expr;
  }

  public override Dupe copy() => new Dupe(expr);
  public override Dupe copy(Func<Node,Node> xlat) => (Dupe)base.copy(xlat);

  public override bool lefty => expr.lefty;
  public override bool ambiguous => expr.ambiguous;
  public override bool singleTerm => expr.singleTerm;
  internal override Source? source => expr.source;

  internal override void expect(Type? type) {
    base.expect(type);
    expr.expect(type);
  }

  internal override void assigningTo(Pair leftPair) {
    base.assigningTo(leftPair);
    expr.assigningTo(leftPair);
  }

  internal override void reposition(Position p) {
    base.reposition(p);
    expr.reposition(p);
  }

  public override void format(Formatter f) {
    expr.format(f);
  }

  protected override Type resolve(Verifier v) {
    expr.verify(v);
    return expr.type;
  }

  internal override ZZZ mk(Solva solva) {
    return expr.zzz(solva);
  }

  protected override Pair toPair(LLVM llvm) {
    expr.emit(llvm);
    return expr.pair;
  }

  internal override void emitAssign(LLVM llvm, Expr right) {
    expr.emitAssign(llvm, right);
  }

}

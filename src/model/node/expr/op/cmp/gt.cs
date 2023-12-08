using Microsoft.Z3;

internal class GtOp:CmpOp {

  protected override Expr result(Expr left, Expr right) {
    return new Gt(left.place);
  }

  static void init(Syntax syntax) {
    syntax.add(">", new GtOp());
  }

}

internal class Gt:Cmp {

  public Gt(Place place) : base(place) {}

  internal override ZZZ mk(Solva solva) {
    return new zzz.Gt(left.zzz(solva), right.zzz(solva));
  }

  protected override string opcode { get {
    if (fp) return "fcmp ogt";
    return signed ? "icmp sgt" : "icmp ugt";
  }}

}

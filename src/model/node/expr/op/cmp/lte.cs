using Microsoft.Z3;

internal class LteOp:CmpOp {

  protected override Expr result(Expr left, Expr right) {
    return new Lte(left.place);
  }

  static void init(Syntax syntax) {
    syntax.add("<=", new LteOp());
  }

}

internal class Lte:Cmp {

  public Lte(Place place) : base(place) {}

  internal override ZZZ mk(Solva solva) {
    return new zzz.Lte(left.zzz(solva), right.zzz(solva));
  }

  protected override string opcode { get {
    if (fp) return "fcmp ole";
    return signed ? "icmp sle" : "icmp ule";
  }}

}

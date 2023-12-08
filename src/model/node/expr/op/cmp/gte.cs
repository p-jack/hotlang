using Microsoft.Z3;

internal class GteOp:CmpOp {

  protected override Expr result(Expr left, Expr right) {
    return new Gte(left.place);
  }

  static void init(Syntax syntax) {
    syntax.add(">=", new GteOp());
  }

}

internal class Gte:Cmp {

  public Gte(Place place) : base(place) {}

  internal override ZZZ mk(Solva solva) {
    return new zzz.Gte(left.zzz(solva), right.zzz(solva));
  }

  protected override string opcode { get {
    if (fp) return "fcmp oge";
    return signed ? "icmp sge" : "icmp uge";
  }}

}

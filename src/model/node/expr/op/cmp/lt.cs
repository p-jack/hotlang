using Microsoft.Z3;

internal class LtOp:CmpOp {

  protected override Expr result(Expr left, Expr right) {
    return new Lt(left.place);
  }

  static void init(Syntax syntax) {
    syntax.add("<", new LtOp());
  }

}

internal class Lt:Cmp {

  public Lt(Place place) : base(place) {}

  internal override ZZZ mk(Solva solva) {
    return new zzz.Lt(left.zzz(solva), right.zzz(solva));
  }

  protected override string opcode { get {
    if (fp) return "fcmp olt";
    return signed ? "icmp slt" : "icmp ult";
  }}

}

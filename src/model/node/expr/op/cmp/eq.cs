internal class EqOp:CmpOp {

  protected override Expr result(Expr left, Expr right) {
    return new Eq(left.place);
  }

  static void init(Syntax syntax) {
    syntax.add("==", new EqOp());
  }

}

internal class Eq:Cmp {

  public Eq(Place place) : base(place) {}

  internal override ZZZ mk(Solva solva) {
    return new zzz.Eq(left.zzz(solva), right.zzz(solva));
  }

  protected override string opcode { get {
    if (fp) return "fcmp oeq";
    return "icmp eq";
  }}

}

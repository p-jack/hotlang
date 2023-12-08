internal class NeOp:CmpOp {

  protected override Expr result(Expr left, Expr right) {
    return new Ne(left.place);
  }

  static void init(Syntax syntax) {
    syntax.add("!=", new NeOp());
  }

}

internal class Ne:Cmp {

  public Ne(Place place) : base(place) {}

  internal override ZZZ mk(Solva solva) {
    return new zzz.Ne(left.zzz(solva), right.zzz(solva));
  }

  protected override string opcode { get {
    if (fp) return "fcmp one";
    return "icmp ne";
  }}

}

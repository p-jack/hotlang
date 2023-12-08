internal class AddOp:MathOp {

  protected override Expr result(Expr left, Expr right) {
    return new Add(left.place);
  }

  static void init(Syntax syntax) {
    syntax.add("+", new AddOp());
  }

}

internal class Add:Math {

  public Add(Place place) : base(place) {}

  protected override bool overflows => true;

  internal override ZZZ mk(Solva solva) {
    return new zzz.Add(left.zzz(solva), right.zzz(solva));
  }

  protected override string opcode { get {
    if (fp) return "fadd";
    if (overflows && !leftInt.overflow) {
      if (signed) {
        return "add nsw";
      } else {
        return "add nuw";
      }
    }
    return "add";
  }}

}

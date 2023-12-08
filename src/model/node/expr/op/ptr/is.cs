internal class IsOp:PtrOp {

  string opcode;

  IsOp(string opcode) {
    this.opcode = opcode;
  }

  protected override Expr result(Expr left, Expr right) {
    return new Is(left.place, opcode);
  }

  static void init(Syntax syntax) {
    syntax.add("is", new IsOp("eq"));
    syntax.add("not", new IsOp("ne"));
  }

}

internal class Is:Ptr {

  public readonly string opcode_;

  public Is(Place place, string opcode_) : base(place) {
    this.opcode_ = opcode_;
  }

  internal override ZZZ mk(Solva solva) {
    throw new Bad("TODO");
  }

  protected override string opcode { get {
    return opcode_;
  }}

}

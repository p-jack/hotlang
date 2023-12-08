internal class ArrayCountHandler:Count.Handler {

  public override string name => "hotc";

  public override bool applies(Count count) {
    return count.holder.type is types.Array;
  }

  public override Expr? expand(Count count, Verifier v) {
    return new ArrayCount(count.place, count.holder);
  }

  static void init(Syntax syntax) {
    syntax.countHandlers.Add(new ArrayCountHandler());
  }

}

internal class ArrayCount:Expr {

  private readonly Expr array;

  public ArrayCount(Place place, Expr array) : base(place) {
    this.array = array;
  }

  protected override Type resolve(Verifier v) {
    var conf = ancestor<Program>()!.conf;
    var focus = Focus.primitive(true);
    return new types.Int(focus, false, true, 0, conf.intBits);
  }

  internal override ZZZ mk(Solva solva) {
    throw new Bad();
  }

  protected override Pair toPair(LLVM llvm) {
    array.emit(llvm);
    return llvm.length(array.pair!);
  }

  /////

  public override void format(Formatter fmt) {
    fmt.print("ARRAYCOUNT#");
  }
}

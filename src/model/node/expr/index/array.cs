internal class ArrayIndexHandler:Index.Handler {

  public override string name => "hotc";

  public override bool applies(Index index) {
    return index.holder.type is types.Array;
  }

  public override Expr? expand(Index index, Verifier v) {
    if (!(index.value.type is types.Int)) {
      v.report(index, $"Can only index an array with an integer, not {index.value.type}");
      return null;
    }
    return new ArrayIndex(index.place, index.holder, index.value);
  }


  static void init(Syntax syntax) {
    syntax.indexHandlers.Add(new ArrayIndexHandler());
  }

}

internal class ArrayIndex:Expr {

  private readonly Expr array;
  private readonly Expr index;

  public ArrayIndex(Place place, Expr array, Expr index) : base(place) {
    this.array = array;
    this.index = index;
  }

  public override bool lefty => true;

  protected override Type resolve(Verifier v) {
    var at = (types.Array)array.type;
    return at.type;
  }

  internal override ZZZ mk(Solva solva) {
    throw new Bad();
  }

  protected override Pair toPair(LLVM llvm) {
    array.emit(llvm);
    index.emit(llvm);
    var elem = type!.llvm;
    var firstPtr = llvm.firstPtr(array.pair!);
    var gep = new Gep(firstPtr, index.pair);
    var ptr = llvm.point(gep, elem);
    return llvm.load(ptr);
  }

  internal override void emitAssign(LLVM llvm, Expr right) {
    array.emit(llvm);
    index.emit(llvm);
    var firstPtr = llvm.firstPtr(array.pair!);
    llvm.assign(firstPtr, index.pair!, right);
  }

  /////

  public override void format(Formatter fmt) {
    fmt.print("ARRAYINDEX[");
    index.format(fmt);
    fmt.print("]");
  }
}

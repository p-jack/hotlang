public partial class LLVM {

  public Pair length(Pair array) {
    var at = (llvm.Array)array.type;
    var r = nextVar();
    println($"  {r} = extractvalue {array}, 1");
    return new Pair(r, at.i0);
  }

  public Pair firstPtr(Pair array) {
    var at = (llvm.Array)array.type;
    var r = nextVar();
    println($"  {r} = extractvalue {array}, 0");
    var result = new Pair(r, at.type.star);
    return result;
  }

}

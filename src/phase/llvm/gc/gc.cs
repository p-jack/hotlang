/*
public partial class LLVM {

  public void setGC(Expr obj, Field field, Expr right) {
    println($"  call void {field.setterName}({obj.pair}, {right.pair})");
  }

  public void root(Pair pair) {
    var controlPtr = this.controlPtr(pair);
    var control = load(controlPtr);
    var increased = increaseLocals(control);
    store(controlPtr, increased);
  }

  public void unroot(Pair pair) {
    var label = nextLabel();
    var controlP = this.controlPtr(pair);
    var control = load(controlP);
    var decreased = decreaseLocals(control);
    store(controlP, decreased);
    var anchored = this.anchored(control);
    br(anchored, $"Continue-{label}", $"NotAnchored-{label}");
    println($"NotAnchored-{label}:");
    var paramz = new List<Pair>() { pair };
    var reached = reachable(pair);
    br(reached, $"Continue-{label}", "NotReached-{label}");
    println($"NotReached-{label}:");
    store(controlP, delete(decreased));
    destroy(pair);
    freeGC(pair);
    println($"  br label %Continue-{label}");
    println($"Continue-{label}:");
  }

  public void freeGC(Pair ptr) {
    debug(ptr, "FREEING!");
    var size = sizeOf(ptr.type, 1);
    var cast = bitcast(ptr, new llvm.Direct("i8*"));
    // TODO, skip this once we're sure everything works
    println($"  call void @memset({cast}, {i0} 51, {size})");
    var controlPtr = this.controlPtr(ptr);
    var v = nextVar();
    println($"  {v} = bitcast {controlPtr.star} to i8*");
    println($"  call void @free(i8* {v})");
  }

}
*/

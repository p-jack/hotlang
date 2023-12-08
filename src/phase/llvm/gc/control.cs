// TODO: Eliminate anchor bit
public partial class LLVM {

  public Pair controlPtr(Pair pair) {
    // TODO, might break if intBits != pointerBits
    var objint = bitcast(pair, i0.star);
    var gep = new Gep(objint, -1);
    return point(gep, i0);
  }

  Pair getControlBit(Pair control, int n) {
    var shift = conf.intBits - n;
    var bit = shl(i0, 1, shift);
    var result = shr(and(control, bit), shift);
    return trunc(result, new llvm.Int(1));
  }

  Pair setControlBit(Pair control, int n) {
    var shift = conf.intBits - n;
    var bit = shl(i0, 1, shift);
    return or(control, bit);
  }

  Pair clearControlBit(Pair control, int n) {
    var shift = conf.intBits - n;
    var bit = shl(i0, 1, shift);
    return and(control, not(bit));
  }

  /////

  public Pair anchored(Pair control) {
    return getControlBit(control, 1);
  }

  public Pair anchor(Pair control) {
    return setControlBit(control, 1);
  }

  public Pair unanchor(Pair control) {
    return clearControlBit(control, 1);
  }

  /////

  public Pair marked(Pair control) {
    return getControlBit(control, 2);
  }

  public Pair mark(Pair control) {
    return setControlBit(control, 2);
  }

  public Pair unmark(Pair control) {
    return clearControlBit(control, 2);
  }

  /////

  public Pair deleting(Pair control) {
    return getControlBit(control, 3);
  }

  public Pair delete(Pair control) {
    return setControlBit(control, 3);
  }

  /////

  Pair localsMask => shl(i0, 7, conf.intBits - 3);

  public Pair locals(Pair control) {
    return and(control, not(localsMask));
  }

  public Pair increaseLocals(Pair control) {
    var mask = localsMask;
    var bits = and(control, mask);
    var locals = and(control, not(mask));
    var increased = add(locals, 1);
    return or(bits, increased);
  }

  public Pair decreaseLocals(Pair control) {
    var mask = localsMask;
    var bits = and(control, mask);
    var locals = and(control, not(mask));
    var decreased = sub(locals, 1);
    return or(bits, decreased);
  }

}


public class Unroot:Mop {

  Pair pair;

  public Unroot(Pair pair) {
    this.pair = pair;
  }

  public override void emit(LLVM llvm) {
    // TODO
  }

}

/*


///

*/

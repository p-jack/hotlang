using Microsoft.Z3;

// TODO, since Is is the only one, just merge them

internal abstract class PtrOp:Op.Handler {

  public override string name => "hotc";

  public override Expr? resolve(Verifier v, Expr left, Expr right) {
    var ltype = left.type!;
    var rtype = right.type!;
    if (!ltype.aggregate) return null;
    if (!rtype.aggregate) return null;
    if (ltype is types.Null || rtype is types.Null) {
      return result(left, right);
    }
    if (!ltype.same(rtype)) return null;
    return result(left, right);
  }

  protected abstract Expr result(Expr left, Expr right);

}

internal abstract class Ptr:OpExpr {

  public Ptr(Place place) : base(place) {}

  protected abstract string opcode { get; }

  protected override Type resolve(Verifier v) {
    return Type.BOOL;
  }

  protected override Pair toPair(LLVM llvm) {
    var result = llvm.nextVar();
    var lptr = llvm.bitcast(this.left.pair, "i8*");
    var rptr = llvm.bitcast(this.right.pair, "i8*");
    var l = llvm.ptrtoint(lptr);
    var r = llvm.ptrtoint(rptr);
    llvm.println($"  {result} = icmp {opcode} {lptr}, {rptr.name}");
    return new Pair(result, LLVM.BOOL);
  }

  public override void format(Formatter fmt) {
    throw new Bad();
  }

}

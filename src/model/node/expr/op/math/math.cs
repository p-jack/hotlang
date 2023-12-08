using Microsoft.Z3;

internal abstract class MathOp:Op.Handler {

  public override string name => "hotc";

  bool valid(Type t) {
    if (t is types.Int) return true;
    if (t is types.Float) return true;
    return false;
  }

  public override Expr? resolve(Verifier v, Expr left, Expr right) {
    var ltype = left.type!;
    var rtype = right.type!;
    if (!valid(ltype)) return null;
    if (!ltype.same(rtype)) return null;
    return result(left, right);
  }

  protected abstract Expr result(Expr left, Expr right);

}

internal abstract class Math:OpExpr {

  public Math(Place place) : base(place) {}

  protected abstract bool overflows { get; }
  protected abstract string opcode { get; }

  protected override Type resolve(Verifier v) {
    return leftType;
  }

  protected override Pair toPair(LLVM llvm) {
    var result = llvm.nextVar();
    llvm.println($"  {result} = {opcode} {left.pair}, {right.pair.name}");
    return new Pair(result, type.llvm);
  }

  public override void format(Formatter fmt) {
    throw new Bad();
  }

}

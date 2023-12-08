using Microsoft.Z3;

namespace zzz {

internal class Add:Math {

  public Add(ZZZ left, ZZZ right) : base(left, right) {}

  public override Microsoft.Z3.Expr z3(Context ctx) {
    // TODO: Overflow
    if (fp) {
      return ctx.MkFPAdd(ctx.MkFPRoundTowardZero(), (FPExpr)left.z3(ctx), (FPExpr)right.z3(ctx));
    } else {
      return ctx.MkBVAdd((BitVecExpr)left.z3(ctx), (BitVecExpr)right.z3(ctx));
    }
  }

  public override ZZZ rewrite(Rewriter rw) {
    return new Add(left.rewrite(rw), right.rewrite(rw));
  }

  public override string ToString() {
    return left + " + " + right;
  }

}

}

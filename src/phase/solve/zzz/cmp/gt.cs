using Microsoft.Z3;

namespace zzz {

internal class Gt:Cmp {

  public Gt(ZZZ left, ZZZ right) : base(left, right) {}

  public override BoolExpr z3(Context ctx) {
    if (fp) {
      return ctx.MkFPGt((FPExpr)left.z3(ctx), (FPExpr)right.z3(ctx));
    } else if (signed) {
      return ctx.MkBVSGT((BitVecExpr)left.z3(ctx), (BitVecExpr)right.z3(ctx));
    } else {
      return ctx.MkBVUGT((BitVecExpr)left.z3(ctx), (BitVecExpr)right.z3(ctx));
    }
  }

  public override BoolZZZ rewriteBool(Rewriter rw) {
    return new Gt(left.rewrite(rw), right.rewrite(rw));
  }

  public override BoolZZZ? not => new Lte(left, right);

  public override string ToString() {
    return left + " > " + right;
  }

}

}

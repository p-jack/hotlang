using Microsoft.Z3;

namespace zzz {

internal class Lt:Cmp {

  public Lt(ZZZ left, ZZZ right) : base(left, right) {}

  public override BoolExpr z3(Context ctx) {
    if (fp) {
      return ctx.MkFPLt((FPExpr)left.z3(ctx), (FPExpr)right.z3(ctx));
    } else if (signed) {
      return ctx.MkBVSLT((BitVecExpr)left.z3(ctx), (BitVecExpr)right.z3(ctx));
    } else {
      return ctx.MkBVULT((BitVecExpr)left.z3(ctx), (BitVecExpr)right.z3(ctx));
    }
  }

  public override BoolZZZ rewriteBool(Rewriter rw) {
    return new Lt(left.rewrite(rw), right.rewrite(rw));
  }

  public override BoolZZZ? not => new Gte(left, right);

  public override string ToString() {
    return left + " < " + right;
  }

}

}

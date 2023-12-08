using Microsoft.Z3;

namespace zzz {

internal class Gte:Cmp {

  public Gte(ZZZ left, ZZZ right) : base(left, right) {}

  public override BoolExpr z3(Context ctx) {
    if (fp) {
      return ctx.MkFPGEq((FPExpr)left.z3(ctx), (FPExpr)right.z3(ctx));
    } else if (signed) {
      return ctx.MkBVSGE((BitVecExpr)left.z3(ctx), (BitVecExpr)right.z3(ctx));
    } else {
      return ctx.MkBVUGE((BitVecExpr)left.z3(ctx), (BitVecExpr)right.z3(ctx));
    }
  }

  public override BoolZZZ rewriteBool(Rewriter rw) {
    return new Gte(left.rewrite(rw), right.rewrite(rw));
  }

  public override BoolZZZ? not => new Lt(left, right);

  public override string ToString() {
    return left + " >= " + right;
  }

}

}

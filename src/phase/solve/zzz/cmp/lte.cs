using Microsoft.Z3;

namespace zzz {

internal class Lte:Cmp {

  public Lte(ZZZ left, ZZZ right) : base(left, right) {}

  public override BoolExpr z3(Context ctx) {
    if (fp) {
      return ctx.MkFPLEq((FPExpr)left.z3(ctx), (FPExpr)right.z3(ctx));
    } else if (signed) {
      return ctx.MkBVSLE((BitVecExpr)left.z3(ctx), (BitVecExpr)right.z3(ctx));
    } else {
      return ctx.MkBVULE((BitVecExpr)left.z3(ctx), (BitVecExpr)right.z3(ctx));
    }
  }

  public override BoolZZZ rewriteBool(Rewriter rw) {
    return new Lte(left.rewrite(rw), right.rewrite(rw));
  }

  public override BoolZZZ? not => new Gt(left, right);

  public override string ToString() {
    return left + " <= " + right;
  }

}

}

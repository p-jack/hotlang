using Microsoft.Z3;

namespace zzz {

internal class Imply:BoolZZZ {

  BoolZZZ left;
  BoolZZZ right;

  public Imply(BoolZZZ left, BoolZZZ right) {
    this.left = left;
    this.right = right;
  }

  public override BoolExpr z3(Context ctx) {
    return ctx.MkImplies(left.z3(ctx), right.z3(ctx));
  }

  public override BoolZZZ rewriteBool(Rewriter rw) {
    return new Imply(left.rewriteBool(rw), right.rewriteBool(rw));
  }

  public override BoolZZZ? not => null;

  public override string ToString() {
    return left + " --> " + right;
  }

}


}

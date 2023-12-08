using Microsoft.Z3;

namespace zzz {

internal class Eq:Cmp {

  public Eq(ZZZ left, ZZZ right) : base(left, right) {}

  public override BoolExpr z3(Context ctx) {
    return ctx.MkEq(left.z3(ctx), right.z3(ctx));
  }

  public override BoolZZZ rewriteBool(Rewriter rw) {
    return new Eq(left.rewrite(rw), right.rewrite(rw));
  }

  public override BoolZZZ? not => new Ne(left, right);

  public override string ToString() {
    return left + " == " + right;
  }

}


}

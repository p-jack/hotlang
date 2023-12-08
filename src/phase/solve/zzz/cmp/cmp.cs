using Microsoft.Z3;

namespace zzz {

internal abstract class Cmp:BoolZZZ {

  public ZZZ left;
  public ZZZ right;

  public Type ltype => left.type!;
  public bool fp => ltype is types.Float;
  public bool signed => (ltype is types.Int) && ((types.Int)ltype).signed;

  public Cmp(ZZZ left, ZZZ right) {
    this.left = left;
    this.right = right;
  }

  // public override BoolExpr z3(Context ctx) {
  //   return ctx.MkEq(left.z3(ctx), right.z3(ctx));
  // }

//  public override BoolZZZ? not => new Ne(left, right);

  // public override string ToString() {
  //   return left + " == " + right;
  // }

}


}

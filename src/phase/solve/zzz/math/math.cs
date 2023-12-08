using Microsoft.Z3;

namespace zzz {

internal abstract class Math:ZZZ {

  public ZZZ left;
  public ZZZ right;

  public override Type type => left.type;

  public Type ltype => left.type;
  public bool fp => ltype is types.Float;
  public bool signed => (ltype is types.Int) && ((types.Int)ltype).signed;

  public Math(ZZZ left, ZZZ right) {
    this.left = left;
    this.right = right;
  }

}


}

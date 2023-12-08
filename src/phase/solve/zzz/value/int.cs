using Microsoft.Z3;

namespace zzz {

internal class Int:ZZZ {

  types.Int typ;
  long value;

  public Int(types.Int type, long value) {
    this.typ = type;
    this.value = value;
  }

  public override types.Int type => typ;

  public override Microsoft.Z3.Expr z3(Context ctx) {
    return ctx.MkBV(value, (uint)type.bits);
  }

  public override ZZZ rewrite(Rewriter rw) => this;

  public override string ToString() {
    return $"{value}";
  }

}


}

using Microsoft.Z3;

namespace zzz {

internal class Negate:ZZZ {

  ZZZ value;

  public Negate(ZZZ value) {
    this.value = value;
  }

  public override Type type => value.type;

  public override Microsoft.Z3.Expr z3(Context ctx) {
    if (type is types.Int) {
      return ctx.MkBVNeg((BitVecExpr)value.z3(ctx));
    }
    throw new Bad($"Unsupported type for z3 negate: {type}");
  }

  public override ZZZ rewrite(Rewriter rw) {
    return new Negate(value.rewrite(rw));
  }

  public override string ToString() {
    return $"-({value})";
  }

}

}

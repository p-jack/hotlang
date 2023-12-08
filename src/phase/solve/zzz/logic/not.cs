using Microsoft.Z3;

namespace zzz {

internal class Not:BoolZZZ {

  BoolZZZ value;

  public Not(BoolZZZ value) {
    this.value = value;
  }

  public override BoolExpr z3(Context ctx) {
    return ctx.MkNot(value.z3(ctx));
  }

  public override BoolZZZ rewriteBool(Rewriter rw) {
    return new Not(value.rewriteBool(rw));
  }

  public override BoolZZZ? not => value;

  public override string ToString() {
    return $"!({value})";
  }

}

}

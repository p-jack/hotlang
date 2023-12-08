using Microsoft.Z3;

namespace zzz {

internal class Bool:BoolZZZ {

  bool value;

  public Bool(bool value) {
    this.value = value;
  }

  public override BoolExpr z3(Context ctx) {
    if (value) return ctx.MkTrue();
    return ctx.MkFalse();
  }

  public override BoolZZZ? not => new Bool(!value);

  public override BoolZZZ rewriteBool(Rewriter rw) => this;

  public override string ToString() {
    return $"{value}";
  }

}

}

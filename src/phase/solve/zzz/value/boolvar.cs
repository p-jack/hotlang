using Microsoft.Z3;

namespace zzz {

internal class BoolVar:BoolZZZ {

  Symbol symbol;

  internal BoolVar(Symbol symbol) {
    this.symbol = symbol;
  }

  public override BoolZZZ? not => null;

  public override BoolExpr z3(Context ctx) {
    return ctx.MkBoolConst(symbol.ToString());
  }

  public override BoolZZZ rewriteBool(Rewriter rw) {
    var ns = rw.replace(symbol);
    return new BoolVar(ns);
  }

  public override string ToString() {
    return symbol.ToString();
  }

}

}

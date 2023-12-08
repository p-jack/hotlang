using Microsoft.Z3;

namespace zzz {

internal class Var:ZZZ {

  public Type typ;
  public Symbol symbol;

  internal Var(Type type, Symbol symbol) {
    this.typ = type;
    this.symbol = symbol;
  }

  public override Type type => typ;

  public override Microsoft.Z3.Expr z3(Context ctx) {
    return ctx.MkConst(symbol.ToString(), type!.z3(ctx));
  }

  public override ZZZ rewrite(Rewriter rw) {
    var ns = rw.replace(symbol);
    return new Var(type, ns);
  }

  public override string ToString() {
    return $"{symbol}";
  }

}

}

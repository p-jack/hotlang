using Microsoft.Z3;

internal abstract class ZZZ {

  public static zzz.Int NULL = new zzz.Int(Type.UINT32, 0);

  public abstract Type type { get; }

  public abstract Microsoft.Z3.Expr z3(Context ctx);

  public abstract ZZZ rewrite(Rewriter rw);

  public override abstract string ToString();

  public static ZZZ var(Type type, Symbol symbol) {
    if (type is types.Bool) {
      return new zzz.BoolVar(symbol);
    } else {
      return new zzz.Var(type, symbol);
    }
  }

  public static Symbol field(Symbol start, Field field) {
    return start.add($"->{field.name}");
  }

}

internal abstract class BoolZZZ:ZZZ {

  public override Type type => Type.BOOL;

  public override abstract BoolExpr z3(Context ctx);

  public abstract BoolZZZ? not { get; }

  public sealed override ZZZ rewrite(Rewriter rw) => rewriteBool(rw);

  public virtual BoolZZZ rewriteBool(Rewriter rw) => this;

  public BoolZZZ and(BoolZZZ other) {
    return new zzz.And(this, other);
  }

}

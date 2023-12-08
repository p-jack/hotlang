namespace types { public class Loop:Type {

  public Type type;

  public Loop(Type type) : base(new Focus(true, false, types.Mutability.IMMUTABLE, types.Scheme.JAILED)) {
    this.type = type;
  }

  public override Microsoft.Z3.Sort z3(Microsoft.Z3.Context ctx) {
    return type.z3(ctx);
  }

  public override string indexKey => "loop";
  public override llvm.Type llvm => throw new Bad();
  public override Loop refocus(Focus f) => new Loop(type);
  public override string ToString() => $"loop[{type}]";
  protected override int hashCode => HashCode.Combine(focus, type);
  public override bool same(Type t) {
    if (t.GetType() != typeof(Loop)) return false;
    var that = (Loop)t;
    return type.same(that.type);
  }

}}

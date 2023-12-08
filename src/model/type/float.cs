namespace types {
public class Float:Type {

  public readonly bool big;
  public int bitSize => big ? 64 : 32;

  public Float(Focus focus, bool big) : base(focus) {
    this.big = big;
  }

  public override Microsoft.Z3.Sort z3(Microsoft.Z3.Context ctx) {
    return big ? ctx.MkFPSortDouble() : ctx.MkFPSortSingle();
  }

  public override string indexKey => "number";
  public override llvm.Type llvm => new llvm.Float(bitSize);
  public override Float refocus(Focus f) => new Float(f, big);
  public override string ToString() => $"f{bitSize}";
  protected override int hashCode => HashCode.Combine(focus, big);
  public override bool same(Type t) {
    if (t.GetType() != typeof(Float)) return false;
    Float f = (Float)t;
    return f.big == big;
  }

}
}

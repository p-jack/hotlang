namespace types {
public class Enum:Type {

  public readonly string fullName;
  public readonly int systemBitSize;

  public Enum(Focus focus, string fullName, int systemBitSize) : base(focus) {
    this.fullName = fullName;
    this.systemBitSize = systemBitSize;
  }

  public override Microsoft.Z3.Sort z3(Microsoft.Z3.Context ctx) {
    return ctx.MkBitVecSort((uint)systemBitSize);
  }

  public override string indexKey => fullName;
  public override llvm.Type llvm => new llvm.Int(systemBitSize);
  public override Enum refocus(Focus f) => new Enum(f, fullName, systemBitSize);
  public override string ToString() => fullName;
  protected override int hashCode => HashCode.Combine(focus, fullName, systemBitSize);
  public override bool same(Type t) {
    if (t.GetType() != typeof(Enum)) return false;
    Enum e = (Enum)t;
    return e.systemBitSize == systemBitSize && e.fullName == fullName;
  }

}
}

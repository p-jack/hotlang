namespace types {

public class Error:Type {

  int systemBits;

  public Error(Focus f, int systemBits) : base(f) {
    this.systemBits = systemBits;
  }

  public override Microsoft.Z3.Sort z3(Microsoft.Z3.Context ctx) {
    return ctx.MkBitVecSort((uint)systemBits);
  }

  public override string indexKey => "error";
  public override llvm.Type llvm => new llvm.Int(systemBits);
  public override Error refocus(Focus f) => new Error(f, systemBits);
  public override string ToString() => "error";
  protected override int hashCode => focus.GetHashCode();
  public override bool same(Type t) => t.GetType() == typeof(Error);

}

}

namespace types {

public class Void:Type {

  public Void(Focus f) : base(f) {}

  public override Microsoft.Z3.Sort z3(Microsoft.Z3.Context ctx) {
    throw new Bad();
  }

  public override string indexKey => "void";
  public override llvm.Type llvm => new llvm.Direct("void");
  public override Void refocus(Focus f) => new Void(f);
  public override string ToString() => "void";
  protected override int hashCode => focus.GetHashCode();
  public override bool same(Type t) {
    if (t.GetType() != typeof(Void)) return false;
    return true;
  }

}}

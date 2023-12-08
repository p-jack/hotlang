namespace types { public class Bool:Type {

  public Bool(Focus f) : base(f) {}

  public override Microsoft.Z3.Sort z3(Microsoft.Z3.Context ctx) {
    return ctx.MkBoolSort();
  }

  public override string indexKey => "bool";
  public override llvm.Type llvm => new llvm.Direct("i1");
  public override Bool refocus(Focus f) => new Bool(f);
  public override string ToString() => "bool";
  protected override int hashCode => focus.GetHashCode();
  public override bool same(Type t) => true;

}}

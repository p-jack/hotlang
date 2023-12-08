namespace types {

public class Null:Type {

  public Null() : base(new Focus(true, false, Mutability.IMMUTABLE, Scheme.DATA)) {}

  public override string indexKey => "null";
  public override bool aggregate => true;
  public override llvm.Type llvm => new llvm.Int(1).star;
  public override string ToString() => "null";
  public override bool same(Type other) => true;
  public override Null refocus(Focus f) { throw new Bad("Can't refocus null type."); }
  protected override int hashCode => 1;

  protected override IList<string> check(Action action, Type formal) {
    if (!formal.pointer) {
      return mismatch(action, formal, this);
    }
    return new List<string>();
  }

}

}

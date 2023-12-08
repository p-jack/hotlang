public class Unknown:Type {

  public static readonly Unknown UNKNOWN = new Unknown();

  private Unknown() : base(Focus.primitive(true)) {}

  public override string indexKey => "";
  public override bool real => false;
  public override llvm.Type llvm { get { throw new Bad(""); }}
  public override string ToString() => "???";
  public override bool same(Type t) => false;
  protected override int hashCode => 0;

  public override Unknown refocus(Focus f) {
    throw new Bad("called Unknown.refocus");
  }

  public override IList<String> passTo(Type formal) {
    throw new Bad("called Unknown.passTo");
  }

  public override IList<String> assignTo(Type formal) {
    throw new Bad("called Unknown.assignTo");
  }

  public override IList<String> returnTo(Type formal) {
    throw new Bad("called Unknown.returnTo");
  }

  public override IList<String> constructTo(Type formal) {
    throw new Bad("called Unknown.constructTo");
  }

}

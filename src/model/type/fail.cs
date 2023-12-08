public class Fail:Type {

  public static readonly Fail FAIL = new Fail();

  private Fail() : base(Focus.primitive(true)) {}

  public override string indexKey => "";
  public override string ToString() => "FAIL";
  public override bool same(Type t) => false;
  protected override int hashCode => 0;

  public override bool real => false;
  public override llvm.Type llvm => throw new Bad();
  public override Type refocus(Focus f) => throw new Bad();

  public override IList<string> passTo(Type formal) => throw new Bad();
  public override IList<string> assignTo(Type formal) => throw new Bad();
  public override IList<string> returnTo(Type formal) => throw new Bad();
  public override IList<string> constructTo(Type formal) => throw new Bad();

}

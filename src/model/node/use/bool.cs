public class Bool:Use {

  public override bool primitive => true;

  public Bool(Place place, Blur blur) : base(place, blur) {}

  public override string ToString() => "bool" + blur;

  protected override Type resolve(Verifier v) {
    var defaults = Focus.primitive(true);
    return new types.Bool(blur.focus(defaults));
  }

  static string parserName() => "boolUse";
  static string[] full() => new string[] { "bool" };

}

public partial class In {

  public Bool boolUse { get {
    var place = skip();
    expect("bool", Flavor.TYPE);
    return new Bool(place, blur);
  }}

}

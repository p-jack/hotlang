public class This:Use {

  public override bool primitive => false;

  public This(Place place, Blur blur) : base(place, blur) {}

  public override string ToString() => blur.ToString();

  protected override Type resolve(Verifier v) {
    var str = ancestor<Struct>();
    if (str == null) {
      v.report(this, "Can only use 'this' shorthand for functions and methods defined in a struct.");
      return Fail.FAIL;
    }
    var defaults = str.defaultFocus(v.oot);
    return new types.StructType(blur.focus(defaults), str);
  }

}

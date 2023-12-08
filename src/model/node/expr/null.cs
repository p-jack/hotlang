public class Null:Head {

  public Null(Place place) : base(place) {}

  protected override Type resolve(Verifier v) {
    if (expected == null) return new types.Null();
    if (!expected.focus.nullable) {
      v.report(this, $"Can't use null for expected type {expected}.");
      return Fail.FAIL;
    }
    return expected;
  }

  internal override ZZZ mk(Solva solva) {
    throw new Bad();
  }

  protected override Pair toPair(LLVM llvm) {
    return new Pair("null", type.llvm);
  }

  public override void format(Formatter fmt) {
    fmt.print("null");
  }

  static string full() => "null";
  static string parserName() => "nullExpr";

}

public partial class In {

  public Null nullExpr { get {
    var place = skip();
    expect("null", Flavor.KEYWORD);
    return new Null(place);
  }}

}

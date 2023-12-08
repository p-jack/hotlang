namespace use {

public class Array:Use {

  public readonly Use use;

  public Array(Place place, Blur blur, Use use) : base(place, blur) {
    this.use = set(use);
  }

  public override bool primitive => false;

  public override string ToString() => "[" + use.ToString() + "]" + blur;

  public static Focus defaults(Type element) {
    if (ofGraph(element)) {
      return new Focus(false, false, types.Mutability.MUTABLE, types.Scheme.UNIQUE);
    }
    return new Focus(false, true, types.Mutability.MUTABLE, types.Scheme.RC);
  }

  static bool ofGraph(Type type) {
    if (type is types.StructType) {
      var st = (types.StructType)type;
      return st.focus.scheme == types.Scheme.GRAPH;
    }
    return false;
  }

  protected override Type resolve(Verifier v) {
    var conf = ancestor<Program>()!.conf;
    use.verify(v);
    var type = use.type!;
    Focus defaults = Array.defaults(type);
    var focus = blur.focus(defaults);
    if (focus.nullable) {
      v.report(this, "Arrays cannot be null.");
    }
    if (ofGraph(type) && focus.scheme != types.Scheme.UNIQUE) {
      v.report(this, "Array of graph objects must be unique.");
    }
    if (ofGraph(type) && focus.variable) {
      v.report(this, "Array of graph objects must be fixed.");
    }
    return new types.Array(focus, type, conf.intBits);
  }

  static string parserName() => "arrayUse";
  static string partial() => "[";

}

}

public partial class In {

  public use.Array arrayUse { get {
    var place = skip();
    expect("[", Flavor.BRACE);
    var use = this.mustUse;
    expect("]", Flavor.BRACE);
    var blur = this.blur;
    return new use.Array(place, blur, use);
  }}

}

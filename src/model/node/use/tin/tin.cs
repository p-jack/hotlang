namespace use {

public partial class Tin:Use {

  public abstract class Handler {
    public abstract string origin { get; }
    public abstract string name { get; }
    public abstract Struct? expand(Verifier v, Node n, Key key);
  }

  public readonly string name;
  public readonly Use use1;
  public readonly Use? use2;
  public readonly List<Pair> paramz;

  public Tin(Place place, Blur blur, string name, Use use1, Use? use2, List<Pair> paramz) : base(place, blur) {
    this.name = name;
    this.use1 = set(use1);
    this.use2 = setNull(use2);
    this.paramz = paramz;
  }

  public override bool primitive => false;

  protected override Type resolve(Verifier v) {
    use1.verify(v);
    var type1 = use1.type!;
    Type? type2 = null;
    if (use2 != null) {
      use2.verify(v);
      type2 = use2.type!;
    }
    if (use1.failed) return Fail.FAIL;
    if (use2 != null && use2.failed) return Fail.FAIL;
    var key = new Key(name, type1, type2, paramz);
    Type? result = (Type?)Tin.handle(v, this, blur, key) ?? (Type)Fail.FAIL;
    return result;
  }

  public static types.TinType? handle(Verifier v, Node n, Blur blur, Key key) {
    var program = n.ancestor<Program>()!;
    var handler = program.syntax.tinHandler(key.name);
    if (handler == null) {
      v.report(n, $"No such tin: #{key.name}");
      return null;
    }
    var type1 = key.type1;
    var type2 = key.type2;
    var access = type1.access;
    if (type2 != null && type2.access < access) {
      access = type2.access;
    }
    var tins = tinsFor(n, access);
    Struct? str;
    if (tins.ContainsKey(key)) {
      str = tins[key];
    } else {
      str = handler.expand(v, n, key);
      if (str == null) return null;
      tins[key] = str;
      var unit = n.ancestor<Unit>()!;
      unit.tops.Add(str);
      unit.adopt(str);
      str.index(v.oot);
      str.setSupers(v.oot);
      str.analyze(v.oot);
    }
    var defaults = key.defaults();
    if (forField(n)) defaults = defaults.vary(false);
    var focus = blur.focus(defaults);
    return new types.TinType(focus, key, str);
  }

  static Dictionary<Key,Struct> tinsFor(Node n, Access access) {
    switch (access) {
      case Access.FILE:
        return n.ancestor<Unit>()!.tins;
      case Access.FOLDER:
        return n.ancestor<Folder>()!.tins;
      default:
        return n.ancestor<Program>()!.root.tins;
    }
  }

  // static Bone parentFor(Access access) {
  //   switch (access) {
  //     case Access.FILE:
  //       return ancestor<Unit>()!;
  //     case Access.FOLDER:
  //       return ancestor<Folder>()!;
  //     default:
  //       return ancestor<Program>()!;
  //   }
  // }

  static bool forField(Node n) {
    if (!(n.parent is NUI)) return false;
    var nui = (NUI)n.parent;
    return nui.parent is Field;
  }

  /////

  public override string ToString() {
    var sb = new System.Text.StringBuilder();
    sb.Append("#");
    sb.Append(name);
    if (use1 != null) {
      sb.Append("[");
      sb.Append(use1.ToString());
      if (use2 != null) {
        sb.Append(":");
        sb.Append(use2.ToString());
      }
      sb.Append("]");
    }
    Pair.addTo(paramz, sb);
    return sb.ToString();
  }

  static string parserName() => "tinUse";
  static string full() => "#";

}}

public partial class In {

  public use.Tin tinUse { get {
    var place = skip();
    expect("#", Flavor.OPERATOR);
    var name = id();
    expect("[", Flavor.BRACE);
    Use use1 = mustUse;
    Use? use2 = null;
    if (peek == ':') {
      expect(":", Flavor.OPERATOR);
      use2 = mustUse;
    }
    expect("]", Flavor.BRACE);
    var pairs = new List<use.Tin.Pair>();
    if (peek == '{') {
      expect("{", Flavor.BRACE);
      while (peek != '}') {
        var p = this.pair;
        if (p == null) throw new Bad($"{place}: Expected key/value pair.");
        pairs.Add(p);
      }
      expect("}", Flavor.BRACE);
    }
    return new use.Tin(place, blur, name, use1, use2, pairs);
  }}

  bool alphaNum { get {
    var ch = peek;
    return isLetter(ch) || isDigit(ch) || ch == '_';
  }}

  string nextString { get {
    skip();
    var sb = new System.Text.StringBuilder();
    while (alphaNum) {
      sb.Append(peek);
      expect(peek, Flavor.KEYWORD);
    }
    return sb.ToString();
  }}

  use.Tin.Pair? pair { get {
    skip();
    if (!alphaNum) return null;
    var k = nextString;
    expect(":", Flavor.OPERATOR);
    var v = nextString;
    return new use.Tin.Pair(k, v);
  }}
}

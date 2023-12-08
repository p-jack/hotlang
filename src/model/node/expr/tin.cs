public class Tin:HeadMacro {

  public readonly string name;
  public readonly IList<Expr> elements;
  public readonly Blur blur;

  public Tin(Place place, string name, List<Expr> elements, Blur blur) : base(place) {
    this.name = name;
    this.elements = set(elements);
    this.blur = set(blur);
  }

  protected override Expr? expand(Verifier v) {
    var type = getType(v);
    if (type == null) return null;
    var n = ancestor<Function>()!.nextLocal;
    var r = $"r{n}_";
    var stmts = new List<Stmt>();
    var declare = input($"let {r} = {type.strct.fullName}{{}};{blur}").mustStmt;
    stmts.Add(declare);
    foreach (var x in elements) {
      var stmt = input($"{r}.add({x})").mustStmt;
      stmts.Add(stmt);
    }
    var give = input($"---> {r}").mustStmt;
    stmts.Add(give);
    var block = new Block(place, false, stmts);
    return new Take(place, block);
  }

  types.TinType? getType(Verifier v) {
    var conf = ancestor<Program>()!.conf;
    blur.verify(v);
    failed = blur.failed;
    foreach (var x in elements) {
      x.reposition(Position.RIGHT);
      if (expectedElement != null) {
        x.expect(expectedElement);
      }
      x.verify(v);
      failed = failed || x.failed;
    }
    if (failed) return null;
    var n = name == "" ? "list" : name;
    var element = elementType;
    var key = new use.Tin.Key(name, element, null, new List<use.Tin.Pair>());
    var type = use.Tin.handle(v, this, blur, key);
    if (type == null) {
      return null;
    }
    if (expected != null && !type.focus.Equals(expected.focus)) {
      v.report(this, $"Differing focus: expected {expected.focus} but you specified {type.focus}.");
      return null;
    }
    return type;
  }

  Type elementType { get {
    if (expectedElement != null) return expectedElement;
    var types = elements.Select(x => x.type).ToList();
    return types.widen();
  }}

  Type? expectedElement { get {
    if (expected == null) return null;
    if (!(expected is types.TinType)) return null;
    var a = (types.TinType)expected;
    return a.element;
  }}

  /////

  public override void format(Formatter fmt) {
    fmt.print("#");
    fmt.print(name);
    fmt.print("[");
    for (var i = 0; i < elements.Count(); i++) {
      if (i > 0) fmt.print(", ");
      elements[i].format(fmt);
    }
    fmt.print("]");
    blur.format(fmt);
  }

  static string full() => "#";

}

public partial class In {

  public Tin tin { get {
    var place = skip();
    expect("#", Flavor.OPERATOR);
    var name = "";
    if (isLetter(peek)) name = id();
    expect("[", Flavor.BRACE);
    var elements = new List<Expr>();
    skip();
    while (peek != ']') {
      if (elements.Count() > 0) {
        expect(",", Flavor.OPERATOR);
        skip();
      }
      elements.Add(mustExpr);
      skip();
    }
    expect("]", Flavor.BRACE);
    var blur = this.blur;
    return new Tin(place, name, elements, blur);
  }}

}

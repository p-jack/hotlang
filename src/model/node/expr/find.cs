public class Find:HeadMacro {

  public readonly string name;
  public readonly IList<Actual> actuals;

  public Find(Place place, string name, List<Actual> actuals) : base(place) {
    this.name = name;
    this.actuals = set(actuals);
  }

  /////

  protected override Expr? expand(Verifier v) {
    var actuality = new Actuality(this, name, actuals);
    var matcher = new Matcher(actuality, true);
    var match = matcher.run(v);
    if (match == null) {
      return null;
    }
    var node = match.node;
    if (node is Function) {
      var f = (Function)node;
      if (f.kind == Kind.FUNCTION) {
        return new Call(place, f.fullName, match.actuals.copy());
      }
      throw new Bad("invoke here");
      // var fetch = new Fetch(place, "this");
      // return new Invoke(place, fetch, f.name);
    }
    if (node is Field) {
      var field = (Field)node;
      var fetch = new Fetch(place, "this");
      return new Refer(place, fetch, field.name);
    }
    if (node is Param) {
      var p = (Param)node;
      return new Fetch(place, p.name);
    }
    if (node is Local) {
      var local = (Local)node;
      return new Fetch(place, local.name);
    }
    if (node is Error) {
      var err = (Error)node;
      return new Fetch(place, err.name);
    }
    if (node is Loop) {
      var loop = (Loop)node;
      var actuals = new List<Actual>();
      for (int i = 0; i < match.formals.Count(); i++) {
        var orig = match.actuals[i];
        var a = new Actual(orig.place, match.formals[i].name, orig.expr);
        actuals.Add(a);
      }
      return new Looper(place, loop, actuals);
    }
    throw new Bad($"don't know how to resolve {node.GetType()}");
  }

  public override void format(Formatter fmt) {
    fmt.print(name);
    if (actuals.Count() > 0) {
      fmt.print("(");
      actuals.format(fmt);
      fmt.print(")");
    }
  }
}

public partial class In {

  public Find find { get {
    var place = skip();
    var name = id();
    List<Actual> actuals;
    if (peek == '(') {
      actuals = this.actuals('(', ')');
    } else {
      actuals = new List<Actual>();
    }
    return new Find(place, name, actuals);
  }}

}

internal class DotDefault:Dot.Handler {

  public override string name => "";
  public override bool applies(Dot dot) => true;

  public override Expr? expand(Dot dot, Verifier v) {
    if (dot.block != null) {
      v.report(dot, $".{dot.name} uses the default handler to find a field, method or function. But the default handler does not use a psuedo-block.");
      return null;
    }
    var result = matchField(dot) ?? matchMethod(dot, v) ?? matchOther(dot, v);
    if (result != null) return result;
    v.report(dot, $".{dot.name} did not match any field, method, or function.");
    return null;
  }

  Refer? matchField(Dot dot) {
    if (dot.hasParams) return null;
    var found = Refer.matchField(dot.holder, dot.name);
    // TODO, if it has inaccessible write access, try a set_ instead
    if (found.error == null) {
      return new Refer(dot.place, dot.holderDupe, dot.name);
    }
    return null;
  }

  Invoke? matchMethod(Dot dot, Verifier v) {
    var fm = Invoke.matchMethod(v, dot.holder, dot.name, dot.actuals);
    if (fm.errors.Count() == 0) {
      return new Invoke(dot.place, dot.holderDupe, dot.name, dot.actuals.copy());
    }
    return null;
  }

  Expr? matchOther(Dot dot, Verifier v) {
    var holder = dot.holderDupe;
    var withThis = new List<Actual>();
    var thisa = new Actual(holder.place, "", holder);
    dot.adopt(thisa);
    withThis.Add(thisa);
    foreach (var x in dot.actuals) {
      var c = x.copy();
      dot.adopt(c);
      withThis.Add(c);
    }
    var actuality = new Actuality(dot, dot.name, withThis);
    var matcher = new Matcher(actuality, true);
    var match = matcher.run(v);
    if (match == null) {
      return null;
    }
    if (match.node is Function) {
      return new Call(dot.place, dot.name, withThis.copy());
    } else if (match.node != null) {
      // TODO: Also support loops
      throw new Bad($"Weird match: {match.node.GetType().Name} {match.node.place}");
    }
    return null;
  }

  static void init(Syntax syntax) {}

}

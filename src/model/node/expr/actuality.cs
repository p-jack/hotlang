public class Actuality {

  public readonly Node node;
  public readonly string name;
  public readonly IList<Actual> actuals;

  public Actuality(Node node, string name, IList<Actual> actuals) {
    this.node = node;
    this.name = name;
    this.actuals = actuals;
  }

  internal bool implicitThis { get {
    var f = node.ancestor<Function>();
    if (f == null) return false;
    if (f.paramz.Count() == 0) return false;
    return f.paramz[0].name == "this";
  }}

  internal IList<Actual> withThis { get {
    if (!implicitThis) return actuals;
    var a = new Actual(node.place, thisName, new Fetch(node.place, "this"));
    List<Actual> result = new List<Actual>();
    result.Add(a);
    result.AddRange(actuals);
    return result;
  }}

  internal string thisName { get {
    if (actuals.Count() == 0) return "";
    return actuals[0].name == "" ? "" : "this";
  }}

}

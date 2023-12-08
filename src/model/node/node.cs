using System.Reflection;

// TODO: Dirty flag
public abstract class Node {

  public readonly Node? parent = null;
  internal List<Node> kids_ = new List<Node>();
  public readonly IList<Node> kids;

  public readonly Place place;
  public bool failed = false;

  public virtual Trunk? trunk => null;

  public Node(Place place) {
    this.place = place;
    kids = kids_.AsReadOnly();
  }

  private static Node identity(Node n) {
    return n;
  }

  public virtual Node copy() {
    return copy(identity);
  }

  public virtual Node copy(Func<Node,Node> xlat) {
    var t = this.GetType().GetTypeInfo();
    var c = t.DeclaredConstructors.First();
    var ps = c.GetParameters();
    var values = new List<object?>();
    foreach (var pi in c.GetParameters()) {
      if (pi.Name == null) throw new Bad("no name");
      var f = t.GetField(pi.Name);
      if (f == null) throw new Bad($"no such field: {t.Name}.{pi.Name}");
      var v = f.GetValue(this);
      if (v is Node) v = ((Node)v).copy(xlat);
      if (v is System.Collections.IList) v = copyAll((System.Collections.IList)v, xlat);
      values.Add(v);
    }
    return xlat((Node)c.Invoke(values.ToArray()));
  }

  static object copyAll(System.Collections.IList nodes, Func<Node,Node> xlat) {
    var t = nodes.GetType().GetGenericArguments()[0];
    var result = Mirror.listOf(t);
    foreach (var o in nodes) {
      var c = ((Node)o).copy(xlat);
      Mirror.add(result, c);
    }
    return result;
  }

  public override bool Equals(object? that) {
    if (that == null) return false;
    if (this.GetType() != that.GetType()) return false;
    foreach (var f in majors) {
      var v1 = f.GetValue(this);
      var v2 = f.GetValue(that);
      if (!Equals(v1, v2)) return false;
    }
    return true;
  }

  public override int GetHashCode() {
    var hash = new HashCode();
    foreach (var f in majors) {
      hash.Add(f.GetValue(this));
    }
    return hash.ToHashCode();
  }

  public abstract override string ToString();

  // public virtual Node expanded => this;

  public void reset() {
    failed = false;
    foreach (var p in minors) {
      p.SetValue(this, Mirror.zeroValue(p.PropertyType));
    }
    foreach (var k in kids) {
      k.reset();
    }
  }

  bool sameLine(Place place) {
    if (this.place.line != place.line) return false;
    foreach (var k in kids) {
      if (!k.sameLine(place)) return false;
    }
    return true;
  }

  public bool oneLiner { get {
    return sameLine(place);
  }}

  // reflection is used to simplify implementations

  public T? ancestor<T>() where T:Node {
    Node? node = this;
    while (node != null && node.GetType() != typeof(T)) {
      node = node.parent;
    }
    return (T?)node;
  }

  internal void newParent(Node newParent) {
    if (this.parent != null) {
      this.parent.kids_.Remove(this);
    }
    var t = this.GetType();
    var p = t.GetField("parent");
    p!.SetValue(this, newParent);
    newParent.kids_.Add(this);
    parentChanged();
  }

  protected T set<T>(T value) where T:Node {
    value.newParent(this);
    return value;
  }

  protected T? setNull<T>(T? value) where T:Node {
    if (value != null) value.newParent(this);
    return value;
  }

  protected IList<T> set<T>(List<T> nodes) where T:Node {
    foreach (var n in nodes) n.newParent(this);
    return nodes;
  }

  protected virtual void parentChanged() {}

  public void adopt(Node kid) {
    var t = kid.GetType();
    var p = t.GetField("parent");
    p!.SetValue(kid, this);
  }

  private bool isMajor(FieldInfo f) {
    if (f.DeclaringType == typeof(Node)) return false;
    if (f.IsInitOnly) return true;
    return f.CustomAttributes.Where(x => x.AttributeType == typeof(Major)).Count() > 0;
  }

  protected IEnumerable<FieldInfo> majors {
    get {
      var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
      return fields.Where(f => isMajor(f));
    }
  }

  protected IEnumerable<PropertyInfo> minors {
    get {
      var props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      return props.Where(p => p.DeclaringType != typeof(Node) && p.SetMethod != null);
    }
  }

  // TODO should be unit
  protected Syntax syntax { get {
    return ancestor<Program>()!.syntax;
  }}

  public In input(string text) {
    return new In(syntax, place, text);
  }

}

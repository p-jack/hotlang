public class Set {

  private HashSet<String> set;
  private bool isSnapshot;

  public Set() {
    this.set = new HashSet<String>();
  }

  public int count => set.Count();

  public Set copy() {
    var result = new Set();
    result.set.UnionWith(this.set);
    return result;
  }

  public Set snapshot() {
    if (isSnapshot) return this;
    var result = new Set();
    result.set.UnionWith(this.set);
    result.isSnapshot = true;
    return result;
  }

  public bool add(string value) {
    if (isSnapshot) throw new Bad("set is readonly snapshot");
    if (value == "") throw new Bad("can't store empty string in set");
    return set.Add(value);
  }

  public void add(Set s) {
    set.UnionWith(s.set);
  }

  public bool has(string value) {
    return set.Contains(value);
  }

  public string? prefixed(string prefix) {
    foreach (var s in set) {
      if (s.StartsWith(prefix)) return s;
    }
    return null;
  }

  public static Set of(params string[] elements) {
    Set s = new Set();
    foreach (string e in elements) {
      s.add(e);
    }
    return s;
  }

  public override string ToString() {
    return WTF.str(set);
  }

}

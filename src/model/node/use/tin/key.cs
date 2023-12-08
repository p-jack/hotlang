using ts = types;

namespace use {

public partial class Tin {

public class Key {

  int hashCode;
  public readonly string name;
  public readonly Type type1;
  public readonly Type? type2;
  public readonly List<Pair> paramz;

  public Key(string name, Type type1, Type? type2, List<Pair> paramz) {
    this.name = name;
    this.type1 = type1;
    this.type2 = type2;
    this.paramz = paramz;
    this.hashCode = HashCode.Combine(name, type1, type2, paramz);
  }

  public override bool Equals(object? o) {
    if (o == null) return false;
    if (o.GetType() != typeof(Key)) return false;
    Key k = (Key)o;
    if (hashCode != k.hashCode) return false;
    if (name != k.name) return false;
    if (!type1.Equals(k.type1)) return false;
    if (!paramz.Equals(k.paramz)) return false;
    if (type2 == null && k.type2 == null) return true;
    if (type2 == null || k.type2 == null) return false;
    return type2.Equals(k.type2);
  }

  public override int GetHashCode() {
    return hashCode;
  }

  public override string ToString() {
    var sb = new System.Text.StringBuilder();
    sb.Append("#");
    sb.Append(name);
    sb.Append("[");
    sb.Append(type1);
    if (type2 != null) {
      sb.Append(":");
      sb.Append(type2);
    }
    sb.Append("]");
    Pair.addTo(paramz, sb);
    return sb.ToString();
  }

  public Focus defaults() {
    var n = false;
    var v = true;
    var m = ts.Mutability.MUTABLE;
    var s = ts.Scheme.RC;
    var type = type2 ?? type1;
    if (type.focus.scheme == ts.Scheme.GRAPH) {
      n = true;
      s = ts.Scheme.UNIQUE;
    }
    return new Focus(n, v, m, s);
  }

}

}
}

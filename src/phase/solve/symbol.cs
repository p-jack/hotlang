public class Symbol {

  public static Symbol RETURN = new Symbol("_r");
  public static Symbol THROW = new Symbol("_t");

  internal readonly Part[] parts;
  int hashCode;

  public Symbol(string name) {
    this.parts = new Part[] { new Part(name, 0) };
    this.hashCode = HashCode.Combine(parts);
  }

  public bool root => parts.Length == 1;

  Symbol(Part[] parts) {
    if (parts.Length == 0) throw new Bad();
    this.parts = parts;
    this.hashCode = HashCode.Combine(parts);
  }

  internal Symbol replaceFirst(Part newFirst) {
    var p = new Part[parts.Length];
    p[0] = newFirst;
    for (int i = 1; i < parts.Length; i++) {
      p[i] = parts[i];
    }
    return new Symbol(p);
  }

  public Symbol bump { get {
    var p = new Part[parts.Length];
    for (int i = 0; i < parts.Length - 1; i++) {
      p[i] = parts[i];
    }
    p[parts.Length - 1] = parts[parts.Length - 1].bump();
    return new Symbol(p);
  }}

  public Symbol add(string name) {
    var p = new Part[parts.Length + 1];
    for (int i = 0; i < parts.Length; i++) {
      p[i] = parts[i];
    }
    p[parts.Length] = new Part(name, 0);
    return new Symbol(p);
  }

  public Symbol previous { get {
    var p = new Part[parts.Length - 1];
    for (int i = 0; i < parts.Length - 1; i++) {
      p[i] = parts[i];
    }
    return new Symbol(p);
  }}

  public string first => parts[0].name;
  internal Part firstPart => parts[0];

  public string last => parts[parts.Length - 1].name;

  public override bool Equals(object? other) {
    if (!(other is Symbol)) return false;
    var that = (Symbol)other;
    return this.hashCode == that.hashCode
      && this.parts == that.parts;
  }

  public override int GetHashCode() {
    return hashCode;
  }

  public override string ToString() {
    var sb = new System.Text.StringBuilder();
    foreach (var x in parts) {
      sb.Append(x.name);
      sb.Append("'");
      sb.Append(x.num);
    }
    return sb.ToString();
  }

  internal class Part {
    internal readonly string name;
    internal readonly int num;
    readonly int hashCode;

    public Part(string name, int num) {
      this.name = name;
      this.num = num;
      this.hashCode = HashCode.Combine(name, num);
    }

    public Part bump() {
      return new Part(name, num + 1);
    }

    public override bool Equals(object? o) {
      if (!(o is Part)) return false;
      var that = (Part)o;
      return this.hashCode == that.hashCode
        && this.num == that.num
        && this.name == that.name;
    }

    public override int GetHashCode() {
      return hashCode;
    }

  }

}

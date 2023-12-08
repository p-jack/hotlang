public abstract class Formality:Top {

  public readonly Access access;
  public string name { get; protected set; }

  public string fullName { get; internal set; }
  public bool used = false;
  public int hashCode;

  public List<Formal> formals { get; protected set; }

  public Formality(Place place, Access access) : base(place) {
    this.access = access;
    this.formals = new List<Formal>();
    this.name = "";
    this.fullName = "";
    idhc++;
    this.hashCode = idhc;
  }

  static int idhc = 0;

  public override bool Equals(object? other) {
    if (other == null) return false;
    if (other.GetType() != GetType()) return false;
    return ((Formality)other).hashCode == hashCode;
  }

  public override void format(Formatter fmt) {
    if (fmt.header && access != Access.EXPORT) {
      return;
    }
    format2(fmt);
  }

  protected abstract void format2(Formatter fmt);

  public override int GetHashCode() {
    return hashCode;
  }

  internal abstract IList<string> check(Type formal, Type actual);

  public bool allows(Node n) {
    return Top.allows(access, this, n);
  }

  public void index(bool other, string lastName) {
    var unit = ancestor<Unit>();
    if (unit == null) throw new Bad($"can't index a toplevel with no unit: {GetType().Name} {place}");
    var folder = unit.ancestor<Folder>();
    if (folder == null) throw new Bad("can't index a toplevel with no folder!");
    var program = folder.ancestor<Program>();
    if (program == null) throw new Bad("can't index a toplevel with no program!");
    this.fullName = makeFullName(lastName);
    if (access == Access.PROJECT || access == Access.EXPORT) {
      program.symbols.index(fullName, other, this);
    } else if (access == Access.FOLDER) {
      folder.symbols.index(fullName, other, this);
    } else {
      unit.symbols.index(fullName, other, this);
    }
  }

  private string makeFullName(string lastName) {
    var unit = ancestor<Unit>()!;
    var program = unit.ancestor<Program>()!;
    if (unit.isHeader) return lastName;
    var parts = new List<string>();
    parts.Add(lastName);
    for (var node = parent; node is Top; node = node.parent) {
      if (node is Formality) {
        var top = (node as Formality)!;
        parts.addPart(top.name);
      }
    }

    parts.addPart(unit.name);
    for (var folder = unit.parent; folder is Folder; folder = folder.parent) {
      var f = (folder as Folder)!;
      parts.addPart(f.name);
    }

    var sb = new System.Text.StringBuilder();
    sb.Append(program.conf.project);
    for (int i = parts.Count() - 1; i >= 0; i--) {
      sb.Append("_");
      sb.Append(parts[i]);
    }

    return sb.ToString();
  }

}

public static class Parts {

  public static bool shouldAdd(this List<string> parts, string part) {
    if (part == "") return false;
    if (parts.Count() == 0) return true;
    return !string.Equals(part, parts.Last(), StringComparison.OrdinalIgnoreCase);
  }

  public static void addPart(this List<string> parts, string part) {
    if (!parts.shouldAdd(part)) return;
    parts.Add(part);
  }

}

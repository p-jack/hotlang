/*
public abstract class NamedTop:Top {

  public readonly Access access;
  public abstract string name { get; }

  public string fullName { get; protected set; }
  public bool used;
  public int hashCode;

  public NamedTop(Place place, Access access) : base(place) {
    this.access = access;
    this.fullName = "";
    idhc++;
    this.hashCode = idhc;
  }

  static int idhc = 0;

  public override bool Equals(object? other) {
    if (other == null) return false;
    if (other.GetType() != GetType()) return false;
    return ((NamedTop)other).hashCode == hashCode;
  }

  public override int GetHashCode() {
    return hashCode;
  }

  public bool allowed(Node n) {
    if (access == Access.FILE) {
      var nunit = n.ancestor<Unit>()!;
      var myUnit = this.ancestor<Unit>()!;
      return ReferenceEquals(nunit, myUnit);
    }
    if (access == Access.FOLDER) {
      var nfolder = n.ancestor<Folder>()!;
      var myFolder = n.ancestor<Folder>()!;
      return ReferenceEquals(nfolder, myFolder);
    }
    return true;
  }

  public void index(bool other, string lastName) {
    var unit = ancestor<Unit>();
    if (unit == null) throw new Bad("can't index a toplevel with no unit!");
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
      if (node is NamedTop) {
        var top = (node as NamedTop)!;
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
*/

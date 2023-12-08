public class Symbols {

  public readonly string name;

  public Symbols? previous;
  readonly bool isLocal;
  readonly bool isFolder;
  Roll<string,Node> typeTable;
  Roll<string,Node> otherTable;

  public Symbols(Symbols? previous, string name, bool local, bool folder) {
    this.previous = previous;
    this.name = name;
    this.isLocal = local;
    this.isFolder = folder;
    this.typeTable = new Roll<string,Node>();
    this.otherTable = new Roll<string,Node>();
  }

  public void indexOther(string fullName, Top top) {
    this.index(otherTable, fullName, top);
  }

  public void indexType(string fullName, Top top) {
    this.index(typeTable, fullName, top);
  }

  public void index(string fullName, bool other, Top top) {
    if (other) {
      indexOther(fullName, top);
    } else {
      indexType(fullName, top);
    }
  }

  void index(Roll<string,Node> table, string fullName, Top top) {
    if (fullName == "") throw new Bad();
    if (table.has(fullName)) throw new Bad($"Duplicate full name: {fullName}");
    int end = fullName.Length;
    for (int i = end - 1; i >= 0; i--) {
      if (fullName[i] == '_') {
        table.add(fullName.Substring(i + 1), top);
      }
    }
    table.add(fullName, top);
  }

  public IList<Node> nodes(string name, bool other) {
    if (other) return this.other(name);
    return this.types(name);
  }

  public IList<Node> types(string name) {
    var r = typeTable.get(name);
    if (r.Count > 0) return r;
    return (previous == null) ? new List<Node>().AsReadOnly() : previous.types(name);
  }

  public IList<Node> other(string name) {
    var r = otherTable.get(name);
    if (r.Count > 0) return r;
    return (previous == null) ? new List<Node>().AsReadOnly() : previous.other(name);
  }

  public bool local(string name) {
    if (!isLocal) return false;
    if (!otherTable.has(name)) return false;
    if (previous == null) return false;
    return previous.local(name);
  }

  public void addLocal(string name, Node node) {
    if (!isLocal) throw new Bad();
    otherTable.add(name, node);
  }

}

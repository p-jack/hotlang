// TODO: Verify folder names are identifiers
public class Folder:Bone {

  public readonly DirectoryInfo dir;
  public readonly IList<Folder> folders;
  public readonly IList<Unit> units;
  public readonly bool root;

  public string name => root ? "" : dir.Name;

  private Symbols symbols_;
  public override Symbols symbols => symbols_;

  internal Dictionary<use.Tin.Key,Struct> tins = new Dictionary<use.Tin.Key,Struct>();

  public Folder(DirectoryInfo dir, bool root) {
    this.dir = dir;
    this.root = root;
    this.symbols_ = new Symbols(null, "/" + dir.Name, false, false);
    var folders = new List<Folder>();
    var units = new List<Unit>();
    foreach (var d in dir.EnumerateDirectories()) {
      folders.Add(new Folder(d, false));
    }
    foreach (var f in dir.EnumerateFiles()) {
      if (f.Extension == ".hot") {
        units.Add(new Unit(f, false));
      }
    }
    this.folders = set(folders);
    this.units = set(units);
  }

  protected override void parentChanged() {
    if (parent is Bone) {
      var bone = parent as Bone;
      symbols.previous = bone!.symbols;
    }
  }

  public override string ToString() {
    return $"Folder:{dir.FullName}";
  }

}

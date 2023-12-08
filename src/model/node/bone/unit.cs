// TODO: Verify unit names are identifiers
// TODO: Think through resetting a unit (and reset in general)
public class Unit:Bone {

  public readonly FileInfo file;
  public readonly bool isHeader;
  public readonly IList<Top> tops;

  public string name => Path.GetFileNameWithoutExtension(file.Name);
  public bool used { get; private set; }

  internal Dictionary<use.Tin.Key,Struct> tins = new Dictionary<use.Tin.Key,Struct>();

  public override Symbols symbols => symbols_;
  private Symbols symbols_;

  private Function? _fake = null;
  public Function? fake {
    get => _fake;
    set {
      _fake = this.setNull(value);
    }
  }

  public Unit(FileInfo file, bool isHeader) {
    this.file = file;
    this.used = false;
    this.isHeader = isHeader;
    this.symbols_ = new Symbols(null, file.FullName, false, false);
    var tops = new List<Top>();
    Syntax syntax = Syntax.core();
    // TODO, parse macros here first
    var text = File.ReadAllText(file.FullName);
    var input = new In(syntax, file.FullName, text, isHeader);
    for (var top = input.top; top != null; top = input.top) {
      tops.Add(top);
    }
    this.tops = set(tops);
    input.skip();
    if (!input.eof) throw new Bad($"{input.place}: expected toplevel element");
  }

  protected override void parentChanged() {
    var f = (parent as Folder)!;
    symbols.previous = f.symbols;
  }

  public override string ToString() {
    return $"Unit:{file.FullName}";
  }

  void format(Formatter f) {
    Top? previous = null;
    foreach (var t in tops) {
      if (previous != null && !previous.keepWithNext(t)) {
        f.println("");
      }
      t.format(f);
      previous = t;
    }
  }

  public override void format() {
    using (FileStream fs = File.Open(file.FullName + "-fmt", FileMode.Create)) {
      format(new Formatter(fs, false));
    }
  }

  public override void formatHeader(Formatter fmt) {
    format(fmt);
  }

  // TODO: Expansions can retroactively add things to a unit
  // So we need a safe way to iterate over them with that in mind
  // Right now they exclusively add, and exclusively at the end
  // So just using the index is fine, but we need something safer

  public override void index(Out oot) {
    for (int i = 0; i < tops.Count(); i++) {
      tops[i].index(oot);
    }
  }

  public override void setSupers(Out oot) {
    for (int i = 0; i < tops.Count(); i++) {
      tops[i].setSupers(oot);
    }
  }

  public override void setMembers(Out oot) {
    for (int i = 0; i < tops.Count(); i++) {
      tops[i].setMembers(oot);
    }
  }

  public override void prepare(Out oot) {
    for (int i = 0; i < tops.Count(); i++) {
      tops[i].prepare(oot);
    }
  }

  public override void analyze(Out oot) {
    for (int i = 0; i < tops.Count(); i++) {
      tops[i].analyze(oot);
    }
  }

  public override void recurse(Out oot) {
    for (int i = 0; i < tops.Count(); i++) {
      tops[i].recurse(oot);
    }
  }

  public override void solve(Out oot) {
    for (int i = 0; i < tops.Count(); i++) {
      tops[i].solve(oot);
    }
  }

  public override void emit(LLVM llvm) {
    for (int i = 0; i < tops.Count(); i++) {
      tops[i].emit(llvm);
    }
  }
}

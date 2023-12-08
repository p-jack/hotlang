public class Program:Bone {

  public readonly Conf conf;
  public readonly Syntax syntax;
  public readonly Folder root;

  private readonly Symbols symbols_;
  public override Symbols symbols => symbols_;

  public Program(string confPath, Syntax syntax) {
    this.conf = new Conf(confPath);
    this.syntax = syntax;
    this.symbols_ = new Symbols(null, "*", false, false);
    root = set(new Folder(conf.root, true));
  }

  public override string ToString() {
    return $"Program:{conf.project}";
  }

}

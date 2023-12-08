public class Let:Stmt {

  public readonly IList<Local> locals;

  public Let(Place place, List<Local> locals) : base(place) {
    if (locals.Count() == 0) throw new Bad();
    this.locals = set(locals);
  }

  public Let(Place place, string name, Expr init) : base(place) {
    var nui = new NUI(place, name, null, init);
    var local = new Local(place, nui);
    this.locals = set(new List<Local> { local });
  }

  public override bool big => true;

  public override void verify(Verifier v) {
    foreach (var x in locals) x.verify(v);
  }

  public override void solve(Solva solva) {
    foreach (var x in locals) x.solve(solva);
  }

  public override void emit(LLVM llvm) {
    foreach (var x in locals) x.emit(llvm);
  }

  /////

  public override void format(Formatter fmt) {
    fmt.print("let ");
    var nuis = locals.Select(x => x.nui).ToList();
    nuis.format(fmt);
  }

  static string[] full() => new string[] { "let" };

}

public partial class In {

  public Let? let { get {
    var place = skip();
    expect("let", Flavor.KEYWORD);
    var locals = nuis.Select(x => new Local(x.place, x)).ToList();
    return new Let(place, locals);
  }}

}

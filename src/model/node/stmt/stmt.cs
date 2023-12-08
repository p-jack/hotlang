public abstract class Stmt:Node {

  public virtual bool gives => false;
  public virtual bool terminates => false;
  public virtual bool endsBlock => false;
  public virtual bool comment => false;
  public virtual bool big => false;
  public override Stmt copy() => (Stmt)base.copy();

  public Stmt(Place place) : base(place) {}

  public Block wrap { get {
    return new Block(place, false, new List<Stmt> { this });
  }}

  public override Stmt copy(Func<Node,Node> xlat) => (Stmt)base.copy(xlat);

  public abstract void format(Formatter f);

  public abstract void verify(Verifier v);

  public virtual void solve(Solva s) {}

  public abstract void emit(LLVM llvm);

  public sealed override String ToString() {
    Formatter f = new Formatter(new MemoryStream());
    format(f);
    return f.ToString();
  }

}

public abstract class StmtMacro:Stmt {

  public Stmt? expanded { get; private set; }

  public StmtMacro(Place place) : base(place) {}

  protected abstract Stmt? expand(Verifier v);

  public override bool gives => expanded?.gives ?? false;
  public override bool terminates => expanded?.terminates ?? false;
  public override bool endsBlock => expanded?.endsBlock ?? false;
  public override bool comment => expanded?.comment ?? false;
  public override bool big => expanded?.big ?? false;

  public override sealed void verify(Verifier v) {
    this.expanded = expand(v);
    if (this.expanded == null) {
      failed = true;
      return;
    }
    adopt(expanded);
    expanded.verify(v);
  }

  public override sealed void solve(Solva s) {
    expanded!.solve(s);
  }

  public override sealed void emit(LLVM llvm) {
    expanded!.emit(llvm);
  }

}

public partial class In {

  public Stmt mustStmt { get {
    var r = stmt;
    if (r == null) throw new Bad(place + ": expected statement");
    return r;
  }}

  public Stmt? stmt { get {
    var result = stmt2;
    if (result == null) return null;
    if (result.big) return result;
    if (skipToEOL()) return result;
    if (token() == "if") {
      var place = skip();
      expect("if", Flavor.KEYWORD);
      var condition = mustExpr;
      return new If(place, condition, result.wrap, null);
    }
    return result;
  }}

  Stmt? stmt2 { get {
    var place = skip();
    var parser = this.parser(syntax.stmts);
    if (parser != null) return parser(this, null);
    var left = expr;
    if (left == null) return null;
    var vvv = this.verb;
    if (vvv == null) return new Scrap(place, left, false);
    return new Assign(place, (Verb)vvv, left, mustExpr);
  }}

}

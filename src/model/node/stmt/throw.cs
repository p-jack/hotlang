public class Throw:Stmt {

  public readonly Expr expr;

  public Throw(Place place, Expr expr) : base(place) {
    this.expr = set(expr);
  }

  public override bool terminates => true;

  public override void verify(Verifier v) {
    expr.verify(v);
    if (!(expr.type is types.Error)) {
      v.report(this, $"Can't throw {expr.type}");
      return;
    }
  }

  public override void solve(Solva solva) {
    var l = ZZZ.var(Type.BOOL, Symbol.THROW);
    var r = new zzz.Bool(true);
    solva.assert(new zzz.Eq(l, r));
  }

  public override void emit(LLVM llvm) {
    expr.emit(llvm);
    llvm.raise(expr.pair!);
  }

  public override void format(Formatter fmt) {
    fmt.print("throw ");
    expr.format(fmt);
  }

  static string parserName() => "throwStmt";
  static string[] full() => new string[] { "throw" };

}

public partial class In {

  public Throw? throwStmt { get {
    var place = skip();
    expect("throw", Flavor.KEYWORD);
    return new Throw(place, mustExpr);
  }}

}

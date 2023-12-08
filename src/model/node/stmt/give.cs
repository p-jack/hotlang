public class Give:Stmt {

  public readonly Expr expr;

  internal bool loop;

  public Give(Place place, Expr expr) : base(place) {
    this.expr = set(expr);
  }

  public override bool gives => true;

  public override void verify(Verifier v) {
    expr.verify(v);
    var take = ancestor<Take>();
    if (take != null) {
      if (take.given == null) {
        take.given = expr;
        return;
      } else {
        v.report(this, "Only one ---> statement may exist in a <--- expression, and it must come last.");
      }
    }
    if (ancestor<Loop>() != null) {
      this.loop = true;
      return;
    }
    v.report(this, "A ---> statement must be nested in a <--- expression.");
  }

  public override void emit(LLVM llvm) {
    expr.emit(llvm);
    // nop
  }

  /////

  public override void format(Formatter fmt) {
    fmt.print("---> ");
    expr.format(fmt);
  }

  static string full() => "--->";

}

public partial class In {

  public Give give { get {
    var place = skip();
    expect("--->", Flavor.KEYWORD);
    var expr = mustExpr;
    return new Give(place, expr);
  }}

}

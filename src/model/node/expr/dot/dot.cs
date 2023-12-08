public class Dot:TailMacro {

  public abstract class Handler {
    public abstract string name { get; }
    public abstract bool applies(Dot dot);
    public abstract Expr? expand(Dot dot, Verifier v);
  }

  public readonly string name;
  public readonly bool hasParams;
  public readonly IList<Actual> actuals;
  public readonly Block? block;

  internal Handler? handler;

  public Dot(Place place, Expr holder, string name, bool hasParams, List<Actual> actuals, Block? block) : base(place, holder) {
    this.name = name;
    this.hasParams = hasParams;
    this.actuals = set(actuals);
    this.block = setNull(block);
  }

  internal override Source? source { get {
    var dot = this;
    if (holder.source != null) return holder.source;
    if (holder is Dot) return ((Dot)holder).source;
    return null;
  }}

  protected override Expr? expand(Verifier v) {
    if (holder.failed) return null;
    this.handler = findHandler(v);
    if (handler == null) return null;
    return handler.expand(this, v);
  }

  Handler? findHandler(Verifier v) {
    Handler? result = null;
    var names = new List<string>();
    foreach (var x in syntax.dotHandlers(name)) {
      if (x.applies(this)) {
        names.Add(x.name);
        result = x;
      }
    }
    if (names.Count() > 1) {
      v.report(this, $"Ambiguous dot expression. Valid handlers provided by: {WTF.str(names)}");
      return null;
    }
    if (result != null) return result;
    return DEFAULT;
  }

  static DotDefault DEFAULT = new DotDefault();

  /////

  public override void format(Formatter fmt) {
    holder.format(fmt);
    fmt.print(".");
    fmt.print(name);
    if (hasParams) {
      fmt.print("(");
      actuals.format(fmt);
      fmt.print(")");
    }
    var pd = psuedoDot;
    if (pd != null) {
      fmt.print(":");
      fmt.print(pd.name);
      if (pd.hasParams) {
        fmt.print("(");
        pd.actuals.format(fmt);
        fmt.print(")");
      }
      return;
    }
    var expr = psuedoExpr;
    if (expr != null) {
      fmt.print(":{");
      expr.format(fmt);
      fmt.print("}");
      return;
    }

    if (block == null) return;
    else if (block != null) {
      fmt.print(":");
      block.format(fmt);
    }
  }

  public Expr? psuedoExpr { get {
    if (block == null) return null;
    if (block.stmts.Count() != 1) return null;
    if (!(block.stmts[0] is Scrap)) return null;
    var scrap = (Scrap)block.stmts[0];
    return scrap.expr;
  }}

  Dot? psuedoDot { get {
    var expr = psuedoExpr;
    if (!(expr is Dot)) return null;
    var dot = (Dot)expr;
    if (dot.block != null) return null;
    if (!(dot.holder is Fetch)) return null;
    var fetch = (Fetch)dot.holder;
    return fetch.name == "x" ? dot : null;
  }}

  static string full() => ".";

}

public partial class In {

  public Dot dot(Expr holder) {
    var place = skip();
    expect(".", Flavor.OPERATOR);
    var name = id();
    var hasParams = false;
    List<Actual> actuals = new List<Actual>();
    if (peek == '(') {
      hasParams = true;
      actuals = this.actuals('(', ')');
    }
    return new Dot(place, holder, name, hasParams, actuals, psuedoBlock);
  }

  Block? psuedoBlock { get {
    if (token() == ":{") {
      expect(":", Flavor.BRACE);
      return this.block;
    }
    if (token() != ":") {
      return null;
    }
    expect(":", Flavor.BRACE);
    var expr = mustExpr;
    var x = new Fetch(this.place, "x");
    if (expr is Fetch) {
      var f = (Fetch)expr;
      var d = new Dot(this.place, x, f.name, false, new List<Actual>(), null);
      var scrap = new Scrap(this.place, d, false);
      return scrap.wrap;
    } else if (expr is Call) {
      var c = (Call)expr;
      var d = new Dot(place, x, c.name, true, new List<Actual>(c.actuals), null);
      var scrap = new Scrap(place, d, false);
      return scrap.wrap;
    } else {
      throw new Bad(this.place + ": expected psuedo-block");
    }
  }}

}

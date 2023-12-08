public class Index:TailMacro {

  public abstract class Handler {
    public abstract string name { get; }
    public abstract bool applies(Index index);
    public abstract Expr? expand(Index index, Verifier v);
  }

  public readonly Expr value;

  public Index(Place place, Expr holder, Expr value) : base(place, holder) {
    this.value = set(value);
  }

  internal Handler? handler;

  protected override Expr? expand(Verifier v) {
    if (holder.failed) return null;
    value.verify(v);
    if (value.failed) return null;
    this.handler = findHandler(v);
    if (handler == null) return null;
    return handler.expand(this, v);
  }

  Handler? findHandler(Verifier v) {
    Handler? result = null;
    var names = new List<string>();
    foreach (var x in syntax.indexHandlers) {
      if (x.applies(this)) {
        names.Add(x.name);
        result = x;
      }
    }
    if (names.Count() > 1) {
      v.report(this, $"Ambiguous index expression. Valid handlers provided by: {WTF.str(names)}");
      return null;
    }
    if (result != null) return result;
    // TODO, return default handler
    throw new Bad("no default handler yet");
  }

  bool sourced = false;
  Source? source_ = null;
  internal override Source? source { get {
    if (!sourced) {
      source_ = Source.get(this);
      sourced = true;
    }
    return source_;
  }}

  /////

  public override void format(Formatter fmt) {
    holder.format(fmt);
    fmt.print("[");
    value.format(fmt);
    fmt.print("]");
  }

  static string partial() => "[";

}

public partial class In {

  public Index index(Expr holder) {
    var place = skip();
    expect("[", Flavor.BRACE);
    var value = mustExpr;
    expect("]", Flavor.BRACE);
    return new Index(place, holder, value);
  }

}

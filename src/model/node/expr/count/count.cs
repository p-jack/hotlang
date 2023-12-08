public class Count:TailMacro {

  public abstract class Handler {
    public abstract string name { get; }
    public abstract bool applies(Count count);
    public abstract Expr? expand(Count count, Verifier v);
  }

  public Count(Place place, Expr holder) : base(place, holder) {}

  internal Handler? handler;

  protected override Expr? expand(Verifier v) {
    if (holder.failed) return null;
    this.handler = findHandler(v);
    if (handler == null) return null;
    return handler.expand(this, v);
  }

  Handler? findHandler(Verifier v) {
    Handler? result = null;
    var names = new List<string>();
    foreach (var x in syntax.countHandlers) {
      if (x.applies(this)) {
        names.Add(x.name);
        result = x;
      }
    }
    if (names.Count() > 1) {
      v.report(this, $"Ambiguous # expression. Valid handlers provided by: {WTF.str(names)}");
      return null;
    }
    if (result != null) return result;
    // TODO, return default handler
    throw new Bad("no default handler yet");
  }

  /////

  public override void format(Formatter fmt) {
    holder.format(fmt);
    fmt.print("#");
  }

  static string partial() => "#";

}

public partial class In {

  public Count count(Expr holder) {
    var place = skip();
    expect("#", Flavor.OPERATOR);
    return new Count(place, holder);
  }

}

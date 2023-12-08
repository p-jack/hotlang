public abstract class Sink:Dot.Handler {

//  public override string name => "hotc";

  public override bool applies(Dot dot) {
    if (dot.holder.type is types.Loop) {
      var l = (types.Loop)dot.holder.type;
      return applies(l.type);
    }
    if (dot.holder.type is types.Array) {
      return true;
    }
    return false;
  }

  protected abstract bool applies(Type subtype);

  public override Expr? expand(Dot dot, Verifier v) {
    var source = dot.source;
    if (source == null) {
      v.report(dot, "Can only apply to a chain." + dot);
      return null;
    }
    return expand(source, dot, v);
  }

  protected abstract Expr? expand(Source source, Dot dot, Verifier v);

}

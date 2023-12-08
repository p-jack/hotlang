public abstract class Chain:Dot.Handler {

  public override bool applies(Dot dot) {
    if (dot.holder.type is types.Loop) {
      var l = (types.Loop)dot.holder.type;
      return applies(l.type);
    }
    return false;
  }

  internal Type subtype(Dot dot) => ((types.Loop)dot.holder.type).type;

  protected abstract bool applies(Type subtype);

  public override Expr? expand(Dot dot, Verifier v) {
    var source = dot.source;
    if (source == null) {
      v.report(dot, "Can only apply to a chain.");
      return null;
    }
    var type = expand(source, dot, v);
    if (type == null) return null;
    return new Chainer(dot.place, new types.Loop(type));
  }

  protected abstract Type? expand(Source source, Dot dot, Verifier v);

}

internal class Chainer:Head {

  public Type type_;

  public Chainer(Place place, Type type) : base(place) {
    this.type_ = type;
  }

  protected override Type resolve(Verifier v) {
    return type_;
  }

  internal override ZZZ mk(Solva solva) {
    return new zzz.Bool(true);
  }

  protected override Pair toPair(LLVM llvm) {
    throw new Bad();
  }

  public override void format(Formatter fmt) {
    fmt.print($"loop<{type_}>@{place}");
  }

}

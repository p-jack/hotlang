public class Looper:Head {

  public Loop loop;
  public IList<Actual> actuals;

  public Looper(Place place, Loop loop, IList<Actual> actuals) : base(place) {
    this.loop = loop; // we do NOT want to change its parent
    this.actuals = actuals; // ditto
  }

  Source? source_ = null;
  internal override Source? source { get {
    if (source_ == null) {
      source_ = loop.newSource(place, actuals);
    }
    return source_;
  }}

  protected override Type resolve(Verifier v) {
    loop.analyze(v.oot);
    if (loop.failed) return Fail.FAIL;
    if (loop.type is Fail) throw new Bad();
    return new types.Loop(loop.type);
  }

  internal override ZZZ mk(Solva solva) {
    throw new Bad();
  }

  protected override Pair toPair(LLVM llvm) {
    throw new Bad();
  }

  public override void format(Formatter fmt) {
    throw new Bad();
  }

}

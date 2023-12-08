public abstract class Use:Node {

  enum State { UNVERIFIED, VERIFYING, VERIFIED }

  public readonly Blur blur;
  public Type? type { get; private set; }

  public abstract bool primitive { get; }
  State state = State.UNVERIFIED;

  public Use(Place place, Blur blur) : base(place) {
    this.blur = set(blur);
  }

  public override Use copy() => (Use)base.copy();

  public void verify(Verifier v) {
    if (state == State.VERIFIED) return;
    if (state == State.VERIFYING) throw new Bad("define loop");
    state = State.VERIFYING;
    if (primitive && !blur.validForPrimitive) {
      v.report(this, "Invalid lens for primitive type.");
    }
    blur.verify(v);
    type = resolve(v);
    state = State.VERIFIED;
  }

  protected abstract Type resolve(Verifier v);

}

public partial class In {

  public Use mustUse { get {
    var use = this.use;
    if (use == null) throw new Bad(place + ": Expected type.");
    return use;
  }}

  public Use? use { get {
    var p = parser(syntax.uses);
    if (p != null) return p(this, null);
    return this.named;
  }}

}

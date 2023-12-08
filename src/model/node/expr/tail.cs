public abstract class Tail:Expr {

  public readonly Expr holder;

  public Tail(Place place, Expr holder) : base(place) {
    this.holder = set(holder);
  }

  public Expr holderDupe { get {
    var r = new Dupe(holder);
    adopt(r);
    return r;
  }}

}

public abstract class TailMacro:Tail {

  public Expr? expanded { get; private set; }

  public TailMacro(Place place, Expr holder) : base(place, holder) {}

  protected abstract Expr? expand(Verifier v);

  /////

  public override bool lefty => expanded?.lefty ?? false;
  internal override bool fresh => expanded?.fresh ?? false;
  public override bool ambiguous => expanded?.ambiguous ?? false;
  public override bool singleTerm => expanded?.singleTerm ?? false;
  internal override Source? source => expanded?.source ?? null;

  internal override void expect(Type? type) {
    base.expect(type);
    expanded?.expect(type);
  }

  internal override void reposition(Position p) {
    base.reposition(p);
    expanded?.reposition(p);
  }

  internal override void assigningTo(Pair leftPair) {
    base.assigningTo(leftPair);
    expanded?.assigningTo(leftPair);
  }

  /////

  protected override sealed Type resolve(Verifier v) {
    holder.verify(v);
    this.expanded = expand(v);
    if (this.expanded == null) {
      failed = true;
      return Fail.FAIL;
    }
    adopt(expanded);
    expanded.verify(v);
    expanded.expect(this.expected);
    expanded.reposition(this.position);
    expanded.assigningTo(this.leftPair);
    return expanded.type;
  }

  internal override ZZZ mk(Solva solva) {
    return expanded!.zzz(solva);
  }

  protected override Pair toPair(LLVM llvm) {
    expanded!.emit(llvm);
    return expanded!.pair;
  }

  internal override void emitAssign(LLVM llvm, Expr right) {
    expanded!.emitAssign(llvm, right);
  }

  internal override void nullOut(LLVM llvm) {
    expanded!.nullOut(llvm);
  }

}

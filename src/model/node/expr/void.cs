using Microsoft.Z3;

public class Void:Head {

  public Void(Place place) : base(place) {}

  protected override Type resolve(Verifier v) {
    return Type.VOID;
  }

  internal override ZZZ mk(Solva solva) {
    throw new Bad("TODO");
  }

  protected override Pair toPair(LLVM llvm) {
    return new Pair("?", LLVM.VOID);
  }

  /////

  public override void format(Formatter fmt) {
    // nop
  }

}

public class Take:Head {

  public readonly Block block;
  internal Expr? given = null;

  public Take(Place place, Block block) : base(place) {
    this.block = set(block);
  }

  protected override Type resolve(Verifier v) {
    block.verify(v);
    if (!block.gives) {
      v.report(this, "A <--- expression must end with a ---> statement.");
      return Fail.FAIL;
    }
    return given!.type;
  }

  internal override ZZZ mk(Solva solva) {
    block.solve(solva);
    return given!.zzz(solva);
  }

  protected override Pair toPair(LLVM llvm) {
    block.emit(llvm);
    return given!.pair;
  }

  /////

  public override void format(Formatter fmt) {
    fmt.print("<--- ");
    block.format(fmt);
  }

  static string full() => "<---";

}

public partial class In {

  public Take take { get {
    // TODO, why were we doing a opaqueNewLine here?
    var place = skip();
    expect("<---", Flavor.KEYWORD);
    //var old = opaqueNewline;
    //opaqueNewline = false;
    var block = this.block;
    // opaqueNewline = old
    return new Take(place, block);
  }}

}

/*


parse {
  let place = in.skip()
  in.expect("<-- ", KEYWORD)
  let old = in.opaqueNewline
  in.opaqueNewline = false
  let block = in.block()
  in.opaqueNewline = old
  return Take_new(place, block)
}

/format {
  fmt.print("<-- ")
  .block.format(fmt)
}

*/

// TODO: Forcing the last statement to be a give means we can't do (eg):
//
// <--- {
//   if flag {
//     ---> 1
//   } else {
//     ---> 0
//   }
// }
//
// One can evade that with extra locals, but that seems silly to require.

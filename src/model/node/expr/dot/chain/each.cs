public class Each:Sink {

  public override string name => "each";

  protected override bool applies(Type type) {
    return true;
  }

  protected override Expr? expand(Source source, Dot dot, Verifier v) {
    if (dot.block == null) {
      v.report(dot, ".each requires a block.");
      return null;
    }
    var renamer = new Renamer();
    renamer.add("x", source.x);
    // TODO, support "i" as well
    var block = dot.block.copy(renamer.xlat);
    source.block.add(block);
    return source.toTake(new Void(dot.place));
//    return new Void(dot.place);
  }

  static void init(Syntax syntax) {
    syntax.add(new Each());
  }
}

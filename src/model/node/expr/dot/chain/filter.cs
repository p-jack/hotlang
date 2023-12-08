public class Filter:Chain {

  static void init(Syntax syntax) {
    syntax.add(new Filter());
  }

  public override string name => "filter";

  protected override bool applies(Type subtype) => true;

  protected override Type? expand(Source source, Dot dot, Verifier v) {
    var expr = dot.psuedoExpr;
    if (expr == null) {
      v.report(dot, ".filter requires a condition in its psuedo-block.");
      return null;
    }
    var renamer = new Renamer();
    renamer.add("x", source.x);
    expr = expr.copy(renamer.xlat);
    var newBlock = new Block(dot.place, false, new List<Stmt>());
    var ifStmt = new If(dot.place, expr, newBlock, null);
    source.block.add(ifStmt);
    source.block = newBlock;
    return subtype(dot);
  }

}

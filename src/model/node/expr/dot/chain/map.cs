public class Map:Chain {

  static void init(Syntax syntax) {
    syntax.add(new Map());
  }

  public override string name => "map";

  protected override bool applies(Type subtype) => true;

  protected override Type? expand(Source source, Dot dot, Verifier v) {
    var expr = dot.psuedoExpr;
    if (expr == null) {
      v.report(dot, ".filter requires a condition in its psuedo-block.");
      return null;
    }
    var f = dot.ancestor<Function>()!;
    var newX = "x" + f.nextLocal + "_";
    var renamer = new Renamer();
    renamer.add("x", source.x);
    expr = expr.copy(renamer.xlat);
    var let = dot.input($"let {newX} = {expr}").mustStmt;
    var newBlock = new Block(dot.place, false, new List<Stmt> { let });
    source.block.add(newBlock);
    source.block = newBlock;
    source.x = newX;
    return subtype(dot);
  }

}

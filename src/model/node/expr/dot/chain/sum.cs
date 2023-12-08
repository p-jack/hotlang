// TODO do float I guess
public class Sum:Sink {

  public override string name => "sum";

  protected override bool applies(Type type) {
    return type is types.Int;
  }

  protected override Expr? expand(Source source, Dot dot, Verifier v) {
    var place = dot.place;
    var f = dot.ancestor<Function>()!;
    var name = $"sum{f.nextLocal}_";
    var let = new Let(place, name, new Number(place, 0));
    source.setup.Add(let);

    var stmt = dot.input($"{name} = {name} + {source.x}").mustStmt;
    source.block.add(stmt);

    var give = new Give(place, new Fetch(place, name));
    var r = source.toTake(new Fetch(place, name));
    return r;
  }

  static void init(Syntax syntax) {
    syntax.add(new Sum());
  }
}

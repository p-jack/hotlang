public class Source {

  public readonly Place place;
  public readonly List<Stmt> setup;
  public readonly Expr yield;

  public readonly While whileStmt;
  public Block block;
  public string x;

  public Source(Place place, List<Stmt> setup, Expr condition, Expr yield, List<Stmt> advance) {
    this.place = place;
    this.setup = setup;
    this.yield = yield;
    this.x = "x";

    this.block = new Block(place, false, new List<Stmt>());
    var wstmts = new List<Stmt>();
    wstmts.Add(yieldStmt);
    wstmts.Add(block);
    foreach (var x in advance) {
      wstmts.Add(x.copy());
    }
    this.whileStmt = new While(place, condition.copy(), new Block(place, false, wstmts));
  }

  Let yieldStmt { get {
    var nui = new NUI(place, this.x, null, yield.copy());
    var local = new Local(place, nui);
    return new Let(place, new List<Local> { local });
  }}

  public Block toBlock(Give? give) {
    var stmts = new List<Stmt>();
    foreach (var x in setup) {
      stmts.Add(x.copy());
    }
    stmts.Add(whileStmt);
    if (give != null) {
      stmts.Add(give);
    }
    return new Block(place, false, stmts);
  }

  public Take toTake(Expr expr) {
    var give = new Give(place, expr);
    var block = toBlock(give);
    return new Take(place, block);
  }

  public static Source? get(Expr n) {
    var place = n.place;
    if (n.type is types.Array) {
      var atype = n.type as types.Array;
      var f = n.ancestor<Function>()!;
      var i = $"i{f.nextLocal}_";
      var a = $"a{f.nextLocal}_";
      var setup = new List<Stmt> {
        In.getStmt(place, $"let {i} = 0"),
        In.getStmt(place, $"let {a} = {n}"),
      };
      var cond = In.getExpr(place, $"{i} < {a}#");
      var yield = In.getExpr(place, $"{a}[ {i} ]");
      var advance = new List<Stmt> { In.getStmt(place, $"{i} = {i} + 1") };
      return new Source(place, setup, cond, yield, advance);
    }
    if (n.type is types.TinType) {
      // TODO TinType
    }
    return null;
  }

}

public class While:Stmt {

  public readonly Expr condition;
  public readonly Block block;

  string begin = "";
  string body = "";
  string end = "";

  public While(Place place, Expr condition, Block block) : base(place) {
    this.condition = set(condition);
    this.block = set(block);
  }

  public override void verify(Verifier v) {
    condition.verify(v);
    block.verify(v);
  }

  public override void emit(LLVM llvm) {
    var num = llvm.nextLabel();
    begin = $"Loop-{num}";
    body = $"Loop-Body-{num}";
    end = $"Look-Done-{num}";
    llvm.br(begin);
    llvm.setLabel(begin);
    condition.emit(llvm);
    llvm.br(condition.pair, body, end);
    llvm.setLabel(body);
    block.emit(llvm);
    llvm.br(begin);
    llvm.setLabel(end);
  }

  /////

  public override void format(Formatter fmt) {
    fmt.print("while ");
    condition.format(fmt);
    fmt.print(" ");
    block.format(fmt);
  }

  static string parserName() => "whileStmt";
  static string full() => "while";

}

public partial class In {

  public While? whileStmt { get {
    var place = skip();
    if (token() != "while") {
      return null;
    }
    expect("while", Flavor.KEYWORD);
    var condition = mustExpr;
    var block = this.block;
    return new While(place, condition, block);
  }}

}

/*

parse {
  let place = in.skip
  in.expect("while", KEYWORD)
  let condition = in.expr
  let block = in.block
  return While_new(place, condition, block)
}



*/

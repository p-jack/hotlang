/*
public class Const:Formality {

  public readonly Expr expr;
  Trunk? _trunk = null;

  public Const(Place place, Access access, string name, Expr expr) : base(place, access, name) {
    this.expr = set(expr);
  }

  public override Trunk? trunk => _trunk;

  protected override void index2(Out oot) {
    index(true, name);
  }

  protected override void analyze2(Out oot) {
    expr.setConstant();
    var unit = ancestor<Unit>()!;
    var v = new Verifier(oot, unit.symbols);
    expr.verify(v);
    var type = expr.type!;
    var name = $"@{fullName}";
    Pair? castFrom = type.aggregate ? new Pair(name, new llvm.Direct($"%{fullName}.tc").star) : null;
    this._trunk = Trunk.forConstant(name, type, castFrom);
  }

  public override void emit(LLVM llvm) {
    expr.leftPair = new Pair(fullName, expr.type!.llvm);
    expr.emit(llvm);
    if (expr.type.aggregate) return;
    llvm.constants.println($"@{fullName} = constant {expr.pair}");
  }

  /////

  public override bool keepWithNext(Top next) {
    if (!(next is Const)) return false;
    var that = (Const)next;
    return this.expr.oneLiner && that.expr.oneLiner;
  }

  public override string ToString() => $"{access}{name} const";

  public override void format(Formatter fmt) {
    fmt.print($"{access}const {name} = ");
    expr.format(fmt);
  }

  static string parserName() => "constTop";
  static string full() => "const";

}

public partial class In {

  public Const constTop() {
    var place = skip();
    var access = this.access;
    var name = id();
    expect("const", Flavor.KEYWORD);
    expect("=", Flavor.OPERATOR);
    var expr = mustExpr;
    return new Const(place, access, name, expr);
  }

}

*/

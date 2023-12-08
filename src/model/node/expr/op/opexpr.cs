internal abstract class OpExpr:Expr {

  public OpExpr(Place place) : base(place) {}

  public Infix infix => (Infix)parent!;
  public Expr left => infix.left;
  public Expr right => infix.right;
  public Type leftType => left.type!;
  public Type rightType => right.type!;
  public types.Int leftInt => (types.Int)leftType;
  public types.Int rightInt => (types.Int)rightType;

  public bool fp => leftType is types.Float;
  public bool signed { get {
    if (leftType is types.Int) return leftInt.signed;
    if (leftType is types.Float) return true;
    return false;
  }}

  public override void format(Formatter fmt) {
    throw new Bad();
  }

}

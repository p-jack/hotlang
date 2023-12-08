public class Op {

  public abstract class Handler {
    public abstract string name { get; }
    public abstract Expr? resolve(Verifier v, Expr left, Expr right);
  }

  // TODO, spread out these values
  public const int MULTIPLICATIVE = 1;
  public const int ADDITIVE = 2;
  public const int SHIFT = 3;
  public const int COMPARISON = 4;
  public const int EQUALITY = 5;
  public const int BITWISEAND = 6;
  public const int BITWISEOR = 7;
  public const int LOGICALAND = 8;
  public const int LOGICALOR = 9;
  public const int ASSIGNMENT = 10;

  public readonly string symbol;
  public readonly int priority;
  public readonly bool assign;

  public static Op EQUALS = new Op("==", EQUALITY, false);

  public Op(string symbol, int priority, bool assign) {
    this.symbol = symbol;
    this.priority = priority;
    this.assign = assign;
  }

  public override bool Equals(object? other) {
    if (other == null) return false;
    if (other.GetType() != typeof(Op)) return false;
    var that = (Op)other;
    return this.assign == that.assign
      && this.priority == that.priority
      && this.symbol == that.symbol;
  }

  public override int GetHashCode() {
    return HashCode.Combine(assign, priority, symbol);
  }

  public override string ToString() {
    return symbol;
  }
}

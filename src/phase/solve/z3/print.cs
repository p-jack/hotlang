using Microsoft.Z3;

// TODO, include data types too
public static class Z3Print {

  public static string human(this Microsoft.Z3.Expr expr) {
    if (expr.IsTrue) return "true";
    if (expr.IsFalse) return "false";
    if (expr.IsNot) return $"!({expr.Arg(0).human()})";
    if (expr.IsImplies) return $"{expr.Arg(0).human()} --> {expr.Arg(1).human()}";
    if (expr.IsBVUMinus) return fromNeg((BitVecExpr)expr);
    if (expr is BitVecNum) return fromInt((BitVecNum)expr);
    if (expr.IsConst) return expr.ToString();
    if (expr is BoolExpr) return fromBool((BoolExpr)expr);
//    throw new Bad($"{expr.GetType().Name}");
    return expr.ToString();
  }

  static string fromInt(BitVecNum n) {
    return n.ToString();
//    return $"{n.Int64}";
  }

  static string fromNeg(BitVecExpr neg) {
    return $"-{neg.Arg(0)}";
  }

  static string fromBool(BoolExpr expr) {
    var op = Z3Print.op(expr);
    if (op != "") {
      var left = expr.Arg(0);
      var right = expr.Arg(1);
      return $"{left.human()} {op} {right.human()}";
    }
    throw new Bad("Unknown Z3.BoolExpr: " + expr);
  }

  static string op(Microsoft.Z3.Expr expr) {
    if (expr.IsEq) return "==";
    if (expr.IsBVSLT) return "<";
    if (expr.IsBVSLE) return "<=";
    if (expr.IsBVSGT) return ">";
    if (expr.IsBVSGE) return ">=";
    if (expr.IsBVULT) return "<";
    if (expr.IsBVULE) return "<=";
    if (expr.IsBVUGT) return ">";
    if (expr.IsBVUGE) return ">=";
    if (expr.IsAnd) return "&&";
    if (expr.IsOr) return "||";
    // TODO float
    return "";
  }

}

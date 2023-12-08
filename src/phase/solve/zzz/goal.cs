using Microsoft.Z3;

namespace zzz {

internal class Goal:BoolZZZ {

  public readonly ZZZ head;
  List<BoolZZZ> list;

  public Goal(ZZZ head, List<BoolZZZ> list) {
    if (list.Count() == 0) throw new Bad();
    this.head = head;
    this.list = list;
  }

  public override BoolExpr z3(Context ctx) {
    BoolExpr? result = null;
    foreach (var x in list) {
      if (result == null) {
        result = x.z3(ctx);
      } else {
        result = ctx.MkAnd(result, x.z3(ctx));
      }
    }
    return result!;
  }

  public override BoolZZZ? not => null;

  public override BoolZZZ rewriteBool(Rewriter rw) {
    var head = this.head.rewrite(rw);
    var list = this.list.Select(x => x.rewriteBool(rw)).ToList();
    return new Goal(head, list);
  }

  public override string ToString() {
    var sb = new System.Text.StringBuilder();
    var first = true;
    foreach (var x in list) {
      if (first) {
        first = false;
      } else {
        sb.Append(" && ");
      }
      sb.Append("(");
      sb.Append(x.ToString());
      sb.Append(")");
    }
    return sb.ToString();
  }

}

}

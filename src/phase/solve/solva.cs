using Microsoft.Z3;

public class Solva {

  Out oot;
  Solver solver;
  public bool logicFailed;
  public Context ctx;
  internal readonly Ledger ledger = new Ledger();

  internal List<BoolZZZ> body = new List<BoolZZZ>();
  internal List<BoolZZZ> implications = new List<BoolZZZ>();
  Dictionary<string,int> prefixes = new Dictionary<string,int>();

  public Solva(Out oot) {
    this.oot = oot;
    this.ctx = new Context(new Dictionary<string, string>() { { "model", "true" } });
    this.solver = ctx.MkSolver();
  }

  public void report(Node n, string error) {
    logicFailed = true;
    oot.report(n, error);
  }

  bool satisfiable { get {
    var result = solver.Check();
    if (result == Microsoft.Z3.Status.UNKNOWN) {
      throw new Bad($"Unknown z3 result: {solver.ReasonUnknown}");
    }
    return result == Microsoft.Z3.Status.SATISFIABLE;
  }}

  public Solved solve(Expr condition) {
    return solve(condition.zzz(this));
  }

  internal Solved solve(ZZZ zzz) {
    if (logicFailed) return Solved.EITHER;
    var z3cond = (BoolExpr)zzz.z3(ctx);
    solver.Push();
    solver.Assert(z3cond);
    if (!satisfiable) {
      solver.Pop();
      return Solved.FALSE;
    }
    solver.Pop();
    solver.Push();
    solver.Assert(ctx.MkNot(z3cond));
    if (!satisfiable) {
      solver.Pop();
      return Solved.TRUE;
    }
    solver.Pop();
    return Solved.EITHER;
  }

  internal bool exists(ZZZ possiblyNull) {
    var ne = new zzz.Ne(possiblyNull, ZZZ.NULL);
    return solve(ne) == Solved.TRUE;
  }

  internal void imply(Expr condition) {
    imply((BoolZZZ)condition.zzz(this));
  }

  internal void implyNot(Expr condition) {
    imply(condition.not(this));
  }

  internal void imply(BoolZZZ condition) {
    if (logicFailed) return;
    solver.Push();
    BoolExpr z3cond = (BoolExpr)condition.z3(ctx);
    solver.Assert(z3cond);
    implications.Add(condition);
  }

  internal void beMoreDirect() {
    if (logicFailed) return;
    implications.RemoveAt(implications.Count() - 1);
    solver.Pop();
  }

  internal void assert(BoolZZZ fact) {
    // TODO, for most asserts we want to ignore the
    // prior implication because it's a priori asserted itself
    for (var i = implications.Count() - 1; i >= 0; i--) {
      fact = new zzz.Imply(implications[i], fact);
    }
    var z3fact = (BoolExpr)fact.z3(ctx);
    solver.Assert(z3fact);
    body.Add(fact);
  }

  internal void assertNotNull(Symbol symbol) {
    var current = ledger.current(symbol);
    var left = ZZZ.var(Type.UINT32, current);
    var ne = new zzz.Ne(left, ZZZ.NULL);
    assert(ne);
  }

  public override string ToString() {
    var sb = new System.Text.StringBuilder();
    foreach (var x in body) {
      sb.Append(x.ToString());
      sb.Append("\n");
    }
    return sb.ToString();
  }

}

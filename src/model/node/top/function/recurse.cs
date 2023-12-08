public partial class Function {

  class Comparer:IComparer<Function> {
    public int Compare(Function? f1, Function? f2) {
      var n1 = f1!.callers.Count();
      var n2 = f2!.callers.Count();
      if (n1 > n2) return 1;
      if (n1 < n2) return -1;
      return 0;
    }
  }

  class Result {
    public bool recurses;
    public bool terminates;
    public Result(bool recurses, bool terminates) {
      this.recurses = recurses;
      this.terminates = terminates;
    }
  }

  protected override void recurse2(Out oot) {
    var set = new SortedSet<Function>(new Comparer());
    var result = recurse(oot, set);
    if (result.recurses && !result.terminates) {
      oot.report(this, "Infinite recursion.");
    }
    if (set.Count() > 0) {
      set.Min()!.terminus = true;
    }
  }

  Result recurse(Out oot, SortedSet<Function> set) {
    phase = Phase.RECURSED;
    if (set.Contains(this)) {
      return new Result(true, false);
    }
    set.Add(this);
    foreach (var call in callers) {
      SortedSet<Function> newSet = new SortedSet<Function>(set, new Comparer());
      var result = call.ancestor<Function>()!.recurse(oot, newSet);
      if (result.recurses) {
        if (result.terminates) {
          return result;
        }
        return new Result(true, mightTerminateBefore(oot, call));
      }
    }
    return new Result(false, false);
  }

  bool mightTerminateBefore(Out oot, Call call) {
    var block = call.function.block;
    var mtb = new MTB();
    checkTermination(mtb, block, call);
    return mtb.terminated;
  }

  class MTB {
    public bool encounteredCall;
    public bool terminated;
  }

  static void checkTermination(MTB result, Node node, Call call) {
    if (result.encounteredCall || result.terminated) return;
    if (node is Stmt) {
      var stmt = (Stmt)node;
      if (stmt.terminates) {
        var ifStmt = stmt.ancestor<If>();
        if (ifStmt != null) {
          result.terminated = true;
          return;
        }
      }
    }
    if (ReferenceEquals(node, call)) {
      result.encounteredCall = true;
      return;
    }
    foreach (var k in node.kids) {
      checkTermination(result, k, call);
    }
  }

}

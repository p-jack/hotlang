public class Infix:Expr {

  public readonly Op op;
  [Major] public Expr left;
  [Major] public Expr right;

  Op.Handler? handler = null;
  Expr? resolved = null;

  public Infix(Place place, Expr left, Op op, Expr right) : base(place) {
    this.left = set(left);
    this.op = op;
    this.right = set(right);
  }

  public Infix merge(Place place, Op op, Expr right) {
    var left = this;
    if (right is Infix) throw new Bad("right is infix?");
    if (left.op.priority < op.priority) {
      return new Infix(place, left, op, right);
    }
    if (left.right is Infix) {
      var iright = (Infix)left.right;
      left.right = iright.merge(place, op, right);
      return left;
    }
    left.right = new Infix(place, left.right, op, right);
    return left;
  }

  public override bool ambiguous => left.ambiguous && right.ambiguous;
  public override bool singleTerm => false;

  protected override Type resolve(Verifier v) {
    if (left.ambiguous && !right.ambiguous) {
      right.verify(v);
      left.expect(right.type);
      left.verify(v);
    } else if (!left.ambiguous && right.ambiguous) {
      left.verify(v);
      right.expect(left.type);
      right.verify(v);
    } else {
      left.verify(v);
      right.verify(v);
    }
    if (left.failed || right.failed) return Fail.FAIL;
    Expr? result = null;
    Op.Handler? handler = null;
    var handlers = syntax.handlers(op);
    var names = new List<string>();
    foreach (var x in handlers) {
      var resolved = x.resolve(v, left, right);
      if (resolved != null) {
        result = resolved;
        handler = x;
        names.Add(x.name);
      }
    }
    if (names.Count() == 0) {
      v.report(this, $"No handler for {left.type} {op} {right.type}.");
      return Fail.FAIL;
    }
    if (names.Count() > 1) {
      v.report(this, $"More than one registered handler for {left.type} {op} {right.type}. Providers: {WTF.str(names)}");
      return Fail.FAIL;
    }
    this.handler = handler!;
    this.resolved = result!;
    adopt(result!);
    result!.verify(v);
    return result!.type;
  }

  internal override ZZZ mk(Solva solva) => resolved!.zzz(solva);

  protected override Pair toPair(LLVM llvm) {
    left.emit(llvm);
    right.emit(llvm);
    resolved!.emit(llvm);
    return resolved.pair!;
  }

  /////

  public override void format(Formatter fmt) {
    left.format(fmt);
    fmt.print($" {op} ");
    right.format(fmt);
  }

  // parsing handled by Expr.expr

}

public class Assign:Stmt {

  public readonly Verb verb;
  public readonly Expr left;
  public readonly Expr right;

  public Assign(Place place, Verb verb, Expr left, Expr right) : base(place) {
    this.verb = verb;
    this.left = set(left);
    this.right = set(right);
  }

  /////

  public override void verify(Verifier v) {
    left.reposition(Position.LEFT);
    left.verify(v);
    right.reposition(Position.RIGHT);
    right.expect(left.type!);
    right.verify(v);
    if (!left.lefty) {
      v.report(this, "Invalid lvalue.");
      return;
    }
    if (left.failed || right.failed) return;
    var ltype = left.type!;
    var rtype = right.type!;
    if (ltype is Unknown || ltype is types.Null) {
      retroactivelySetType(v);
      return;
    }
    foreach (var x in rtype.assignTo(ltype)) {
      v.report(this, x);
    }
  }

  void retroactivelySetType(Verifier v) {
    var trunk = left.trunk!;
    var type = right.type!;
    if (!type.focus.variable) {
      type = type.vary(true);
    }
    trunk.type = type;
  }

  /////

  public override void solve(Solva solva) {
    // TODO, embeds probably work different?
    // ...
    var lz = (zzz.Var)left.zzz(solva);
    if (left.type!.aggregate) {
      right.rightSymbol = lz.symbol;
      var rz = (zzz.Var)right.zzz(solva);
      if (rz.symbol != lz.symbol) {
        var bumped = solva.ledger.bump(lz.symbol);
        solva.ledger.alias(bumped, rz.symbol);
      }
    } else {
      var rz = right.zzz(solva);
      var bumped = solva.ledger.bump(lz.symbol);
      var lv = ZZZ.var(lz.type, bumped);
      var eq = new zzz.Eq(lv, rz);
      solva.assert(eq);
    }
  }

  /////

  public override void emit(LLVM llvm) {
    var left = this.left;
    var right = this.right;
    left.emit(llvm);
    right.assigningTo(left.pair);
    // right.expect(left.type); // TODO huh?
    right.emit(llvm);
    left.emitAssign(llvm, right);
  }

  /////

  public override void format(Formatter fmt) {
    left.format(fmt);
    fmt.print(" ");
    fmt.print(verb.human());
    fmt.print(" ");
    right.format(fmt);
  }

}

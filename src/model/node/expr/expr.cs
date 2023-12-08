public enum Position {
  UNKNOWN, LEFT, RIGHT, CONSTANT
}

public abstract class Expr:Node {

  public Type type { get; private set; }
  public Pair pair { get; internal set; }
  public Pair leftPair { get; private set; }
  public Type? expected { get; private set; }

  public Position position { get; private set; }

  public Expr(Place place) : base(place) {
    this.type = Fail.FAIL;
    this.pair = new Pair("?", new llvm.Direct("?"));
    this.leftPair = new Pair("?", new llvm.Direct("?"));
  }

  public override Expr copy() => (Expr)base.copy();
  public override Expr copy(Func<Node,Node> xlat) => (Expr)base.copy(xlat);

  public virtual bool lefty => false;
  internal virtual bool fresh => false;
  public bool onLeft => position == Position.LEFT;
  internal Symbol? rightSymbol = null;

  public virtual bool ambiguous => false;
  public virtual bool singleTerm => true;
  internal virtual Source? source => null;

  internal virtual void expect(Type? type) {
    if (type == null) {
      expected = null;
    } else if (type.real) {
      expected = type;
    } else {
      expected = null;
    }
  }

  internal virtual void assigningTo(Pair leftPair) {
    this.leftPair = leftPair;
  }

  internal virtual void reposition(Position p) {
    this.position = p;
  }

  public abstract void format(Formatter f);

  public sealed override String ToString() {
    Formatter f = new Formatter(new MemoryStream());
    format(f);
    return f.ToString();
  }

  public void verify(Verifier v) {
    this.type = resolve(v);
    if (type is Fail) {
      failed = true;
    }
  }

  ZZZ? cached;

  internal ZZZ zzz(Solva solva) {
    if (cached == null) cached = mk(solva);
    return cached;
  }

  internal abstract ZZZ mk(Solva solva);

  internal BoolZZZ not(Solva solva) {
    var ez = (BoolZZZ)zzz(solva);
    var n = ez.not;
    return n == null ? new zzz.Not(ez) : n;
  }

  protected abstract Type resolve(Verifier v);

  public void emit(LLVM llvm) {
    llvm.println($"; {GetType().Name} {place}");
    this.pair = toPair(llvm);
  }

  internal virtual void emitAssign(LLVM llvm, Expr right) {
    throw new Bad($"{GetType()} emitAssign {place}");
  }

  internal virtual void nullOut(LLVM llvm) {
    // nop by default for things like New and Call
  }

  protected abstract Pair toPair(LLVM llvm);

  public void unemit() {
    this.pair = new Pair("?", new llvm.Direct("?"));
  }

}

public partial class In {

  public Expr? expr { get {
    var term = this.term();
    if (term == null) return null;
    return infix(term);
  }}

  public Expr mustExpr { get {
    var expr = this.expr;
    if (expr == null) throw new Bad($"{place}: Expected expression.");
    return infix(expr);
  }}

  Expr? term() {
    var place = skip();
    var parser = this.parser(syntax.heads);
    if (parser != null) return tail(parser(this, null)!);
    if (!isLetter(peek)) return null;
    mark();
    var name = id();
    if (peek == '{') {
      recall();
      return tail(newExpr);
    }
    recall();
    return tail(find);
  }

  Expr tail(Expr left) {
    if (skipToEOL()) return left;
    var place = skip();
    var parser = this.parser(syntax.tails);
    if (parser == null) return left;
    return tail(parser(this, left)!);
  }

  Expr infix(Expr left) {
    if (skipToEOL()) return left;
    if (peek == '\n') return left;
    var place = skip();
    var key = token();
    if (!syntax.ops.ContainsKey(key)) return left;
    var op = syntax.ops[key];
    expect(key, Flavor.OPERATOR);
    var right = term();
    if (right == null) {
      throw new Bad($"{place}: Expected right-hand expression.");
    }
    if (left is Infix) {
      var infix = (Infix)left;
      var result2 = infix.merge(place, op, right);
      return this.infix(result2);
    }
    var result = new Infix(place, left, op, right);
    return infix(result);
  }

}

/*

+class Expr/Node {

  +.iterator(:) Iterator:rn { return null }

  \setConstant2(:m)

  .prepareConstant(:mr, llvm LLVM:m) {
    this+>emit(llvm)
  }

  +setConstant(Node:pmn) {
    return if node is null
    return if !node/?Expr
    node/!Expr+>setConstant2()
  }

  +all_setConstant([Node]:pmn) {
    return if nodes is null
    nodes.each:{ x.setConstant() }
  }


*/

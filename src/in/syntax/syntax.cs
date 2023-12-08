using System.Reflection;

public partial class Syntax {

  private static Assembly assembly;
  private static Syntax coreSyntax;

  public readonly Level<Top> rawTops = new Level<Top>("raw tops");
  public readonly Level<Top> namedTops = new Level<Top>("named tops");
  public readonly Level<Use> uses = new Level<Use>("uses");
  public readonly Level<Stmt> stmts = new Level<Stmt>("statements");
  public readonly Level<Head> heads = new Level<Head>("head expressions");
  public readonly Level<Tail> tails = new Level<Tail>("tail expressions");
  public readonly Dictionary<string,Op> ops = new Dictionary<string,Op>();
  public readonly Roll<string,Op.Handler> opHandlers = new Roll<string,Op.Handler>();
  public readonly Roll<string,Dot.Handler> _dotHandlers = new Roll<string,Dot.Handler>();
  public readonly List<Index.Handler> indexHandlers = new List<Index.Handler>();
  public readonly List<Count.Handler> countHandlers = new List<Count.Handler>();
  internal readonly Dictionary<string,use.Tin.Handler> tinHandlers = new Dictionary<string,use.Tin.Handler>();

  private Syntax() {}

  public Syntax(Syntax orig) {
    this.uses = orig.uses.copy();
    this.stmts = orig.stmts.copy();
    this.rawTops = orig.rawTops.copy();
    this.namedTops = orig.namedTops.copy();
    this.heads = orig.heads.copy();
    this.tails = orig.tails.copy();
    this.ops = new Dictionary<string,Op>(orig.ops);
    this.opHandlers = orig.opHandlers.copy();
    this._dotHandlers = orig._dotHandlers.copy();
    this.indexHandlers = new List<Index.Handler>(orig.indexHandlers);
    this.countHandlers = new List<Count.Handler>(orig.countHandlers);
    this.tinHandlers = new Dictionary<string,use.Tin.Handler>(orig.tinHandlers);
    this._dotHandlers = orig._dotHandlers.copy();
  }

  public static Syntax core() => new Syntax(coreSyntax);

  static Syntax() {
    Console.WriteLine("Creating base syntax.");
    assembly = typeof(Syntax).Assembly;
    coreSyntax = new Syntax();
    coreSyntax.add(new Op("==", Op.EQUALITY, false));
    coreSyntax.add(new Op("!=", Op.EQUALITY, false));
    coreSyntax.add(new Op("is", Op.EQUALITY, false));
    coreSyntax.add(new Op("not", Op.EQUALITY, false));
    coreSyntax.add(new Op("<", Op.COMPARISON, false));
    coreSyntax.add(new Op("<=", Op.COMPARISON, false));
    coreSyntax.add(new Op(">", Op.COMPARISON, false));
    coreSyntax.add(new Op(">=", Op.COMPARISON, false));
    coreSyntax.add(new Op("+", Op.ADDITIVE, true));
    coreSyntax.add(new Op("-", Op.ADDITIVE, true));
    coreSyntax.add(new Op("*", Op.MULTIPLICATIVE, true));
    coreSyntax.add(new Op("/", Op.MULTIPLICATIVE, true));
    coreSyntax.add(new Op("%", Op.MULTIPLICATIVE, true));
    coreSyntax.scanTops();
    coreSyntax.scan<Use>(coreSyntax.uses);
    coreSyntax.scan<Stmt>(coreSyntax.stmts);
    coreSyntax.scan<Head>(coreSyntax.heads);
    coreSyntax.scan<Tail>(coreSyntax.tails);
    coreSyntax.init<Op.Handler>();
    coreSyntax.init<Dot.Handler>();
    coreSyntax.init<Index.Handler>();
    coreSyntax.init<Count.Handler>();
    coreSyntax.init<use.Tin.Handler>();
  }

  void scan<T>(Level<T> level) where T:Node {
    foreach (var t in assembly.subclasses(typeof(T))) {
      scan(level, t);
    }
  }

  void scanTops() {
    rawTops.add(new string[]{"main"}, new string[]{}, (In i, Expr? e) => i.function);
    foreach (var t in assembly.subclasses(typeof(Top))) {
      var level = t.IsSubclassOf(typeof(Formality)) ? namedTops : rawTops;
      scan(level, t);
    }
  }

  static void scan<T>(Level<T> level, System.Type t) where T:Node {
    var ti = t.GetTypeInfo();
    var full = ti.full();
    var partial = ti.partial();
    if (full.Length + partial.Length == 0) {
      Console.WriteLine($"...not adding {typeof(T).Name}:{ti.Name} to core Syntax.");
      return;
    }
    var parser = Syntax.parser<T>(ti);
    level.add(full, partial, parser);
  }

  static Func<In,Expr?,T?> parser<T>(TypeInfo ti) {
    var p = typeof(In).GetProperty(ti.parserName(), BindingFlags.Public | BindingFlags.Instance);
    if (p != null) return (In i, Expr? e) => (T?)(p.GetValue(i));
    var m = typeof(In).GetMethod(ti.parserName(), BindingFlags.Public | BindingFlags.Instance);
    if (m == null) throw new Bad($"No parser for {ti.parserName()}!");
    return (In i, Expr? e) => (T?)m.Invoke(i, new object?[] { e });
  }

  /////

  public void add(Op op) {
    if (ops.ContainsKey(op.symbol)) throw new Bad($"Duplicate op: {op.symbol}");
    ops[op.symbol] = op;
  }

  public void add(string symbol, Op.Handler handler) {
    if (!ops.ContainsKey(symbol)) throw new Bad($"No such op: {symbol}");
    opHandlers.add(symbol, handler);
  }

  public IList<Op.Handler> handlers(Op op) {
    return opHandlers.get(op.symbol);
  }

  /////

  void init<T>() {
    foreach (var t in assembly.subclasses(typeof(T))) {
      var ti = t.GetTypeInfo();
      var m = ti.GetMethod("init", BindingFlags.NonPublic | BindingFlags.Static)!;
      m.Invoke(null, new object?[] { this });
    }
  }

  /////

  public void add(Dot.Handler dh) {
    _dotHandlers.add(dh.name, dh);
  }

  public IList<Dot.Handler> dotHandlers(string name) {
    return _dotHandlers.get(name);
  }

  /////

  public void add(use.Tin.Handler ch) {
    if (tinHandlers.ContainsKey(ch.name)) {
      var existing = tinHandlers[ch.name].origin;
      throw new Bad($"Duplicate handler for #{ch.name}, existing one is from {existing} and new one is from {ch.origin}.");
    }
    tinHandlers[ch.name] = ch;
  }

  public use.Tin.Handler? tinHandler(string name) {
    if (tinHandlers.ContainsKey(name)) {
      return tinHandlers[name];
    }
    return null;
  }

}

static class Mirrors {

  public static string[] full(this TypeInfo t) {
    var m = t.GetMethod("full", BindingFlags.NonPublic | BindingFlags.Static);
    if (m == null) return new string[]{};
    var r = m.Invoke(null, null)!;
    if (r is string) return new string[] { (string)r };
    return (string[])r;
  }

  public static string[] partial(this TypeInfo t) {
    var m = t.GetMethod("partial", BindingFlags.NonPublic | BindingFlags.Static);
    if (m == null) return new string[]{};
    var r = m.Invoke(null, null)!;
    if (r is string) return new string[] { (string)r };
    return (string[])r;
  }

  public static string parserName(this TypeInfo t) {
    var m = t.GetMethod("parserName", BindingFlags.NonPublic | BindingFlags.Static);
    if (m == null) return t.Name.ToLower();
    var r = m.Invoke(null, null)!;
    return (string)r;
  }

  public static IEnumerable<System.Type> subclasses(this Assembly assembly, System.Type baseType) {
    return assembly.GetTypes().Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract);
  }

}

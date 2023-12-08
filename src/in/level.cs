public class Level<T> where T:Node {

  public Tokens tokens { get; }
  Dictionary<string,Func<In,Expr?,T?>> parsers { get; }

  public Level(string category) {
    this.tokens = new Tokens(category);
    this.parsers = new Dictionary<string,Func<In,Expr?,T?>>();
  }

  Level(Level<T> orig) {
    this.tokens = orig.tokens.copy();
    this.parsers = new Dictionary<string,Func<In,Expr?,T?>>(orig.parsers);
  }

  public Level<T> copy() {
    return new Level<T>(this);
  }

  public void add(string[] full, string[] partial, Func<In,Expr?,T?> parse) {
    foreach (var f in full) {
      tokens.addFull(f);
      parsers.Add(f, parse);
    }
    foreach (var p in partial) {
      tokens.addPartial(p);
      parsers.Add(p, parse);
    }
  }

  public Func<In,Expr?,T?>? get(string token) {
    if (!parsers.ContainsKey(token)) {
      return null;
    }
    return parsers[token];
  }

}

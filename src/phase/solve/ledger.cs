internal class Ledger {

  public int nv;
  Facts root = new Facts(new Symbol("_root_"), true);
//  Dictionary<string,Facts> dict = new Dictionary<string,Facts>();

  public Symbol newVar { get {
    nv++;
    return new Symbol($"${nv}");
  }}

  public Symbol current(string name) {
    return root.child(name).current;
  }

  Facts facts(Symbol symbol) {
    var result = root;
    foreach (var x in symbol.parts) {
      result = result.child(x.name);
    }
    return result;
  }

  public Symbol current(Symbol symbol) {
    return this.facts(symbol).current;
  }

  public Symbol current(Symbol symbol, string token) {
    var facts = this.facts(symbol);
    return facts.child(token).current;
  }

  public void alias(Symbol left, Symbol right) {
    var lf = left.root ? root : facts(left.previous);
    var rf = facts(right);
    lf.replace(left.last, rf);
  }

  public Symbol bump(Symbol symbol) {
    var facts = this.facts(symbol);
    facts.current = facts.current.bump;
    return facts.current;
  }

}

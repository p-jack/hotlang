using Microsoft.Z3;

internal class Facts {

  internal Symbol current;
  Dictionary<string,Facts> dict = new Dictionary<string,Facts>();
  bool root;

  public Facts(Symbol symbol, bool root) {
    this.current = symbol;
    this.root = root;
  }

  public void replace(string token, Facts facts) {
    dict[token] = facts;
  }

  public Facts child(string token) {
    var n = root ? new Symbol(token) : current.add(token);
    if (!dict.ContainsKey(token)) {
      dict[token] = new Facts(n, false);
    }
    return dict[token];
  }

}

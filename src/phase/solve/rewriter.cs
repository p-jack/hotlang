using Microsoft.Z3;

internal class Rewriter {

  Ledger ledger;
  public Symbol retVar;
  Dictionary<Symbol.Part,Symbol.Part> varMap;

  public Rewriter(Ledger ledger, Dictionary<Symbol.Part,Symbol.Part> varMap, Symbol retVar) {
    this.ledger = ledger;
    this.retVar = retVar;
    this.varMap = varMap;
  }

  public Symbol replace(Symbol s) {
    var name = s.firstPart;
    if (varMap.ContainsKey(name)) {
      return s.replaceFirst(varMap[name]);
    }
    if (s == Symbol.RETURN) {
      return retVar;
    }
    if (s == Symbol.THROW) {
      return s;
    }
    var ns = ledger.newVar;
    var p = ns.firstPart;
    varMap[p] = p;
    return ns;
  }

}

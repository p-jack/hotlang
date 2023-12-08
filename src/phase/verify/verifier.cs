public class Verifier {

  public readonly Out oot;
  public Symbols symbols { get; private set; }

  public Verifier(Out oot, Symbols symbols) {
    this.oot = oot;
    this.symbols = symbols;
  }

  public void report(Node n, string error) {
    oot.report(n, error);
  }

  public void push() {
    symbols = new Symbols(symbols, "local", true, false);
  }

  public void pop() {
    symbols = symbols.previous!;
  }

}

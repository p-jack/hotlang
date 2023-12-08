public class Issue {

  public readonly Node source;
  public readonly string message;

  public Issue(Node source, string message) {
    this.source = source;
    this.message = message;
  }

  public override string ToString() {
    return $"{source.place}: {message}";
  }
}

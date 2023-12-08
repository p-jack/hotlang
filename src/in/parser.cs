public class Parser<T> where T:Node {

  public Set full { get; }
  public Set partial { get; }
  public Func<In,T> parser { get; }

  public Parser(Set full, Set partial, Func<In,T> parser) {
    this.full = full.snapshot();
    this.partial = partial.snapshot();
    this.parser = parser;
  }

}

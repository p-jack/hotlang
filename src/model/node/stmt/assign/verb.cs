public enum Verb {

  ASSIGN, MOVE, SWAP

}

public static class Verbs {

  public static string human(this Verb verb) {
    switch (verb) {
      case Verb.ASSIGN: return "=";
      case Verb.MOVE: return "<-";
      case Verb.SWAP: return "<->";
      default: throw new Bad("unknown verb");
    }
  }

}

public partial class In {

  public Verb? verb { get {
    var token = this.token();
    switch (token) {
      case "=":
        expect("=", Flavor.OPERATOR);
        return Verb.ASSIGN;
      case "<-":
        expect("<-", Flavor.OPERATOR);
        return Verb.MOVE;
      case "<->":
        expect("<->", Flavor.OPERATOR);
        return Verb.SWAP;
      default: return null;
    }
  }}

}

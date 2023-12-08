namespace types {

  public enum Action { PASS, ASSIGN, RETURN, CONSTRUCT }
  public enum Mutability { MUTABLE, IMMUTABLE, LOCKED, WHICHEVER }
  public enum Scheme {
    BRAND_NEW,
    DATA,
    ELEMENT,
    GRAPH,
    JAILED,
    PRIMITIVE,
    RC,
    STACK,
    UNIQUE,
    VALUE,
    WEAK
  }

}

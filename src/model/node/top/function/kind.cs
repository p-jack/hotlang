public enum Kind {
  FUNCTION, METHOD, OVERRIDE, ABSTRACT
}

public partial class In {

  public Kind kind { get {
    char ch = peek;
    if (ch == '.') {
      expect(".", Flavor.IDENTIFIER);
      return Kind.METHOD;
    }
    if (ch == '/') {
      expect("/", Flavor.IDENTIFIER);
      return Kind.OVERRIDE;
    }
    if (ch == '\\') {
      expect("\\", Flavor.IDENTIFIER);
      return Kind.ABSTRACT;
    }
    return Kind.FUNCTION;
  }}

}

public static class Kinds {

  public static void format(this Kind kind, Formatter f) {
    switch (kind) {
      case Kind.METHOD: f.print("."); break;
      case Kind.OVERRIDE: f.print("/"); break;
      case Kind.ABSTRACT: f.print("\\"); break;
    }
  }

}

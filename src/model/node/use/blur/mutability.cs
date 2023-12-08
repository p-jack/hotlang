using static Mutability;

public static class Mutabilities {

  public static string ToString2(this Mutability m) {
    switch (m) {
      case MUTABLE: return "m";
      case IMMUTABLE: return "i";
      case LOCKED: return "l";
      case WHICHEVER: return "z";
      default: return "";
    }
  }

  public static Mutability mutability(this char ch) {
    switch (ch) {
      case 'm': return MUTABLE;
      case 'i': return IMMUTABLE;
      case 'l': return LOCKED;
      case 'z': return WHICHEVER;
      default: return UNKNOWN;
    }
  }

  public static bool has(char ch) {
    return ch.mutability() != UNKNOWN;
  }

  public static types.Mutability focus(this Mutability m, types.Mutability def) {
    switch (m) {
      case MUTABLE: return types.Mutability.MUTABLE;
      case IMMUTABLE: return types.Mutability.IMMUTABLE;
      case LOCKED: return types.Mutability.LOCKED;
      case WHICHEVER: return types.Mutability.WHICHEVER;
      default: return def;
    }
  }

}

public partial class In {

  public Mutability mutability {
    get {
      var ch = this.peek;
      var result = ch.mutability();
      if (result != UNKNOWN) {
        expect(ch, Flavor.BLUR);
      }
      return result;
    }
  }

}

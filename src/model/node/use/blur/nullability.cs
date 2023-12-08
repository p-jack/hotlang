using static Nullability;

static class Nullabilities {

  public static string ToString2(this Nullability n) {
    switch (n) {
      case NEVER_NULL: return "N";
      case NULL_ALLOWED: return "n";
      default: return "";
    }
  }

  public static Nullability nullability(this char ch) {
    switch (ch) {
      case 'n': return NULL_ALLOWED;
      case 'N': return NEVER_NULL;
      default: return UNKNOWN;
    }
  }

  public static bool has(char ch) {
    return ch.nullability() != UNKNOWN;
  }

  public static bool focus(this Nullability n, bool def) {
    switch (n) {
      case NEVER_NULL: return false;
      case NULL_ALLOWED: return true;
      default: return def;
    }
  }

}

public partial class In {

  public Nullability nullability {
    get {
      var ch = this.peek;
      var result = ch.nullability();
      if (result != UNKNOWN) {
        expect(ch, Flavor.BLUR);
      }
      return result;
    }
  }

}

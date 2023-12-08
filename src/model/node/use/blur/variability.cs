using static Variability;

static class Variabilities {

  public static string ToString2(this Variability v) {
    switch (v) {
      case FINAL: return "f";
      case VARIABLE: return "a";
      default: return "";
    }
  }

  public static Variability variability(this char ch) {
    switch (ch) {
      case 'f': return FINAL;
      case 'a': return VARIABLE;
      default: return UNKNOWN;
    }
  }

  public static bool has(char ch) {
    return ch.variability() != UNKNOWN;
  }

  public static bool focus(this Variability v, bool def) {
    switch (v) {
      case FINAL: return false;
      case VARIABLE: return true;
      default: return def;
    }
  }

}

public partial class In {

  public Variability variability {
    get {
      var ch = this.peek;
      var result = ch.variability();
      if (result != UNKNOWN) {
        expect(ch, Flavor.BLUR);
      }
      return result;
    }
  }

}

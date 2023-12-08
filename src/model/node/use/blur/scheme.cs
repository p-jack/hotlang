using static Scheme;

static class Schemes {

  public static string ToString2(this Scheme s) {
    switch (s) {
      case BRAND_NEW: return "b";
      case DATA: return "d";
      case GRAPH: return "g";
      case JAILED: return "j";
      case PRIMITIVE: return "p";
      case RC: return "r";
      case STACK: return "s";
      case UNIQUE: return "u";
      case VALUE: return "v";
      case WEAK: return "w";
      default: return ""; // PRIMITIVE, UNKNOWN
    }
  }

  public static Scheme scheme(this char ch) {
    switch (ch) {
      case 'b': return BRAND_NEW;
      case 'd': return DATA;
      case 'e': return ELEMENT;
      case 'g': return GRAPH;
      case 'j': return JAILED;
      case 'p': return PRIMITIVE;
      case 'r': return RC;
      case 's': return STACK;
      case 'u': return UNIQUE;
      case 'v': return VALUE;
      case 'w': return WEAK;
      default: return UNKNOWN;
    }
  }

  public static types.Scheme focus(this Scheme s, types.Scheme def) {
    switch (s) {
      case STACK: return types.Scheme.STACK;
      case JAILED: return types.Scheme.JAILED;
      case DATA: return types.Scheme.DATA;
      case VALUE: return types.Scheme.VALUE;
      case BRAND_NEW: return types.Scheme.BRAND_NEW;
      case GRAPH: return types.Scheme.GRAPH;
      case RC: return types.Scheme.RC;
      case UNIQUE: return types.Scheme.UNIQUE;
      case PRIMITIVE: return types.Scheme.PRIMITIVE;
      case WEAK: return types.Scheme.WEAK;
      default: return def;
    }
  }

  public static bool has(char ch) {
    return ch.scheme() != UNKNOWN;
  }
}

public partial class In {

  public Scheme scheme {
    get {
      var ch = this.peek;
      var result = ch.scheme();
      if (result != UNKNOWN) {
        expect(ch, Flavor.BLUR);
      }
      return result;
    }
  }

}

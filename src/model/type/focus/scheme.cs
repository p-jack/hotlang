using static types.Scheme;

namespace types { static class Schemes {

  public static bool needsDestroy(this Scheme s) {
    switch (s) {
      case RC: return true;
      case UNIQUE: return true;
      case GRAPH: return true;
      default: return false;
    }
  }

  public static bool needsDestroy2(Scheme s) {
    return s.needsDestroy();
  }

  public static bool pointer(this Scheme s) {
    // TODO, what about BRAND_NEW here?
    return s != PRIMITIVE && s != VALUE;
  }

  public static bool canBeField(this Scheme s) {
    return s != BRAND_NEW && s != STACK;
  }

  public static string? assignTo(this Scheme actual, Scheme formal) {
    var action = Action.ASSIGN;
    if (formal == WEAK && actual == UNIQUE) return null;
    if (formal == ELEMENT && actual == WEAK) return null;
    if (actual == VALUE) return badActual(action, actual);
    if (formal == BRAND_NEW) return badFormal(action, formal);
    if (actual == JAILED) return "Can't assign a jailed reference."; // TODO, yes you can, to an auto's field
    if (formal == VALUE && actual != BRAND_NEW) return needsCopy(action, actual);
    if (actual == BRAND_NEW) return null; // TODO, this ain't right
    if (formal != actual) return mismatch(action, formal, actual);
    return null;
  }

  public static string? passTo(this Scheme actual, Scheme formal) {
    // TODO: support embedded here
    var action = Action.PASS;
    if (formal == STACK || formal == BRAND_NEW || formal == VALUE) {
      return badFormal(action, formal);
    }
    if (formal == JAILED) return null;
    if (actual == BRAND_NEW) return null;
    if (formal != actual) return mismatch(action, formal, actual);
    return null;
  }

  public static string? returnTo(this Scheme actual, Scheme formal) {
    var action = Action.RETURN;
    if (formal == WEAK && actual == UNIQUE) return null;
    if (formal == ELEMENT && actual == WEAK) return null;
    if (formal == BRAND_NEW || formal == STACK || formal == VALUE) {
      return badFormal(action, formal);
    }
    if (actual == VALUE) return needsCopy(action, actual);
  	if (actual == JAILED) return "Can't return a jailed pointer.";
  	if (actual == BRAND_NEW || formal == BRAND_NEW) return null;
    if (formal != actual) return mismatch(action, formal, actual);
  	return null;
  }

  public static string? constructTo(this Scheme actual, Scheme formal) {
    var action = Action.CONSTRUCT;
    if (formal == WEAK && actual == UNIQUE) return null;
    if (formal == ELEMENT && actual == WEAK) return null;
    if (formal == BRAND_NEW) return badFormal(action, formal);
    if (actual == JAILED) return "Can't assign a jailed pointer.";
    if (actual == STACK && formal == STACK) return needsCopy(action, actual);
    if (formal == VALUE && actual != VALUE && actual != BRAND_NEW) {
      return needsCopy(action, actual);
    }
    if (actual == BRAND_NEW) return null;
    if (formal != actual) return mismatch(action, formal, actual);
    return null;
  }

  static string badActual(Action action, Scheme actual) {
    return $"Can't use the {actual} scheme as an rvalue for {action}.";
  }

  static string badFormal(Action action, Scheme formal) {
    return $"Can't use the {formal} scheme as an lvalue for ${action}.";
  }

  static string needsCopy(Action action, Scheme actual) {
    return $"Can't {action} an rvalue with scheme {actual}, you need to copy it.";
  }

  static string mismatch(Action action, Scheme formal, Scheme actual) {
    return $"Can't {action} the {actual} scheme to the ${formal} scheme.";
  }

}}

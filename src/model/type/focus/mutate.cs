using static types.Mutability;

namespace types { static class Mutabilitys {

  public static string? passTo(this Mutability actual, Mutability formal) {
    var action = Action.PASS;
    if (actual == WHICHEVER) return null;
    if (formal == IMMUTABLE && actual == MUTABLE) {
      return mismatch(action, formal, actual);
    }
    if (formal == IMMUTABLE && actual == LOCKED) {
      return mismatch(action, formal, actual);
    }
    if (formal == MUTABLE && actual == IMMUTABLE) {
      return mismatch(action, formal, actual);
    }
    if (formal == MUTABLE && actual == LOCKED) {
      return mismatch(action, formal, actual);
    }
    return null;
  }

  public static string? assignTo(this Mutability actual, Mutability formal) {
    var action = Action.ASSIGN;
    if (actual == WHICHEVER) return null;
    if (formal == IMMUTABLE && actual == MUTABLE) return mismatch(action, formal, actual);
    if (formal == IMMUTABLE && actual == LOCKED) return mismatch(action, formal, actual);
    if (formal == MUTABLE && actual == IMMUTABLE) return mismatch(action, formal, actual);
    if (formal == MUTABLE && actual == LOCKED) return mismatch(action, formal, actual);
    return null;
  }

  public static string? returnTo(this Mutability actual, Mutability formal) {
    var action = Action.RETURN;
    if (actual == WHICHEVER) return mismatch(action, formal, actual);
    if (formal == IMMUTABLE && actual == MUTABLE) return mismatch(action, formal, actual);
    if (formal == IMMUTABLE && actual == LOCKED)  return mismatch(action, formal, actual);
    if (formal == MUTABLE && actual == IMMUTABLE) return mismatch(action, formal, actual);
    if (formal == MUTABLE && actual == LOCKED) return mismatch(action, formal, actual);
    return null;
  }

  public static string? constructTo(this Mutability actual, Mutability formal) {
    var action = Action.CONSTRUCT;
    if (actual == WHICHEVER) return null;
    if (formal == IMMUTABLE) return null; // TODO, wrong, only allow if also brand_new
    if (formal == MUTABLE && actual == IMMUTABLE) return mismatch(action, formal, actual);
    if (formal == MUTABLE && actual == LOCKED) return mismatch(action, formal, actual);
    return null;
  }

  static string mismatch(Action action, Mutability formal, Mutability actual) {
    return $"Can't {action} the {actual} scheme to the ${formal} scheme.";
  }

}}

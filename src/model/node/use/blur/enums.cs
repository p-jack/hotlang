public enum Scheme {
  UNKNOWN,
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

public enum Mutability {
  UNKNOWN, IMMUTABLE, MUTABLE, LOCKED, WHICHEVER
}

public enum Nullability {
  UNKNOWN, NEVER_NULL, NULL_ALLOWED
}

public enum Variability {
  UNKNOWN, VARIABLE, FINAL
}

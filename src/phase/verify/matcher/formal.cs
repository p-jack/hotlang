public class Formal {

  public Formality source;
  public string name;
  public Type type;
  public Expr? initial;
  public int index = -1;

  public Formal(Formality source, string name, Type type, Expr? initial, int index) {
    this.source = source;
    this.name = name;
    this.type = type;
    this.initial = initial;
    this.index = index;
  }

  public override string ToString() {
    return $"{name} {type}";
  }

}

public static class Formals {

  public static int uninitialized(this IList<Formal> formals) {
    int c = 0;
    foreach (var x in formals) {
      if (x.initial != null) return c;
      c++;
    }
    return c;
    // for (int i = formals.Count() - 1; i >= 0; i--) {
    //   Print.ln(i + ":" +formals[i].ToString());
    //   if (formals[i].initial == null) {
    //     Print.ln("  " + i + ": fc:" + formals.Count() + ", r:" + (formals.Count() - i - 1));
    //     return formals.Count() - i - 1;
    //   }
    // }
    // return 0;
  }

  public static int indexOf(this IList<Formal> formals, string name) {
    for (int i = 0; i < formals.Count(); i++) {
      if (formals[i].name == name) {
        return i;
      }
    }
    return -1;
  }

}

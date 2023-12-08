public enum Access {

  FILE, FOLDER, PROJECT, EXPORT

}

public partial class In {

  public Access access { get {
    var ch = peek;
    if (ch == '*') {
      expect("*", Flavor.ACCESS);
      return Access.EXPORT;
    }
    if (ch == '+') {
      expect("+", Flavor.ACCESS);
      return Access.PROJECT;
    }
    if (ch == '~') {
      expect("~", Flavor.ACCESS);
      return Access.FOLDER;
    }
    return Access.FILE;
  }}

}

public static class Accesses {

  public static void format(this Access access, Formatter fmt) {
    switch (access) {
      case Access.FOLDER: fmt.print("~"); break;
      case Access.PROJECT: fmt.print("+"); break;
      case Access.EXPORT: fmt.print("*"); break;
    }
  }

}

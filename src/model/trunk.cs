public class Trunk {

  public readonly string name;
  public Type type;
  public readonly bool param;
  public readonly bool minty;
  public readonly bool constant;
  public readonly Pair? castFrom;

  public Pair pair => new Pair(name, type.llvm);

  private Trunk(string name, Type type, bool param, bool minty, bool constant, Pair? castFrom) {
    this.name = name;
    this.type = type;
    this.param = param;
    this.minty = minty;
    this.constant = constant;
    this.castFrom = castFrom;
  }

  public string rawName { get {
    var p = name.IndexOf(".");
    if (p < 0) {
      return name.Substring(1);
    }
    return name.Substring(1, p - 1);
  }}

  public static Trunk forParam(string name, Type type) {
    return new Trunk($"%{name}", type, true, false, false, null);
  }

  public static Trunk forLocal(string name, Type type) {
    return new Trunk(name, type, false, false, false, null);
  }

  public static Trunk forError(string name, Type type) {
    return new Trunk(name, type, false, false, true, null);
  }

  public static Trunk forConstant(string name, Type type, Pair? castFrom) {
    return new Trunk(name, type, false, false, true, castFrom);
  }

}

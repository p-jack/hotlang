public class Pair {

  public string name { get; private set; }
  public llvm.Type type { get; private set; }

  public Pair(string name, llvm.Type type) {
    this.name = name;
    this.type = type;
  }

  public override string ToString() {
    return $"{type} {name}";
  }

  public Pair star => new Pair(name, type.star);
  public Pair unstar => new Pair(name, type.unstar);
  public bool isNull => name == "null";

  // public static Pair forStruct(string name, Struct str) {
  //   return new Pair(name, new llvm.StructType(str).star);
  // }

  public static Pair nul(llvm.Type type) {
    return new Pair("null", type);
  }

  public static Pair nul(string type) {
    return nul(new llvm.Direct(type));
  }


}

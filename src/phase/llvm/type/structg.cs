namespace llvm {

  public class StructTypeG:StructType {

    public StructTypeG(Struct s, Focus focus) : base(s, focus) {}

    public override string ToString() {
      return $"%{str.fullName}.tg";
    }

    public StructType raw => new StructType(str, focus);

  }

}

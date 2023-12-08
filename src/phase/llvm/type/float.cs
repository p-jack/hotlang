namespace llvm {

  public class Float:Type {

    public readonly int bitSize;

    public Float(int bitSize) : base() {
      this.bitSize = bitSize;
    }

    public override bool Equals(object? o) {
      if (o == null) return false;
      if (o.GetType() != typeof(Float)) return false;
      return bitSize == ((Float)o).bitSize;
    }

    public override int GetHashCode() {
      return HashCode.Combine("float", bitSize);
    }

    public override string ToString() {
      return $"f{bitSize}";
    }

  }

}

namespace llvm {

  public class Int:Type {

    public readonly int bitSize;

    public Int(int bitSize) : base() {
      this.bitSize = bitSize;
    }

    public override bool Equals(object? o) {
      if (o == null) return false;
      if (o.GetType() != typeof(Int)) return false;
      return bitSize == ((Int)o).bitSize;
    }

    public override int GetHashCode() {
      return HashCode.Combine("int", bitSize);
    }

    public override string ToString() {
      return $"i{bitSize}";
    }

    public Pair pair(int value) {
      return new Pair($"{value}", this);
    }

  }

}

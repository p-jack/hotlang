namespace types {
public class Int:Type {

  public readonly bool overflow;
  public readonly bool signed;
  public readonly int bitSize;
  public readonly int systemBitSize;

  public Int(Focus focus, bool overflow, bool signed, int bitSize, int systemBitSize) : base(focus) {
    this.overflow = overflow;
    this.signed = signed;
    this.bitSize = bitSize;
    this.systemBitSize = systemBitSize;
  }

  public int bits => bitSize > 0 ? bitSize : systemBitSize;

  public override string indexKey => "number";
  public override Int refocus(Focus f) => new Int(f, overflow, signed, bitSize, systemBitSize);
  public override llvm.Type llvm => new llvm.Int(bits);
  protected override int hashCode => HashCode.Combine(focus, overflow, signed, bitSize, systemBitSize);

  public override bool same(Type t) {
    if (t.GetType() != typeof(Int)) return false;
    var that = (Int)t;
    return this.overflow == that.overflow
      && this.signed == that.signed
      && this.bitSize == that.bitSize
      && this.systemBitSize == that.systemBitSize;
  }

  public override string ToString() {
    var sb = new System.Text.StringBuilder();
    if (overflow) {
      sb.Append("overflowable_");
    }
    if (signed) {
      sb.Append("int");
    } else {
      sb.Append("uint");
    }
    if (bitSize > 0) {
      sb.Append($"{bitSize}");
    }
    sb.Append(":");
    sb.Append(focus.ToString());
    return sb.ToString();
  }

  public override Microsoft.Z3.Sort z3(Microsoft.Z3.Context ctx) {
    return ctx.MkBitVecSort((uint)bits);
  }

}
}

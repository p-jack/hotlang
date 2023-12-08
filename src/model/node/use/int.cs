// TODO: Should byte be overflowable

public class Int:Use {

  public readonly bool overflow;
  public readonly bool signed;
  public readonly int bitSize;

  public Int(Place place, Blur blur, bool overflow, bool signed, int bitSize) : base(place, blur) {
    this.overflow = overflow;
    this.signed = signed;
    this.bitSize = bitSize;
  }

  public override bool primitive => true;

  protected override Type resolve(Verifier v) {
    var defaults = Focus.primitive(true);
    var focus = blur.focus(defaults);
    var systemBitSize = v.oot.conf.intBits;
    return new types.Int(focus, overflow, signed, bitSize, systemBitSize);
  }

  public override string ToString() {
    if (!overflow && !signed && bitSize == 8) return "byte";
    var sb = new System.Text.StringBuilder();
    if (overflow) {
      sb.Append("overflowable_");
    }
    if (!signed) {
      sb.Append("u");
    }
    sb.Append("int");
    if (bitSize > 0) {
      sb.Append($"{bitSize}");
    }
    return sb.ToString();
  }

  static string parserName() => "intUse";
  static string[] full() => new string[] {
    "int", "int8", "int16", "int32", "int64",
    "uint", "byte", "uint8", "uint16", "uint32", "uint64",
    "overflowable_int", "overflowable_int8", "overflowable_int16", "overflowable_int32", "overflowable_int64",
    "overflowable_uint", "overflowable_uint8", "overflowable_uint16", "overflowable_uint32", "overflowable_uint64"
  };

}

public partial class In {

  public Int intUse { get {
    var place = skip();
    var token = this.token();
    expect(token, Flavor.TYPE);
    switch (token) {
      case "int": return new Int(place, blur, false, true, 0);
      case "int8": return new Int(place, blur, false, true, 8);
      case "int16": return new Int(place, blur, false, true, 16);
      case "int32": return new Int(place, blur, false, true, 32);
      case "int64": return new Int(place, blur, false, true, 64);
      case "uint": return new Int(place, blur, false, false, 0);
      case "uint8": return new Int(place, blur, false, false, 8);
      case "byte": return new Int(place, blur, false, false, 8);
      case "uint16": return new Int(place, blur, false, false, 16);
      case "uint32": return new Int(place, blur, false, false, 32);
      case "uint64": return new Int(place, blur, false, false, 64);
      case "overflowable_int": return new Int(place, blur, true, true, 0);
      case "overflowable_int8": return new Int(place, blur, true, true, 8);
      case "overflowable_int16": return new Int(place, blur, true, true, 16);
      case "overflowable_int32": return new Int(place, blur, true, true, 32);
      case "overflowable_int64": return new Int(place, blur, true, true, 64);
      case "overflowable_uint": return new Int(place, blur, true, false, 0);
      case "overflowable_uint8": return new Int(place, blur, true, false, 8);
      case "overflowable_uint16": return new Int(place, blur, true, false, 16);
      case "overflowable_uint32": return new Int(place, blur, true, false, 32);
      case "overflowable_uint64": return new Int(place, blur, true, false, 64);
      default: throw new Bad("expected integer type");
    }
  }}

}

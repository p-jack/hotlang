// TODO: hex, octal, etc
// TODO: plugins here?
// TODO: overflow for uint64
public class Number:Head {

  public readonly long value;

  public Number(Place place, long value) : base(place) {
    this.value = value;
  }

  public override bool ambiguous => true;

  protected override Type resolve(Verifier v) {
    if (this.expected?.GetType() == typeof(types.Int)) return expected;
    var focus = Focus.primitive(true);
    return new types.Int(focus, false, true, 0, v.oot.conf.intBits);
  }

  internal override ZZZ mk(Solva solva) {
    var itype = (types.Int)(type!);
    return new zzz.Int(itype, value);
  }

  protected override Pair toPair(LLVM llvm) {
    return new Pair($"{value}", type.llvm);
  }

  public override void format(Formatter f) {
    f.print($"{value}");
  }

  static string[] partial() => new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

}

public partial class In {

  public Number? number { get {
    var place = skip();
    (long v,bool ok) r = this.int64;
    return r.ok ? new Number(place, r.v) : null;
  }}

  public long mustInt64 { get {
    (long v, bool ok) = this.int64;
    if (!ok) throw new Bad("Expected integer.");
    return v;
  }}

  public (long,bool) int64 { get {
    var place = skip();
    mark();
    var negate = false;
    if (peek == '-') {
      negate = true;
      expect("-", Flavor.DIGIT);
    }
    long value = 0;
    int count = 0;
    for (var ch = peek; isDigit(ch); ch = peek) {
      count++;
      expect(ch, Flavor.DIGIT);
      value = value * 10 + ch - 48;
    }
    if (count == 0) {
      recall();
      return (0, false);
    }
    return (negate ? -value : value, true);
  }}

}

namespace types { public class Array:Type {

  public readonly int systemBitSize;
  public readonly Type type;

  public Array(Focus focus, Type type, int systemBitSize) : base(focus) {
    if (type.focus.scheme == types.Scheme.GRAPH) {
      throw new Bad("Can't have an array of graphed objects.");
    }
    this.type = type;
    this.systemBitSize = systemBitSize;
  }

  public override bool needsDestroy { get {
    return type.needsDestroy || focus.scheme.needsDestroy();
  }}

  public override bool aggregate => true;
  public override Type element => type;
  public override string? varName => type.plural;
  public override string? plural => type.plural;
  // public override Type zzz => Type.UINT32; // TODO

  public override string indexKey => "[" + type.indexKey + "]";
  public override llvm.Array llvm => new llvm.Array(focus, type.llvm, new llvm.Int(systemBitSize));
  public override Array refocus(Focus f) => new Array(f, type, systemBitSize);
  public override string ToString() => "[" + type + "]:" + focus;
  protected override int hashCode => HashCode.Combine(focus, type, systemBitSize);
  public override bool same(Type t) {
    if (t.GetType() != typeof(Array)) return false;
    var a = (Array)t;
    return type.same(a.type);
  }

}}

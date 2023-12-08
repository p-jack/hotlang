namespace types {

public class StructType:Type {

  public readonly Struct strct;

  public StructType(Focus focus, Struct strct) : base(focus) {
    if (strct == null) throw new Bad();
    this.strct = strct;
  }

  public override StructType refocus(Focus f) => new StructType(f, strct);
  public override Access access => strct.access;
  public override bool aggregate => true;
  public override string varName => strct.name;
  public override string plural => strct.plural;
  public override llvm.Type llvm => new llvm.StructType(strct, focus).star;
  public override string ToString() => strct.fullName;
  public override Type zzz => Type.UINT32;
  protected override int hashCode => strct.GetHashCode();
  public override bool same(Type type) {
    if (type.GetType() != typeof(StructType)) return false;
    var that = (type as StructType)!;
    return this.strct == that.strct;
  }

  public override string indexKey { get {
    var s = strct;
    while (s.superStruct != null) {
      s = s.superStruct;
    }
    return s.fullName;
  }}

  public bool subclassOf(Type type) {
    if (!(type is StructType)) return false;
    var target = ((StructType)type).strct;
    for (var c = strct; c != null; c = c.superStruct) {
      if (c.fullName == target.fullName) return true;
    }
    return false;
  }

  protected override IList<string> check(Action action, Type formal) {
    var actual = this;
    if (formal.GetType() != typeof(StructType)) return mismatch(action, formal, actual);
    var st = (StructType)formal;
    if (this == formal || actual.subclassOf(formal)) {
      return actual.focus.actionTo(action, formal.focus);
    }
    return mismatch(action, formal, actual);
  }

  public override Microsoft.Z3.Sort z3(Microsoft.Z3.Context ctx) {
    return ctx.MkBitVecSort(32);
  }

}

}

namespace types {

public class TinType:Type {

  public readonly use.Tin.Key key;
  public readonly Struct strct;

  public TinType(Focus focus, use.Tin.Key key, Struct strct) : base(focus) {
    this.key = key;
    this.strct = strct;
  }

  public override TinType refocus(Focus f) => new TinType(f, key, strct);
  public override Type element => key.type2 == null ? key.type1 : key.type2;
  public override Access access => strct.access;
  public override bool aggregate => true;
  public override string varName => strct.plural;
  public override string plural => strct.plural;
  public override llvm.Type llvm => new llvm.StructType(strct, focus).star;
  public override Type zzz => Type.UINT32;
  protected override int hashCode => key.GetHashCode();
  public override bool same(Type type) {
    if (type.GetType() != typeof(TinType)) return false;
    var that = (type as TinType)!;
    return this.key.Equals(that.key);
  }

  public override string ToString() => key.ToString();

  public override string indexKey => "TODO";

  public override Microsoft.Z3.Sort z3(Microsoft.Z3.Context ctx) {
    return ctx.MkBitVecSort(32);
  }

}}

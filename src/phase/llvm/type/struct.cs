namespace llvm {

  public class StructType:Type {

    Struct _str;
    public override Struct str => _str;
    internal Focus focus;

    public override bool rc => focus.scheme == types.Scheme.RC;
    public override bool unique => focus.scheme == types.Scheme.UNIQUE;
    public override bool nullable => focus.nullable;
    public override bool needsMop { get {
      if (_str.needsDestroy) return true;
      return rc || unique;
    }}

    public StructType(Struct s, Focus focus) : base() {
      if (s == null) throw new Bad();
      if (focus == null) throw new Bad();
      this.focus = focus;
      this._str = s;
    }

    public override bool Equals(object? o) {
      if (o == null) return false;
      if (o.GetType() != typeof(StructType)) return false;
      return str == ((StructType)o).str;
    }

    public override int GetHashCode() {
      return HashCode.Combine("struct", str);
    }

    public override string ToString() {
      return $"%{str.fullName}.t";
    }

    public override string Debug { get {
      return ToString() + ";rc:" + rc + ";unique:" + unique + ";null:" + nullable;
    }}

    public StructTypeG gc => new StructTypeG(str, focus);

  }

}

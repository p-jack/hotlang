namespace llvm {

  public class VTable:Type {

    Struct _str;
    public override Struct str => _str;

    public VTable(Struct str) {
      this._str = str;
    }

    public override bool Equals(object? o) {
      if (o == null) return false;
      if (o.GetType() != typeof(VTable)) return false;
      return str == ((VTable)o).str;
    }

    public override int GetHashCode() {
      return HashCode.Combine("vtable", str);
    }

    public override string ToString() {
      return LLVM.classType(str);
    }

  }

}

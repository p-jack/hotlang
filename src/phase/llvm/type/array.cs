namespace llvm {

  public class Array:Type {

    public Type type;
    public Int i0;
    Focus focus;

    public override bool rc => focus.scheme == types.Scheme.RC;
    public override bool unique => focus.scheme == types.Scheme.UNIQUE;
    public override bool nullable => focus.nullable;
    public override bool needsMop { get {
      if (type.needsMop) return true;
      return rc || unique;
    }}


    public Array(Focus focus, Type type, Int i0) : base() {
      this.focus = focus;
      this.type = type;
      this.i0 = i0;
    }

    public override bool Equals(object? o) {
      if (o == null) return false;
      if (o.GetType() != typeof(Array)) return false;
      return type.Equals(((Array)o).type);
    }

    public override int GetHashCode() {
      return HashCode.Combine("array", type, i0);
    }

    public override string ToString() {
      return $"{{{type.star.ToString()}, {i0}}}";
    }

  }

}

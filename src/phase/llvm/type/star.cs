namespace llvm {

  public class Star:Type {

    public readonly Type type;

    public Star(Type type) : base() { this.type = type; }

    public override bool rc => type.rc;
    public override bool unique => type.unique;
    public override bool nullable => type.nullable;
    public override bool needsMop => type.needsMop;

    public override bool Equals(object? o) {
      if (o == null) return false;
      if (o.GetType() != typeof(Star)) return false;
      return type.Equals(((Star)o).type);
    }

    public override int GetHashCode() {
      return HashCode.Combine("star", type.GetHashCode());
    }

    public override string ToString() {
      return $"{type}*";
    }

    public override string Debug => type.Debug + ";Star";

    public override Struct? str => type.str;

  }

}

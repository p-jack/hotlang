namespace llvm {

  public class Direct:Type {

    public readonly string text;

    public Direct(string text) : base() {
      this.text = text;
    }

    public override bool Equals(object? o) {
      if (o == null) return false;
      if (o.GetType() != typeof(Direct)) return false;
      return text == ((Direct)o).text;
    }

    public override int GetHashCode() {
      return HashCode.Combine("direct", text.GetHashCode());
    }

    public override string ToString() {
      return text;
    }

    public override string Debug => ToString() + ";Direct";
  }

}

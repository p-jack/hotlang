public class Place {

  public string path { get; }
  public int line { get; }
  public int column { get; }

  public Place(string path, int line, int column) {
    this.path = path;
    this.line = line;
    this.column = column;
  }

  public override bool Equals(object? o) {
    if (!(o is Place)) return false;
    Place that = (Place)o;
    return this.path == that.path
      && this.line == that.line
      && this.column == that.column;
  }

  public override int GetHashCode() {
    return HashCode.Combine(path, line, column);
  }

  public override string ToString() {
    return $"{path}@{line},{column}";
  }
}

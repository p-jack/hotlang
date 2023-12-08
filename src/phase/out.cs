public class Out {

  public readonly Conf conf;
  public IList<string> errors => _errors;
  public bool anyErrors => errors.Count() > 0;

  private List<string> _errors = new List<string>();

  HashSet<ID> topSet = new HashSet<ID>();
  List<ID> tops = new List<ID>();

  public Out(Conf conf) {
    this.conf = conf;
  }

  class ID {

    public readonly string type;
    public readonly string fullName;
    public readonly Place place;
    public readonly Phase phase;

    public ID(Top top) {
      this.type = top.GetType().Name;
      this.phase = top.phase;
      if (top is Formality) {
        this.fullName = ((Formality)top).fullName;
      } else if (top is Field) {
        this.fullName = ((Field)top).nui.name!;
      } else {
        this.fullName = "<noname>";
      }
      this.place = top.place;
    }

    public override bool Equals(object? o) {
      if (o == null) return false;
      var that = (ID)o;
      return this.type == that.type
        && this.phase == that.phase
        && this.fullName == that.fullName
        && this.place.Equals(that.place);
    }

    public override int GetHashCode() {
      return HashCode.Combine(type, phase, fullName, place);
    }

    public override string ToString() {
      return $"[{phase},{type},{fullName},{place}]";
    }

  }

  string fullName(Top top) {
    if (top is Formality) {
      return ((Formality)top).fullName;
    }
    return $"{top.GetType().Name}";
  }

  public void push(Top top) {
    var id = new ID(top);
    if (topSet.Contains(id)) {
      throw new Bad("Define loop: " + id + ", after " + WTF.str(tops));
    }
    topSet.Add(id);
    tops.Add(id);
  }

  public void pop() {
    ID id = tops.Last();
    tops.RemoveAt(tops.Count() - 1);
    topSet.Remove(id);
  }

  public void report(Node node, string message) {
    node.failed = true;
    Console.WriteLine(node.place + ": " + message);
    errors.Add(message);
  }

}

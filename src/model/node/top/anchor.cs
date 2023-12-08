public class Anchor:Node {

  public readonly string typeName;
  public readonly string fieldName;

  bool resolved = false;
  public Struct? str = null;
  public Field? field = null;

  public Anchor(Place place, string type, string field) : base(place) {
    this.typeName = type;
    this.fieldName = field;
  }

  /////

  public void resolve(Out oot) {
    // TODO: Allow Foo.*
    var unit = ancestor<Unit>()!;
    var types = unit.symbols.types(typeName);
    if (types.Count() == 0) {
      oot.report(this, $"No such str or class: {typeName}");
      return;
    }
    if (types.Count() == 0) {
      var names = types.OfType<Formality>().Select(x => x.name);
      oot.report(this, $"Ambiguous anchor. Might be: {WTF.str(names)}");
      return;
    }
    if (!(types[0] is Struct)) {
      oot.report(this, $"{typeName} is not a struct or class.");
      return;
    }
    var type = (Struct)types[0];
    if (fieldName == "*") {
      this.resolved = true;
      this.failed = false;
      this.str = type;
      this.field = null;
      return;
    }
    var field = type.field(fieldName);
    if (field == null) {
      oot.report(this, $"No such field: {typeName}.{fieldName}");
      return;
    }

    if (field.inbound) {
      oot.report(this, $"{typeName}.{fieldName} is already inbound!");
      return;
    }

    this.resolved = true;
    this.failed = false;
    this.str = type;
    this.field = field;
  }

  /////

  public bool matches(Field field) {
    if (!resolved) throw new Bad("not resolved");
    if (failed) return false;
    if (this.field == null) {
      // TODO, or an embedded tin
      return field.type.focus.scheme == types.Scheme.UNIQUE
       && str == field.ancestor<Struct>();
    }
    return this.field == field;
  }

  public int ordinal(IList<Anchor> anchors, Field field) {
    for (var i = 0; i < anchors.Count(); i++) {
      if (anchors[i].matches(field)) return i;
    }
    return -1;
  }

  /////

  public override string ToString() => $" <- {typeName}.{fieldName}";

}

public partial class In {

  string starOrID() {
    if (peek != '*') return id();
    expect("*", Flavor.IDENTIFIER);
    return "*";
  }

  public Anchor? anchor { get {
    skip();
    if (token() != "<-") return null;
    expect("<-", Flavor.OPERATOR);
    var place = skip();
    var type = id();
    expect(".", Flavor.OPERATOR);
    var field = starOrID();
    return new Anchor(place, type, field);
  }}

}

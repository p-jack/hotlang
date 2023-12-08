public class Field:Top {

  public readonly Access access;
  public readonly NUI nui;
  public readonly Anchor? anchor;

  public int ordinal = 0;
  public string name = "";
  public Type type = Fail.FAIL;

  public Field(Place place, Access access, NUI nui, Anchor? anchor) : base(place) {
    this.access = access;
    this.nui = set(nui);
    this.anchor = setNull(anchor);
  }

  public bool allows(Node n) {
    return Top.allows(access, this, n);
  }

  /////

  public bool inbound => anchor != null;

  public Field? inboundField { get {
    if (!(type is types.StructType)) return null;
    var t = (types.StructType)type;
    return t.strct.inbound(this);
  }}

  public string setterName => $"@{ancestor<Struct>()!.fullName}_{name}_s_";

  /////

  public bool detectCycles(Out oot, Set set) {
    if (type is types.StructType) {
      return detectCycles(oot, set, (types.StructType)type);
    }
    // TODO, arrays and containers
    return false;
  }

  bool detectCycles(Out oot, Set set, types.StructType type) {
    var str = type.strct;
    if (set.has(str.fullName)) {
      if (str.inbound(this) == null) {
        // TODO: better output
        oot.report(this, "Reference cycle detected.");
      }
      return true;
    }
    if (type.focus.scheme == types.Scheme.GRAPH) {
      // point here is to NOT add the name for unique/embedded
      // (because they can't possibly cause a reference cycle directly)
      set.add(str.fullName);
    }
    return str.detectCycles(oot, set);
  }

  /////

  protected override void index2(Out oot) {
    return;
  }

  protected override void analyze2(Out oot) {
    var str = ancestor<Struct>();
    if (str == null) {
      oot.report(this, "No global variables!");
      return;
    }
    name = nui.resolveName(oot);
    // if (inbound) {
    //   var focus = new Focus(true, true, types.Mutability.MUTABLE, types.Scheme.GRAPH);
    //   type = new types.StructType(focus, anchor!.str!);
    // } else {
//      type = nui.resolveType(oot);
//    }
    type = nui.resolveType(oot);
    if (nui.defaultedToUnique) {
      type = type.rescheme(types.Scheme.WEAK);
    }
    // failsafes
    if (name == null) throw new Bad("field has no name?");
    if (type == null) throw new Bad("type has no name?");
  }

  public override void emit(LLVM llvm) {
    return;
  }

  /////

  public override bool keepWithNext(Top next) {
    if (next is Field) return true;
    if (next is Nest) return ((Nest)next).onlyFields;
    return false;
  }

  public override void format(Formatter fmt) {
    nui.format(fmt);
    if (anchor != null) {
      fmt.print(anchor.ToString());
    }
    fmt.println("");
  }

  public override string ToString() {
    if (anchor == null) return nui.ToString();
    return nui.ToString() + anchor.ToString();
  }

}

public partial class In {

  Top? fields { get {
    var access = this.access;
    var nuis = this.nuis;
    if (nuis == null) return null;
    if (nuis.Count() == 0) return null;
    if (nuis.Count() == 1) return new Field(nuis[0].place, access, nuis[0], this.anchor);
    var list = nuis.Select(x => new Field(x.place, access, x, null)).OfType<Top>().ToList();
    return new Nest(nuis[0].place, list, false);
  }}

}

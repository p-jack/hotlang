public class Param:Node {

  public readonly NUI nui;

  public string name { get; private set; }
  public Type type => trunk!.type;
  private Trunk? _trunk;
  public override Trunk? trunk => _trunk;
  private bool prepared = false;

  public Param(NUI nui) : base(nui.place) {
    this.nui = set(nui);
    this.name = "";
    this._trunk = null;
  }

  public override Param copy() => (base.copy() as Param)!;

  public override string ToString() => nui.ToString();

  public void prepare(Out oot) {
    if (prepared) return;
    this.prepared = true;
    this.name = nui.resolveName(oot);
    var type = nui.resolveType(oot);
    this._trunk = Trunk.forParam(name, type);
    if (type.focus.scheme == types.Scheme.BRAND_NEW) {
      oot.report(this, "Can't use brand new scheme for a parameter, only for return types.");
    }
  }

}

public static class Params {

  public static void format(this IList<Param> paramz, Formatter fmt) {
    fmt.print("(");
    paramz.Select(x => x.nui).format(fmt);
    fmt.print(")");
  }

}

public partial class In {

  List<Param> paramz { get {
    List<Param> result;
    this.expect("(", Flavor.BRACE);
    if (peek == ')') {
      result = new List<Param>();
    } else {
      result = this.nuis.Select(x => new Param(x)).ToList();
    }
    this.expect(")", Flavor.BRACE);
    return result;
  }}

}

/*

*+class Param/Node

nui NUI

name string:n = null
trunk Trunk:n = null

new(nui NUI:rm) Param:r {
  let result = Param:r{place: nui.place, nui: nui}
  nui.parent = result
  return result
}

/nodeName { return "Param" }

/children {
  let r ^[Node] = ^[]
  r.add(.nui)
  return r
}

/reset2 {
  .nui.reset()
  .name = null
  .trunk = null
}

copy(:) Param:r {
  return Param_new(.nui.copy)
}

all_copy([Param]) ^[Param]:r {
  let result ^[Param]:r = ^[]
  params.each:{
    result.add(x.copy)
  }
  return result
}

/equal {
  return false if !node/?Param
  return .nui == node/!Param.nui
}

/printTo {
  .nui.printTo(writer)
}

/format {
  .printTo(fmt)
}

// TODO, what with NUIs this is much harder than this
+format(fmt Formatter:m, [Param]) {
  fmt.print("(")
  let first = true
  params.each:{
    if !first {
      fmt.print(", ")
    }
    x.format(fmt)
    first = false
  }
  fmt.print(")")
}

prepare(:m, Out:rm) {
  .name = .nui.name(out)
  let type = .nui.type(out)
  let trunk = Trunk_new("%$.name".copy, type)
  trunk.param = true
  this->trunk = trunk
}

/emit {
  // nop
}

*/

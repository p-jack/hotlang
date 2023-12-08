public class Local:Stmt {

  public readonly NUI nui;

  public string name { get; private set; }
  private Trunk? _trunk = null;
  private Assign? assign = null;

  public Local(Place place, NUI nui) : base(place) {
    this.nui = set(nui);
    this.name = "";
  }

  public override Trunk? trunk => _trunk;

  public override void format(Formatter f) {
    nui.format(f);
  }

  /////

  public override void verify(Verifier v) {
    var type = this.resolveType(v);
    setName(v, type);
    if (failed) return;
    if (nui.initial != null) {
      var fetch = new Fetch(place, name);
      this.assign = new Assign(place, Verb.ASSIGN, fetch, nui.initial.copy());
      adopt(assign);
    }
    var f = ancestor<Function>()!;
    var n = f.nextLocal;
    _trunk = Trunk.forLocal($"%{name}.{n}", type);
    f.addLocal(_trunk);
    assign?.verify(v);
  }

  void setName(Verifier v, Type type) {
    if (nui.name != null) {
      name = nui.name;
    } else {
      if (type.varName == null) {
        v.report(this, "Local has no name.");
        return;
      }
      name = type.varName;
    }
    if (v.symbols.local(name)) {
      v.report(this, $"Duplicate local: {name}");
      return;
    }
    v.symbols.addLocal(name, this);
  }

  Type resolveType(Verifier v) {
    if (nui.use != null) {
      nui.use.verify(v);
      return nui.use.type!;
    }
    if (nui.initial != null) {
      nui.initial.verify(v);
      return nui.initial.type!;
    }
    return Unknown.UNKNOWN;
  }

  /////

  public override void solve(Solva solva) {
    assign?.solve(solva);
  }

  /////

  public override void emit(LLVM llvm) {
    assign?.emit(llvm);
  }

}

/*
*+node Local/Stmt

// TODO, parse NUIs here

/renameLocal {
  if .name not null {
    if .name == from {
      .name = to
      .nui.name = to
    }
  } else if .nui.name not null {
    if .nui.name == from {
      .nui.name = to
    }
  }
}

+locals_copy([Local]) ^[Local]:r {
  let r ^[Local]:r = ^[]
  locals.each:{
    r.add(x.copy/!Local)
  }
  return r
}

/defaultFocus {
  return Focus:r{exist: NEVER_NULL, vary: VARIABLE, mutate: MUTABLE, scheme: AUTO}
}


-class Issue/scope_Issue {
  .name(:) string { return .source/!Local.name }
}

:( DupeVar en "Duplicate local variable $.name."
:( NoFunc en "Variable declaration outside of function."
:( NoName en "You need to specify a name for this variable."
*/

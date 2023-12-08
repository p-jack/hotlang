public class Struct:Formality {

  public readonly bool isClass;
  public readonly string superclass;
  public readonly Nest nest;

  public Struct? superStruct = null;
  public IList<Field> fields { get; private set; }
  public IList<Function> methods { get; private set; }
  public bool cycles  { get; private set; }
  public bool needsDestroy { get; private set; }
  public bool needsReach { get; private set; }
  private Function? destructor = null;

  types.Scheme _defaultScheme;

  public Struct(Place place, Access access, string name, bool isClass, string superclass, Nest nest) : base(place, access) {
    this.name = name;
    this.isClass = isClass;
    this.superclass = superclass;
    this.nest = set(nest);
    fields = new List<Field>();
    methods = new List<Function>();
    _defaultScheme = types.Scheme.STACK;
  }

  public string plural => name + "s"; // TODO

  public Field? field(string name) {
    foreach (var x in fields) {
      if (x.name == name) {
        return x;
      }
    }
    return null;
  }

  public Field? parentField { get {
    foreach (var x in fields.Where(x => x.inbound)) {
      if (x.type.focus.scheme == types.Scheme.UNIQUE) {
        return x;
      }
    }
    return null;
  }}

  public Field? inbound(Field outbound) {
    foreach (var x in fields.Where(x => x.inbound)) {
      var f = x.anchor!.field;
      if (f == null || f == outbound) return x;
    }
    return null;
  }

  public Function? method(string name) {
    return methods.Where(x => x.name == name).First();
  }

  public Param thisParam(Blur? blur) {
    if (blur == null) blur = new Blur(place);
    var use = new Named(place, blur, fullName);
    var nui = new NUI(place, "this", use, null);
    return new Param(nui);
  }

  internal override IList<string> check(Type formal, Type actual) {
    return actual.assignTo(formal);
  }

  /////

  protected override void index2(Out oot) {
    index(false, name);
    nest.index(oot);
  }

  protected override void setSupers2(Out oot) {
    nest.setSupers(oot);
    if (!isClass) return;
    if (superStruct != null) return;
    if (superclass == "") return;
    var unit = ancestor<Unit>()!;
    var candidates = unit.symbols.types(superclass);
    var matches = new List<Struct>();
    foreach (var x in candidates.OfType<Struct>().Where(x => x.isClass)) {
      matches.Add(x);
    }
    if (matches.Count() == 0) {
      oot.report(this, $"No such superclass: {superclass}");
      return;
    }
    if (matches.Count() > 1) {
      var names = matches.Select(x => x.fullName);
      oot.report(this, $"Ambiguous superclass: {names}");
      return;
    }
    superStruct = matches[0];
    var seen = new Set();
    var supers = new List<Struct>();
    supers.Add(this);
    for (var c = superStruct; c != null; c = c.superStruct) {
      supers.Add(c);
      if (seen.has(c.fullName)) {
        var names = supers.Select(x => x.fullName);
        oot.report(this, $"Circular class hierarchy: {names}");
        return;
      }
      seen.add(c.fullName);
    }
  }

  /////

  public types.Scheme defaultScheme(Out oot) {
    if (phase == Phase.MEMBERING) {
      // We necessarily have a reference cycle here.
      return types.Scheme.UNIQUE;
    } else {
      setMembers(oot);
      return _defaultScheme;
    }
  }

  public Focus defaultFocus(Out oot) {
    var scheme = defaultScheme(oot);
    var nullable = scheme == types.Scheme.UNIQUE || scheme == types.Scheme.WEAK;
    return new Focus(nullable, true, types.Mutability.MUTABLE, scheme);
  }

  protected override void setMembers2(Out oot) {
    if (isClass) {
      setClassMembers(oot);
    } else {
      setStaticMembers(oot);
    }
    formals = new List<Formal>();
    foreach (var x in fields) {
      if (x.inbound) {
        this.needsDestroy = true;
      } else {
        var formal = new Formal(this, x.name, x.type, x.nui.initialFormal(), x.ordinal);
        formals.Add(formal);
        this.needsDestroy |= x.type.needsDestroy;
      }
    }
    nest.setMembers(oot);
  }

  void setStaticMembers(Out oot) {
    fields = nest.fields;
    analyzeFields(oot, 0);
  }

  void setClassMembers(Out oot) {
    fields = new List<Field>();
    methods = new List<Function>();
    var strs = new List<Struct>();
    for (var str = this; str != null; str = str.superStruct) {
      strs.Add(str);
    }
    for (var i = strs.Count() - 1; i >= 0; i--) {
      foreach (var x in strs[i].nest.fields) fields.Add(x);
      foreach (var x in strs[i].nest.methods) addMethod(oot, x);
    }
    analyzeFields(oot, 1);
    analyzeMethods(oot);
  }

  void addMethod(Out oot, Function method) {
    bool found = false;
    for (int i = 0; i < methods.Count(); i++) {
      var existing = methods[i];
      if (existing.name == method.name) {
        found = true;
        if (method.kind == Kind.OVERRIDE) {
          methods[i] = method;
        } else {
          oot.report(method, $"Duplicate method: {name}.{method.name}");
        }
      }
    }
    if (!found) methods.Add(method);
  }

  void analyzeFields(Out oot, int start) {
    var seen = new Set();
    var ordinal = start;
    var jailed = false;
//    var gc = false;
    var weak = false;
    var unique = false;
    var inbound = false;
    foreach (var x in fields) {
      x.ordinal = ordinal;
      ordinal++;
      x.analyze(oot);
      if (seen.has(x.name)) {
        oot.report(x, $"Duplicate field: {name}->{x.name}");
      } else {
        seen.add(x.name);
      }
      // TODO: let #hash[type_scheme] // ^_^
      var scheme = x.type!.focus.scheme;
      if (scheme == types.Scheme.JAILED) {
        jailed = true;
      // } else if (scheme == types.Scheme.GRAPH) {
      //   gc = true;
      } else if (scheme == types.Scheme.WEAK) {
        weak = true;
      } else if (scheme == types.Scheme.UNIQUE) {
        unique = true;
      }
      if (x.inbound) {
        inbound = true;
      }
    }
    if (weak && jailed) {
      oot.report(this, "A struct can't have both jailed fields and weak fields.");
      _defaultScheme = types.Scheme.UNIQUE;
    } else if (inbound && jailed) {
      oot.report(this, "A struct can't have both jailed fields and bidirectional fields.");
      _defaultScheme = types.Scheme.UNIQUE;
    } else if (weak || inbound || unique) {
      _defaultScheme = types.Scheme.UNIQUE;
    } else if (jailed) {
      _defaultScheme = types.Scheme.STACK;
    } else {
      _defaultScheme = types.Scheme.RC;
    }

    // TODO: Decide whether to keep GRAPH or not
    // if ((gc || inbound) && jailed) {
    //   oot.report(this, "A struct can't have both jailed fields and bidirectional fields.");
    //   _defaultScheme = types.Scheme.GRAPH;
    // } else if (gc || inbound) {
    //   _defaultScheme = types.Scheme.GRAPH;
    // } else if (jailed) {
    //   _defaultScheme = types.Scheme.STACK;
    // } else {
    //   _defaultScheme = types.Scheme.RC;
    // }
  }

  void analyzeMethods(Out oot) {
    for (var i = 0; i < methods.Count(); i++) {
      var method = methods[i];
      method.ordinal = i + 3;
    }
  }

  /////

  protected override void prepare2(Out oot) {
    foreach (var x in fields.Where(x => x.inbound)) {
      x.anchor!.resolve(oot);
    }
    nest.prepare(oot);
  }


  /////

  public override void detectCycles(Out oot) {
    var set = new Set();
    set.add(fullName);
    cycles = detectCycles(oot, set);
  }

  public bool detectCycles(Out oot, Set set) {
    foreach (var x in fields) {
      if (x.detectCycles(oot, set)) {
        return true;
      }
    }
    return false;
  }

  /////

  protected override void analyze2(Out oot) {
    if (formals == null) {
      setMembers(oot); // TODO huh?
    }
    foreach (var x in fields.Where(x => x.inbound)) {
      x.anchor!.resolve(oot);
    }
    nest.analyze(oot);
    buildDestructor(oot);
  }

  public override void emit(LLVM llvm) {
    if (destructor != null) {
      Console.WriteLine(fullName + " has a destructor!");
    }
    destructor?.emit(llvm);
    llvm.emitStruct(this);
    nest.emit(llvm);
  }

  /////

  private void buildDestructor(Out oot) {
    if (!needsDestroy) return;
    // TODO, do we even need a destructor?
    var paramz = new List<Param>();
    var blur = new Blur(place, Nullability.NULL_ALLOWED, Variability.FINAL, Mutability.MUTABLE, Scheme.JAILED, null);
    var use = new Named(place, blur, fullName);
    var nui = new NUI(place, "this", use, null);
    paramz.Add(new Param(nui));
    var stmts = new List<Stmt>();
    // stmts.Add(new Scrap(place, new Void(place), false));
    // TODO, clear each inbound field first
    foreach (var x in fields.Where(x => !x.inbound && x.type.needsDestroy)) {
      var fetch = new Fetch(place, "this");
      var refer = new Refer(place, fetch, x.name);
      stmts.Add(new DestroyStmt(place, refer));
    }
    var block = new Block(place, false, stmts);
    var f = new Function(place, access, Kind.FUNCTION, "d_", paramz, null, block);
    adopt(f);
    Formatter fmt = new Formatter(new MemoryStream());
    f.format(fmt);
    Console.WriteLine(fmt.ToString());
    f.setSupers(oot);
    f.analyze(oot);
    f.fullName = fullName + "_d_";
    destructor = f;
  }

  /////

  public override bool keepWithNext(Top next) => false;

  public override string ToString() {
    if (isClass) return $"{name}/{superclass}";
    return $"{name} struct";
  }

  protected override void format2(Formatter fmt) {
    access.format(fmt);
    fmt.print(name);
    if (isClass) fmt.print($"/{superclass}");
    fmt.print(" ");
    nest.format(fmt);
  }

  static string[] full() => new string[] { "/", "{" };
  static string parserName() => "str";
}

public partial class In {

  public Struct? str { get {
    var place = skip();
    var access = this.access;
    var name = id();
    bool isClass = false;
    string super = "";
    if (peek == '/') {
      expect("/", Flavor.BRACE);
      isClass = true;
      if (isLetter(peek)) super = id();
    }
    var nest = this.getNest(true);
    if (nest == null) throw new Bad($"{this.place}: Expected {{");
    return new Struct(place, access, name, isClass, super, nest);
  }}

}

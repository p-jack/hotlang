using Microsoft.Z3;

public partial class Function:Formality {

  public readonly Kind kind;
  public readonly IList<Param> declaredParams;
  public readonly Use? declaredReturn;
  public readonly Block block;

  public int ordinal;
  public List<Trunk> locals { get; private set; }
  public IList<Param> paramz { get; private set; }
  internal List<BoolZZZ> facts { get; private set; }
  public Type? returnType { get; private set; }
  public bool minty => (returnType == null) ? false : returnType.minty;
  public List<Call> callers = new List<Call>();
  internal bool terminus = false;

  public Function(Place place, Access access, Kind kind, string name, List<Param> declaredParams, Use? declaredReturn, Block block) : base(place, access) {
    this.kind = kind;
    this.name = name;
    this.declaredParams = set(declaredParams);
    this.declaredReturn = setNull(declaredReturn);
    this.block = set(block);
    this.locals = new List<Trunk>();
    this.paramz = new List<Param>();
    this.facts = new List<BoolZZZ>();
  }

  public bool isMain => name == "main";

  public void addLocal(Trunk t) {
    locals.Add(t);
  }

  int _nextLocal = 0;
  public int nextLocal { get {
    _nextLocal++;
    return _nextLocal;
  }}

  internal override IList<string> check(Type formal, Type actual) {
    return actual.passTo(formal);
  }

  /////

  protected override void index2(Out oot) {
    if (isMain) {
      this.fullName = "main";
      ancestor<Unit>()!.symbols.index("main", true, this);
    } else {
      index(true, name);
    }
  }

  public string llvmName { get {
    if (kind == Kind.FUNCTION) {
      return fullName;
    } else {
      return $"{ancestor<Struct>()!.fullName}_{name}.m";
    }
  }}

  /////

  protected override void analyze2(Out oot) {
    var unit = ancestor<Unit>()!;
    var v = new Verifier(oot, unit.symbols);
    var str = ancestor<Struct>();
    if (str != null && hasThis) {
      v.push();
      foreach (var x in str.fields) {
        v.symbols.addLocal(x.name, x);
      }
      foreach (var x in str.methods) {
        v.symbols.addLocal(x.name, x);
      }
    }
    v.push();
    foreach (var x in paramz) {
      v.symbols.addLocal(x.name, x);
      addLocal(x.trunk!);
    }
    block.verify(v);
  }

  bool hasThis { get {
    if (paramz.Count() == 0) return false;
    return paramz[0].name == "this";
  }}

  protected override void solve2(Out oot) {
    var solva = new Solva(oot);
    foreach (var x in paramz) {
      if (x.type.aggregate && !x.type.focus.nullable) {
        // TODO use trunk name
        solva.assertNotNull(new Symbol(x.name));
      }
    }
    block.solve(solva);
    this.facts = solva.body;
    Console.WriteLine("SOLVED " + fullName);
    Console.WriteLine(solva);
    Console.WriteLine("---");
  }

  public override void emit(LLVM llvm) {
    llvm.startFunction(this);
    block.emit(llvm);
    llvm.endFunction();
  }

  public string returnLLVM { get {
    var i0 = ancestor<Program>()!.conf.intBits;
    if (returnType == null) {
      return $"i{i0}";
    }
    if (returnType.minty) {
      return $"i{i0}";
    }
    return $"{{ i{i0}, {returnType.llvm}}} ";
  }}

  /////

  bool keeper { get {
    return kind == Kind.ABSTRACT || block.onlyReturns;
  }}

  public override bool keepWithNext(Top next) {
    if (!(next is Function)) return false;
    var f = (Function)next;
    return keeper && f.keeper;
  }

  bool showParams { get {
    if (isMain) return false;
    if (kind == Kind.OVERRIDE && declaredParams.Count() == 0 && declaredReturn == null) return false;
    return true;
  }}

  protected override void format2(Formatter fmt) {
    access.format(fmt);
    kind.format(fmt);
    fmt.print(name);
    if (showParams) {
      declaredParams.format(fmt);
    }
    if (declaredReturn != null) {
      fmt.print(" ");
      fmt.print(declaredReturn.ToString());
    }
    if (fmt.header) {
      fmt.print(" {}");
      return;
    }
    if (block.onlyReturns) {
      fmt.print(", ");
      ((Return)block.stmts[0]).expr!.format(fmt);
      fmt.println("");
      return;
    }
    fmt.print(" ");
    block.format(fmt);
    fmt.println("");
  }

  public override String ToString() {
    var fmt = new Formatter(new MemoryStream());
    access.format(fmt);
    kind.format(fmt);
    fmt.print(name);
    if (!isMain) {
      declaredParams.format(fmt);
    }
    if (declaredReturn != null) {
      fmt.print(" ");
      fmt.print(declaredReturn.ToString());
    }
    return fmt.ToString();
  }

  /////

  static string[] partial() => new []{"("};

}

public partial class In {

  public Function function { get {
    var place = this.place;
    var access = this.access;
    var kind = this.kind;
    var name = this.id();
    List<Param> paramz;
    if (name == "main") {
      paramz = new List<Param>();
    } else if (kind != Kind.OVERRIDE || peek == '(') {
      paramz = this.paramz;
    } else {
      paramz = new List<Param>();
    }
    var ret = this.use;
    Block block;
    if (peek == ',') {
      expect(",", Flavor.OPERATOR);
      block = new Return(place, mustExpr).wrap;
    } else {
      block = this.block;
    }
    return new Function(place, access, kind, name, paramz, ret, block);
  }}

}

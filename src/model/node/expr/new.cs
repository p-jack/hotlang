public class New:Head {

  public readonly Blur blur;
  public readonly string name;
  public readonly IList<Actual> actuals;

  Match? match;
  bool embedded;
  Struct str => (Struct)match!.node;

  public New(Place place, Blur blur, string name, List<Actual> actuals) : base(place) {
    this.blur = blur;
    this.name = name;
    this.actuals = set(actuals);
  }

  internal override bool fresh => true;

  string found = "";

  string findName(Verifier v) {
    if (expected == null) {
      if (name == "") {
        v.report(this, "No name for constructor.");
        return "";
      }
      return name;
    }
    if (name == "") {
      if (expected is types.StructType) {
        return ((types.StructType)expected).strct.name;
      }
      v.report(this, $"Can't construct {expected}.");
      return "";
    }
    // We have both a specified name and an expected type.
    // They might not agree; that's OK, we'll use the specified name and whoever
    // set the expected type can check that the constructed thing matches.
    return name;
  }

  protected override Type resolve(Verifier v) {
    found = findName(v);
    if (found == "") return Fail.FAIL;
    var actuality = new Actuality(this, found, actuals);
    var matcher = new Matcher(actuality, false);
    var match = matcher.run(v);
    if (match == null) return Fail.FAIL;
    if (!(match.node is Struct)) return Fail.FAIL;
    this.match = match;
    foreach (var x in match.actuals) {
      if (x.failed) {
        v.report(this, $"{x} failed1");
      } else if (x.expr.failed) {
        v.report(this, $"{x.expr} failed2");
      } else if (x.type is Fail) {
        v.report(this, $"{x} failed3");
      } else if (x.expr.type is Fail) {
        v.report(this, $"{x} failed4");
      } else {
        x.expr.reposition(Position.RIGHT);
      }
    }
    var str = (Struct)match.node;
    Focus defaults = new Focus(false, true, types.Mutability.MUTABLE, str.defaultScheme(v.oot));
    Focus focus = blur.focus(defaults);
    return new types.StructType(focus, str);
  }

  internal override ZZZ mk(Solva solva) {
    var root = rightSymbol ?? solva.ledger.newVar;
    var match = this.match!;
    var str = (Struct)match.node;
    var actuals = match.actuals;
    var formals = match.formals;
    // var list = new List<BoolZZZ>();
    var ne = new zzz.Ne(ZZZ.var(Type.UINT32, root), new zzz.Int(Type.UINT32, 0));
    solva.assert(ne);
    // list.Add(ne);
    for (int i = 0; i < actuals.Count(); i++) {
      var actual = actuals[i];
      var formal = formals[i];
      var field = str.fields[formal.index];
      var name = ZZZ.field(root, field);
      var left = ZZZ.var(field.type, name);
      var right = actual.expr.zzz(solva);
      var eq = new zzz.Eq(left, right);
      solva.assert(eq);
      // list.Add(eq);
    }
    var head = ZZZ.var(Type.UINT32, root);
    return head; // new zzz.Goal(head, list);
  }

  /////

  protected override Pair toPair(LLVM llvm) {
    // return .emitConstant(llvm) if .position == CONSTANT
    var match = this.match!;
    var str = (Struct)match.node;
    var actuals = match.actuals;
    var formals = match.formals;

    // First, emit all params. If any raise an error, we shouldn't allocate
    // anything, nor should we overwrite any existing embedded fields.
    for (int i = 0; i < match!.actuals.Count(); i++) {
      var actual = actuals[i];
      var formal = formals[i];
      if (formal.type.focus.scheme == types.Scheme.VALUE) {
        ((New)actual.expr).embedded = true;
      }
      actual.expr.emit(llvm);
    }
    if (embedded) {
      // This bogus pair will never be used. Hacky, but it works. :/
      return new Pair("", LLVM.BOOL);
    } else {
      return embed(llvm);
    }
    // TODO: Clean up the constructed thing
  }

  Pair embed(LLVM llvm) {
    var match = this.match!;
    var str = (Struct)match.node;
    var actuals = match.actuals;
    var formals = match.formals;
    var result = basis(llvm);
    this.pair = result;
    // Store all params in the str.
    var gep = new Gep(result, 0);
    if (str.isClass) {
      llvm.store(gep, llvm.classPair(str).star, 0);
    }
    for (int i = 0; i < actuals.Count(); i++) {
      var actual = actuals[i];
      var formal = formals[i];
      var scheme = formal.type.focus.scheme;
      if (scheme == types.Scheme.VALUE) {
        gep.push(formal.index);
        var lp = llvm.point(gep, actual.type.llvm);
        actual.expr.assigningTo(lp);
        ((New)actual.expr).embed(llvm);
        gep.pop();
      } else {
        var field = str.fields[formal.index];
        llvm.assign(this, field, actual.expr);
      }
    }
    return result;
  }

  Pair basis(LLVM llvm) {
    if (type!.focus.scheme == types.Scheme.BRAND_NEW) {
      return this.leftPair;
    } else {
      var func = ancestor<Function>()!;
      return llvm.allocObject(func, type!, position != Position.RIGHT);
    }
  }

  bool isEmbedded(Expr expr) {
    if (expr is New) return ((New)expr).embedded;
    return false;
  }

  /////

  public override void format(Formatter fmt) {
    if (name != "") {
      fmt.print(name);
    }
    fmt.print("{");
    for (int i = 0; i < actuals.Count(); i++) {
      if (i > 0) {
        fmt.print(", ");
      }
      actuals[i].format(fmt);
    }
    fmt.print("}");
    if (blur != null) {
      blur.format(fmt);
    }
  }

  static string parserName() => "newExpr";
  static string partial() => "{";

}

public partial class In {

  public New newExpr { get {
    var place = skip();
    var name = "";
    var blur = new Blur(place);
    if (isLetter(peek)) {
      name = id();
    }
    var actuals = this.actuals('{', '}');
    if (peek == ':' || peek == '#') {
      blur = this.blur;
    }
    return new New(place, blur, name, actuals);
  }}

}

/*





-emitConstant(:mr, llvm LLVM:m) Pair:r {
  .prepareConstant(llvm)
//  llvm.println("${.leftPair.type.llvmBase} {")
  return .leftPair
}

-constantBase(:m, num int) string:rl {
  if .leftPair is null {
    return ".C$num"
  }
  return .leftPair.name
}

/prepareConstant {
  let actuals = .match.actuals
  actuals.each:{
    x.prepareConstant(llvm)
  }
  let byter = Byter:r{bytes:^[]}
  let base = .constantBase(llvm.nextConstant())
  let typeName = "%$base.tc"
  let constantName = "@$base"
  byter.print("$typeName = type {")
  // TODO, add class if necessary
  for let i = 0; i < actuals.length; i++ {
    if i > 0 {
      byter.print(", ")
    }
    actuals[i].expr.pair.type.printTo(byter)
  }
  byter.print("}")
  llvm.types.println(byter.bytes)
  let direct = Direct{text: typeName}
  if .type.focus.scheme == BRAND_NEW {
    .pair = Pair{ name: .constantBody, type: direct }
    return
  }
  .pair = Pair{name: constantName, type: direct.star }
  byter.bytes = ^[]
  byter.print("$constantName = ")
  if .leftPair is null {
    byter.print("private unnamed_addr ")
  }
  byter.println("constant $typeName $.constantBody")
  llvm.constants.println(byter.bytes)
}

-constantBody(:mr) string:r {
  // TODO add class if necessary
  let byter = Byter:r{bytes:^[]}
  byter.print("{ ")
  let actuals = .match.actuals
  for let i = 0; i < actuals.length; i++ {
    if i > 0 {
      byter.print(", ")
    }
    actuals[i].expr.pair.printTo(byter)
  }
  byter.print(" }")
  return byter.bytes
}

-class Issue/scope_Issue {
  new(:m) New:mr { return .source/!New }
}

:( NoName en "I can't infer a type name here, please specify a type."
:( NotNamed en "I expected ${.new.expected} here, not a class or str."

*/

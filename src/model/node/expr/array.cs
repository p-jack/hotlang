public class Array:Head {

  public readonly IList<Expr> elements;
  public readonly Blur blur;

  public Array(Place place, List<Expr> elements, Blur blur) : base(place) {
    this.elements = set(elements);
    this.blur = set(blur);
    // if (blur.size == null) throw new Bad("No size.");
  }

  Trunk? trunk_;

  protected override Type resolve(Verifier v) {
    var conf = ancestor<Program>()!.conf;
    if (blur.size != null && elements.Count() != 1) {
      v.report(this, "Since you specified a size for the array, you can only specify one element to repeat.");
    }
    blur.verify(v);
    failed = blur.failed;
    foreach (var x in elements) {
      x.reposition(Position.RIGHT);
      if (expectedElement != null) {
        x.expect(expectedElement);
      }
      x.verify(v);
      failed = failed || x.failed;
    }
    if (failed) return Fail.FAIL;
    addTrunk();
    var type = elementType;
    if (expectedElement != null) {
      type = expectedElement;
    }
    var defaults = use.Array.defaults(type);
    var focus = blur.focus(defaults);
    if (expected != null && !focus.Equals(expected.focus)) {
      v.report(this, $"Differing focus: expected {expected.focus} but you specified {focus}.");
      focus = expected.focus;
      return Fail.FAIL;
    }
    return new types.Array(focus, type, conf.intBits);
  }

  void addTrunk() {
    if (blur.size == null) return;
    var conf = ancestor<Program>()!.conf;
    var f = ancestor<Function>()!;
    var n = f.nextLocal;
    var focus = Focus.primitive(true);
    var type = new types.Int(focus, false, true, conf.intBits, conf.intBits);
    trunk_ = Trunk.forLocal($"%.i{n}", type);
    ancestor<Function>()!.addLocal(trunk_);
  }

  Type elementType { get {
    var c = elements.Count();
    if (c >= 2) return widen;
    if (c == 1) return elements[0].type;
    if (expected == null) throw new Bad("TODO: Empty type");
    if (!(expected is types.Array)) throw new Bad("TODO: Empty type");
    var a = (types.Array)expected;
    return a.type;
  }}

  Type widen { get {
    var w = elements[0].type.widen(elements[1].type);
    for (int i = 2; i < elements.Count(); i++) {
      w = w.result.widen(elements[i].type);
    }
    return w.result;
  }}

  Type? expectedElement { get {
    if (expected == null) return null;
    if (!(expected is types.Array)) return null;
    var a = (types.Array)expected;
    return a.type;
  }}

  /////

  internal override ZZZ mk(Solva solva) {
    // TODO: Ensure size is non-negative
    throw new Bad("TODO");
  }

  protected override Pair toPair(LLVM llvm) {
    var i0 = llvm.i0;
    var at = (types.Array)type;
    var lat = (llvm.Array)type.llvm;
    var size = calcSize(llvm);
    var func = ancestor<Function>()!;
    var r = llvm.allocArray(func, at, size, position != Position.RIGHT);
    var firstPtr = llvm.firstPtr(r);
    if (blur.size != null) {
      repeat(llvm, firstPtr, size);
    } else {
      populate(llvm, firstPtr);
    }
    return r;
  }

  Pair calcSize(LLVM llvm) {
    if (blur.size != null) {
      blur.size.emit(llvm);
      return blur.size.pair;
    }
    return new Pair($"{elements.Count()}", llvm.i0);
  }

  void populate(LLVM llvm, Pair firstPtr) {
    for (int i = 0; i < elements.Count(); i++) {
      elements[i].emit(llvm);
      var index = new Pair($"{i}", llvm.i0);
      llvm.assign(firstPtr, index, elements[i]);
    }
  }

  void repeat(LLVM llvm, Pair firstPtr, Pair size) {
    var elem = elements[0];
    var i = trunk_!.pair;
    var num = llvm.nextLabel();
    var begin = $"Array-{num}";
    var body = $"Array-Body-{num}";
    var end = $"Array-Done-{num}";
    llvm.br(begin);
    llvm.setLabel(begin);
    var iv = llvm.load(i);
    var c = llvm.icmp("slt", iv, size.name);
    llvm.br(c, body, end);
    llvm.setLabel(body);
    elem.emit(llvm);
    llvm.assign(firstPtr, iv, elements[0]);
    var p = llvm.add(iv, "1");
    llvm.store(i, p);
    llvm.br(begin);
    llvm.setLabel(end);
  }

  /////

  public override void format(Formatter fmt) {
    fmt.print("[");
    for (int i = 0; i < elements.Count(); i++) {
      if (i > 0) {
        fmt.print(", ");
      }
      elements[i].format(fmt);
    }
    fmt.print("]");
    if (blur != null) blur.format(fmt);
  }

  static string partial() => "[";

}

public partial class In {

  public Array array { get {
    var place = skip();
    expect("[", Flavor.BRACE);
    var elements = new List<Expr>();
    skip();
    while (peek != ']') {
      if (elements.Count() > 0) {
        expect(",", Flavor.OPERATOR);
        skip();
      }
      elements.Add(mustExpr);
      skip();
    }
    expect("]", Flavor.BRACE);
    var blur = this.blur;
    return new Array(place, elements, blur);
  }}

}

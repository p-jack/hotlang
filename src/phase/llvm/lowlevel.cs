public partial class LLVM {

  public Pair bitcast(Pair source, llvm.Type dest) {
    if (source.type == dest) {
      return new Pair(source.name, dest);
    }
    var result = nextVar();
    println($"  {result} = bitcast {source} to {dest}");
    return new Pair(result, dest);
  }

  public Pair bitcast(Pair source, string dest) {
    var result = nextVar();
    println($"  {result} = bitcast {source} to {dest}");
    return new Pair(result, new llvm.Direct(dest));
  }

  public Pair trunc(Pair from, llvm.Type to) {
    var result = nextVar();
    println($"  {result} = trunc {from} to {to}");
    return new Pair(result, to);
  }

  public Pair ptrtoint(Pair from) {
    var result = nextVar();
    println($"  {result} = ptrtoint {from} to {ptr}");
    return new Pair(result, ptr);
  }

  public Pair inttoptr(Pair from, llvm.Type to) {
    var result = nextVar();
    println($"  {result} = inttoptr {from} to {to}");
    return new Pair(result, to);
  }

  public Pair inttoptr(Pair from, Struct to, Focus focus) {
    return inttoptr(from, new llvm.StructType(to, focus).star);
  }

  /////

  public Pair icmp(string op, Pair pair, string value) {
    var result = nextVar();
    println($"  {result} = icmp {op} {pair}, {value}");
    return new Pair(result, new llvm.Direct("i1"));
  }

  public Pair icmp(string op, Pair pair, int value) {
    return icmp(op, pair, value.ToString());
  }

  public Pair eq(Pair pair, string value) {
    return icmp("eq", pair, value);
  }

  public Pair eq(Pair pair, int value) {
    return icmp("eq", pair, value);
  }

  public Pair ne(Pair pair, string value) {
    return icmp("ne", pair, value);
  }

  public Pair ugt(Pair pair, string value) {
    return icmp("ugt", pair, value);
  }

  public Pair slt(Pair pair, string value) {
    return icmp("slt", pair, value);
  }

  /////

  public void br(Pair condition, string truth, string fiction) {
    println($"  br {condition}, label %{truth}, label %{fiction}");
  }

  public void br(string label) {
    println($"  br label %{label}");
  }

  /////

  public void store(Pair dest, Pair value) {
//    debugPtr(dest.star);
    println($"  store {value}, {dest.star}");
  }

  public Pair load(Pair source) {
    var result = nextVar();
    println($"  {result} = load {source.type}, {source.star}");
    return new Pair(result, source.type);
  }

  /////

  public void assert(Pair test, Pair error) {
    var label = nextLabel();
    var success = $"Success-{label}";
    var failure = $"Failure-{label}";
    br(test, success, failure);
    setLabel(failure);
    abort(error);
    setLabel(success);
  }

  public void assert(Pair result) {
    var test = eq(result, 0);
    assert(test, result);
  }

  /////

  public Pair add(Pair left, string right) {
    var r = nextVar();
    println($"  {r} = add {left}, {right}");
    return new Pair(r, left.type);
  }

  public Pair add(Pair left, Pair right) {
    var r = nextVar();
    println($"  {r} = add {left}, {right.name}");
    return new Pair(r, left.type);
  }

  public Pair add(Pair left, int right) {
    var r = nextVar();
    println($"  {r} = add {left}, {right}");
    return new Pair(r, left.type);
  }

  public Pair sub(Pair left, Pair right) {
    var r = nextVar();
    println($"  {r} = sub {left}, {right.name}");
    return new Pair(r, left.type);
  }

  public Pair sub(Pair left, int right) {
    var r = nextVar();
    println($"  {r} = sub {left}, {right}");
    return new Pair(r, left.type);
  }

  /////

  public Pair not(Pair pair) {
    var r = nextVar();
    println($"  {r} = xor {pair}, -1");
    return new Pair(r, pair.type);
  }

  public Pair not(llvm.Type type, int value) {
    var r = nextVar();
    println($"  {r} = xor {type} {value}, -1");
    return new Pair(r, type);
  }

  /////

  public Pair shl(llvm.Type type, int left, int right) {
    var r = nextVar();
    println($"  {r} = shl {type} {left}, {right}");
    return new Pair(r, type);
  }

  public Pair shr(llvm.Type type, int left, int right) {
    var r = nextVar();
    println($"  {r} = lshr {type} {left}, {right}");
    return new Pair(r, type);
  }

  public Pair shr(Pair left, int right) {
    var r = nextVar();
    println($"  {r} = lshr {left}, {right}");
    return new Pair(r, left.type);
  }

  public Pair or(Pair left, Pair right) {
    var r = nextVar();
    println($"  {r} = or {left}, {right.name}");
    return new Pair(r, left.type);
  }

  public Pair or(Pair left, int right) {
    var r = nextVar();
    println($"  {r} = or {left}, {right}");
    return new Pair(r, left.type);
  }

  public Pair and(Pair left, Pair right) {
    var r = nextVar();
    println($"  {r} = and {left}, {right.name}");
    return new Pair(r, left.type);
  }

  public Pair and(Pair left, int right) {
    var r = nextVar();
    println($"  {r} = and {left}, {right}");
    return new Pair(r, left.type);
  }

  public Pair xor(Pair left, Pair right) {
    var r = nextVar();
    println($"  {r} = xor {left}, {right.name}");
    return new Pair(r, left.type);
  }

  /////

  public Pair alloca(llvm.Type type, Pair num) {
    var result = nextVar();
    println($"  {result} = alloca {type}, {num}");
    return new Pair(result, type.star);
  }

  public Pair alloca(llvm.Type type) {
    var result = nextVar();
    println($"  {result} = alloca {type}");
    return new Pair(result, type.star);
  }

  public Pair extract(Pair source, int index, llvm.Type type) {
    var result = nextVar();
    println($"  {result} = extractvalue {source}, {index}");
    return new Pair(result, type);
  }

  /////

  public Pair call(llvm.Type ret, string name, Pair param) {
    return call(ret, name, new List<Pair> { param });
  }

  public Pair call(llvm.Type ret, string name, IList<Pair> paramz) {
    var result = nextVar();
    print($"  {result} = call {ret} {name}(");
    for (int i = 0; i < paramz.Count(); i++) {
      if (i > 0) print(", ");
      print(paramz[i].ToString());
    }
    println(")");
    return new Pair(result, ret);
  }

  public void call(string name, IList<Pair> paramz) {
    print($"  call void {name}(");
    for (int i = 0; i < paramz.Count(); i++) {
      if (i > 0) print(", ");
      print(paramz[i].ToString());
    }
    println(")");
  }

  public void call(string name, Pair param) {
    call(name, new List<Pair> { param });
  }

}

public partial class LLVM {

  private Function? function = null;
  private bool justExited = false;

  public void startFunction(Function f) {
    this.function = f;
    if (f.isMain) {
      addStart();
    }
    justExited = false;
    resetRooms();
    if (f.kind == Kind.ABSTRACT) { return; }
    var unit = f.ancestor<Unit>()!;
    if (unit.isHeader && !unit.used) { return; }
    print(unit.isHeader ? "declare " : "define ");
    print($"{f.returnLLVM} @{f.llvmName}(");
    for (var i = 0; i < f.paramz!.Count(); i++) {
      if (i > 0) print(", ");
      print(formal(f.paramz![i].trunk!));
    }
    if (f.minty) {
      if (f.paramz!.Count() > 0) { print(", "); }
      print($"{f.returnType!.llvm} %.out");
    }
    print(")");
    if (f.access != Access.EXPORT) {
      // TODO, depends on debug info
      print(" unnamed_addr");
    }
    if (unit.isHeader) {
      println("");
      return;
    }
    println(" {");
    funcMem = new MemoryStream();
    funcStream = new StreamWriter(funcMem);
    enterRoom();
  }

  void addStart() {
    println($"declare {i0} @hot_usedmem()");
    println("declare void @hot_dumpmem()");
    println("define void @\"\\01start\"() {");
    println($"  %v1 = call {i0} @main()");
    println($"  %v2 = call {i0} @hot_usedmem()");
    println($"  %v3 = icmp eq {i0} %v2, 0");
    println("  br i1 %v3, label %ok, label %leak");
    println("leak:");
    println("  call void @hot_dumpmem()");
    println("  br label %ok");
    println("ok:");
    print("  ");
    println(conf.exitASM);
    println("  unreachable");
    println("}");
    println("");
  }

  void declareLocalVars(Function f) {
    foreach (var x in f.locals!) {
      this.println($"  {x.pair.name} = alloca {x.pair.type}");
    }
  }

  void setupLocalVars(Function f) {
    foreach (var x in f.locals!) {
      // TODO: Reference counted objects need to be set to null here
      // (they might get "destroyed" before they're actually created?)
      // TODO: Stack-based arrays need to be set here too
    }
  }

  static string formal(Trunk trunk) {
    return $"{trunk.type.llvm} {trunk.name}.p";
  }

  static string local(Trunk trunk) {
    return $"{trunk.type.llvm}* {trunk.name}";
  }

  /////

  public void endFunction() {
    var f = this.function!;
    if (f.ancestor<Unit>()!.isHeader) { return; }
    funcStream!.Flush();
    funcMem!.Position = 0;
    var sr = new StreamReader(funcMem);
    var code = sr.ReadToEnd();
    funcMem = null;
    funcStream = null;
    declareLocalVars(f);
    setupLocalVars(f);
    foreach (var x in f.paramz!) {
      println($"  store {formal(x.trunk!)}, {local(x.trunk!)}");
    }
    println(code);
    if (f.kind == Kind.ABSTRACT) { return; }
    leaveRoom(false);
    if (!f.block.terminates && !justExited) {
      println($"  ret {i0} 0");
    }
    println("\n}");
  }

  /////

  public string funcType(Function f) {
    var sb = new System.Text.StringBuilder();
    sb.Append(f.returnLLVM);
    sb.Append("(");
    for (int i = 0; i < f.paramz.Count(); i++) {
      if (i > 0) sb.Append(", ");
      sb.Append(f.paramz[i].trunk!.pair.type); // formal(f.paramz[i].trunk!));
    }
    if (f.returnType != null) {
      if (f.returnType!.minty) {
        if (f.paramz.Count() > 0) sb.Append(", ");
        sb.Append(f.returnType.llvm);
      }
    }
    sb.Append(")*");
    return sb.ToString();
  }

  public void voidReturn() {
    cleanRooms();
    justExited = true;
    println($"  ret {i0} 0");
  }

  public void returnValue(Expr expr) {
    var scheme = expr.type!.focus.scheme;
    var pair = expr.pair!;
    // if (scheme == GRAPH) {
    //   root(pair); // TODO, this shouldn't be necessary
    // } // TODO rc and u
    cleanRooms();
    justExited = true;
    var v1 = nextVar();
    var v2 = nextVar();
    var f = function!;
    println($"  {v1} = insertvalue {f.returnLLVM} undef, {i0} 0, 0");
    println($"  {v2} = insertvalue {f.returnLLVM} {v1}, {pair}, 1");
    println($"  ret {f.returnLLVM} {v2}");
  }

  public void abort(Pair pair) {
    var f = function!;
    cleanRooms();
    if (f.returnType == null) {
      println($"  ret {pair}");
      return;
    }
    var v = nextVar();
    println($"  {v} = insertvalue {f.returnLLVM} undef, {pair}, 0");
    println($"  ret {f.returnLLVM} {v}");
  }

  public void abort(int error) {
    abort(new Pair(error.ToString(), i0));
  }

  public void raise(Pair pair) {
    abort(pair);
    justExited = true;
  }

  public Pair handleError(Function function, string tuple) {
    if (function.returnType == null) {
      return noResultPair(function, tuple);
    } else {
      return resultPair(function, tuple);
    }
  }

  Pair noResultPair(Function function, string r) {
    var errPair = new Pair(r, i0);
    propagate(errPair);
    return new Pair("???", new llvm.Direct("???"));
  }

  Pair resultPair(Function function, string tuple) {
    var err = nextVar();
    var ret = function.returnLLVM;
    println($"  {err} = extractvalue {ret} {tuple}, 0");
    var errPair = new Pair(err, i0);
    propagate(errPair);
    var result = nextVar();
    println($"  {result} = extractvalue {ret} {tuple}, 1");
    return new Pair(result, function.returnType!.llvm);
  }

  void propagate(Pair errPair) {
    var label = nextLabel();
    br(eq(errPair, 0), $"Success-{label}", $"Failure-{label}");
    setLabel($"Failure-{label}");
    abort(errPair);
    setLabel($"Success-{label}");
  }

}

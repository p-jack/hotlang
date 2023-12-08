// Move DestroyMop to mop.cs
// decrease() needs to support classes

public class DestroyStmt:Stmt {

  public readonly Expr expr;
  private DestroyMop? mop = null;

  public DestroyStmt(Place place, Expr expr) : base(place) {
    this.expr = set(expr);
  }

  public override void verify(Verifier v) {
    expr.verify(v);
    if (!expr.type.needsDestroy) {
      v.report(this, "Expression does not need to be destroyed.");
      return;
    }
    mop = new DestroyMop(ancestor<Function>()!, expr.type!.llvm);
  }

  public override void solve(Solva solva) {
    return;
  }

  public override void emit(LLVM llvm) {
    expr.emit(llvm);
    mop!.pair = expr.pair;
    mop.emit(llvm);
  }

  public override void format(Formatter fmt) {
    fmt.print("_DESTROY ");
    expr.format(fmt);
  }

}

internal class DestroyMop:Mop {

  // Type type;
  string i;
  DestroyMop? next;
  public Pair? pair = null;

  public static DestroyMop make(Expr expr) {
    var mop = new DestroyMop(expr.ancestor<Function>()!, expr.type!.llvm);
    mop.pair = expr.pair!;
    return mop;
  }

  public DestroyMop(Function f, llvm.Type type) {
    // this.type = type;
    if (isArray(type)) {
      this.i = $"%i.{f.nextLocal}";
      var systemBitSize = f.ancestor<Program>()!.conf.intBits;
      var t = new types.Int(Focus.PRIMITIVE, false, false, 0, systemBitSize);
      f.addLocal(Trunk.forLocal(i, t));
      var atype = (llvm.Array)type;
      if (atype.type.needsMop) {
        this.next = new DestroyMop(f, atype.type);
      }
    } else {
      this.i = "";
      this.next = null;
    }
  }

  bool isObject(Pair pair) {
    var type = pair.type;
    return (type is llvm.StructType) || (type is llvm.Star && type.unstar is llvm.StructType);
  }

  bool isArray(llvm.Type type) {
    return (type is llvm.Array) || (type is llvm.Star && type.unstar is llvm.Array);
  }

  public override void emit(LLVM llvm) {
    if (pair == null) throw new Bad("didn't set the pair for DestroyMop");
    llvm.println("; DestroyMop " + pair);
    if (isObject(pair)) {
      destroyObject(llvm);
    } else if (isArray(pair.type)) {
      destroyArray(llvm);
    } else {
      throw new Bad("Can't destroy " + pair + " " + pair.type.GetType());
    }
  }

  void destroyObject(LLVM llvm) {
    string nullLabel = "";
    var pair = this.pair!;
    if (pair.type.nullable) {
      var label = llvm.nextLabel();
      nullLabel = $"Null-{label}";
      var notNullLabel = $"NotNull-{label}";
      llvm.br(llvm.eq(pair, "null"), nullLabel, notNullLabel);
      llvm.setLabel(notNullLabel);
    }
    if (pair.type.rc) {
      llvm.decrease(pair);
    } else if (pair.type.unique) {
      destroy(llvm, pair);
      llvm.free(pair);
    } else {
      throw new Bad("Can't destroy " + pair);
    }
    if (pair.type.nullable) {
      llvm.setLabel(nullLabel);
    }
  }

  void destroy(LLVM llvm, Pair obj) {
    var str = obj.type.str!;
    if (!str.needsDestroy) return;
    if (!str.isClass) {
      llvm.call(llvm.destroyFunc(str), obj);
      return;
    }
    var cast = llvm.bitcast(obj, LLVM.OBJECT);
    llvm.invoke("void(i8*)*", 1, obj);
    // var n = nextLabel();
    // br(eq(obj, "null"), $"Done-{n}", $"NotNull-{n}");
    // setLabel($"NotNull-{n}");
    // var cast = bitcast(obj, OBJECT);
    // invoke("void(i8*)*", 1, obj);
    // setLabel($"Done-{n}");
  }


  void destroyArray(LLVM llvm) {
    var pair = this.pair!;
    var storage = llvm.firstPtr(pair);
    if (pair.type.rc) {
      var rc = llvm.call(llvm.i0, "@hotr_rc_adec", storage);
      int label = llvm.nextLabel();
      var zero = $"RC-Zero-{label}";
      var nonzero = $"RC-NonZero-{label}";
      llvm.br(llvm.eq(rc, "0"), zero, nonzero);
      llvm.setLabel(zero);
      destroyElements(llvm);

      llvm.free(llvm.rcPtr(storage));

      llvm.br(nonzero);
      llvm.setLabel(nonzero);
    } else if (pair.type.unique) {
      destroyElements(llvm);
      llvm.free(storage);
    } else {
      throw new Bad("Can't destroy " + pair);
    }
  }

  void destroyElements(LLVM llvm) {
    var pair = this.pair!;
    var type = (llvm.Array)pair.type;
    if (!type.type.needsMop) return;
    var size = llvm.length(pair);
    var num = llvm.nextLabel();
    var begin = $"Array-{num}";
    var body = $"Array-Body-{num}";
    var end = $"Array-Done-{num}";
    var i = new Pair(this.i, llvm.i0);
    var firstPtr = llvm.firstPtr(pair);
    llvm.store(i, new Pair("0", llvm.i0));
    llvm.br(begin);
    llvm.setLabel(begin);
    var iv = llvm.load(i);
    var c = llvm.icmp("slt", iv, size.name);
    llvm.br(c, body, end);
    llvm.setLabel(body);

    var gep = new Gep(firstPtr, iv);
    var ptr = llvm.point(gep, type.type);
    var element = llvm.load(ptr);
    next!.pair = element;
    next.emit(llvm);

    var p = llvm.add(iv, "1");
    llvm.store(i, p);
    llvm.br(begin);
    llvm.setLabel(end);
  }

}

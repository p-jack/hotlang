public partial class LLVM {

  public Pair sizeOf(llvm.Type type, int count) {
    return sizeOf(type, new Pair($"{count}", i0));
  }

  public Pair sizeOf(llvm.Type type, Pair count) {
    var nul = Pair.nul(type);
    var gep = new Gep(Pair.nul(type), count);
    var ptr = point(gep, type);
    return ptrtoint(ptr);
  }

  public Pair malloc(llvm.Type type, int count) {
    return malloc(type, new Pair($"{count}", i0));
  }

  public Pair malloc(llvm.Type type, Pair count) {
    var uncast = malloc(sizeOf(type, count));
    return inttoptr(uncast, type);
  }

  public Pair malloc(Pair bytes) {
    var uncast = call(ptr, "@malloc", new List<Pair> { bytes });
    var test = ne(uncast, "0");
    assert(test, new Pair("64", i0));
    return uncast;
  }

  public void free(Pair pair) {
    var v = nextVar();
    debugPtr(pair);
    println($"  {v} = bitcast {pair} to i8*");
    println($"  call void @free(i8* {v})");
  }

  public Pair allocObject(Function func, Type type, bool shouldMop) {
    DestroyMop? mop = shouldMop ? new DestroyMop(func, type.llvm) : null;
    var scheme = type.focus.scheme;
    if (scheme == types.Scheme.STACK) {
      var result = alloca(type.llvm.unstar);
      if (mop != null) {
        mop.pair = result;
        this.mop(mop);
      }
      return result;
    }
    if (scheme == types.Scheme.GRAPH || scheme == types.Scheme.RC) {
      return allocGC(type.llvm, mop);
    }
    if (scheme == types.Scheme.UNIQUE) {
      var result = malloc(type.llvm, 1);
      if (mop != null) {
        mop.pair = result;
        this.mop(mop);
      }
      return result;
    }
    throw new Bad("Unsupported scheme " + scheme);
  }

  Pair allocGC(llvm.Type type, DestroyMop? mop) {
    if (!(type.unstar is llvm.StructType)) {
      throw new Bad($"gc alloc attempt on {type}");
    }
    var stype = (llvm.StructType)type.unstar;
    var prefixed = malloc(stype.gc.star, 1);
    var gep = new Gep(prefixed, 0);
    gep.push(0);
    store(point(gep, i0), new Pair(mop == null ? "0" : "1", i0));
    gep.pop();
    gep.push(1);
    var result = point(gep, type);
    if (mop != null) {
      mop.pair = result;
      this.mop(mop);
    }
    return result;
  }

  public Pair allocArray(Function func, types.Array arrayType, Pair size, bool shouldMop) {
    if (!(size.type is llvm.Int)) {
      throw new Bad($"Got type {size.type.GetType().Name} {size.type}");
    }
    var scheme = arrayType.focus.scheme;
    var at = arrayType.llvm;
    var elem = at.type;
    Pair elemPtr;
    if (scheme == types.Scheme.STACK) {
      elemPtr = alloca(elem, size);
    } else {
      var bytes = sizeOf(elem.star, size);
      var intSize = new Pair($"{conf.intBits / 8}", ptr);
      if (extra(scheme)) {
        println("; extra word for rc array");
        bytes = add(bytes, intSize);
      }
      var elemPtrInt = malloc(bytes);
      // TODO: If elem type is an aggregate, use calloc instead
      if (extra(scheme)) {
        var refPtr = inttoptr(elemPtrInt, i0.star);
        store(refPtr.unstar, new Pair(shouldMop ? "1" : "0", i0));
        elemPtrInt = add(elemPtrInt, intSize);
      }
      elemPtr = inttoptr(elemPtrInt, elem.star);
    }
    var v1 = nextVar();
    var v2 = nextVar();
    println($"  {v1} = insertvalue {at} undef, {elemPtr}, 0");
    println($"  {v2} = insertvalue {at} {v1}, {size}, 1");
    var r = new Pair(v2, at);
    if (shouldMop) {
      var mop = new DestroyMop(func, arrayType.llvm);
      mop.pair = r;
      this.mop(mop);
    }
    return r;
  }

  static bool extra(types.Scheme scheme) {
    return scheme == types.Scheme.GRAPH || scheme == types.Scheme.RC;
  }

}

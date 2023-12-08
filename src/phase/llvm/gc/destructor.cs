public partial class LLVM {

/*
  public void destructor(Struct str) {
    // if (!str.needsDestroy) return;
    var obj = new Pair("%obj", new llvm.StructType(str).star);
    println($"define void {destroyFunc(str)}({obj}) {{");
    if (hasArrays(str)) {
      println("  %i.d = alloca " + i0);
    }
    br(eq(obj, "null"), "Null", "NotNull");
    setLabel("Null");
    println("  ret void");
    setLabel("NotNull");
    foreach (var x in str.fields) {
      destroyField(str, obj, x);
    }
    println("  ret void");
    println("}\n");
  }

  void destroyField(Struct str, Pair obj, Field f) {
    if (f.type.primitive) {
      return;
    } else if (f.type is types.StructType) {
      destroyStructField(str, obj, f);
    } else {
      throw new Bad($"Can't destroy {f}");
    }
  }

  void destroyStructField(Struct str, Pair obj, Field f) {
    if (f.inbound) return; // TODO, clear weak inbounds
    var scheme = f.type.focus.scheme;
    var fstruct = ((types.StructType)f.type).strct;
    var nul = new Pair("null", new llvm.StructType(str).star);
    // TODO, if it's a container (we'll need to enterRoom etc first!)
    if (scheme == types.Scheme.GRAPH) {
      println($"  call void {f.setterName}({obj}, {nul})");
    } else if (scheme == types.Scheme.RC) {
      setRC(obj, f, nul);
    } else if (scheme == types.Scheme.UNIQUE) {
      var ptr = this.field(obj, f);
      var loaded = load(ptr);
      println($"  call void {destroyFunc(fstruct)}({loaded})");
      free(loaded);
    }
  }

  bool hasArrays(Struct str) {
    foreach (var x in str.fields) {
      if (x.type is Array) return true;
    }
    return false;
  }

  /*

  Conceptually:


  my_proj_foo.arrd({first,size}* arrayPtr) {
    a = *arrayPtr
    len = array.len
    first = array.ptr
    for (int i = 0; i < len; i++) {
      hotr_rc_dec(first[i], elem-destructor)
    }
    free(COUNT_PTR(first))
  }

  my_proj_foo.aurd({first,size}* arrayPtr) {
    a = *arrayPtr
    len = array.len
    first = array.ptr
    for (int i = 0; i < len; i++) {
      hotr_rc_dec(first[i], elem-destructor)
    }
    free(first)
  }

  my_proj_foo.arud({first,size}* arrayPtr) {
    let a = *arrayPtr, len = array.len, first = array.ptr
    for (int i = 0; i < len; i++) {
      let e = first[i]
      destroy(e)
      free(e)
    }
    free(COUNT_PTR())
  }

  */
  // myproj_foo.ard({first,size}*) {
  // }


  // Each array type gets these as-needed:
  //
  //  my_type.rd({first,size}*)
  //  my_type.ud({first,size}*)
  //
  //  ...then we just invoke that function like we invoke destructors
  //  No need



  // [[foo:u]:r]:r
  //   hotr_rc_array_dec({first*, size}*)




  // :u array of :u
  //    1. set all elements to null
  //    2. free storage
  // :u array of :r
  //    1. Invoke hotr_rc_array_elem_dec (nonexistent)
  //    2. free storage
  // :u of anything else
  //    1. free storage
  // :r array of :u
  //
  // :r array of :r
  //    Invoke hotr_rc_array_dec

/*
  void destroyArrayField(Struct str, Pair obj, Field f) {
    var ptr = this.field(obj, f);
    var loaded = load(ptr);
    var size = length(loaded);

    // println($"  call void {destroyFunc(fstruct)}({loaded})");
    // free(loaded);

    // var elem = elements[0];
    // var i = trunk_!.pair;
    var num = nextLabel();
    var begin = $"Array-{num}";
    var body = $"Array-Body-{num}";
    var end = $"Array-Done-{num}";
    llvm.br(begin);
    llvm.setLabel(begin);
    var i = new Pair("%i.d", i0);
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
  */

}

public partial class LLVM {

  Pair castDestroy(Pair obj) {
    var type = new llvm.Direct("void(i8*)*");
    var strct = obj.type.str!;
    if (strct.isClass) {
      var destructor = methodFunc(obj, "void(i8*)*", 1);
      return destructor;
    } else if (strct.needsDestroy) {
      var r = nextVar();
      println($"  {r} = bitcast void(%{strct.fullName}.t*)* {destroyFunc(strct)} to void(i8*)*");
      return new Pair(r, type);
    } else {
      return new Pair("null", type);
    }
  }

  Pair castDestroy(Struct str) {
    var type = new llvm.Direct("void(i8*)*");
    if (!str.needsDestroy) return new Pair("null", type);
    var r = nextVar();
    println($"  {r} = bitcast void(%{str.fullName}.t*)* {destroyFunc(str)} to void(i8*)*");
    return new Pair(r, type);
  }

  public void setRC(Pair obj, Field f, Pair rvalue) {
    var fstruct = ((types.StructType)f.type).strct;
    var lvalue = field(obj, f);
    var lcast = bitcast(lvalue.star, new llvm.Int(8).star.star);
    var rcast = bitcast(rvalue, new llvm.Int(8).star);
    var destroy = castDestroy(fstruct);
    println($"  call void @hotr_rc_set({lcast}, {rcast}, {destroy})");
  }

  // TODO, the array operations here aren't necessary anymore
  public void decrease(Pair obj) {
    var cast = bitcast(obj, new llvm.Int(8).star);
    Pair destroy;
    // if (obj.type is llvm.Array) {
    //   var a = (llvm.Array)obj.type;
    //   var elem = a.type;
    //   if (elem.str == null) {
    //     destroy = Pair.nul("void(i8*)*");
    //   } else {
    //     destroy = castDestroy(elem.str);
    //   }
    //   var len = length(obj);
    //   // TODO arrays of unique pointers
    //   println($"  call void @hotr_rc_array_dec({cast}, {len}, {destroy})");
    // } else {
      destroy = castDestroy(obj.type.str!);
      println($"  call void @hotr_rc_dec({cast}, {destroy})");
//    }
  }

  public Pair rcPtr(Pair obj) {
    var cast = bitcast(obj, i0.star);
    var p = nextVar();
    println($"  {p} = getelementptr {i0}, {cast}, i8 -1");
    return new Pair(p, i0.star);
  }

  Pair basis(Pair obj) {
    var t = obj.type;
    if (t is llvm.Array) {
      return firstPtr(obj);
    }
    return obj;
  }

  public void increase(Pair obj) {
    var cast = bitcast(basis(obj), new llvm.Int(8).star);
    println($"  call void @hotr_rc_inc({cast})");
  }

}

// public class DecreaseRC:Mop {
//
//   Pair pair;
//
//   public DecreaseRC(Pair pair) {
//     this.pair = pair;
//   }
//
//   public override void emit(LLVM llvm) {
//     llvm.decrease(pair);
//   }
//
// }

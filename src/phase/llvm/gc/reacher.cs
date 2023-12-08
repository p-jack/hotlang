/*
public partial class LLVM {

  public Pair reachable(Pair obj) {
    var str = obj.type.str!;
    if (!str.isClass) {
      return call(BOOL, reachFunc(str), obj);
    }
    var cast = bitcast(obj, OBJECT);
    return invoke("i1", "i1(i8*)*", 2, cast);
  }


  public void reacher(Struct str) {
//    if (!str.needsReach) return;
    var obj = new Pair("%obj", new llvm.StructType(str).star);
    println($"define i1 {reachFunc(str)}({obj}) {{");
    br(eq(obj, "null"), "Null", "NotNull");
    setLabel(obj, "Null");
    println("  ret i1 0");
    setLabel(obj, "NotNull");
    var controlPtr = this.controlPtr(obj);
    var control = load(controlPtr);
    br(marked(control), "Marked", "Unmarked");
    setLabel(obj, "Marked");
    println("  ret i1 0");
    setLabel(obj, "Unmarked");
    br(ugt(locals(control), "0"), "Jailed", "NotJailed");
    setLabel(obj, "Jailed");
    store(controlPtr, unanchor(control));
    println("  ret i1 1");
    setLabel(obj, "NotJailed");
    store(controlPtr, mark(control));
    foreach (var x in str.fields.Where(x => x.inbound)) {
      println($"; {x.name}");
      var fieldPtr = field(obj, x);
      var cast = load(fieldPtr);
//      var exotic = and(bits, conf.inboundExoticMask);
      var label = nextLabel();
//      br(ugt(exotic, "0"), $"Exotic-{label}", $"Drab-{label}");
//      println($"Exotic-{label}:");
//      store(controlPtr, anchor(control));
//      println($"  ret i1 1");
//      println($"Drab-{label}:");
      //var astruct = x.anchor!.str!;
  //    var ptrbits = and(bits, not(ptr, conf.inboundMask));
  //    var cast = inttoptr(ptrbits, astruct);
      //var cast = inttoptr(bits, astruct);
      var reach = reachable(cast);
      println($"  br {reach}, label %Reachable-{label}, label %Continue-{label}");
      println($"Reachable-{label}:");
      store(controlPtr, anchor(control));
      println($"  ret i1 1");
      println($"Continue-{label}:");
    }
    println("  ret i1 0");
    println("}\n");
  }

}
*/

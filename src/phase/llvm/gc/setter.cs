/*
public partial class LLVM {

  public void setter(Field field) {
    if (field.inbound) return;
    if (field.type!.focus.scheme != types.Scheme.GRAPH) return;
//    if (!cycles(field.type)) return;
    // TODO, no embed, what about arrays, etc
    var owner = new Pair("%owner", new llvm.StructType(field.ancestor<Struct>()!).star);
    var newValue = new Pair("%new", field.type.llvm);
    println($"define void {field.setterName}({owner}, {newValue}) {{");
    debug(newValue, " <- new");
    var fieldPtr = this.field(owner, field);
    var old = load(fieldPtr);
    debug(old, " <- old");
    br(eq(old, "%new"), "Same", "Different");
    setLabel(owner, "Same");
    println("  ret void");
    setLabel(owner, "Different");
    br(ne(old, "null"), "ReleaseOld", "StoreNew");
    setLabel(owner, "ReleaseOld");
    var oldStr = this.fieldValueStruct(field);
    var inbound = oldStr.inbound(field)!;
    clearInbound(field, old);
    var reach = reachable(old);
    br(ne(reach, "1"), "CheckDestroyOld", "StoreNew");
    setLabel(owner, "CheckDestroyOld");
    var oldControlPtr = this.controlPtr(old);
    var oldControl = load(oldControlPtr);
    br(deleting(oldControl), "StoreNew", "DestroyOld");
    setLabel(owner, "DestroyOld");
    store(oldControlPtr, delete(oldControl));
    destroy(old);
    freeGC(old);
    br("StoreNew");
    setLabel(owner, "StoreNew");
    store(fieldPtr, newValue);
    br(ne(newValue, "null"), "CheckAnchored", "Done");
    setLabel(owner, "CheckAnchored");
    var controlPtr = this.controlPtr(newValue);
    var control = load(controlPtr);
    var locals = this.locals(control);
    br(eq(locals, 0), "SetAnchored", "SetInbound");
    setLabel(owner, "SetAnchored");
    store(controlPtr, anchor(control));
    br("SetInbound");
    setLabel(owner, "SetInbound");
    setInbound(field, owner, newValue);
    br("Done");
    setLabel(owner, "Done");
    println("  ret void");
    println("}\n");
  }

  Struct fieldValueStruct(Field field) {
    return ((types.StructType)field.type).strct;
  }

  void setInbound(Field field, Pair owner, Pair newValue) {
    var str = this.fieldValueStruct(field);
    var inbound = str.inbound(field)!;
    var gep = new Gep(newValue, 0);
    gep.push(inbound.ordinal);

    var oldPtr = load(point(gep, inbound.type.llvm));
//    var oldPtr = load(point(gep, ptr));
    // var oldPacked = load(inbptr);
    // var packed = .or(.ptrtoint(owner), exotic)
    // .store(inbptr, packed)
    br(eq(oldPtr, "null"), "Done", "DealWithOldInbound");
    setLabel(owner, "DealWithOldInbound");
    // var unmasked = .and(oldPacked, .not(.ptr, .conf.inboundMask))
    // var oldPtr = .inttoptr(unmasked, owner.type)
    var f = this.field(oldPtr, field);
    store(f, Pair.nul(f.type));
//    var paramz = new List<Pair> { newValue };
    var reach = reachable(newValue);
//    var reach = call(BOOL, field.setterName, paramz);
    br(ne(reach, "1"), "CheckDestroyX", "Done");
    setLabel(owner, "CheckDestroyX");
    var controlPtr = this.controlPtr(newValue);
    var control = load(controlPtr);
    br(deleting(control), "Done", "DestroyX");
    setLabel(owner, "DestroyX");
    store(controlPtr, delete(control));
    destroy(newValue);
    freeGC(newValue);
    br("Done");
  }

  void clearInbound(Field field, Pair value) {
    var inbound = fieldValueStruct(field).inbound(field)!;
    var owner = new Pair("null", new llvm.Direct("i8*"));
    var gep = new Gep(value, 0);
    gep.push(inbound.ordinal);
    println("; HERE");
    store(point(gep, field.type.llvm), new Pair("null", field.type.llvm));
  }

  bool cycles(Type type) {
    if (!(type is types.StructType)) return false;
    return ((types.StructType)type).strct.cycles;
  }

}
*/

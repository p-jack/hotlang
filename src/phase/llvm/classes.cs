public partial class LLVM {

  public Pair invoke(Function f, IList<Pair> paramz) {
    var r = f.returnLLVM;
    if (r == "") {
      invoke(funcType(f), f.ordinal, paramz);
      return new Pair("?", VOID);
    } else {
      return invoke(r, funcType(f), f.ordinal, paramz);
    }
  }

  public Pair invoke(string ret, string funcType, int ordinal, Pair obj) {
    return invoke(ret, funcType, ordinal, new List<Pair> { obj });
  }

  public Pair invoke(string ret, string funcType, int ordinal, IList<Pair> paramz) {
    var f = methodFunc(paramz[0], funcType, ordinal);
    return call(new llvm.Direct(ret), f.name, paramz);
  }

  public void invoke(string funcType, int ordinal, Pair obj) {
    invoke(funcType, ordinal, new List<Pair> { obj });
  }

  public void invoke(string funcType, int ordinal, IList<Pair> paramz) {
    var f = methodFunc(paramz[0], funcType, ordinal);
    call(f.name, paramz);
  }

  internal Pair methodFunc(Pair obj, string funcType, int ordinal) {
    var str = obj.type.str!;
    var objGep = new Gep(obj, 0);
    objGep.push(0);
    var cls = load(point(objGep, new llvm.Direct(classType(str)).star));
    var clsGep = new Gep(cls, 0);
    clsGep.push(ordinal);
    return load(point(clsGep, new llvm.Direct(funcType)));
  }

  public Pair classPair(Struct str) {
    return new Pair(classConstant(str), new llvm.VTable(str));
  }

  public string classConstant(Struct str) {
    return $"@{str.fullName}_cls_";
  }

  public static string classType(Struct str) {
    return $"%{str.fullName}.cls.t";
  }

}

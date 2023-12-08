public partial class LLVM {

  public void emitStruct(Struct str) {
    if (str.isClass) {
      emitClass(str);
    }
    var name = str.fullName!;
    typeStream.Write($"%{name}.t = type {{ ");
    if (str.isClass) {
      typeStream.Write(classType(str));
      typeStream.Write("*");
    }
    foreach (var x in str.fields) {
      if (x.ordinal > 0) typeStream.Write(", ");
      if (x.type!.focus.scheme == types.Scheme.VALUE) {
        typeStream.Write(x.type!.llvm.unstar);
      } else {
        typeStream.Write(x.type!.llvm);
      }
    }
    typeStream.Write(" }\n");
    typeStream.Write($"%{name}.tg = type {{ {i0}, %{name}.t }}\n");
//    destructor(str);
//    reacher(str);
    // foreach (var x in str.fields.Where(x => x.type!.focus.scheme == types.Scheme.GRAPH)) {
    //   setter(x);
    // }
  }

  void emitClass(Struct str) {
    var unit = str.ancestor<Unit>()!;
    // if (str.superStruct == null) return;
    typeStream.Write($"{classType(str)} = type {{ {superType(str)}*,void(i8*)*,i1(i8*)*");
    foreach (var x in str.methods) {
      typeStream.Write($",{funcType(x)}");
    }
    typeStream.Write("}\n");
    if (unit.isHeader) {
      constantStream.Write($"{classConstant(str)} = external global {classType(str)}\n");
      return;
    }
    constantStream.Write($"{classConstant(str)} = constant {classType(str)} {{");
    if (str.superStruct == null) {
      constantStream.Write("i8* null");
    } else {
      constantStream.Write($"{classType(str.superStruct)}* {classConstant(str.superStruct)}");
    }
    var vf = "void(i8*)*";
    constantStream.Write($",{vf} bitcast (void(%{str.fullName}.t*)* {destroyFunc(str)} to {vf})");
    vf = "i1(i8*)*";
    constantStream.Write($",{vf} bitcast (i1(%{str.fullName}.t*)* {reachFunc(str)} to {vf})");
    foreach (var x in str.methods) {
      constantStream.Write($",{funcType(x)} ");
      if (x.kind == Kind.ABSTRACT) {
        constantStream.Write("null");
      } else {
        var declarer = x.ancestor<Struct>()!;
        constantStream.Write($" @{declarer.fullName}_{x.name}.m");
      }
    }
    constantStream.Write("}\n");
  }

  public string destroyFunc(Struct str) {
    return $"@{str.fullName}_d_";
  }

  public string reachFunc(Struct str) {
    return $"@{str.fullName}_r_";
  }


  string superType(Struct str) {
    if (str.superStruct == null) {
      return "i8";
    } else {
      return classType(str.superStruct);
    }
  }

  public Pair field(Pair holder, Field field) {
    return this.field(holder, field.ordinal, field.type.llvm);
    // var gep = new Gep(holder, 0);
    // gep.push(field.ordinal);
    // return point(gep, field.type.llvm);
  }

  public Pair field(Pair holder, int ordinal, llvm.Type type) {
    var gep = new Gep(holder, 0);
    gep.push(ordinal);
    return point(gep, type);
  }

}

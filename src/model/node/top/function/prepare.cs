public partial class Function:Formality {

  protected override void prepare2(Out oot) {
    locals = new List<Trunk>();
    determineParams(oot);
    checkReturn(oot);
    formals = new List<Formal>();
    for (int i = 0; i < paramz.Count(); i++) {
      Param x = paramz[i];
      x.prepare(oot);
      if (x.type is Fail) {
        oot.report(this, $"Mysterious failure on parameter {x}.");
      }
      var formal = new Formal(this, x.name, x.trunk!.type, x.nui.initialFormal(), i);
      formals.Add(formal);
    }
  }

  void determineParams(Out oot) {
    if (kind != Kind.OVERRIDE) {
      this.paramz = declaredParams;
      return;
    }
    var str = ancestor<Struct>()!;
    if (declaredParams.Count() == 0) {
      this.paramz = copySuperclassParams(oot);
    } else {
      this.paramz = verifyOverrideParams(oot);
    }
    foreach (var x in paramz) {
      if (x.parent == null) {
        adopt(x);
      }
    }
  }

  List<Param> copySuperclassParams(Out oot) {
    var result = new List<Param>();
    var method = overriding(oot);
    if (method == null) return result;
    method.prepare(oot);
    var str = ancestor<Struct>()!;
    result.Add(str.thisParam(method.thisBlur));
    for (var i = 1; i < method.paramz.Count(); i++) {
      result.Add(method.paramz[i].copy());
    }
    return result;
  }

  List<Param> verifyOverrideParams(Out oot) {
    var result = new List<Param>();
    var method = overriding(oot);
    if (method == null) return result;
    method.prepare(oot);
    var mcount = method.paramz.Count();
    var ocount = declaredParams.Count();
    if (method.paramz.Count() != declaredParams.Count()) {
      oot.report(this, $"Wrong number of paramters for override: {ocount} vs {mcount}");
      return result;
    }
    var str = ancestor<Struct>()!;
    result.Add(str.thisParam(method.thisBlur));
    for (var i = 1; i < method.paramz.Count(); i++) {
      var mp = method.paramz[i];
      var dp = declaredParams[i];
      result.Add(dp);
      dp.prepare(oot);
      if (!dp.failed && !mp.failed) {
        if (dp.name != mp.name) {
          oot.report(dp, $"Can't change name of override parameter {i+1} from {mp.name} to {dp.name}.");
        }
        IList<string> errors = mp.type.passTo(dp.type);
        if (errors.Count() > 0 || dp.type.focus != mp.type.focus) {
          oot.report(dp, $"Can't change type of override parameter {i+1} from {mp.type} to {dp.type}.");
        }
      }
    }
    return result;
  }

  Function? overriding(Out? oot) {
    for (var str = ancestor<Struct>()!.superStruct; str != null; str = str.superStruct) {
      var method = str.method(name);
      if (method == null) continue;
      if (method.kind == Kind.FUNCTION) {
        oot?.report(this, $"Can't override a static function: {method.fullName}");
        return null;
      }
      return method;
    }
    oot?.report(this, $"No superclass defines {name} to override.");
    return null;
  }

  Blur? thisBlur { get {
    return paramz[0].nui.use?.blur;
  }}

  void checkReturn(Out oot) {
    if (declaredReturn != null) {
      var unit = ancestor<Unit>()!;
      var v = new Verifier(oot, unit.symbols);
      v.push();
      declaredReturn.verify(v);
    }
    this.returnType = declaredReturn?.type;
    if (kind != Kind.OVERRIDE) {
      return;
    }
    var method = overriding(null);
    if (method == null) return;
    method.prepare(oot);
    if (method.returnType == null && declaredReturn == null) return;
    if (method.returnType == null) {
      oot.report(declaredReturn!, "Can't add return type to override.");
      return;
    }
    if (declaredReturn == null) {
      if (method.returnType.primitive) {
        this.returnType = method.returnType;
        return;
      } else {
        oot.report(declaredReturn!, $"Need to specify return type for override.");
        return;
      }
    }
    var mr = method.returnType;
    var rt = this.returnType!;
    if (rt.passTo(mr).Count() > 0 || !mr.focus.Equals(rt.focus)) {
      oot.report(declaredReturn, $"Can't change return type of override from {mr} to {rt}.");
    }
  }

}

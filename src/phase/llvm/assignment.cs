// Do the things:
//   Struct's parent field
//     - should only ever be one!!!
//   Delete old lvalue!!!
//   Destructors!!!
//     - need to do a sever
//     - also we can just call free() in the destructor now

public partial class LLVM {

  public void assignLocal(Fetch left, Expr right) {
    var scheme = left.type.focus.scheme;
    switch (scheme) {
      // case types.Scheme.GRAPH:
      //   store(left.pair, right.pair);
      //   root(right.pair);
      //   mop(new Unroot(right.pair));
      //   break;
      case types.Scheme.RC:
        store(left.pair, right.pair);
        increase(right.pair);
        mop(DestroyMop.make(right));
        break;
      case types.Scheme.UNIQUE:
        setUnique(left.pair, right, false); // TODO set first
        mop(DestroyMop.make(right));
        break;
      case types.Scheme.PRIMITIVE:
      case types.Scheme.JAILED:
      case types.Scheme.DATA:
      case types.Scheme.STACK:
        store(left.pair, right.pair);
        break;
      default:
        throw new Bad($"Unsupported scheme: {scheme}");
    }
  }

  public void assign(Expr owner, Field field, Expr right) {
    var scheme = field.type.focus.scheme;
    switch (scheme) {
      // case types.Scheme.GRAPH:
      //   setGC(owner, field, right);
      //   break;
      case types.Scheme.RC:
        if (right.type is types.Array) {
          store(this.field(owner.pair, field), right.pair);
          increase(right.pair);
        } else {
          setRC(owner.pair, field, right.pair);
        }
        break;
      case types.Scheme.UNIQUE:
        setUnique(owner, field, right);
        break;
      case types.Scheme.PRIMITIVE:
      case types.Scheme.JAILED:
      case types.Scheme.DATA:
        store(this.field(owner.pair, field), right.pair);
        break;
      default:
        throw new Bad($"Unsupported scheme: {scheme}");
    }
  }

  Field? parentField(Expr right) {
    if (!(right.type is types.StructType)) {
      return null;
    }
    var st = (types.StructType)right.type;
    return st.strct.parentField;
  }

  // TODO: Two null checks, one for rvalue to set its parent field,
  // and one for the old lvalue to destroy it. Both can frequently
  // be eliminated.

  void setUniqueLeft(Function func, Pair leftPtr, Pair right) {
    println("; setUniqueLeft");
    var lv = load(leftPtr);
    var label = nextLabel();
    var nullLabel = $"OldLeftOK-{label}";
    var notNullLabel = $"OldLeftDestroy-{label}";
    if (lv.type is llvm.Array) {
      var ptr = firstPtr(lv);
      br(eq(ptr, "null"), nullLabel, notNullLabel);
    } else {
      br(eq(lv, "null"), nullLabel, notNullLabel);
    }
    setLabel(notNullLabel);

    var mop = new DestroyMop(func, lv.type);
    mop.pair = lv;
    mop.emit(this);

    setLabel(nullLabel);
    store(leftPtr, right);
  }

  void setParent(Pair owner, Field? field, Expr right) {
    if (field == null) {
      println($"; setParent {owner} <- {right}");
    } else {
      println($"; setParent {owner}.{field.name} <- {right}");
    }
    if (right.pair.isNull) return;
    right.nullOut(this);
    var inbound = field?.inboundField;
    var parent = parentField(right);
    if (inbound != null) {
      var q = this.field(right.pair, inbound);
      store(q, owner);
    } else if (parent != null) {
      var q = this.field(right.pair, parent);
      store(q, Pair.nul(parent.type.llvm));
    }

  }

  void manageParent(Pair owner, Field? field, Expr right) {
    if (right.type is types.Array) return;
    if (field == null) {
      println($"; manageParent {owner} <- {right}");
    } else {
      println($"; manageParent {owner}.{field.name} <- {right}");
    }
    if (right.pair.isNull) return;
    // var inbound = field?.inboundField;
    // var parent = parentField(right);
    var label = nextLabel();
    var rn = $"Right-Null-{label}";
    var rnn = $"Right-Not-Null-{label}";
    br(eq(right.pair, "null"), rn, rnn);
    setLabel(rnn);
    setParent(owner, field, right);
    // right.nullOut(this);
    // if (inbound != null) {
    //   var q = this.field(right.pair, inbound);
    //   store(q, owner);
    // } else if (parent != null) {
    //   var q = this.field(right.pair, parent);
    //   store(q, Pair.nul(parent.type.llvm));
    // }
    setLabel(rn);
  }

  void setUnique(Expr owner, Field field, Expr right) {
    if (parentField(right) != null) {
      if (right.fresh) {
        setParent(owner.pair, field, right);
      } else {
        manageParent(owner.pair, field, right);
      }
    }
    var leftPtr = this.field(owner.pair, field);
    if (owner.fresh) {
      store(leftPtr, right.pair);
    } else {
      setUniqueLeft(owner.ancestor<Function>()!, leftPtr, right.pair);
    }
  }

  void setUnique(Pair left, Expr right, bool first) {
    if (!right.fresh) {
      manageParent(left, null, right);
    }
    if (first) {
      store(left, right.pair);
    } else {
      setUniqueLeft(right.ancestor<Function>()!, left, right.pair);
    }
  }

  /////

  public void assign(Pair firstPtr, Pair index, Expr value) {
    var scheme = value.type.focus.scheme;
    var elem = firstPtr.type.unstar;
    var gep = new Gep(firstPtr, index);
    var ptr = point(gep, elem);
    if (scheme == types.Scheme.RC) {
      var old = load(ptr);
      decrease(old);
    }
    switch (scheme) {
      case types.Scheme.RC:
        store(ptr, value.pair);
        increase(value.pair);
        break;
      case types.Scheme.UNIQUE:
        setUnique(ptr, value, false); // TODO, compute first
        break;
      case types.Scheme.GRAPH:
        throw new Bad();
      case types.Scheme.PRIMITIVE:
        store(ptr, value.pair);
        break;
      default:
        break;
    }
  }
}

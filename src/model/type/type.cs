using types;
using static types.Scheme;
using static types.Mutability;
using static types.Schemes; // C# hates us
using static types.Mutabilitys; // C# hates us

public abstract class Type {

  public static types.Void VOID = new types.Void(Focus.PRIMITIVE);
  public static types.Bool BOOL = new types.Bool(Focus.PRIMITIVE);
  public static types.Int UINT32 = new types.Int(Focus.PRIMITIVE, false, false, 32, 32);

  public readonly Focus focus;

  public Type(Focus focus) {
    this.focus = focus;
  }

  public virtual Access access => Access.EXPORT;
  public virtual Type element => Fail.FAIL;
  public virtual bool minty => false;
  public virtual bool aggregate => false;
  public virtual bool needsDestroy => focus.scheme.needsDestroy();
  public bool primitive => !aggregate;
  public virtual string? varName => null;
  public virtual string? plural => null;
  public virtual bool real => true;
  public virtual Type zzz => this;
  public bool pointer => focus.scheme.pointer();
  public abstract llvm.Type llvm { get; }
  public abstract string indexKey { get; }
  public virtual Microsoft.Z3.Sort z3(Microsoft.Z3.Context ctx) {
    throw new Bad($"TODO {GetType().Name}");
  }

  public bool nullable => focus.nullable;

  public override sealed bool Equals(Object? o) {
    if (o == null) return false;
    if (o.GetType() != GetType()) return false;
    Type t = (Type)o;
    return t.focus.Equals(focus) && same(t);
  }

  public override sealed int GetHashCode() {
    return hashCode;
  }

  public abstract bool same(Type t);   // Doesn't consider focus
  protected abstract int hashCode { get; } // DOES consider focus
  public abstract override string ToString();

  protected virtual IList<string> check(types.Action action, Type formal) {
    var actual = this;
    if (actual.GetType() != formal.GetType()) return mismatch(action, formal, actual);
    if (!actual.same(formal)) return mismatch(action, formal, actual);
    return actual.focus.actionTo(action, formal.focus);
  }

  public virtual IList<string> passTo(Type formal) {
    return this.check(types.Action.PASS, formal);
  }

  public virtual IList<string> assignTo(Type formal) {
    return this.check(types.Action.ASSIGN, formal);
  }

  public virtual IList<string> returnTo(Type formal) {
    return this.check(types.Action.RETURN, formal);
  }

  public virtual IList<string> constructTo(Type formal) {
    return this.check(types.Action.CONSTRUCT, formal);
  }

  public virtual Widen widen(Type other) {
    if (this.Equals(other)) return new Widen(this, other, this);
    return new Widen(this, other, Fail.FAIL);
  }

  public abstract Type refocus(Focus focus);
  public Type nullify(bool n) => refocus(focus.nullify(n));
  public Type vary(bool v) => refocus(focus.vary(v));
  public Type mutate(types.Mutability m) => refocus(focus.mutate(m));
  public Type rescheme(types.Scheme s) => refocus(focus.rescheme(s));

  public Type child(Type child) {
    if (focus.mutability == IMMUTABLE) {
      child = child.vary(false);
      if (child.focus.mutability != IMMUTABLE) {
        child = child.mutate(IMMUTABLE);
      }
    } else if (focus.mutability == LOCKED) {
      child = child.vary(false);
      if (child.focus.mutability == MUTABLE) {
        child = child.mutate(LOCKED);
      }
    }
    if (child.focus.scheme == VALUE) {
      if (focus.scheme == DATA) {
        child = child.rescheme(DATA);
      } else if (focus.scheme == STACK) {
        child = child.rescheme(STACK);
      } else {
        child = child.rescheme(JAILED);
      }
    }
    return child;
  }

  public Type inboundChild(Type child) {
    // assert .scheme == g && child.scheme == g
    // TODO: An inbound field can change suddenly from the perspective
    // of its owner. So, FINAL doesn't make sense here.
    // We could have a separate INBOUND constant for Vary?
    child = child.vary(true);
    if (focus.mutability == MUTABLE) {
      return child.mutate(MUTABLE);
    }
    // Otherwise, the inbound field's owner is either LOCKED or IMMUTABLE.
    // If locked, just do the normal thing (make it also locked.)
    // If immutability, the referent may or may not be immutability (you can add
    // an immutability outbound field to an otherwise mutability struct.)
    // So just call it locked there, too.
    return child.mutate(LOCKED);
  }

  public static IList<string> mismatch(types.Action action, Type formal, Type actual) {
    var msg = $"Can't {action} from {actual} to {formal}.";
    var result = new List<string>();
    result.Add(msg);
    return result.AsReadOnly();
  }

}

public static class Types {

  public static Type widen(this IList<Type> types) {
    var c = types.Count();
    if (c == 0) throw new Bad("TODO: Empty type");
    if (c == 1) return types[0];
    var w = types[0].widen(types[1]);
    for (int i = 2; i < c; i++) {
      w = w.result.widen(types[i]);
    }
    return w.result;
  }

}

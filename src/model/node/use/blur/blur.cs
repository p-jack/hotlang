public class Blur:Node {

  public readonly Nullability nullability;
  public readonly Variability variability;
  public readonly Mutability mutability;
  [Major] public Scheme scheme;
  public readonly Expr? size;

  public Blur(Place place, Nullability nullability, Variability variability, Mutability mutability, Scheme scheme, Expr? size) : base(place) {
    this.nullability = nullability;
    this.variability = variability;
    this.mutability = mutability;
    this.scheme = scheme;
    this.size = setNull(size);
  }

  public Blur(Place place) : base(place) {
    this.nullability = Nullability.UNKNOWN;
    this.variability = Variability.UNKNOWN;
    this.mutability = Mutability.UNKNOWN;
    this.scheme = Scheme.UNKNOWN;
    this.size = setNull<Expr>(null);
  }

  public void verify(Verifier v) {
    if (size == null) return;
    size.verify(v);
    if (size.failed) {
      failed = true;
      return;
    }
    if (!(size.type is types.Int)) {
      v.report(this, "Size hint must be an integer.");
    }
  }

  public override bool Equals(object? o) {
    if (o == null) return false;
    if (o.GetType() != typeof(Blur)) return false;
    Blur blur = (Blur)o;
    if (nullability != blur.nullability) return false;
    if (variability != blur.variability) return false;
    if (mutability != blur.mutability) return false;
    if (scheme != blur.scheme) return false;
    if (size != blur.size) return false;
    return true;
  }

  public override int GetHashCode() {
    return HashCode.Combine(nullability, variability, mutability, scheme, size);
  }

  public bool anyKnown {
    get {
      return nullability != Nullability.UNKNOWN
       || variability != Variability.UNKNOWN
       || mutability != Mutability.UNKNOWN
       || scheme != Scheme.UNKNOWN;
    }
  }

  string lenses { get {
    if (anyKnown) {
      return $":{variability.ToString2()}{mutability.ToString2()}{scheme.ToString2()}{nullability.ToString2()}";
    } else {
      return "";
    }
  }}

  public sealed override String ToString() {
    Formatter f = new Formatter(new MemoryStream());
    format(f);
    return f.ToString();
  }

  public void format(Formatter fmt) {
    if (size != null) {
      fmt.print(":#");
      size.format(fmt);
    }
    fmt.print(lenses);
  }

  public Focus focus(Focus def) {
    return new Focus(nullability.focus(def.nullable),
      variability.focus(def.variable),
      mutability.focus(def.mutability),
      scheme.focus(def.scheme));
  }

  public bool validForPrimitive {
    get {
      if (nullability != Nullability.UNKNOWN) return false;
      if (mutability != Mutability.UNKNOWN) return false;
      if (scheme != Scheme.UNKNOWN) return false;
      return true;
    }
  }

}

public partial class In {

  public Blur blur {
    get {
      var place = this.place;
      var n = Nullability.UNKNOWN;
      var v = Variability.UNKNOWN;
      var m = Mutability.UNKNOWN;
      var s = Scheme.UNKNOWN;
      Expr? size = null;
      if (token() == ":#") {
        expect(":#", Flavor.BLUR);
        size = mustExpr;
      }
      if (peek != ':') return new Blur(place, n, v, m, s, size);
      expect(":", Flavor.BLUR);
      for (char ch = peek; Char.IsLetter(ch); ch = peek) {
        if (ch.variability() != Variability.UNKNOWN) {
          if (v != Variability.UNKNOWN) throw new Bad($"{place}: Variability specified more than once.");
          v = this.variability;
        } else if (ch.mutability() != Mutability.UNKNOWN) {
          if (m != Mutability.UNKNOWN) throw new Bad($"{place}: Mutability specified more than once.");
          m = this.mutability;
        } else if (ch.nullability() != Nullability.UNKNOWN) {
          if (n != Nullability.UNKNOWN) throw new Bad($"{place}: Nullability specified more than once.");
          n = this.nullability;
        } else if (ch.scheme() != Scheme.UNKNOWN) {
          if (s != Scheme.UNKNOWN) throw new Bad($"{place}: Scheme specified more than once.");
          s = this.scheme;
        } else {
          throw new Bad($"{place}: Invalid lens - {ch}.");
        }
      }
      return new Blur(place, n, v, m, s, size);
    }
  }

}

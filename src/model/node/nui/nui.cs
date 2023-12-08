public class NUI:Node {

  public readonly string? name;
  public readonly Use? use;
  public readonly Expr? initial;

  internal string? resolvedName { get; set; }
  Type? resolvedType { get; set; }

  public NUI(Place place, string? name, Use? use, Expr? initial) : base(place) {
    this.name = name;
    this.use = setNull(use);
    this.initial = setNull(initial);
  }

  public Expr? initialCopy() {
    if (initial == null) return null;
    return initial.copy() as Expr;
  }

  public Expr? initialFormal() {
    var result = initialCopy()!;
    if (!embeddedNew) return result;
    var n = ((New)result);
    n.blur.scheme = Scheme.BRAND_NEW;
    return n;
  }

  bool embeddedNew { get {
    if (initial == null) return false;
    if (name != null) return false;
    if (use != null) return false;
    if (!(initial is New)) return false;
    var n = (New)initial;
    return n.blur.scheme == Scheme.VALUE;
  }}

  public string resolveName(Out oot) {
    if (resolvedName == null) {
      resolvedName = resolveName2(oot);
    }
    return resolvedName;
  }

  string resolveName2(Out oot) {
    if (name != null) return name;
    var type = resolveType(oot);
    var result = type.varName;
    if (result == null) {
      oot.report(this, "No name.");
      return "???";
    }
    return result;
  }

  public Type resolveType(Out oot) {
    if (resolvedType == null) {
      resolvedType = resolveType2(oot);
    }
    return resolvedType;
  }

  private Type resolveType2(Out oot) {
    var unit = this.ancestor<Unit>();
    if (unit == null) {
      oot.report(this, "no unit");
      return Fail.FAIL;
    }
    var v = new Verifier(oot, unit.symbols);
    if (use != null) {
      use.verify(v);
      return use.type!;
    }
    if (initial == null) {
      oot.report(this, "no type");
      return Fail.FAIL;
    }
    if (ancestor<Function>() != null) {
      // TODO what? why not? don't locals need this?
      throw new Bad("Can't call nui.type(oot) from within a function!");
    }

    // TODO, need a better way to do this
    // Might need a "valid initializer" pass on Expr
    var init = (initial.copy() as Expr)!;
    var scrap = new Scrap(place, init, true);
    var empty = new List<Param>();
    var f = new Function(place, Access.FILE, Kind.FUNCTION, "_", empty, null, scrap.wrap);
    unit.fake = f;
    f.setSupers(oot);
    f.analyze(oot);
    return init.type;
  }

  internal bool defaultedToUnique { get {
    if (resolvedType!.focus.scheme != types.Scheme.UNIQUE) return false;
    if (use == null) return true;
    return use.blur.scheme == Scheme.UNKNOWN;
  }}

  public override string ToString() {
    return $"[name:{name} use:{use} initial:{initial} place:{place}]";
  }

  public void format(Formatter f) {
    if (name == "this" && use is This && initial == null) {
      if (use.blur.anyKnown) {
        f.print(use.blur.ToString());
      } else {
        f.print(":");
      }
      return;
    }
    var needsSpace = false;
    if (name != null) {
      needsSpace = true;
      f.print(name);
    }
    if (use != null) {
      if (needsSpace) f.print(" ");
      needsSpace = true;
      f.print(use.ToString());
    }
    if (initial != null) {
      if (needsSpace) {
        f.print(" = ");
      } else {
        f.print("=");
      }
      initial.format(f);
    }
  }

}

public partial class In {

  public NUI? nui {
    get {
      var place = skip();
      if (peek == '=') {
        expect('=', Flavor.OPERATOR);
        return new NUI(place, null, null, mustExpr);
      }
      if (peek == ':') {
        return new NUI(place, "this", new This(place, blur), null);
      }
      mark();
      string? name = null;
      if (isLetter(peek)) {
        name = id();
        if (peek == ':') {
          name = null;
          recall();
        }
      }
      var use = this.use;
      // if (use == null && name != null) {
      //   use = new Named(place, new Blur(place), name);
      //   name = null;
      // }
      if (skipToEOL()) {
        if (use == null) return new NUI(place, name, use, null);
        return new NUI(place, name, use, null);
      }
      if (peek == '=') {
        expect('=', Flavor.OPERATOR);
        var e = mustExpr;
        return new NUI(place, name, use, e);
      }
      return new NUI(place, name, use, null);
    }
  }

  public IList<NUI> nuis {
    get {
      var result = new List<NUI>();
      var n = nui;
      if (n == null) return result.AsReadOnly();
      result.Add(n);
      var place = skip();
      while (peek == ',') {
        expect(",", Flavor.OPERATOR);
        n = nui;
        if (n == null) {
          throw new Bad($"{place}: expected name, type, or initializer");
        }
        result.Add(n);
        place = skip();
      }
      return result.AsReadOnly();
    }
  }

}

public static class NUIs {

  public static void format(this IEnumerable<NUI> nuis, Formatter fmt) {
    var first = true;
    foreach (var x in nuis) {
      if (first) {
        first = false;
      } else {
        fmt.print(", ");
      }
      x.format(fmt);
    }
  }

}

/*



test() {
  piece_test()
}





-next(nuis [NUI], index int) int {
  let first = nuis[index]
  for let i = index + 1; i < nuis.length; i++ {
    let nui = nuis[i]
    let use = first.use != nui.use
    let initial = first.initial != nui.initial
    if use || initial {
      return i
    }
  }
  return nuis.length
}


+nameOnly(:) bool {
  return false if .name is null
  return false if .use not null
  return .initial is null
}

+all_nameOnly(nuis [NUI]) bool {
  return false if nuis.length != 1
  return nuis[0].nameOnly
}


*/

public class Nest:Top {

  public readonly IList<Top> tops;
  public readonly bool root;

  public Nest(Place place, List<Top> tops, bool root) : base(place) {
    this.tops = set(tops);
    this.root = root;
  }

  public override bool keepWithNext(Top next) {
    // TODO, commenting a field is common
    if (!onlyFields) return false;
    if (next is Field) return true;
    return next is Nest && ((Nest)next).onlyFields;
  }

  /////

  // Fields declared in this nest and any sub-nests.
  public List<Field> fields { get {
    var list = new List<Field>();
    collectFields(list);
    return list;
  }}

  void collectFields(List<Field> result) {
    foreach (var top in tops) {
      if (top is Field) {
        result.Add((Field)top);
      } else if (top is Nest) {
        ((Nest)top).collectFields(result);
      }
    }
  }

  // Methods declared in this nest and any sub-nests.
  public List<Function> methods { get {
    var list = new List<Function>();
    collectMethods(list);
    return list;
  }}

  void collectMethods(List<Function> result) {
    foreach (var x in tops) {
      if (isMethod(x)) {
        result.Add((Function)x);
      } else if (x is Nest) {
        ((Nest)x).collectMethods(result);
      }
    }
  }

  bool isMethod(Top x) {
    if (!(x is Function)) return false;
    var f = (Function)x;
    return f.kind == Kind.METHOD || f.kind == Kind.ABSTRACT || f.kind == Kind.OVERRIDE;
  }

  /////

  protected override void index2(Out oot) {
    foreach (var x in tops) x.index(oot);
  }

  protected override void setSupers2(Out oot) {
    foreach (var x in tops) x.setSupers(oot);
  }

  protected override void setMembers2(Out oot) {
    foreach (var x in tops) x.setMembers(oot);
  }

  public override void detectCycles(Out oot) {
    foreach (var x in tops) x.detectCycles(oot);
  }

  protected override void prepare2(Out oot) {
    foreach (var x in tops) x.prepare(oot);
  }

  protected override void analyze2(Out oot) {
    foreach (var x in tops) x.analyze(oot);
  }

  protected override void solve2(Out oot) {
    foreach (var x in tops) x.solve(oot);
  }

  public override void emit(LLVM llvm) {
    foreach (var x in tops) x.emit(llvm);
  }

  /////

  public override string ToString() {
    return $"Nest({tops.Count()})";
  }

  public override void format(Formatter fmt) {
    if (tops.Count() == 0) {
      fmt.println("{}");
      return;
    }
    if (onlyFields && !root) {
      formatFields(fmt);
      return;
    }
    fmt.println("{");
    var condensed = this.condensed;
    if (!condensed) {
      fmt.println("");
    }
    fmt.level++;
    foreach (var x in tops) {
      fmt.indent();
      x.format(fmt);
      if (!condensed) {
        fmt.println("");
      }
    }
    fmt.level--;
    fmt.indent();
    fmt.println("}");
  }

  void formatFields(Formatter fmt) {
    if (tops.Count() == 1) {
      var field = (Field)tops[0];
      field.format(fmt);
      return;
    }
    var nuis = tops.OfType<Field>().Select(x => x.nui).ToList();
    nuis.format(fmt);
  }

  bool condensed => tops.All(x => canCondense(x));

  bool canCondense(Top top) {
    if (top is Field) return true;
    return top is Nest && ((Nest)top).onlyFields;
  }

  public bool onlyFields => tops.All(x => x is Field && !((Field)x).inbound);

  static string[] full() => new string[] { "{" };

}

public partial class In {

  public Nest? getNest(bool root) {
    var place = skip();
    if (peek != '{') return null;
    expect("{", Flavor.BRACE);
    var tops = new List<Top>();
    for (var top = this.top; top != null; top = this.top) {
      tops.Add(top);
    }
    skip();
    expect("}", Flavor.BRACE);
    return new Nest(place, tops, root);
  }

  public Nest? nest { get {
    return getNest(false);
    // var place = skip();
    // if (peek != '{') return null;
    // expect("{", Flavor.BRACE);
    // var tops = new List<Top>();
    // for (var top = this.top; top != null; top = this.top) {
    //   tops.Add(top);
    // }
    // skip();
    // expect("}", Flavor.BRACE);
    // return new Nest(place, tops);
  }}

}

/*

members(in Input:m) Nest:r {
  let place = in.skip()
  in.expect("{", BRACE)
  let members ^[Top]:r = ^[]
  in.skip()
  while in.peek != '}' {
    let member = in.member
    if member is null {
      STDOUT.println("${in.place}: Expected member.")
      throw PARSE
    }
    members.add(member)
    in.skip()
  }
  in.expect("}", BRACE)
  return Nest_new(place, EXPORT, members)
}


=override(Scope:m, methods[Function]:m, [Node]) {
  nodes.each:{
    if x /? Function {
      override2(scope, methods, x/!Function)
    } else if x /? Nest {
      override(scope, methods, x/!Nest.tops)
    }
  }
}

-override2(Scope:m, methods [Function]:m, override Function:mr) {
  for let i = 0; i < methods.length; i++ {
    if methods[i].name == override.name {
      methods[i] = override
      return
    }
  }
  scope.out.report(override, BadOverride{})
}





-class Issue/scope_Issue {
  function(:) Function:l { return .source/!Function }
}

:( BadOverride en "No superclass of ${.function.struct.name} provides a method named ${.function.name}."
*/

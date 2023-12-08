public class Named:Use {

  public readonly string name;

  public Named(Place place, Blur blur, string name) : base(place, blur) {
    this.name = name;
  }

  public override bool primitive => false;

  public override string ToString() => name + blur;

  protected override Type resolve(Verifier v) {
    // TODO, support enums when they exist
    var nodes = v.symbols.types(name).OfType<Struct>();
    var c = nodes.Count();
    if (c == 0) {
      v.report(this, $"No such type: {name}");
      return Fail.FAIL;
    }
    if (c > 1) {
      var names = nodes.Select(x => x.fullName);
      v.report(this, $"Ambiguous type. {name} might mean: {names}");
      return Fail.FAIL;
    }
    var str = nodes.First()!;
    str.used = true;
    var defaults = str.defaultFocus(v.oot);
    return new types.StructType(blur.focus(defaults), str);
  }

}

public partial class In {

  public Named? named { get {
    var place = skip();
    if (!isLetter(peek)) return null;
    var name = id();
    var blur = this.blur;
    return new Named(place, blur, name);
  }}

}

/*

parsed {
  name string
}

resolve after {
  let nodes = scope.symbols.types(.name)
  let matches ^[Top]:r = ^[]
  nodes.each:{
    if x/?Struct {
      matches.add(x/!Struct)
    }
    // TODO check for enum here
  }
  if matches.length == 0 {
    scope.out.report(this, NoSuch{})
    return fail()
  }
  if matches.length > 1 {
    let names ^[string]:r = ^[]
    matches.each:{ names.add(x.fullName) }
    scope.out.report(this, Ambiguous{links: matches})
    return fail()
  }
  // TODO, check for enum here, handle focus
  let struct = matches[0]/!Struct
  struct.used = true
  let focus = .focus
  let result = Named{focus: .focus, struct: struct}
  return result
}

-class Issue/scope_Issue {
  .name(:) string { return .source/!NamedUse.name }
}

:( NoSuch en "No such type: $.name."
:( Ambiguous en "Ambiguous type: $.name. It may refer to:"

*/

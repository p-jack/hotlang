public class Error:Formality {

  public readonly long value;

  Trunk? _trunk = null;

  public Error(Place place, Access access, string name, long value) : base(place, access) {
    this.name = name;
    this.value = value;
  }

  public override Trunk? trunk => _trunk;

  internal override IList<string> check(Type formal, Type actual) {
    return new List<string>();
  }

  protected override void index2(Out oot) {
    index(true, name);
  }

  protected override void analyze2(Out oot) {
    if (value == 0) {
      oot.report(this, "Can't use zero as an error code!");
      return;
    }
    var type = new types.Error(Focus.primitive(false), oot.conf.intBits);
    _trunk = Trunk.forError($"{value}", type);
  }

  public override void emit(LLVM llvm) {
    // nop
  }

  public override bool keepWithNext(Top next) => next is Error;

  protected override void format2(Formatter fmt) {
    fmt.println(ToString());
  }

  public override string ToString() {
    return $"{name} error {value}";
  }

  static string[] full() => new string[] { "error" };

}

public partial class In {

  public Error? error { get {
    var place = skip();
    var access = this.access;
    skip();
    var name = id();
    skip();
    expect("error", Flavor.KEYWORD);
    return new Error(place, access, name, mustInt64);
  }}

}

/*

/index {
  index(this, true, .name)
}

prepare after {
  // nop
}

analyze after {
}

emit after {
  // nop
}

/trunk {
  return this->trunk
}

full error

parse {
  let place = in.skip()
  in.expect("error", KEYWORD)
  in.skip()
  let name = in.id
  in.skip()
  let value = in.int64
  return Error_new(place, access, name, value)
}

/printTo {
  writer.print("error $.name $.value")
}

*/

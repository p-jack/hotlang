public class Truth:Head {

  public readonly bool value;

  public Truth(Place place, bool value) : base(place) {
    this.value = value;
  }

  protected override Type resolve(Verifier v) {
    return new types.Bool(Focus.PRIMITIVE);
  }

  internal override ZZZ mk(Solva solva) {
    return new zzz.Bool(value);
  }

  protected override Pair toPair(LLVM llvm) {
    return new Pair(value ? "1" : "0", LLVM.BOOL);
  }

  public override void format(Formatter fmt) {
    fmt.print(value ? "true" : "false");
  }

  static string[] full() => new string[] { "true", "false" };

}

public partial class In {

  public Truth? truth { get {
    var place = skip();
    bool value;
    if (token() == "true") {
      expect("true", Flavor.DIGIT);
      value = true;
    } else if (token() == "false") {
      expect("false", Flavor.DIGIT);
      value = false;
    } else {
      return null;
    }
    if (peek == 't') {
    } else {
    }
    return new Truth(place, value);
  }}

}

/*

parse {
}

/format {
  fmt.print("$.value")
}


*/

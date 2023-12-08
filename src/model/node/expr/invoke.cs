public class Invoke:Tail {

  public readonly string name;
  public readonly IList<Actual> actuals;

  public Invoke(Place place, Expr holder, string name, List<Actual> actuals) : base(place, holder) {
    this.name = name;
    this.actuals = set(actuals);
  }

  internal Match? match = null;
  internal Function function => (Function)match!.node;

  protected override Type resolve(Verifier v) {
    holder.verify(v);
    if (holder.failed) return Fail.FAIL;
    var m = matchMethod(v, holder, name, actuals);
    foreach (var x in m.errors) {
      v.report(this, x);
    }
    if (failed) return Fail.FAIL;
    match = m;
    return function.returnType ?? Type.VOID;
  }

  public static Match matchMethod(Verifier v, Expr holder, string name, IList<Actual> actuals) {
    if (holder.failed) {
      return new Match(holder, $"Could not determine type of object. {holder.GetType()}:{holder.type}");
    }
    if (!(holder.type is types.StructType)) {
      return new Match(holder, $"Can only use +> on classes, not {holder.type}.");
    }
    var cls = ((types.StructType)holder.type).strct;
    if (!cls.isClass) {
      return new Match(holder, $"Can only use +> on classes and structs, not {holder.type}.");
    }
    var method = cls.method(name);
    if (method == null) {
      return new Match(holder, $"No such method: {cls.name}.{name}");
    }
    if (!cls.allows(holder)) {
      return new Match(holder, $"{cls.fullName} has {cls.access} access, so it can't be accessed here.");
    }
    if (!method.allows(holder)) {
      return new Match(holder, $"{cls.name}.{name} has {method.access} access, so it can't be accessed here.");
    }
    var withThis = new List<Actual>();
    withThis.Add(new Actual(holder.place, "", new Dupe(holder)));
    withThis.AddRange(actuals);
    var match = new Match(method, withThis);
    match.run(v);
    return match;
  }

  /////

  internal override ZZZ mk(Solva solva) {
    throw new Bad("");
  }

  protected override Pair toPair(LLVM llvm) {
    holder.emit(llvm);
    List<Pair> pairs = new List<Pair>();
    foreach (var x in match!.actuals) {
      x.emit(llvm);
      pairs.Add(x.pair);
    }
    var r = llvm.invoke(function, pairs);
    return llvm.handleError(function, r.name);
  }

  /////

  public override void format(Formatter fmt) {
    holder.format(fmt);
    fmt.print("+>");
    fmt.print(name);
    fmt.print("(");
    actuals.format(fmt);
    fmt.print(")");
  }

  static string full() => "+>";

}

public partial class In {

  public Invoke invoke(Expr holder) {
    var place = skip();
    expect("+>", Flavor.OPERATOR);
    var name = id();
    var actuals = this.actuals('(', ')');
    return new Invoke(place, holder, name, actuals);
  }

}

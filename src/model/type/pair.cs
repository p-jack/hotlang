/*
namespace types {

// TODO: Override widen so class:class works
internal class Pair:Type {

  public readonly Type key;
  public readonly Type value;

  public Pair(Focus focus, Type key, Type value) : base(focus) {
    this.key = key;
    this.value = value;
  }

  public override Microsoft.Z3.Sort z3(Microsoft.Z3.Context ctx) {
    throw new Bad();
  }

  public override string indexKey => "pair"; // TODO
  public override llvm.Type llvm { get { throw new Bad(); }}
  public override Pair refocus(Focus f) => new Pair(f, key, value);
  public override string ToString() => key + ":" + value;
  protected override int hashCode => HashCode.Combine(key, value);
  public override bool same(Type t) {
    if (t.GetType() != typeof(Pair)) return false;
    Pair p = (Pair)t;
    return p.key.Equals(key) && p.value.Equals(value);
  }

}

}
*/

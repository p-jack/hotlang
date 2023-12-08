public class Gep {

  Pair agg;
  List<Pair> indices;

  public Gep(Pair agg, Pair firstIndex) {
    this.agg = agg;
    this.indices = new List<Pair>();
    indices.Add(firstIndex);
  }

  public Gep(Pair agg, int firstIndex) {
    this.agg = agg;
    this.indices = new List<Pair>();
    indices.Add(new Pair($"{firstIndex}", new llvm.Int(32)));
  }

  public void push(int index) {
    indices.Add(new Pair($"{index}", new llvm.Int(32)));
  }

  public void push(Pair index) {
    indices.Add(index);
  }

  public void pop() {
    indices.RemoveAt(indices.Count() - 1);
  }

  public void print(StreamWriter w, string var) {
    w.Write($"  {var} = getelementptr {agg.type.unstar}, {agg}");
    foreach (var x in indices) {
      w.Write($", {x}");
    }
    w.Write("\n");
  }

}

public partial class LLVM {

  public Pair point(Gep gep, llvm.Type type) {
    var name = nextVar();
    var result = new Pair(name, type);
    gep.print(funcStream ?? codeStream, name);
    return result;
  }

  public void store(Gep gep, Pair pair, int index) {
    gep.push(index);
    var lvalue = point(gep, pair.type);
    store(lvalue, pair);
    gep.pop();
  }
}

public abstract class Mop {

  public abstract void emit(LLVM llvm);

}

/*
public class Destroy:Mop {

  Pair pair;

  public Destroy(Pair pair) {
    this.pair = pair;
  }

  public override void emit(LLVM llvm) {
    llvm.destroy(pair);
  }

}

public class Free:Mop {

  Pair pair;

  public Free(Pair pair) {
    this.pair = pair;
  }

  public override void emit(LLVM llvm) {
    llvm.destroy(pair);
    llvm.free(pair);
  }

}

public class FreeIf:Mop {

  Pair pair;

  public FreeIf(Pair pair) {
    this.pair = pair;
  }

  public override void emit(LLVM llvm) {
    var l = llvm.nextLabel();
    var isNull = $"IsNull-{l}";
    var notNull = $"NotNull-{l}";
    var loaded = llvm.load(pair);
    var cond = llvm.eq(loaded, "null");
    llvm.br(cond, isNull, notNull);
    llvm.setLabel(notNull);
    llvm.destroy(loaded);
    llvm.free(loaded);
    llvm.setLabel(isNull);
  }

}
*/

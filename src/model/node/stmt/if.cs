public class If:Stmt {

  public Expr condition;
  public Block block;
  public Block? next;

  public If(Place place, Expr condition, Block block, Block? next) : base(place) {
    this.condition = set(condition);
    this.block = set(block);
    this.next = setNull(next);
  }

  public override bool big => true;
  public override bool terminates { get {
    if (next == null) return false;
    return block.terminates && next.terminates;
  }}

  public override void verify(Verifier v) {
    condition.verify(v);
    if (condition.failed) return;
    if (!(condition.type is types.Bool)) {
      v.report(this, "Conditions must have the boolean type.");
    }
    block.verify(v);
    next?.verify(v);
  }

  bool solveCondition(Solva solva) {
    if (solva.logicFailed) return false;
    var logic = solva.solve(condition);
    if (logic == Solved.TRUE) {
      solva.report(this, "Condition is always true.");
      return false;
    } else if (logic == Solved.FALSE) {
      solva.report(this, "Condition is always false.");
      return false;
    }
    return true;
  }

  public override void solve(Solva solva) {
    if (!solveCondition(solva)) {
      return;
    }
    solva.imply(condition);
    block.solve(solva);
    solva.beMoreDirect();
    if (next != null) {
      solva.implyNot(condition);
      next.solve(solva);
      solva.beMoreDirect();
    }
    var nextTerminates = (next == null) ? false : next.terminates;
    if (block.terminates && !nextTerminates) {
      solva.implyNot(condition);
    } else if (!block.terminates && nextTerminates) {
      solva.imply(condition);
    }
  }

  public override void emit(LLVM llvm) {
    var label = llvm.nextLabel();
    var goOn = $"If{label}-Continue";
    var truth = $"If{label}-True";
    var fiction = (next == null) ? goOn : $"If{label}-False";
    condition.emit(llvm);
    llvm.br(condition.pair!, truth, fiction);
    emit(llvm, truth, block, goOn);
    if (next != null) {
      emit(llvm, fiction, next, goOn);
    }
    if (!terminates) {
      llvm.setLabel(goOn);
    }
  }

  static void emit(LLVM llvm, string label, Block block, string goOn) {
    llvm.setLabel(label);
    block.emit(llvm);
    if (!block.terminates) {
      llvm.br(goOn);
    }
  }

  public override void format(Formatter fmt) {
    if (block.stmts.Count() == 1 && next == null) {
      var first = block.stmts[0];
      if (!first.big) {
        first.format(fmt);
        fmt.print(" if ");
        condition.format(fmt);
        return;
      }
    }
    fmt.print("if ");
    condition.format(fmt);
    fmt.print(" ");
    block.format(fmt);
    if (next == null) return;
    fmt.print(" else ");
    if (next.stmts.Count() == 1) {
      var f = next.stmts[0];
      if (f is If) {
        f.format(fmt);
        return;
      }
    }
    next.format(fmt);
  }

  static string parserName() => "ifStmt";
  static string[] full() => new string[] { "if" };

}

public partial class In {

  public If? ifStmt { get {
    var place = skip();
    if (token() != "if") return null;
    expect("if", Flavor.KEYWORD);
    var condition = mustExpr;
    var block = this.block;
    Block? next = null;
    skip();
    if (token() == "else") {
      expect("else", Flavor.KEYWORD);
      skip();
      if (token() == "if") {
        next = ifStmt!.wrap;
      } else {
        next = this.block;
      }
    }
    return new If(place, condition, block, next);
  }}

}

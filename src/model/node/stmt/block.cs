public class Block:Stmt {

  public readonly bool inline;
  public readonly IList<Stmt> stmts;

  public bool manualScope = false;

  public Block(Place place, bool inline, List<Stmt> stmts) : base(place) {
    this.inline = inline;
    this.stmts = set(stmts);
  }

  public void add(Stmt stmt) {
    stmts.Add(stmt);
    kids_.Add(stmt);
    stmt.newParent(this);
  }

  public bool onlyReturns { get {
    if (stmts.Count() != 1) return false;
    if (!(stmts[0] is Return)) return false;
    return ((Return)stmts[0]).expr != null;
  }}

  public bool empty => !stmts.Any();
  public Stmt last => stmts.Last(); // TODO skip comments

  public override bool big => true;

  public override bool gives { get {
    if (empty) { return false; }
    return last.gives;
  }}

  public override bool terminates { get {
    if (empty) { return false; }
    return last.terminates;
  }}

  /////

  public override void verify(Verifier v) {
    if (!manualScope && !inline) { v.push(); }
    if (stmts.Count() == 0) {
      v.report(this, "Empty block.");
      return;
    }
    var ended = false;
    foreach (var x in stmts) {
      if (ended && !x.comment) {
        v.report(x, "Unreachable code.");
        return;
      }
      x.verify(v);
      if (x.endsBlock) { ended = true; }
    }

    if (!manualScope && !inline) { v.pop(); }
  }

  public override void solve(Solva solva) {
    foreach (var x in stmts) {
      if (solva.logicFailed) return;
      x.solve(solva);
    }
  }

  /////

  public override void emit(LLVM llvm) {
    if (!inline) llvm.enterRoom();
    foreach (var s in stmts) {
      s.emit(llvm);
    }
    if (!inline) llvm.leaveRoom(!manualScope);
  }


  /////

  public override void format(Formatter fmt) {
    fmt.println(inline ? "|{" : "{");
    fmt.level++;
    foreach (var x in stmts) {
      fmt.indent();
      x.format(fmt);
      fmt.println("");
    }
    fmt.level--;
    fmt.indent();
    fmt.print("}");
  }

  static string[] full() => new []{"{"};

}

public partial class In {

  public Block block { get {
    var place = skip();
    var stmts = new List<Stmt>();
    bool inline;
    if (peek == '|') {
      expect("|", Flavor.BRACE);
      inline = true;
    } else {
      inline = false;
    }
    expect("{", Flavor.BRACE);
    for (var s = stmt; s != null; s = stmt) {
      stmts.Add(s);
    }
    skip();
    expect("}", Flavor.BRACE);
    return new Block(place, inline, stmts);
  }}

}

/*


*+node Block/Stmt

parsed { inline bool }
major { ^[Stmt] }
=manualScope bool = false

+empty(Place:r) Block:r {
  let stmts ^[Stmt] = ^[]
  return Block_new(place, false, stmts)
}

add(:mr, Stmt:mr) {
  .stmts.add(stmt)
  stmt.parent = this
}

block_copy(:n) Block:rn {
  return null if this is null
  return this+>copy()/!Block
}

/terminates {
  return false if .stmts.count == 0
  return .stmts.last.terminates
}

/gives {
  return .stmts.count > 0 && .stmts.last.gives
}

full { |{

parse {
  return in.block
}

+block(in Input:m) Block:r {
  let place = in.skip()
  let inline bool
  let stmts ^[Stmt]:r = []
  if in.peek == '|' {
    in.expect("|", BRACE)
    inline = true
  } else {
    inline = false
  }
  in.expect("{", BRACE)
  for let s = in.stmt; s not null; s = in.stmt {
    stmts.add(s)
  }
  in.skip()
  in.expect("}", BRACE)
  return Block_new(place, inline, stmts)
}

+wrap(Stmt:mr) Block:r {
  let stmts ^[Stmt]:r = ^[stmt]
  return Block_new(stmt.place, false, stmts)
}

/format {
  fmt.println(.inline ? "|{" : "{")
  fmt.level++
  .stmts.each:{
    fmt.indent()
    x.format(fmt)
    fmt.println("")
  }
  fmt.level--
  fmt.indent()
  fmt.print("}")
}



*/

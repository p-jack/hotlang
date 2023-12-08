public abstract class Top:Node {

  public Phase phase { get; internal set; }

  public Top(Place place) : base(place) {}

  internal static bool allows(Access access, Top top, Node n) {
    if (access == Access.FILE) {
      var nunit = n.ancestor<Unit>()!;
      var myUnit = top.ancestor<Unit>()!;
      return ReferenceEquals(nunit, myUnit);
    }
    if (access == Access.FOLDER) {
      var nfolder = n.ancestor<Folder>()!;
      var myFolder = top.ancestor<Folder>()!;
      return ReferenceEquals(nfolder, myFolder);
    }
    return true;
  }


  public abstract bool keepWithNext(Top next);

  public abstract void format(Formatter f);

  // Phases

  public void index(Out oot) {
    if (failed) throw new Bad("can't index failed node");
    if (phase >= Phase.INDEXED) return;
    index2(oot);
    phase = Phase.INDEXED;
  }

  protected abstract void index2(Out oot);

  public void setSupers(Out oot) {
    if (failed) throw new Bad("can't set superclass on failed node");
    if (phase >= Phase.SUPERED) return;
    if (phase == Phase.SUPERING) throw new Bad("define loop");
    phase = Phase.SUPERING;
    setSupers2(oot);
    phase = Phase.SUPERED;
  }

  protected virtual void setSupers2(Out oot) {}

  public void setMembers(Out oot) {
    if (failed) throw new Bad("can't set members on failed node");
    if (phase < Phase.SUPERED) throw new Bad($"call setSupers on everything before calling setMembers {GetType().Name} {phase} {place}");
    if (phase >= Phase.MEMBERED) return;
    oot.push(this);
    if (phase == Phase.MEMBERING) throw new Bad("define loop");
    phase = Phase.MEMBERING;
    setMembers2(oot);
    phase = Phase.MEMBERED;
    oot.pop();
  }

  protected virtual void setMembers2(Out oot) {}

  public void prepare(Out oot) {
    if (phase < Phase.MEMBERED) setMembers(oot);
    if (failed) throw new Bad("can't prepare a failed node");
    if (phase >= Phase.PREPARED) return;
    oot.push(this);
    if (phase == Phase.PREPARING) throw new Bad("define loop");
    phase = Phase.PREPARING;
    prepare2(oot);
    phase = Phase.PREPARED;
    oot.pop();
  }

  protected virtual void prepare2(Out oot) {}

  public void analyze(Out oot) {
    if (phase < Phase.PREPARED) {
      prepare(oot);
    }
    if (failed) throw new Bad("can't analyze a failed node");
    if (phase >= Phase.VERIFIED) return;
    oot.push(this);
    if (phase == Phase.VERIFYING) throw new Bad("Define loop");
    phase = Phase.VERIFYING;
    analyze2(oot);
    phase = Phase.VERIFIED;
    oot.pop();
  }

  protected abstract void analyze2(Out oot);

  public void recurse(Out oot) {
    if (phase < Phase.VERIFIED) analyze(oot);
    if (failed) throw new Bad("can't recurse a failed node");
    if (phase >= Phase.RECURSED) return;
    if (phase == Phase.RECURSING) throw new Bad("Define loop");
    phase = Phase.RECURSING;
    recurse2(oot);
    phase = Phase.RECURSED;
  }

  protected virtual void recurse2(Out oot) {}

  public void solve(Out oot) {
    if (failed) throw new Bad("can't solve a failed node");
    if (phase < Phase.RECURSED) throw new Bad("can't solve a non-recursed node");
    if (phase >= Phase.SOLVED) return;
    if (phase == Phase.SOLVING) throw new Bad("define loop");
    phase = Phase.SOLVING;
    solve2(oot);
    phase = Phase.SOLVED;
  }

  protected virtual void solve2(Out oot) {}

  public virtual void detectCycles(Out oot) {}

  public abstract void emit(LLVM llvm);

}

public partial class In {

  public Top? top { get {
    skip();
    mark();
    var access = this.access;
    if (peek == '.' || peek == '/' || peek == '\\') {
      // TODO, should just integrate this directly in core syntax
      recall();
      return this.function;
    }
    var raw = this.parser(syntax.rawTops);
    if (raw != null) {
      recall();
      return raw(this, null);
    }
    if (peek == '=') return fields;
    if (!Char.IsLetter(peek)) {
      recall();
      return null;
    }
    var name = this.id();
    skip();
    var named = this.parser(syntax.namedTops);
    if (named != null) {
      recall();
      return named(this, null);
    }
    recall();
    return fields;
  }}

}

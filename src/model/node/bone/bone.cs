public abstract class Bone:Node {

  public abstract Symbols symbols { get; }

  public Bone() : base(new Place("", 0, 0)) {}

  public virtual void format() {
    foreach (var k in kids.OfType<Bone>()) {
      k.format();
    }
  }

  public virtual void formatHeader(Formatter fmt) {
    foreach (var k in kids.OfType<Bone>()) {
      k.formatHeader(fmt);
    }
  }

  public virtual void index(Out oot) {
    foreach (var k in kids.OfType<Bone>()) {
      k.index(oot);
    }
  }

  public virtual void setSupers(Out oot) {
    foreach (var k in kids.OfType<Bone>()) {
      k.setSupers(oot);
    }
  }

  public virtual void setMembers(Out oot) {
    foreach (var k in kids.OfType<Bone>()) {
      k.setMembers(oot);
    }
  }

  public virtual void prepare(Out oot) {
    foreach (var k in kids.OfType<Bone>()) {
      k.prepare(oot);
    }
  }

  public virtual void analyze(Out oot) {
    foreach (var k in kids.OfType<Bone>()) {
      k.analyze(oot);
    }
  }

  public virtual void recurse(Out oot) {
    foreach (var k in kids.OfType<Bone>()) {
      k.recurse(oot);
    }
  }

  public virtual void solve(Out oot) {
    foreach (var k in kids.OfType<Bone>()) {
      k.solve(oot);
    }
  }

  public virtual void emit(LLVM llvm) {
    foreach (var k in kids.OfType<Bone>()) {
      k.emit(llvm);
    }
  }

}

namespace llvm {

  public abstract class Type {

    public abstract override bool Equals(object? o);
    public abstract override int GetHashCode();
    public abstract override string ToString();
    public virtual string Debug => ToString() + ";" + GetType();
    public virtual Struct? str => null;
    public virtual bool rc => false;
    public virtual bool unique => false;
    public virtual bool nullable => false;
    public virtual bool needsMop => false;

    public Type star => new Star(this);
    public Type unstar {
      get {
        if (!(this is Star)) throw new Bad($"Can't unstar {this}");
        return ((Star)this).type;
      }
    }
  }

}

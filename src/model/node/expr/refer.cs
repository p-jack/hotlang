public class Refer:Tail {

  public readonly string field;

  public Field? matched = null;
  public override bool lefty => true;

  public Refer(Place place, Expr holder, string field) : base(place, holder) {
    this.field = field;
  }

  public static (Field? field,string? error) matchField(Expr holder, string name) {
    if (holder.failed) {
      return (null, $"Could not determine type of object. {holder.GetType()}:{holder.type}");
    }
    if (!(holder.type is types.StructType)) {
      return (null, $"Can only use -> on classes and structs, not {holder.type}.");
    }
    var str = ((types.StructType)holder.type).strct;
    var field = str.field(name);
    if (field == null) return (null, $"No such field: {str.fullName}->{name}");
    if (!str.allows(holder)) return (null, $"{str.fullName} has {str.access} access, so it can't be accessed here.");
    if (!field.allows(holder)) return (null, $"{str.name}->{field.name} has {field.access} access, so it can't be accessed here.");
    return (field, null);
  }

  protected override Type resolve(Verifier v) {
    holder.verify(v);
    var referred = matchField(holder, this.field);
    if (referred.error != null) {
      v.report(this, referred.error);
      return Fail.FAIL;
    }
    var field = referred.field!;
    if (onLeft) {
      if (field.inbound) {
        v.report(this, "Can't assign to an inbound field.");
      } else if (field.type.focus.scheme == types.Scheme.GRAPH && field.inboundField == null) {
        v.report(this, "No inbound field for assignment.");
      }
    }
    matched = field;
    if (field.inbound) {
      return holder.type!.inboundChild(field.type);
    }
    return holder.type!.child(field.type);
  }

  internal override ZZZ mk(Solva solva) {
    zzz.Var hv = (zzz.Var)holder.zzz(solva);
    if (!solva.exists(hv)) {
      solva.report(this, $"Object might be null. {hv}");
    }
    var field = matched!;
    var symbol = solva.ledger.current(hv.symbol, $"->{field.name}");
    return ZZZ.var(field.type, symbol);
  }

  bool sourced = false;
  Source? source_ = null;
  internal override Source? source { get {
    if (!sourced) {
      source_ = Source.get(this);
      sourced = true;
    }
    return source_;
  }}

  protected override Pair toPair(LLVM llvm) {
    holder.emit(llvm);
    var matched = this.matched!;
    var ptr = llvm.field(holder.pair!, matched);
    if (onLeft || matched!.type!.focus.scheme == types.Scheme.VALUE) {
      return ptr;
    }
    return llvm.load(ptr);
  }

  internal override void emitAssign(LLVM llvm, Expr right) {
    llvm.assign(holder, matched!, right);
  }

  public override void format(Formatter f) {
    f.print($"{holder}->{field}");
  }

  static string[] full() => new string[] { "->" };

}

public partial class In {

  public Refer? refer(Expr holder) {
    var place = this.skip();
    expect("->", Flavor.OPERATOR);
    return new Refer(place, holder, id());
  }

}

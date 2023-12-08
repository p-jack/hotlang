using Microsoft.Z3;

public class Fetch:Expr {

  public readonly string name;

  public Node? matched = null;
  public override Trunk? trunk => matched?.trunk;


  public Fetch(Place place, string name) : base(place) {
    this.name = name;
  }

  internal override ZZZ mk(Solva solva) {
    // TODO, this makes things more readable for testing,
    // but we need to do the other thing in case of dupe locals
    var n = name;
    // var n = matched!.trunk!.name;
    var current = solva.ledger.current(n);
    return ZZZ.var(type.zzz, current);
  }

  public override bool lefty => true;

  protected override Type resolve(Verifier v) {
    var nodes = v.symbols.other(name);
    if (nodes.Count() == 0) {
      v.report(this, $"No such symbol: {name}");
      return Fail.FAIL;
    }
    if (nodes.Count() == 1) {
      return resolveOne(v, nodes[0]);
    }
    return resolveMany(v, nodes);
  }

  Type resolveOne(Verifier v, Node node) {
    var trunk = node.trunk!;
    // Verifier v2 = new Verifier(v.oot);
    // v2.analyze(node)
    matched = node;
    // ignore if !node.lvalue {
    //   // TODO, this is harder, and may have to be a separate pass.
    //   // This Fetch might get reset many times, which might incorrectly
    //   // mark many things as used.
    //   node.used = true
    // }
    if (onLeft) {
      if (trunk.type is Unknown) {
        v.report(this, $"{name} was never assigned.");
        return Fail.FAIL;
      }
      if (trunk.constant) {
        v.report(this, "Can't assign to a constant.");
      }
    }
    return trunk.type;
  }

  Type resolveMany(Verifier v, IList<Node> nodes) {
    // TODO if expected is an enum type, then check if any of the nodes belong to that enum
    // TODO Multitype for var x = default?
    return Fail.FAIL;
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


  /////

  protected override Pair toPair(LLVM llvm) {
    var trunk = matched!.trunk!;
    var pair = trunk.pair;
    if (onLeft) return pair;
    if (trunk.castFrom != null) {
      return llvm.bitcast(trunk.castFrom, pair.type);
    }
    if (trunk.type is types.Error) return pair;
    return llvm.load(pair);
  }

  internal override void emitAssign(LLVM llvm, Expr right) {
    llvm.assignLocal(this, right);
  }

  internal override void nullOut(LLVM llvm) {
    llvm.println($"; nullOut {name}");
    var pair = matched!.trunk!.pair;
    llvm.store(pair, new Pair("null", pair.type));
  }

  /////

  public override void format(Formatter f) {
    f.print(name);
  }

}

/*

resolve after {
}

llvm after {
}

+fetch(in Input:m) Fetch:r {
  let place = in.place
  let name = in.token
  in.expect(name, LOCAL)
  return Fetch_new(place, name)
}

/format {
  fmt.print(.name)
}

-class Issue/scope_Issue {
  name(:) string:r { return .source/!Fetch.name }
}

:( NoSuch en "Unknown symbol: $.name"
:( NoTrunk en "$.name never got a trunk?"
:( Unassigned en "$.name was never assigned."
:( Constancy en "Can't assign to a constant."

*/

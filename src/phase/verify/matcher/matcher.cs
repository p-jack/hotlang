public class Matcher {

  Actuality actuality;
  Node source => actuality.node;
  Place place => source.place;
  IList<Actual> actuals => actuality.actuals;
  bool other;
  List<Match> bad = new List<Match>();

  public Matcher(Actuality actuality, bool other) {
    this.actuality = actuality;
    this.other = other;
  }

  public Match? run(Verifier v) {
    if (actuals.nameless() < 0) {
      v.report(source, "Can't mix named and unnamed parameters!");
      return null;
    }

    var seen = new Set();
    for (var symbols = v.symbols; symbols != null; symbols = symbols.previous) {
      var nodes = symbols.nodes(actuality.name, other);
      var matches = this.matches(v, seen, nodes);
      if (matches.Count() > 0) {
        var good = report(matches, v);
        if (good != null) return good;
        var successes = matches.successes();
        if (successes.Count() > 0) {
          v.report(source, "Multiple matches found: " + successes.names());
          return null;
        }
      }
    }
    v.report(source, "No match found. Tried:\n" + bad.fullReport());
    return null;
  }

  IList<Match> matches(Verifier v, Set seen, IList<Node> nodes) {
    List<Match> result = new List<Match>();
    foreach (var x in nodes) {
      var fn = fullName(x);
      if (!seen.has(fn)) {
        seen.add(fn);
        var match = new Match(x, actuals);
        match.run(v);
        result.Add(match);
        if (actuality.implicitThis) {
          var match2 = new Match(x, actuality.withThis);
          match2.run(v);
          result.Add(match2);
        }
      }
    }
    return result;
  }

  string fullName(Node n) {
    if (n is Formality) {
      return ((Formality)n).fullName;
    }
    if (n is Local) {
      return ((Local)n).name;
    }
    if (n is Param) {
      return ((Param)n).name;
    }
    if (n is Field) {
      return ((Field)n).name;
    }
    throw new Bad(n.GetType().Name);
  }

  Match? report(IList<Match> matches, Verifier v) {
    if (matches.Count() == 0) return null;
    var successes = matches.successes();
    if (successes.Count() == 0) {
      bad.AddRange(matches);
      return null;
    }
    if (successes.Count() == 1) {
      successes[0].reverify(v); // TODO, forget why we do this here?
      return successes[0];
    }
    v.report(source, "Multiple matches found: " + matches.names());
    return null;
  }

}

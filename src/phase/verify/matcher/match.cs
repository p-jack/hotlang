public class Match {

  public readonly Node node;
  public IList<Actual> actuals;
  public readonly List<string> errors;

  public IList<Formal> formals { get {
    if (node is Formality) {
      return ((Formality)node).formals;
    } else {
      // locals, parameters, global constants
      return new List<Formal>();
    }
  }}

  public string fullName { get {
    if (node is Formality) {
      return ((Formality)node).fullName;
    } else if (node is Local) {
      return ((Local)node).name;
    } else if (node is Param) {
      return ((Param)node).name;
    } else {
      throw new Bad();
    }
  }}


  public Match(Node node, IList<Actual> actuals) {
    this.node = node;
    this.actuals = actuals;
    this.errors = new List<string>();
  }

  public Match(Node node, string error) {
    this.node = node;
    this.actuals = new List<Actual>();
    this.errors = new List<string> { error };
  }

  public bool success => errors.Count() == 0;

  public void run(Verifier v) {
    if (formals.Count() == 0 && actuals.Count() == 0) {
      return;
    }
    if (actuals.Count() > formals.Count()) {
      errors.Add($"Too many parameters ({actuals.Count()} vs {formals.Count()})");
      return;
    }
    if (actuals.Count() < formals.uninitialized()) {
      errors.Add($"At least {formals.uninitialized()} parameters needed, but only {actuals.Count()} given.");
      return;
    }
    if (!rearrange(v)) return;
    for (var i = 0; i < formals.Count(); i++) {
      var actual = actuals[i];
      var formal = formals[i];
      var errs = actual.reverify(v, formal.type);
      if (actual.type is Fail) {
        this.errors.AddRange(errs);
      } else {
        errs = ((Formality)node).check(formal.type, actual.type);
        this.errors.AddRange(errs);
        if (!formal.type.same(actual.expr.type)) {
          actual.castTo = formal.type;
        }
      }
    }
  }

  // (1) ensures actuals are in the same order as formals
  // (2) creates new actuals if they're missing and the corresponding formal
  //      has an initial value
  bool rearrange(Verifier v) {
    var result = new List<Actual>();
    var fake = new Actual(node.place, "", new Truth(node.place, false));
    foreach (var x in formals) { result.Add(fake); }
    var seen = new Set();
    for (var i = 0; i < actuals.nameless(); i++) {
      result[i] = actuals[i];
      seen.add(formals[i].name);
    }
    for (var i = actuals.nameless(); i < actuals.Count(); i++) {
      var name = actuals[i].name;
      if (seen.has(name)) {
        errors.Add($"Duplicate parameter: {name}");
      }
      var index = formals.indexOf(name);
      if (index < 0) {
        errors.Add($"No such parameter: {name}");
      } else {
        seen.add(name);
        result[index] = actuals[i];
      }
    }
    for (var i = 0; i < formals.Count(); i++) {
      var formal = formals[i];
      var name = formal.name;
      if (!seen.has(name)) {
        if (formal.initial == null) {
          errors.Add($"Missing parameter: {name}");
        } else {
          var a = new Actual(formal.initial.place, name, (formal.initial.copy() as Expr)!);
          a.preverify(formal, v.oot);
          errors.AddRange(a.preIssues);
          result[i] = a;
        }
      }
    }
    this.actuals = result;
    return errors.Count() == 0;
  }

  public void reverify(Verifier v) {
    for (var i = 0; i < formals.Count(); i++) {
      var actual = actuals[i];
      var formal = formals[i];
      actual.reverify(v, formal.type);
    }
  }

  public string fullReport() {
    var sb = new System.Text.StringBuilder();
    sb.Append($"  {fullName}:\n");
    foreach (var x in errors) {
      sb.Append("    ");
      sb.Append(x);
      sb.Append("\n");
    }
    return sb.ToString();
  }

}

public static class Matches {

  public static IList<Match> successes(this IList<Match> matches) {
    return matches.Where(x => x.success).ToList();
  }

  public static IList<string> names(this IList<Match> matches) {
    return matches.Select(x => x.fullName).ToList();
  }

  public static string fullReport(this IList<Match> matches) {
    var sb = new System.Text.StringBuilder();
    foreach (var x in matches) {
      sb.Append(x.fullReport());
    }
    return sb.ToString();
  }

}

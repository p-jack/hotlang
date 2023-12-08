using types;

public class Focus {

  public static Focus PRIMITIVE = primitive(true);

  public readonly bool nullable;
  public readonly bool variable;
  public readonly types.Mutability mutability;
  public readonly types.Scheme scheme;

  public Focus(bool nullable, bool variable, types.Mutability mutability, types.Scheme scheme) {
    this.nullable = nullable;
    this.variable = variable;
    this.mutability = mutability;
    this.scheme = scheme;
  }

  public static Focus primitive(bool variable) {
    return new Focus(false, variable, types.Mutability.IMMUTABLE, types.Scheme.PRIMITIVE);
  }

  public override bool Equals(object? o) {
    if (o == null) return false;
    if (o.GetType() != typeof(Focus)) return false;
    Focus f = (Focus)o;
    if (nullable != f.nullable) return false;
    if (variable != f.variable) return false;
    if (mutability != f.mutability) return false;
    if (scheme != f.scheme) return false;
    return true;
  }

  public override int GetHashCode() {
    return HashCode.Combine(nullable, variable, mutability, scheme);
  }

  public override string ToString() {
    var sb = new System.Text.StringBuilder();
    sb.Append(nullable ? "n" : "N");
    sb.Append(variable ? "v" : "f");
    sb.Append(mutability.ToString().ToLower()[0]);
    sb.Append(scheme.ToString().ToLower()[0]);
    return sb.ToString();
  }

  public Focus nullify(bool nullable) {
    return new Focus(nullable, variable, mutability, scheme);
  }

  public Focus vary(bool variable) {
    return new Focus(nullable, variable, mutability, scheme);
  }

  public Focus mutate(types.Mutability mutability) {
    return new Focus(nullable, variable, mutability, scheme);
  }

  public Focus rescheme(types.Scheme scheme) {
    return new Focus(nullable, variable, mutability, scheme);
  }

  public IList<string> actionTo(types.Action action, Focus formal) {
    switch (action) {
      case types.Action.PASS: return passTo(formal);
      case types.Action.ASSIGN: return assignTo(formal);
      case types.Action.RETURN: return returnTo(formal);
      case types.Action.CONSTRUCT: return constructTo(formal);
      default: throw new Bad();
    }
  }

  public IList<string> passTo(Focus formal) {
    Focus actual = this;
    var result = new List<string>();
    var issue = actual.scheme.passTo(formal.scheme);
    if (issue != null) result.Add(issue);
    issue = actual.mutability.passTo(formal.mutability);
    if (issue != null) result.Add(issue);
    // variability doesn't matter for passing parameters
    // nullability is checked by the solver in a later phase
    return result.AsReadOnly();
  }

  public IList<string> assignTo(Focus formal) {
    Focus actual = this;
    var result = new List<string>();
    var issue = actual.scheme.assignTo(formal.scheme);
    if (issue != null) result.Add(issue);
    issue = actual.mutability.assignTo(formal.mutability);
    if (issue != null) result.Add(issue);
    if (!formal.variable) result.Add("Can't assign to a final target.");
    // nullability is checked by the solver in a later phase
    return result.AsReadOnly();
  }

  public IList<string> returnTo(Focus formal) {
    Focus actual = this;
    var result = new List<string>();
    var issue = actual.scheme.returnTo(formal.scheme);
    if (issue != null) result.Add(issue);
    issue = actual.mutability.returnTo(formal.mutability);
    if (issue != null) result.Add(issue);
    // variability doesn't matter for returning
    // nullability is checked by the solver in a later phase
    return result.AsReadOnly();
  }

  public IList<string> constructTo(Focus formal) {
    // TODO, override immutability target if scheme is brand_new here
    Focus actual = this;
    var result = new List<string>();
    var issue = actual.scheme.constructTo(formal.scheme);
    if (issue != null) result.Add(issue);
    issue = actual.mutability.constructTo(formal.mutability);
    if (issue != null) result.Add(issue);
    // variability doesn't matter for construction
    // nullability is checked by the solver in a later phase
    return result.AsReadOnly();
  }

}

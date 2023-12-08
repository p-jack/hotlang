public class Tokens {

  public string category { get; set; }
  public Set full = new Set();
  public Set partial = new Set();

  public Tokens(string category) {
    this.category = category;
  }

  public Tokens copy() {
    var r = new Tokens(category);
    r.full.add(full);
    r.partial.add(partial);
    return r;
  }

  private void check(string token) {
    for (int i = 1; i < token.Length; i++) {
      var prefix = token.Substring(0, i);
      var existing = partial.prefixed(prefix);
      if (existing != null) {
        throw new Bad($"{category} has a conflict in its set of partial tokens: {prefix} vs {existing}");
      }
    }
  }

  public void addFull(string token) {
    check(token);
    if (!full.add(token)) {
      throw new Bad($"{category} has a duplicate complete token: {token}");
    }
  }

  public void addPartial(string token) {
    check(token);
    var dupe = full.prefixed(token);
    if (dupe != null) {
      throw new Bad($"{category} has a full token -- {dupe} -- that starts with a desired partial token: {token}");
    }
    if (!partial.add(token)) {
      throw new Bad($"{category} has a duplicate partial token: {token}");
    }
  }

}

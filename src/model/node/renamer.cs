public class Renamer {

  Dictionary<string,string> renames = new Dictionary<string,string>();

  public string get(string n) {
    return renames[n];
  }

  public void add(string original, string renamed) {
    renames[original] = renamed;
  }

  public Node xlat(Node n) {
    if (n is Find) {
      var f = (Find)n;
      if (renames.ContainsKey(f.name)) {
        return new Fetch(f.place, renames[f.name]);
      }
    } else if (n is NUI) {
      var nui = (NUI)n;
      var name = nui.resolvedName ?? nui.name;
      if (name == null) throw new Bad("nui has no name");
      if (renames.ContainsKey(name)) {
        return new NUI(nui.place, renames[name], nui.use?.copy(), nui.initial?.copy());
      }
    }
    return n;
  }

}

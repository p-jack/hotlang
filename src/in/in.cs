using System.Text;

public partial class In {

  public Syntax syntax { get; }
  private readonly string text;
  private readonly string path;
  public readonly bool header;

  public bool opaque;
  private readonly Place? origin;

  private int offset = 0;
  private int line = 1;
  private int column = 1;
  private Mark marked = new Mark(0, 1, 1);

  private readonly List<Taste> tastes = new List<Taste>();
  public IList<Taste> meal => tastes.AsReadOnly();

  public In(Syntax syntax, string path, string text, bool header) {
    this.syntax = syntax;
    this.path = path;
    this.text = text;
    this.header = header;
  }

  public In(Syntax syntax, Place origin, string text) {
    this.syntax = syntax;
    this.path = "";
    this.origin = origin;
    this.text = text;
    this.header = false;
  }

  public static Stmt getStmt(Place origin, string text) {
    return new In(Syntax.core(), origin, text).mustStmt;
  }

  public static Expr getExpr(Place origin, string text) {
    return new In(Syntax.core(), origin, text).mustExpr;
  }

  public Place place {
    get {
      if (origin != null) return origin;
      return new Place(path, line, column);
    }
  }

  public void mark() {
    this.marked = new Mark(offset, line, column);
  }

  // TODO: A total recall needs to adjust flavors too
  public void recall() {
    recall(marked);
  }

  private void recall(Mark m) {
    this.offset = m.offset;
    this.line = m.line;
    this.column = m.column;
  }

  public bool eof => offset >= text.Length;

  private char rawRead() {
    if (eof) return (char)0;
    var r = text[offset];
    offset++;
    return r;
  }

  public char read() {
    var ch = rawRead();
    if (ch == '\n') {
      line++;
      column = 1;
    } else if (ch != 0) {
      column++;
    }
    return ch;
  }

  public char peek {
    get {
      if (eof) return (char)0;
      return text[offset];
    }
  }

  private bool spacy {
    get {
      var ch = peek;
      if (opaque) return ch == ' ' || ch == '\t';
      return ch == ' ' || ch == '\t' || ch == '\n';
    }
  }

  public Place skip() {
    while (spacy) read();
    return place;
  }

  public bool skipToEOL() {
    var old = opaque;
    opaque = true;
    skip();
    opaque = old;
    return peek == '\n';
  }

  public void expect(string token, Flavor flavor) {
    var place = skip();
    for (int i = 0; i < token.Length; i++) {
      var ch = read();
      if (token[i] != ch) throw new Bad($"{place}: expected {token}");
    }
    tastes.Add(new Taste(flavor, place, token));
  }

  public void expect(char ch, Flavor flavor) {
    expect(ch.ToString(), flavor);
  }

  public string token() {
    skip();
    if (eof) return "";
    var mark = new Mark(offset, line, column);
    var result = new StringBuilder(8);
    var m = matcher();
    for (var ch = peek; m(ch); ch = peek) {
      read();
      result.Append(ch);
    }
    if (result.Length > 0 && result[result.Length - 1] == '-') {
      if (isDigit(peek)) {
        result.Length--;
      }
    }
    recall(mark);
    return result.ToString();
  }

  public string id() {
    skip();
    if (!isLetter(peek)) throw new Bad($"{place}: Expected identifier.");
    var result = token();
    expect(result, Flavor.IDENTIFIER);
    return result;
  }

  public Func<In,Expr?,T?>? parser<T>(Level<T> level) where T:Node  {
    var token = this.token(level.tokens);
    if (token == "") return null;
    return level.get(token)!;
  }

  string token(Tokens tokens) {
    skip();
    var mark = new Mark(offset, line, column);
    if (eof) return "";
    var result = new StringBuilder();
    var m = matcher();
    for (char ch = peek; m(ch); ch = peek) {
      read();
      result.Append(ch);
      if (tokens.partial.has(result.ToString())) {
        recall(mark);
        return result.ToString();
      }
    }
    recall(mark);
    var s = result.ToString();
    if (tokens.full.has(s)) {
      return s;
    }
    return "";
  }

}

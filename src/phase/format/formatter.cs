using System.IO;
using System.Text;

public class Formatter {

  private UTF8Encoding utf8 = new UTF8Encoding();
  private Stream stream;
  public int level = 0;
  public bool header;

  public Formatter(Stream stream, bool header = false) {
    this.stream = stream;
    this.header = header;
  }

  public void indent() {
    for (var i = 0; i < level; i++) {
      print("  ");
    }
  }

  public void print(string s) {
    stream.Write(utf8.GetBytes(s));
  }

  public void println(string s) {
    print(s);
    print("\n");
    stream.Flush();
  }

  public void print<T>(IList<T> exprs) where T:Expr {
    var first = true;
    foreach (var x in exprs) {
      if (first) {
        first = false;
      } else {
        print(", ");
      }
      x.format(this);
    }
  }

  public override String ToString() {
    if (stream is MemoryStream) {
      var ms = (MemoryStream)stream;
      ms.Seek(0, SeekOrigin.Begin);
      StreamReader reader = new StreamReader(ms, System.Text.Encoding.UTF8, true);
      return reader.ReadToEnd(); //return utf8.GetStream(ms.ToArray()).ToString();
    }
    return "Formatter:???";
  }

}

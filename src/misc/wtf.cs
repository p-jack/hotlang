using System.Text;

public class WTF {

  public static string str(IEnumerable<object> list) {
    StringBuilder sb = new StringBuilder();
    sb.Append("[");
    var i = 0;
    foreach (var x in list) {
      if (i > 0) sb.Append(", ");
      sb.Append(x.ToString());
      i++;
    }
    sb.Append("]");
    return sb.ToString();
  }

}

namespace use {

public partial class Tin {

  public class Pair {

    public readonly string key;
    public readonly string value;

    public Pair(string key, string value) {
      this.key = key;
      this.value = value;
    }

    public override bool Equals(object? o) {
      if (!(o is Pair)) return false;
      var that = (Pair)o;
      return this.key == that.key && this.value == that.value;
    }

    public override int GetHashCode() {
      return HashCode.Combine(key, value);
    }

    public override string ToString() {
      return key + ":" + value;
    }

    public static void addTo(List<Pair> pairs, System.Text.StringBuilder sb) {
      if (pairs.Count() == 0) return;
      sb.Append("{");
      for (int i = 0; i < pairs.Count(); i++) {
        if (i > 0) sb.Append(", ");
        var x = pairs[i];
        sb.Append(x.key);
        sb.Append(":");
        sb.Append(x.value);
      }
      sb.Append("}");
    }
  }

}

}

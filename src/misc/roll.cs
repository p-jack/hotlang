public class Roll<K,V> where K:IEquatable<K> {

  Dictionary<K,List<V>> dict = new Dictionary<K,List<V>>();

  public Roll() {}

  public bool has(K k) {
    return dict.ContainsKey(k);
  }

  public IList<V> get(K k) {
    if (!has(k)) return new List<V>();
    return dict[k];
  }

  public void add(K k, V v) {
    if (!has(k)) {
      dict[k] = new List<V>();
    }
    dict[k].Add(v);
  }

  public Roll<K,V> copy() {
    var result = new Roll<K,V>();
    foreach (var k in dict.Keys) {
      foreach (var v in dict[k]) {
        result.add(k, v);
      }
    }
    return result;
  }

}

using System.Reflection;

public static class Mirror {

  public static object? zeroValue(System.Type t) {
    if (!t.IsPrimitive) return null;
    if (t == typeof(bool)) return false;
    if (t == typeof(int)) return 0;
    throw new Bad($"no zero value for type: {t}");
  }

  public static int count(object v) {
    var m = v.GetType().GetMethod("Count")!;
    return (int)(m.Invoke(v, null)!);
  }

  public static object nth(object v, int n) {
    var p = v.GetType().GetProperty("Item")!;
    return (object)(p.GetValue(new object[] { n })!);
  }

  public static object listOf(System.Type t) {
    var listType = typeof(List<>);
    // var genericArgs = prop.PropertyType.GetGenericArguments();
    var concreteType = listType.MakeGenericType(new System.Type[] { t });
    return Activator.CreateInstance(concreteType)!;
  }

  public static void add(object list, object value) {
    var m = list.GetType().GetMethod("Add")!;
    m.Invoke(list, new object[] { value });
  }

}

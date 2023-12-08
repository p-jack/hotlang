public static class Assert {

  public static void same(object? o1, object? o2) {
    if (!ReferenceEquals(o1, o2)) throw new Bad();
  }

  public static void yes(bool flag) {
    if (!flag) throw new Bad();
  }

  public static void no(bool flag) {
    if (flag) throw new Bad();
  }


}

public static class MirrorTest {

  public static void run() {
    var place = new Place("/foo", 1, 1);
    var nui = new NUI(place, null, null, null);
    nui.reset();
    var nui2 = new NUI(place, "name", null, null);
    Assert.no(nui.Equals(nui2));
  }



}

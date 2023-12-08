namespace nui {

  public enum State {
    INVALID,
    ID_ONLY,
    USE_ONLY,
    INITIAL_ONLY,
    ID_AND_USE,
    ID_AND_INITIAL,
    USE_AND_INITIAL,
    FULL
  }

  public static class States {
    public static bool concrete(State state) {
      return state != State.ID_ONLY;
    }
  }

}

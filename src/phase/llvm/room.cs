public class Room {

  public List<Mop> mops = new List<Mop>();

  public void clean(LLVM llvm) {
    for (var i = mops.Count() - 1; i >= 0; i--) {
      llvm.println($"; MOP {mops[i].GetType().Name}");
      mops[i].emit(llvm);
    }
  }

}

public partial class LLVM {

  List<Room> rooms = new List<Room>();
  public Room lastRoom => rooms[rooms.Count() - 1];

  public void resetRooms() {
    rooms = new List<Room>();
  }

  public void enterRoom() {
    var room = new Room();
    rooms.Add(room);
  }

  public void leaveRoom(bool clear) {
    if (!justExited) {
      lastRoom.clean(this);
    }
    rooms.RemoveAt(rooms.Count() - 1);
    if (clear) {
      justExited = false;
    }
  }

  public void cleanRooms() {
    for (var i = rooms.Count() - 1; i >= 0; i--) {
      rooms[i].clean(this);
    }
  }

  public void mop(Mop? mop) {
    if (mop != null) {
      lastRoom.mops.Add(mop);
    }
  }

}
/*


// V3: This shouldn't be necessary. In all cases, we are either
// returning something that's already anchored or we're returning
// something we constructed ourselves. An analysis pass should
// be able to identify what we're returning and prevent it from
// being mopped, meaning a function can return an object with a
// refcount of zero -- and the caller will bump it once that
// object is assigned, or we detect that it is never assigned
// and manually bump it.
//
// But for now, this works.
+cleanUp(this LLVM:m, Expr:rm) {
  let scheme = expr.type.focus.scheme
  if scheme == GRAPH {
    .mop(Unroot{pair: expr.pair})
  }
  // TODO RC
}



*/

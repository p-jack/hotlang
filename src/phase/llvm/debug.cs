public partial class LLVM {

  public void debug(Pair ptr, string s) {
    var len = s.Length + 2;
    var c = nextConstant();
    // TODO escapes
    constantStream.Write($"@.{c} = private unnamed_addr constant [{len} x i8] c\" {s}\\0A\"\n");
    var num = ptrtoint(ptr);
    var v = nextVar();
    println($"  {v} = getelementptr [{len} x i8], [{len} x i8]* @.{c}, {i0} 0, {i0} 0");
    println($"  call i64 @xisop_io_int_print({num})");
    println($"  call i64 @xisop_io_print({i0} {len}, i8* {v})");
  }

  public void setLabel(Pair ptr, string name) {
    println($"{name}:");
    debug(ptr, name);
  }

}

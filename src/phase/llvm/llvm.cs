public partial class LLVM:IDisposable {

  public static llvm.Int BOOL = new llvm.Int(1);
  public static llvm.Direct VOID = new llvm.Direct("void");
  public static llvm.Type OBJECT = new llvm.Int(8).star;

  Conf conf;
  StreamWriter constantStream;
  StreamWriter typeStream;
  StreamWriter codeStream;
  MemoryStream? funcMem;
  StreamWriter? funcStream;

  string label = "";
  bool justLabelled = false;
  int labelNum;

  int varNum;
  int constantNum;

  public readonly llvm.Int i0;
  public readonly llvm.Int ptr;

  public LLVM(Conf conf) {
    // TODO, path separators blech
    this.conf = conf;
    this.typeStream = create(conf.build + "/hot-types.ll");
    this.codeStream = create(conf.build + "/hot-code.ll");
    this.constantStream = create(conf.build + "/hot-constants.ll");
    this.i0 = new llvm.Int(conf.intBits);
    this.ptr = new llvm.Int(conf.pointerBits);

    println("declare void @abort()");
    println("declare i64 @xisop_io_print(i64, i8*)"); // TODO, don't do this
    println("declare i64 @xisop_io_int_print(i64)"); // TODO, don't do this
    println("declare i64 @xisop_io_println()"); // TODO, don't do this
    if (conf.heap) {
      println($"declare i{conf.pointerBits} @malloc({i0})");
      println("declare void @free(i8*)");
      println($"declare void @memset(i8*, {i0}, {i0})"); // TODO, debug only
      println($"declare void @hotr_u_array_clear(i8*,{i0},void(i8*)*)");
    }
    if (conf.gc) {
      println("declare void @hotr_rc_dec(i8*,void(i8*)*)");
      println($"declare {i0} @hotr_rc_adec(i8*)");
      println($"declare void @hotr_rc_array_dec(i8*,{i0},void(i8*)*)");
      println("declare void @hotr_rc_inc(i8*)");
      println("declare void @hotr_rc_set(i8**,i8*,void(i8*)*)");
    }
  }

  static StreamWriter create(string path) {
    return new StreamWriter(new FileStream(path, FileMode.Create));
  }

  public void Dispose() {
    typeStream.Flush();
    typeStream.Close();
    codeStream.Flush();
    codeStream.Close();
    constantStream.Flush();
    constantStream.Close();
  }

  /////

  public void debugInt(Pair p) {
    call(i0, "@xisop_io_int_print", new Pair[] { p });
    call(i0, "@xisop_io_println", new Pair[] {});
  }

  public void debugPtr(Pair p) {
    var i = ptrtoint(p);
    call(i0, "@xisop_io_int_print", new Pair[] { i });
    call(i0, "@xisop_io_println", new Pair[] {});
  }

  public void print(string s) {
    if (funcStream == null) {
      codeStream.Write(s);
    } else {
      funcStream.Write(s);
    }
  }

  public void println(string s) {
    if (funcStream == null) {
      codeStream.Write(s);
      codeStream.Write("\n");
    } else {
      funcStream.Write(s);
      funcStream.Write("\n");
    }
  }

  /////

  public void setLabel(string label) {
    if (justLabelled) br(label);
    print(label);
    println(":");
    this.label = label;
    this.justLabelled = true;
  }

  public int nextLabel() {
    labelNum++;
    return labelNum;
  }

  /////

  public string nextVar() {
    varNum++;
    return $"%.{varNum}";
  }

  public int nextConstant()  {
    constantNum++;
    return constantNum;
  }

}

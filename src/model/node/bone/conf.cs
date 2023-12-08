public class Conf {

  public DirectoryInfo root { get; }
  public DirectoryInfo build { get; }
  public string project => "myproj";
  public int intBits => 64;
  public int pointerBits => 64;
  public string exitASM => "call void asm sideeffect \"mov $$0x2000001,%rax; mov $0,%rdi; syscall\", \"m\" (i64 %v1)";
  public bool gc => true;
  public bool heap => true;

  public Conf(string path) {
    this.root = new DirectoryInfo(path);
    // TODO, then find conf file from path
    this.build = root.CreateSubdirectory(".hot_build");
  }

}

/*

*+struct Conf

+prefix(:) string:r {
  return "hotc"
}

+sourcePaths(:) [string]:r {
  let result ^[string]:r = []
  result.add(".")
  return result
}

+includePaths(:) [string]:r {
  let result ^[string]:r = []
  return result
}

+buildPath(:) string:r {
  return ".hot_build/"
}

+intSize(:) int {
  return 64
}

+pointerSize(:) int {
  return 64
}

+exitASM(:) string:r {
  let x string:ri = "call void asm sideeffect \"mov \$\$0x2000001,%rax; mov \$0,%rdi; syscall\", \"m\" (i64 %v1)"
  return x
}

+heap(:) bool {
  return true
}

+gc(:) bool {
  return true
}

+inboundMask(:) int {
  return 1
}

+inboundExoticMask(:) int {
  return 1
}

*/

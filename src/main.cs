// Negative numbers require parsing weirdness
// make sure access to named toplevels is OK
// need another pass to find unused toplevel elements

class CLI {

  static void Main(string[] args) {
    runTests();
    if (args.Count() == 0) {
      return;
    }
    var path = args[0];
    Directory.SetCurrentDirectory(path);
    Console.WriteLine("\nPARSING...");
    var program = new Program(".", Syntax.core());
    var conf = program.conf;

    // Formatting has to happen first, since other phases
    // may manufacture toplevel elements that we don't want
    // in the formatted code.
    // TODO: A "synthesized" flag on all nodes at every level
    //  that lets us ignore stuff when we need to

    Console.WriteLine("\nFORMATTING...");
    program.format();

    Out oot = new Out(program.conf);

    Console.WriteLine("\nINDEXING...");
    program.index(oot);
    if (oot.anyErrors) exit(1);

    Console.WriteLine("\nSET SUPERS...");
    program.setSupers(oot);
    if (oot.anyErrors) exit(2);

    Console.WriteLine("\nSET MEMBERS...");
    program.setMembers(oot);
    if (oot.anyErrors) exit(3);

    // TODO, don't think we need this?
    // Console.WriteLine("\nDETECT CYCLES...");
    // program.detectCycles(oot);
    // if (oot.anyErrors) exit(4);

    Console.WriteLine("\nPREPARE...");
    program.prepare(oot);
    if (oot.anyErrors) exit(5);

    Console.WriteLine("\nANALYZING...");
    program.analyze(oot);
    if (oot.anyErrors) exit(6);

    Console.WriteLine("\nRECURSING...");
    program.recurse(oot);
    if (oot.anyErrors) exit(7);

    // Console.WriteLine("\nSOLVING...");
    // program.solve(oot);
    // if (oot.anyErrors) exit(8);

    Console.WriteLine("\nEMITTING...");
    using (var llvm = new LLVM(program.conf)) {
      program.emit(llvm);
    }

    using (var fs = new FileStream(conf.build + "/" + conf.project + ".hoth", FileMode.Create)) {
      Formatter fmt = new Formatter(fs, true);
      program.formatHeader(fmt);
    }

  }

  static void runTests() {
    MirrorTest.run();
    Console.WriteLine("tests passed");
  }

  static void exit(int code) {
    System.Environment.Exit(code);
  }

}

# hotlang

Hotlang is an experimental programming language. Its notable
features will include:

- Proofs. The hot compiler integrates an SMT Solver (Microsoft's
  Z3) to perform interprocedural logical analysis of your code, 
  catching any logical contradictions at compile-time.
- Safety. The solver can check bounds at compile-time and ensure
  that pointers are valid at compile-time. Hot programs are
  provably memory-safe and thread-safe.
- Performance. Hot can check for most problems at compile-time,
  avoiding expensive runtime check for bounds, null pointers,
  and arithmetic overflow. A hot program should perform as well
  or better than an equivalent C program, but be much safer.
- Extensibility. You can write extensions to the hot compiler
  in hot, and link those extensions when compiling a program.
  For instance, you can plausibly integrate another language's
  syntax, such as SQL, directly into hot, and therefore check
  query syntax at compile-time instead of test-time.
- Trivial C integration. Hotlang does no name mangling, so
  it can emit a C header file without issue.

Before you get excited, please note that this project isn't even
alpha quality yet. It's an experimental playground. The code is
messy (you'll note that many source files are entirely commented
out), there are TODOs everywhere, and in general this is not
ready for prime-time. I only added it to GitHub to convince you,
dear reader, of my intentions.

I'm currently working on various forms of garbage collection; after 
that I need to add support for enums, strings, and libraries; after
*that* I need to refactor the solver; and after *that* I can finally
get it to self-compile. So, a few years.

Having said that: It already *is* a fully functional, Turing-complete
programming language. The compiler takes in hot source code and
emits LLVM, which llc can compile to emit a static binary.

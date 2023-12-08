
/*
namespace nui {

  public class Piece {

    Place place;
    string? name;
    Use? use;
    Expr? initial;

    bool valid => name != null || use != null || initial != null;

    bool terminus => use != null || initial != null;

    public override string ToString() {
      var sb = new System.Text.StringBuilder();
      if (name != null) {
        sb.Append("NAME:");
        sb.Append(name);
      }
      if (use != null) {
        if (sb.Length > 0) sb.Append(" ");
        sb.Append("USE:");
        sb.Append(use);
      }
      if (initial != null) {
        if (sb.Length > 0) sb.Append(" ");
        sb.Append("INITIAL:");
        sb.Append(initial);
      }
      return sb.ToString();
    }
  }
}

*/

/*
public partial class In {

  public nui.Piece? piece(char delim) {
    var place = in.skip();
    if (in.peek == '=') {
      return new nui.Piece(place, in.initial);
    }
    var use = in.use;
    if (use is null) {

    }
  }


}

  =piece(in Input:m, delim char) Piece:rn {
    let place = in.skip()
    if in.peek == '=' {
      return Piece{place: place, initial: in.initial}
    }
    if in.peek.isLower {
      return Piece{place: place, name: in.id, use: in.use, initial: in.initial}
    }
    return Piece{place: place, use: in.use, initial: in.initial}
  }

  -named(in Input:m, use NamedUse:rm) Piece:rn {
    let place = use.place
    if use.name[0].isLower {
      let name = use.name
      return Piece{place: place, name: name, use: in.use, initial: in.initial}
    }
    if !use.blur.allUnknown && in.peek.isLetter {
      return Piece{place: place, name: use.name, use: in.use, initial: in.initial}
    }
    return Piece{place: place, use: use, initial: in.initial }
  }

  -initial(in Input:m) Expr:rn {
    in.skip()
    return null if in.peek != '='
    in.expect("=", OPERATOR)
    let place = in.skip()
    let result = in.expr
    if result is null {
      STDOUT.println("$place: Expected expression.")
      throw PARSE
    }
    return result
  }

  -empty(in Input:m) NamedUse:r {
    return NamedUse_new(in.skip(), in.blur, "")
  }

  // =piece_test2() {
  //   let text = " = Bar:s{}"
  //   let syntax = Syntax_new()
  //   let place = Place:r{path: "", line: 0, column: 0}
  //   let piece = string_new(syntax, place, text).piece(')')
  //   throw WTF
  // }
  //
  // =piece_test() {
  //   test("this", true, false, false)
  //   test("=123", false, false, true)
  //   test("Foo", false, true, false)
  //   test("this Foo", true, true, false)
  //   test("this = 123", true, false, true)
  //   test("Foo = 123", false, true, true)
  //   test("this Foo = 123", true, true, true)
  //   test("Foo:n = 123", false, true, true)
  // }
  //
  // -piece_case_test(text string:r, name, use, initial bool) {
  //   let syntax = Syntax_new()
  //   let place = Place:r{path: "piece_case_test", line: 0, column: 0}
  //   let piece = string_new(syntax, place, text).piece(')')
  //   if name {
  //     if piece.name is null {
  //       STDOUT.println("($text) Expected a name, got null.")
  //       throw WTF
  //     } else if piece.name != "this" {
  //       STDOUT.println("($text) Expected abc, got ${piece.name}.")
  //       throw WTF
  //     }
  //   } else {
  //     if piece.name not null {
  //       STDOUT.println("($text) Expected null name, got ${piece.name}.")
  //       throw WTF
  //     }
  //   }
  //   if use {
  //     if piece.use is null {
  //       STDOUT.println("($text) Expected a use, got null.")
  //       throw WTF
  //     } else if !piece.use/?NamedUse {
  //       STDOUT.println("($text) Expected Foo, got ${piece.use}.")
  //       throw WTF
  //     }
  //   } else {
  //     if piece.use not null {
  //       STDOUT.println("($text) Expected no use, got ${piece.name}.")
  //       throw WTF
  //     }
  //   }
  //   if initial {
  //     if piece.initial is null {
  //       STDOUT.println("($text) Expected an initial, got null.")
  //       throw WTF
  //     } else if !piece.initial/?Number {
  //       STDOUT.println("($text) Expected 123, got ${piece.initial}.")
  //       throw WTF
  //     }
  //   } else {
  //     if piece.initial not null {
  //       STDOUT.println("($text) Expected no initial, got ${piece.initial}.")
  //       throw WTF
  //     }
  //   }
  // }



}
*/

public class Tester:use.Tin.Handler {

  static void init(Syntax syntax) {
    syntax.add(new Tester());
  }

  public override string origin => "hotc";
  public override string name => "tester";

  public override Struct? expand(Verifier v, Node n, use.Tin.Key key) {
    if (key.type2 != null) {
      v.report(n, "tester tin needs 1 type");
      return null;
    }
    var template = @"
      tester {

        full error 111
        bounds error 112
        missing error 113

        one Tn = null, two Tn = null, three Tn = null

        count(:) int {
          let r = 0
          r = r + 1 if one not null
          r = r + 1 if two not null
          r = r + 1 if three not null
          return r
        }

        add(:, elem T) {
          if one is null {
            one := elem
          } else if two is null {
            two := elem
          } else if three is null {
            three := elem
          } else {
            throw full
          }
        }

        index_remove(:, i int) {
          throw bounds if i < 0
          throw bounds if i >= count
          if i == 0 {
            one := two
            two := three
            three := null
          } else if i == 1 {
            two := three
            three := null
          } else if i == 2 {
            three := null
          }
        }

        remove(:, elem T) {
          let i = 0
          while i < 3 {
            let e = fetch(i)
            if e is null {
              throw missing
            }
            if e is elem {
              index_remove(i)
            }
          }
        }

        fetch(:, i int) Tj {
          if i == 0 return one
          if i == 1 return two
          if i == 2 return three
          throw bounds
        }

        forward loop(:) {
          let i = 0
          while i < count {
            ---> this.fetch(i)
            i = i + 1
          }
        }

      }
    ";
    // TODO, convert n to use.Tin if we can (for better substitution)
    var text = template.Replace("T", key.type1.ToString());
    Console.WriteLine(text);
    var input = new In(Syntax.core(), "#tester", text, false);
    return input.str;
  }

}

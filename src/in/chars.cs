public partial class In {

  Func<char,bool> matcher() {
    var ch = this.peek;
    if (isSymbol(ch)) return isSymbol;
    if (isIdentifier(ch)) return isIdentifier;
    return doNotMatch;
  }

  private static bool isOperator(char ch) => "!$%&*+-./<=>?\\^`~|:\"".Contains(ch);

  private static bool isSymbol(char ch) => isOperator(ch) || "()[]{}@#'\"".Contains(ch);

  private static bool doNotMatch(char ch) => false;

  private static bool isDigit(char ch) => ch >= '0' && ch <= '9';

  private static bool isLetter(char ch) {
    if (ch >= 'a' && ch <= 'z') {
      return true;
    }
    if (ch >= 'A' && ch <= 'Z') {
      return true;
    }
    return false;
  }

  private static bool isIdentifier(char ch) {
    return isLetter(ch) || isDigit(ch) || ch == '_';
  }

}

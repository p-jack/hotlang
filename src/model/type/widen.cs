public class Widen {

  public readonly Type input1;
  public readonly Type input2;
  public readonly Type result;

  public Widen(Type input1, Type input2, Type result) {
    this.input1 = input1;
    this.input2 = input2;
    this.result = result;
  }

  public override string ToString() {
    return $"in1:{input1} in2:{input2} result:{result}";
  }

}

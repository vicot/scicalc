namespace SciCalc.Tokens.Operators
{
    public class MulOperator : Operator
    {
        public MulOperator() : base()
        {
            this.ArgumentCount = 2;
            this.Priority = 2;
            this.Symbol = " * ";
        }

        public override double Execute(double arg1, double arg2) {
            return arg1 * arg2;
        }
    }
}
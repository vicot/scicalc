namespace SciCalc.Tokens.Operators
{
    public class DivOperator : Operator
    {
        public DivOperator() : base(" / ")
        {
            this.ArgumentCount = 2;
            this.Priority = 2;
        }

        public override double Execute(double arg1, double arg2)
        {
            return arg1 / arg2;
        }
    }
}

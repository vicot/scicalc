namespace SciCalc.Tokens.Operators
{
    public class SubOperator : Operator
    {
        public SubOperator()
        {
            this.ArgumentCount = 2;
            this.Priority = 1;
            this.Symbol = " - ";
        }

        public override double Execute(double arg1, double arg2)
        {
            return arg1 - arg2;
        }
    }
}

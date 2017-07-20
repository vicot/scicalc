namespace SciCalc.Tokens.Operators
{
    public class SubOperator : Operator
    {
        public SubOperator() : base(" - ")
        {
            this.ArgumentCount = 2;
            this.Priority = 1;
        }

        public override double Execute(double arg1, double arg2)
        {
            return arg1 - arg2;
        }
    }
}

namespace SciCalc.Tokens.Operators
{
    public class FractionOperator : Operator
    {
        public FractionOperator() : base("_")
        {
            this.ArgumentCount = 2;
            this.Priority = 3;
        }

        public override double Execute(double arg1, double arg2)
        {
            return arg1 / arg2;
        }
    }
}

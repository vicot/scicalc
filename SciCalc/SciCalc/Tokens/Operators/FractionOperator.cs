namespace SciCalc.Tokens.Operators
{
    public class FractionOperator : Operator
    {
        public FractionOperator()
        {
            this.ArgumentCount = 2;
            this.Priority = 3;
            this.Symbol = "_";
        }

        public override double Execute(double arg1, double arg2)
        {
            return arg1 / arg2;
        }
    }
}

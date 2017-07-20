namespace SciCalc.Tokens.Operators
{
    public class PercentOperator : Operator
    {
        public PercentOperator()
        {
            this.ArgumentCount = 1;
            this.Priority = 4;
            this.Symbol = "% ";
        }

        public override double Execute(double arg)
        {
            return arg / 100.0;
        }
    }
}

namespace SciCalc.Tokens.Operators
{
    public class PercentOperator : Operator
    {
        public PercentOperator() : base()
        {
            this.ArgumentCount = 1;
            this.Priority = 4;
            this.Symbol = "%";
        }
    }
}
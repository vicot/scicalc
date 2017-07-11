namespace SciCalc.Tokens.Operators
{
    public class RootOperator : Operator
    {
        public RootOperator() : base()
        {
            this.ArgumentCount = 2;
            this.Priority = 3;
            this.Symbol = "√";
        }


    }
}
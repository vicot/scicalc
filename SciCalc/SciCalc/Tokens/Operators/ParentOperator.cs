namespace SciCalc.Tokens.Operators
{
    public class ParentOperator : Operator
    {
        public ParentOperator()
        {
            this.ArgumentCount = 2;
            this.Priority = 0;
            this.Symbol = " (";
        }
    }
}

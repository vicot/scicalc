namespace SciCalc.Tokens.Operators
{
    public class ParentOperator : Operator
    {
        public ParentOperator() : base(" (")
        {
            this.ArgumentCount = 2;
            this.Priority = 0;
        }
    }
}

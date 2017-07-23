namespace SciCalc.Tokens.Operators
{
    public class ParentOperator : Operator
    {
        public ParentOperator() : base(" (")
        {
            this.ArgumentCount = 0;
            this.Priority = 0;
        }
    }
}

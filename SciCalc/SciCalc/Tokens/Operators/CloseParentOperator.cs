namespace SciCalc.Tokens.Operators
{
    public class CloseParentOperator : Operator
    {
        public CloseParentOperator() : base(") ")
        {
            this.ArgumentCount = 0;
            this.Priority = 0;
        }

        public override string ErrorMessage => "Unexpected ')'. No matching '(' found.";
    }
}

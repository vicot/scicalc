namespace SciCalc.Tokens.Operators
{
    public class NegateOperator : Operator
    {
        public NegateOperator() : base(" -")
        {
            this.ArgumentCount = 1;
            this.Priority = 4;
            this.Symbol = " -";
        }

        public override double Execute(double arg)
        {
            return -arg;
        }
    }
}

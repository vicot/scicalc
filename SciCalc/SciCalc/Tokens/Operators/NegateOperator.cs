namespace SciCalc.Tokens.Operators
{
    public class NegateOperator : Operator
    {
        public NegateOperator()
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

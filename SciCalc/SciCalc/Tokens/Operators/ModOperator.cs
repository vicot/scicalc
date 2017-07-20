namespace SciCalc.Tokens.Operators
{
    public class ModOperator : Operator
    {
        public ModOperator()
        {
            this.ArgumentCount = 2;
            this.Priority = 2;
            this.Symbol = " m ";
        }

        public override double Execute(double arg1, double arg2)
        {
            return arg1 % arg2;
        }
    }
}

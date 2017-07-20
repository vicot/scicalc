namespace SciCalc.Tokens.Operators
{
    public class ModOperator : Operator
    {
        public ModOperator() : base(" m ")
        {
            this.ArgumentCount = 2;
            this.Priority = 2;
        }

        public override double Execute(double arg1, double arg2)
        {
            return arg1 % arg2;
        }
    }
}

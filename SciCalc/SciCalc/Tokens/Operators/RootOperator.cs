using System;

namespace SciCalc.Tokens.Operators
{
    public class RootOperator : Operator
    {
        public RootOperator(bool unary) : base("√")
        {
            this.ArgumentCount = unary ? 1 : 2;
            this.Priority = 3;
        }

        public override double Execute(double arg1, double arg2)
        {
            return Math.Pow(arg2, 1.0 / arg1);
        }

        public override double Execute(double arg)
        {
            return Math.Sqrt(arg);
        }
    }
}

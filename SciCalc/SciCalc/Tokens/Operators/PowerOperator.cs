using System;

namespace SciCalc.Tokens.Operators
{
    public class PowerOperator : Operator
    {
        public PowerOperator() : base()
        {
            this.ArgumentCount = 2;
            this.Priority = 3;
            this.Symbol = "^";
        }

        public override double Execute(double arg1, double arg2) {
            return Math.Pow(arg1,arg2);
        }
    }
}
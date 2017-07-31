using System;
using System.Collections.Generic;

namespace SciCalc.Tokens.Operators
{
    public class RootOperator : Operator
    {
        public RootOperator(bool unary) : base("√")
        {
            this.ArgumentCount = unary ? 1 : 2;
            this.Priority = 3;

            if (unary)
            {
                //unary version of this operator doesn't require anything on the left, and can also follow anything
                this.leftBinding = new List<Type>
                {
                    null,
                    typeof(Token)
                };
            }
            else
            {
                //binary version of root operator requires an explicit value on the left (used as a root's base) - don't allow calculated root bases
                this.leftBinding = new List<Type>
                {
                    typeof(Values.Value),
                    typeof(Values.Constant)
                };
            }
        }

        public override double Execute(double arg1, double arg2)
        {
            int sign = (arg1 < 0) ? -1 : 1;
            return sign * Math.Pow(arg2, 1.0 / Math.Abs(arg1));
        }

        public override double Execute(double arg)
        {
            return Math.Sqrt(arg);
        }
    }
}

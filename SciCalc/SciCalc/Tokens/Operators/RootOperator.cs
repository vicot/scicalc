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

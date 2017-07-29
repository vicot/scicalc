using System;
using System.Collections.Generic;

namespace SciCalc.Tokens.Operators
{
    public class NegateOperator : Operator
    {
        public NegateOperator() : base(" -")
        {
            this.ArgumentCount = 1;
            this.Priority = 4;
            this.Symbol = " -";

            //unary - isn't left bound, allow anything there
            this.leftBinding = new List<Type>
            {
                null,
                typeof(Token)
            };
        }

        public override double Execute(double arg)
        {
            return -arg;
        }
    }
}

using System;
using System.Collections.Generic;

namespace SciCalc.Tokens.Operators
{
    public class FactorialOperator : Operator
    {
        public FactorialOperator() : base("! ")
        {
            this.ArgumentCount = 1;
            this.Priority = 4;

            //this operator doesn't require second parameter. Can also be followed by any other token including operators
            this.rightBinding = new List<Type>
            {
                null,
                typeof(Token)
            };
        }

        public override double Execute(double arg)
        {
            double result = 1;
            while (arg > 0)
            {
                result *= arg--;
            }

            return result;
        }
    }
}

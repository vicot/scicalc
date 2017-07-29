using System;
using System.Collections.Generic;

namespace SciCalc.Tokens.Operators
{
    public class Operator : Token
    {
        public Operator(string symbol, bool valid = true) : base(symbol)
        {
            this.TokenType = TokenType.Operator;
            this.Value = 0.0;
            if (!valid)
            {
                this.IsValid = false;
            }

            // by default, operators need a non-operator (or parenthesis) on both ends, or unary operators that are left-bound (factorial and percent on the left), or right-bound on the right (root, negate)
            this.leftBinding = new List<Type>
            {
                typeof(Values.Value),
                typeof(Values.Constant),
                typeof(Functions.Function),
                typeof(CloseParentOperator),
                typeof(PercentOperator),
                typeof(FactorialOperator)
            };
            this.rightBinding = new List<Type>
            {
                typeof(Values.Value),
                typeof(Values.Constant),
                typeof(Functions.Function),
                typeof(ParentOperator),
                typeof(RootOperator),
                typeof(NegateOperator)
            };
        }
    }
}

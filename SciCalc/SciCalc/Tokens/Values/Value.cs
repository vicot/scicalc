using System;
using System.Collections.Generic;
using System.Globalization;

namespace SciCalc.Tokens.Values
{
    public abstract class Value : Token
    {
        protected Value(double value) : base(value.ToString(CultureInfo.InvariantCulture))
        {
            this.Priority = 0;
            this.ArgumentCount = 0;
            this.TokenType = TokenType.Value;
            this.Value = value;

            // values can bind with constants on the right, such as "2PI" and operators or functions (with auto * operator) on both ends
            // also, values can stand by themselves, without anything on the left or right
            this.leftBinding = new List<Type>
            {
                null,
                typeof(Operators.Operator),
                typeof(Functions.Function)
            };
            this.rightBinding = new List<Type>
            {
                null,
                typeof(Constant),
                typeof(Operators.Operator),
                typeof(Functions.Function)
            };
        }
    }
}

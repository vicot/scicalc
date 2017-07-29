using System;
using System.Collections.Generic;

namespace SciCalc.Tokens.Functions
{
    public class Function : Token
    {
        public Function(string name, bool valid = true) : base($" {name}")
        {
            this.TokenType = TokenType.Function;
            this.Value = 0.0;
            this.Priority = 6; //functions get the top priority
            if (!valid)
            {
                this.IsValid = false;
            }

            // functions by default can have any token (or none) on the left, and require ( on the right
            this.leftBinding = new List<Type>
            {
                null,
                typeof(Token)
            };
            this.rightBinding = new List<Type>
            {
                typeof(Operators.ParentOperator)
            };
        }
    }
}

using System;
using System.Collections.Generic;

namespace SciCalc.Tokens.Values
{
    public class Constant : Token
    {
        public Constant(string name) : base(name)
        {
            this.Priority = 0;
            this.ArgumentCount = 0;
            this.TokenType = TokenType.Value;
            this.IsValid = false;
            // constants can bind with values on the left, such as "2PI" and operators or functions (with auto * operator) on both ends
            // also, constants can stand by themselves, without anything on the left or right
            this.leftBinding = new List<Type>
            {
                null,
                typeof(Value),
                typeof(Operators.Operator),
                typeof(Functions.Function)
            };
            this.rightBinding = new List<Type>
            {
                null,
                typeof(Operators.Operator),
                typeof(Functions.Function)
            };
        }

        public Constant(string name, double value) : this(name)
        {
            this.Value = value;
            this.IsValid = true;
        }

        public override string ErrorMessage => $"Undefined constant '{this.Symbol}'";

        public override string ToString()
        {
            return $"{this.Symbol} = {this.Value:##.####}";
        }
    }
}

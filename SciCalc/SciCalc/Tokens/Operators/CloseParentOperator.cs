using System;
using System.Collections.Generic;

namespace SciCalc.Tokens.Operators
{
    public class CloseParentOperator : Operator
    {
        public CloseParentOperator() : base(") ")
        {
            this.ArgumentCount = 0;
            this.Priority = 0;

            //allow anything after )
            this.rightBinding = new List<Type>
            {
                null,
                typeof(Token)
            };
        }

        public override string ErrorMessage => "Unexpected ')'. No matching '(' found.";
    }
}

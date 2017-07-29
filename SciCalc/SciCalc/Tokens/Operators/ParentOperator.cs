using System;
using System.Collections.Generic;
using SciCalc.Tokens.Values;

namespace SciCalc.Tokens.Operators
{
    public class ParentOperator : Operator
    {
        public ParentOperator() : base(" (")
        {
            this.ArgumentCount = 0;
            this.Priority = 0;

            //( can start the expression, can also follow anything
            this.leftBinding = new List<Type>
            {
                null,
                typeof(Token)
            };
        }
    }
}

using System;
using System.Collections.Generic;

namespace SciCalc.Tokens
{
    public class ExcessiveDotToken : Token
    {
        public ExcessiveDotToken() : base(" . ")
        {
            //not an actual operator, only used to display excessive dots in expression view
            this.ArgumentCount = 0;
            this.Priority = 0;
            this.IsValid = false;

            //this "operator" is just bad, it should not exist, and thus doesn't bind to anything at all
            //allow anything though, or better error handling
            this.leftBinding = new List<Type> {null, typeof(Token)};
            this.rightBinding = new List<Type> {null, typeof(Token)};
        }

        public override string ErrorMessage => "Unexpected dot '.'";
    }
}

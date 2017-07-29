using System;
using System.Collections.Generic;

namespace SciCalc.Tokens.Functions
{
    public class LogFunction : Function
    {
        public LogFunction() : base("log")
        {
            this.ArgumentCount = 2;

            // log function has special format of logx(y) where x is the base, ie. log3(9)=2
            this.rightBinding = new List<Type>
            {
                typeof(Values.Value),
                typeof(Values.Constant)
            };
        }

        public override double Execute(double arg1, double arg2)
        {
            return Math.Log(arg2, arg1);
        }
    }
}

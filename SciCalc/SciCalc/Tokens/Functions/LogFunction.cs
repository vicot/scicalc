using System;

namespace SciCalc.Tokens.Functions
{
    public class LogFunction : Function
    {
        public LogFunction() : base("log")
        {
            this.ArgumentCount = 2;
        }

        public override double Execute(double arg1, double arg2)
        {
            return Math.Log(arg2, arg1);
        }
    }
}

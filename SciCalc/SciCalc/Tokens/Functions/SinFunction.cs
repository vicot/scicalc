using System;

namespace SciCalc.Tokens.Functions
{
    public class SinFunction : Function
    {
        public SinFunction()
        {
            this.Symbol = " sin";
            this.ArgumentCount = 1;
        }

        public override double Execute(double arg)
        {
            return Math.Sin(arg);
        }
    }
}

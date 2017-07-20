using System;

namespace SciCalc.Tokens.Functions
{
    public class TanFunction : Function
    {
        public TanFunction()
        {
            this.Symbol = " tan";
            this.ArgumentCount = 1;
        }

        public override double Execute(double arg)
        {
            return Math.Tan(arg);
        }
    }
}

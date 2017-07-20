using System;

namespace SciCalc.Tokens.Functions
{
    public class LnFunction : Function
    {
        public LnFunction()
        {
            this.Symbol = " ln";
            this.ArgumentCount = 1;
        }

        public override double Execute(double arg)
        {
            return Math.Log(arg);
        }
    }
}

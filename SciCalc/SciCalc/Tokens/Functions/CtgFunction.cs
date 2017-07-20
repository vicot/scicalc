using System;

namespace SciCalc.Tokens.Functions
{
    public class CtgFunction : Function
    {
        public CtgFunction()
        {
            this.Symbol = " ctg";
            this.ArgumentCount = 1;
        }

        public override double Execute(double arg)
        {
            return 1.0 / Math.Tan(arg);
        }
    }
}

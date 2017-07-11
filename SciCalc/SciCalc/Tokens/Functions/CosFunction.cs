using System;

namespace SciCalc.Tokens.Functions
{
    public class CosFunction : Function
    {
        public CosFunction() : base()
        {
            this.Symbol = "cos";
            this.ArgumentCount = 1;
        }

        public override double Execute(double arg)
        {
            return Math.Cos(arg);
        }
    }
}
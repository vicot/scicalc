using System;

namespace SciCalc.Tokens.Values
{
    public class DoubleValue : Value
    {
        public DoubleValue(double value) : base(value)
        {
            //if value is integer, add missing dot to symbol string
            if (Math.Abs(value % 1) <= (double.Epsilon * 100))
            {
                this.Symbol = $"{value}.";
            }
        }
    }
}
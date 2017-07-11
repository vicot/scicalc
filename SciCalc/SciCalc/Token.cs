using System;

namespace SciCalc
{
    public abstract class Token
    {
        public TokenType Type { get; protected set; }
        public double Value { get; protected set; }
        public string Symbol { get; protected set; }
        public int ArgumentCount { get; protected set; }
        public int Priority { get; protected set; }

        public virtual double Execute(double arg)
        {
            throw new NotImplementedException();
        }

        public virtual double Execute(double arg1, double arg2)
        {
            throw new NotImplementedException();
        }
    }

    public enum TokenType
    {
        Value,
        Operator,
        Function,
        Constant
    }

}
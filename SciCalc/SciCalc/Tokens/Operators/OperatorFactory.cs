using System;

namespace SciCalc.Tokens.Operators
{
    public static class OperatorFactory
    {
        public static Token GetToken(char name, bool alternative=false)
        {
            switch (name)
            {
                case '+': return new SumOperator();
                case '-': return alternative ? (Token) new NegateOperator() : new SubOperator();
                case '/': return new DivOperator();
                case '*': return new MulOperator();
                case 'm': return new ModOperator();
                case '^': return new PowerOperator();
                case '%': return new PercentOperator();
                case '√': return new RootOperator();
                case '!': return new FactorialOperator();
                case '(': return new ParentOperator();
                default: throw new ArgumentException($"'{name}' is not a valid operator.");
            }
        }
    }
}
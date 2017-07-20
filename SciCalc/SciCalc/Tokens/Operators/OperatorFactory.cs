using System;

namespace SciCalc.Tokens.Operators
{
    public static class OperatorFactory
    {
        public static Token GetToken(char symbol, bool alternative = false)
        {
            switch (symbol)
            {
                case '+':
                    return new SumOperator();
                case '-':
                    return alternative ? (Token) new NegateOperator() : new SubOperator();
                case '/':
                    return new DivOperator();
                case '*':
                    return new MulOperator();
                case 'm':
                    return new ModOperator();
                case '^':
                    return new PowerOperator();
                case '%':
                    return new PercentOperator();
                case '√':
                    return new RootOperator(alternative);
                case '!':
                    return new FactorialOperator();
                case '(':
                    return new ParentOperator();
                case '_':
                    return new FractionOperator();
                default:
                    return new Operator($" {symbol} ");
                //throw new ArgumentException($"'{symbol}' is not a valid operator.");
            }
        }
    }
}

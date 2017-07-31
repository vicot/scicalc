// OperatorFactory.cs Copyright (c) 2017 vicot
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
                case '#':
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
                case ')':
                    return new CloseParentOperator();
                case '_':
                    return new FractionOperator();
                default:
                    return new Operator($" {symbol} ", false);
            }
        }
    }
}

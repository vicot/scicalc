// Operator.cs Copyright (c) 2017 vicot
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

using System;
using System.Collections.Generic;
using SciCalc.Tokens.Functions;
using SciCalc.Tokens.Values;

namespace SciCalc.Tokens.Operators
{
    public class Operator : Token
    {
        public Operator(string symbol, bool valid = true) : base(symbol)
        {
            this.TokenType = TokenType.Operator;
            this.Value = 0.0;
            if (!valid)
            {
                this.IsValid = false;
            }

            // by default, operators need a non-operator (or parenthesis) on both ends, or unary operators that are left-bound (factorial and percent on the left), or right-bound on the right (root, negate)
            this.leftBinding = new List<Type>
            {
                typeof(Value),
                typeof(Constant),
                typeof(Function),
                typeof(CloseParentOperator),
                typeof(PercentOperator),
                typeof(FactorialOperator)
            };
            this.rightBinding = new List<Type>
            {
                typeof(Value),
                typeof(Constant),
                typeof(Function),
                typeof(ParentOperator),
                typeof(RootOperator),
                typeof(NegateOperator)
            };
        }
    }
}

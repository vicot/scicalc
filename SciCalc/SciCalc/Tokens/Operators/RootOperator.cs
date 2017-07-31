// RootOperator.cs Copyright (c) 2017 vicot
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
using SciCalc.Tokens.Values;

namespace SciCalc.Tokens.Operators
{
    public class RootOperator : Operator
    {
        public RootOperator(bool unary) : base("√")
        {
            this.ArgumentCount = unary ? 1 : 2;
            this.Priority = 3;

            if (unary)
            {
                //unary version of this operator doesn't require anything on the left, and can also follow anything
                this.leftBinding = new List<Type>
                {
                    null,
                    typeof(Token)
                };
            }
            else
            {
                //binary version of root operator requires an explicit value on the left (used as a root's base) - don't allow calculated root bases
                this.leftBinding = new List<Type>
                {
                    typeof(Value),
                    typeof(Constant)
                };
            }
        }

        public override double Execute(double arg1, double arg2)
        {
            int sign = arg1 < 0 ? -1 : 1;
            return sign * Math.Pow(arg2, 1.0 / Math.Abs(arg1));
        }

        public override double Execute(double arg)
        {
            return Math.Sqrt(arg);
        }
    }
}

// Value.cs Copyright (c) 2017 vicot
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
using System.Globalization;
using SciCalc.Tokens.Functions;
using SciCalc.Tokens.Operators;

namespace SciCalc.Tokens.Values
{
    public abstract class Value : Token
    {
        public int InsignificantZeros
        {
            set => this.Symbol+=new string('0', value);
        }

        protected Value(double value) : base(value.ToString(CultureInfo.InvariantCulture))
        {
            this.Priority = 0;
            this.ArgumentCount = 0;
            this.TokenType = TokenType.Value;
            this.Value = value;

            // values can bind with constants on the right, such as "2PI" and operators or functions (with auto * operator) on both ends
            // also, values can stand by themselves, without anything on the left or right
            this.leftBinding = new List<Type>
            {
                null,
                typeof(Operator),
                typeof(Function)
            };
            this.rightBinding = new List<Type>
            {
                null,
                typeof(Constant),
                typeof(Operator),
                typeof(Function)
            };
        }
    }
}

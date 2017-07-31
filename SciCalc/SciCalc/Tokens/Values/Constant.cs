// Constant.cs Copyright (c) 2017 vicot
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
using SciCalc.Tokens.Operators;

namespace SciCalc.Tokens.Values
{
    public class Constant : Token
    {
        public Constant(string name) : base(name)
        {
            this.Priority = 0;
            this.ArgumentCount = 0;
            this.TokenType = TokenType.Value;
            this.IsValid = false;
            // constants can bind with values on the left, such as "2PI" and operators or functions (with auto * operator) on both ends
            // also, constants can stand by themselves, without anything on the left or right
            this.leftBinding = new List<Type>
            {
                null,
                typeof(Value),
                typeof(Operator),
                typeof(Function)
            };
            this.rightBinding = new List<Type>
            {
                null,
                typeof(Operator),
                typeof(Function)
            };
        }

        public Constant(string name, double value) : this(name)
        {
            this.Value = value;
            this.IsValid = true;
        }

        public override string ErrorMessage => $"Undefined constant '{this.Symbol}'";

        public override string ToString()
        {
            return $"{this.Symbol} = {this.Value:##.####}";
        }
    }
}

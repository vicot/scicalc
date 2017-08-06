// Function.cs Copyright (c) 2017 vicot
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
using SciCalc.Tokens.Operators;

namespace SciCalc.Tokens.Functions
{
    public class Function : Token
    {
        public Function(string name, bool valid = true) : base($" {name}")
        {
            this.TokenType = TokenType.Function;
            this.Value = 0.0;
            this.Priority = 6; //functions get the top priority
            if (!valid)
            {
                this.IsValid = false;
            }

            // functions by default can have any token (or none) on the left, and require ( on the right
            this.leftBinding = new List<Type>
            {
                null,
                typeof(Token)
            };
            this.rightBinding = new List<Type>
            {
                typeof(ParentOperator)
            };
        }

        public override string ErrorMessage => $"Unknown function '{this.Symbol}'";
    }
}

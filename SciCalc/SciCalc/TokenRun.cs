// TokenRun.cs Copyright (c) 2017 vicot
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

using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;
using SciCalc.Tokens;

namespace SciCalc
{
    public class TokenRun : Run
    {
        private readonly Token token;

        public TokenRun()
        {
        }

        public TokenRun(string text) : base(text)
        {
            this.token = new EmptyToken();
        }

        public TokenRun(Token token) : this(token.Symbol)
        {
            this.token = token;

            //set color
            if (!token.IsValid)
            {
                this.Foreground = Brushes.Red;
            }
            else
            {
                switch (token.TokenType)
                {
                    case TokenType.Value:
                        this.Foreground = Brushes.Black;
                        break;

                    case TokenType.Function:
                        this.Foreground = Brushes.DarkGreen;
                        break;

                    case TokenType.Operator:
                        this.Foreground = Brushes.Purple;
                        break;
                }

                //if (token.Inferred)
                //{
                //    this.Foreground = Brushes.Gray;
                //}
            }
        }

        public TokenRun(string text, TextPointer insertionPosition) : base(text, insertionPosition)
        {
        }

        public List<TokenRun> ConnectToken(Token nextToken)
        {
            var runs = new List<TokenRun>(3);
            if (this.token.PostfixSpace || nextToken.PrefixSpace)
            {
                runs.Add(new TokenRun(" "));
            }
            runs.Add(new TokenRun(nextToken));

            return runs;
        }
    }
}

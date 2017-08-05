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
    /// <summary>
    ///     Custom Run class that formats itself based on the token
    /// </summary>
    /// <seealso cref="System.Windows.Documents.Run" />
    public class TokenRun : Run
    {
        private readonly Token token;

        public TokenRun()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TokenRun" /> class.
        /// </summary>
        /// <param name="text">
        ///     A string specifying the initial contents of the <see cref="T:System.Windows.Documents.Run" />
        ///     object.
        /// </param>
        public TokenRun(string text) : base(text)
        {
            this.token = new EmptyToken();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TokenRun" /> class.
        ///     Sets a color depending on token's type and validity
        /// </summary>
        /// <param name="token">The token.</param>
        public TokenRun(Token token) : this(token.Symbol)
        {
            this.token = token;

            //set the color
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
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TokenRun" /> class. Required by interface
        /// </summary>
        /// <param name="text">
        ///     A string specifying the initial contents of the <see cref="T:System.Windows.Documents.Run" />
        ///     object.
        /// </param>
        /// <param name="insertionPosition">
        ///     A <see cref="T:System.Windows.Documents.TextPointer" /> specifying an insertion
        ///     position at which to insert the text run after it is created, or null for no automatic insertion.
        /// </param>
        public TokenRun(string text, TextPointer insertionPosition) : base(text, insertionPosition)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="TokenRun" /> based on the nextToken parameter. Inserts a space between them if needed.
        ///     This method is used to create a formatted chain of Tokens.
        /// </summary>
        /// <param name="nextToken">The next token.</param>
        /// <returns></returns>
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

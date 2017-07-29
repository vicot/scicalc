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

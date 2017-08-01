// Token.cs Copyright (c) 2017 vicot
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
using System.Linq;
using SciCalc.Tokens.Operators;

namespace SciCalc.Tokens
{
    public abstract class Token
    {
        //by default, don't require any tokens on either side
        protected List<Type> leftBinding;
        protected List<Type> rightBinding;

        protected Token(string symbol)
        {
            this.Symbol = symbol.Trim();
            if (symbol.Length > 1)
            {
                this.PrefixSpace = symbol.First() == ' ';
                this.PostfixSpace = symbol.Last() == ' ';
            }
        }

        public TokenType TokenType { get; protected set; }
        public double Value { get; set; }
        public string Symbol { get; set; }
        public int ArgumentCount { get; protected set; }
        public int Priority { get; protected set; }

        public bool PrefixSpace { get; }
        public bool PostfixSpace { get; }

        //by default all tokens are valid unless explicitly invalidated
        public bool IsValid { get; set; } = true;
        public virtual string ErrorMessage => $"Invalid token '{this.Symbol}'";

        //check if token type is assignable to any of the valid bindings, or check if can be empty (null binding && null token)
        public bool IsLeftBound(Token token) => token is ExcessiveDotToken || this.leftBinding.Any(t => token == null && t == null || t != null && t.IsInstanceOfType(token));
        public bool IsRightBound(Token token) => token is ExcessiveDotToken || this.rightBinding.Any(t => token == null && t == null || t != null && t.IsInstanceOfType(token));

        public virtual double Execute(double arg)
        {
            throw new NotImplementedException();
        }

        public virtual double Execute(double arg1, double arg2)
        {
            throw new NotImplementedException();
        }

        #region equality

        protected bool Equals(Token other)
        {
            return this.TokenType == other.TokenType && this.Value.Equals(other.Value) &&
                   string.Equals(this.Symbol, other.Symbol) && this.ArgumentCount == other.ArgumentCount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((Token) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) this.TokenType;
                hashCode = (hashCode * 397) ^ this.Value.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Symbol.GetHashCode();
                hashCode = (hashCode * 397) ^ this.ArgumentCount;
                return hashCode;
            }
        }

        public static bool operator ==(Token left, Token right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Token left, Token right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}

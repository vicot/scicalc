﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using SciCalc.Tokens.Operators;

namespace SciCalc.Tokens
{
    public abstract class Token
    {
        private static int superIndex;

        protected Token(string symbol)
        {
            this.Symbol = symbol.Trim();
            if (symbol.Length > 1)
            {
                this.PrefixSpace = symbol.First() == ' ';
                this.PostfixSpace = symbol.Last() == ' ';
            }

            this.Index = superIndex++;
        }

        public TokenType TokenType { get; protected set; }
        public double Value { get; protected set; }
        public string Symbol { get; protected set; }
        public int ArgumentCount { get; protected set; }
        public int Priority { get; protected set; }
        public long Index { get; private set; }

        public bool PrefixSpace { get; }
        public bool PostfixSpace { get; }

        //by default, don't require any tokens on either side
        protected List<Type> leftBinding;
        protected List<Type> rightBinding;

        //check if token type is assignable to any of the valid bindings, or check if can be empty (null binding && null token)
        public bool IsLeftBound(Token token) => this.leftBinding.Any(t => (token == null && t == null) || (t != null && t.IsInstanceOfType(token)));
        public bool IsRightBound(Token token) => this.rightBinding.Any(t => (token == null && t == null) || (t != null && t.IsInstanceOfType(token)));

        //by default all tokens are valid unless explicitly invalidated
        public bool IsValid { get; set; } = true;
        public virtual string ErrorMessage => $"Invalid token '{this.Symbol}'.";

        //autogenerated token, such as closing parent, or missing 0 before dot
        public bool Inferred { get; set; } = false;

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
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

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
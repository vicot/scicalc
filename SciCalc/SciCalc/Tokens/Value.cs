﻿using System.Globalization;

namespace SciCalc.Tokens
{
    public class Value : Token
    {
        public Value(double value) : base(value.ToString(CultureInfo.InvariantCulture))
        {
            this.Priority = 0;
            this.ArgumentCount = 0;
            this.Type = TokenType.Value;
            this.Value = value;
        }
    }
}

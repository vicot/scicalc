namespace SciCalc.Tokens.Operators
{
    public class Operator : Token
    {
        public Operator(string symbol, bool valid = true) : base(symbol)
        {
            this.Type = TokenType.Operator;
            this.Value = 0.0;
            if (!valid)
            {
                this.IsValid = false;
            }
        }
    }
}

namespace SciCalc.Tokens.Operators
{
    public class Operator : Token
    {
        public Operator(string symbol) : base(symbol)
        {
            this.Type = TokenType.Operator;
            this.Value = 0.0;
        }
    }
}

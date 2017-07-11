namespace SciCalc.Tokens.Operators
{
    public abstract class Operator : Token
    {
        protected Operator() : base()
        {
            this.Type = TokenType.Operator;
            this.Value = 0.0;
        }
    }
}
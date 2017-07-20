namespace SciCalc.Tokens.Operators
{
    public abstract class Operator : Token
    {
        protected Operator()
        {
            this.Type = TokenType.Operator;
            this.Value = 0.0;
        }
    }
}

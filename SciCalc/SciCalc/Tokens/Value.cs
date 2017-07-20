namespace SciCalc.Tokens
{
    public class Value : Token
    {
        public Value()
        {
            this.Priority = 0;
            this.ArgumentCount = 0;
            this.Symbol = "";
            this.Type = TokenType.Value;
            this.Value = 0.0;
        }

        public Value(double value) : this()
        {
            this.Value = value;
        }
    }
}

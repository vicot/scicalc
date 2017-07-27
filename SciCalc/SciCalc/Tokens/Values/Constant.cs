namespace SciCalc.Tokens.Values
{
    public class Constant : Token
    {
        public Constant(string name) : base(name)
        {
            this.Priority = 0;
            this.ArgumentCount = 0;
            this.Type = TokenType.Constant;
            this.IsValid = false;
        }

        public Constant(string name, double value) : this(name)
        {
            this.Value = value;
            this.IsValid = true;
        }
    }
}

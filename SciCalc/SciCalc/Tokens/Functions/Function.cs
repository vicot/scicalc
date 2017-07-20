namespace SciCalc.Tokens.Functions
{
    public class Function : Token
    {
        public Function(string name) : base($" {name}")
        {
            this.Type = TokenType.Function;
            this.Value = 0.0;
            this.Priority = 6; //functions get the top priority
        }
    }
}

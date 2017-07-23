namespace SciCalc.Tokens
{
    public class EmptyToken : Token
    {
        public EmptyToken() : base("")
        {
            this.ArgumentCount = 0;
            this.Priority = 0;
        }
    }
}

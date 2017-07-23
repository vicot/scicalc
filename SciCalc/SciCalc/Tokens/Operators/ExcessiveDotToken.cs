namespace SciCalc.Tokens
{
    public class ExcessiveDotToken : Token
    {
        public ExcessiveDotToken() : base(".")
        {
            //not an actual operator, only used to display excessive dots in expression view
            this.ArgumentCount = 0;
            this.Priority = 0;
            this.IsValid = false;
        }

        public override string ErrorMessage => "Excessive '.' in expression.";
    }
}

namespace SciCalc.Tokens.Operators
{
    public class FactorialOperator : Operator
    {
        public FactorialOperator()
        {
            this.ArgumentCount = 1;
            this.Priority = 4;
            this.Symbol = "! ";
        }

        public override double Execute(double arg)
        {
            double result = 1;
            while (arg > 0)
            {
                result *= arg--;
            }

            return result;
        }
    }
}

namespace SciCalc
{
    public class HistoryEntry
    {
        public HistoryEntry(string expression, double result)
        {
            this.Expression = expression;
            this.Result = result;
        }

        public string Expression { get; set; }
        public double Result { get; set; }
    }
}
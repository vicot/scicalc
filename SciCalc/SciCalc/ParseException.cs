using System;

namespace SciCalc
{
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message)
        {
        }
    }
}
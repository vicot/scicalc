using System;

namespace SciCalc.Tokens.Functions
{
    public static class FunctionFactory
    {
        public static Token GetToken(string name)
        {
            switch (name)
            {
                case "sin": return new SinFunction();
                case "cos": return new CosFunction();
                case "tan": return new TanFunction();
                case "ctg": return new CtgFunction();
                case "ln": return new LnFunction();
                case "log": return new LogFunction();
                default: throw new ArgumentException($"'{name}' is not a valid function.");
            }
        }
    }
}
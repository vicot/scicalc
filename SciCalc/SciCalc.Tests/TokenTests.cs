using System;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SciCalc.Tokens.Operators;
using SciCalc.Tokens.Values;

namespace SciCalc.Tests
{
    [TestFixture]
    public class TokenTests
    {
        private Parser parser;

        [Test]
        public void TokenBindings()
        {
            var op = new SumOperator();
            var token1 = new IntegerValue(0);
            var token2 = new ParentOperator();
            var token3 = new CloseParentOperator();
            var token4 = new SubOperator();
            
            Assert.IsTrue(op.IsLeftBound(token1));
            Assert.IsFalse(op.IsLeftBound(token2));
            Assert.IsTrue(op.IsLeftBound(token3));
            Assert.IsFalse(op.IsLeftBound(token4));

            Assert.IsTrue(op.IsRightBound(token1));
            Assert.IsTrue(op.IsRightBound(token2));
            Assert.IsFalse(op.IsRightBound(token3));
            Assert.IsFalse(op.IsRightBound(token4));
        }


        [OneTimeSetUp]
        public void Setup()
        {
            this.parser = new Parser();
        }

        [TestCase("X+3-X", ExpectedResult = 3)]
        [TestCase("2*X-Y", ExpectedResult = 13)]
        public double Constants(string expr)
        {
            this.parser.SetConstant("X", 8);
            this.parser.SetConstant("Y", 3);

            this.parser.LoadToPostfix(expr);
            return this.parser.Solve();
        }

        [TestCase("sin(0)", ExpectedResult = 0)]
        [TestCase("sin(PI/2)", ExpectedResult = 1)]
        [TestCase("sin(PI/6)", ExpectedResult = 0.5)]
        public double FunctionSin(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return (float)this.parser.Solve();
        }

        [TestCase("cos(0)", ExpectedResult = 1)]
        [TestCase("cos(1_3PI)", ExpectedResult = 0.5)]
        public double FunctionCos(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return (float)this.parser.Solve();
        }

        [TestCase("tan(0)", ExpectedResult = 0)]
        [TestCase("tan(PI/4)", ExpectedResult = 1)]
        public double FunctionTan(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return (float)this.parser.Solve();
        }

        [TestCase("ctg(PI/4)", ExpectedResult = 1)]
        public double FunctionCtg(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return (float)this.parser.Solve();
        }

        [TestCase("ln(1)", ExpectedResult = 0)]
        [TestCase("ln(E^2)", ExpectedResult = 2)]
        public double FunctionLn(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return this.parser.Solve();
        }

        [TestCase("log2(8)", ExpectedResult = 3)]
        [TestCase("log10(100)", ExpectedResult = 2)]
        public double FunctionLog(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return this.parser.Solve();
        }

        [TestCase("2+2", ExpectedResult = 4)]
        [TestCase("2.3+2", ExpectedResult = 4.3)]
        public double OperatorSum(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return this.parser.Solve();
        }

        [TestCase("4-1", ExpectedResult = 3)]
        [TestCase("7.34-2.12", ExpectedResult = 5.22)]
        [TestCase("1-1+2", ExpectedResult = 2)]
        public double OperatorSub(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return this.parser.Solve();
        }

        [TestCase("2*3", ExpectedResult = 6)]
        [TestCase("2.5*3", ExpectedResult = 7.5)]
        public double OperatorMul(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return this.parser.Solve();
        }

        [TestCase("7/2", ExpectedResult = 3.5)]
        [TestCase("2.3/4.2", ExpectedResult = 2.3/4.2)]
        public double OperatorDiv(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return this.parser.Solve();
        }

        [TestCase("-3", ExpectedResult = -3)]
        [TestCase("7+-3", ExpectedResult = 4)]
        [TestCase("(3+3)-3", ExpectedResult = 3)]
        public double OperatorNegate(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return this.parser.Solve();
        }

        [TestCase("5_2", ExpectedResult = 2.5)]
        [TestCase("1_2/3_4", ExpectedResult = 2.0/3.0)]
        public double OperatorFraction(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return this.parser.Solve();
        }

        [TestCase("5#2", ExpectedResult = 1)]
        public double OperatorModulo(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return this.parser.Solve();
        }

        [TestCase("(5)", ExpectedResult = 5)]
        [TestCase("(((5)))", ExpectedResult = 5)]
        [TestCase("(5*(1+(2-1)))", ExpectedResult = 10)]
        public double OperatorParenthesis(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return this.parser.Solve();
        }

        [TestCase("7%", ExpectedResult = 0.07)]
        [TestCase("10+30%", ExpectedResult = 10.3)]
        [TestCase("5+5+30%", ExpectedResult = 10.3)]
        [TestCase("5+5+30%+5", ExpectedResult = 15.3)]
        [TestCase("5*50%", ExpectedResult = 2.5)]
        [TestCase("3+5*200%", ExpectedResult = 13)]
        [TestCase("10-30%", ExpectedResult = 9.7)]
        public double OperatorPercent(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return this.parser.Solve();
        }

        [TestCase("2^3", ExpectedResult = 8)]
        public double OperatorPower(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return this.parser.Solve();
        }

        [TestCase("√4", ExpectedResult = 2)]
        [TestCase("3√27", ExpectedResult = 3)]
        [TestCase("3√(3*3*3)", ExpectedResult = 3)]
        public double OperatorRoot(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return this.parser.Solve();
        }

        [TestCase("5!", ExpectedResult = 120)]
        public double OperatorFactorial(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return this.parser.Solve();
        }

        [TestCase("2+2*2", ExpectedResult = 6)]
        [TestCase("2*(2+2)", ExpectedResult = 8)]
        [TestCase("2*3^2", ExpectedResult = 18)]
        [TestCase("3+2+50%+5", ExpectedResult = 10.5)]
        [TestCase("√4+5", ExpectedResult = 7)]
        [TestCase("3+7*log3(7+2)-8", ExpectedResult = 9)]
        [TestCase("sin(1-1+1_2*PI)", ExpectedResult = 1)]
        public double OperatorPriority(string expr)
        {
            this.parser.LoadToPostfix(expr);
            return this.parser.Solve();
        }
    }
}

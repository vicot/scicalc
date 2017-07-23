using System;
using NUnit.Framework;

namespace SciCalc.Tests
{
    [TestFixture]
    public class TokenTests
    {
        [Test]
        public void Constants()
        {
            var parser = new Parser();
            parser.SetVariable("X", 8);
            parser.LoadToPostfix("X+3-X");
            double result = parser.Solve();
            double expected = 3;
            Assert.That(result, Is.EqualTo(expected));

            parser.SetVariable("Y", 3);
            parser.LoadToPostfix("2*X-Y");
            result = parser.Solve();
            expected = 13;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionCos()
        {
            var parser = new Parser();
            parser.LoadToPostfix("cos(17)");
            double result = parser.Solve();
            double expected = Math.Cos(17);
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("cos(PI/2)");
            result = parser.Solve();
            expected = Math.Cos(Math.PI / 2);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionCtg()
        {
            var parser = new Parser();
            parser.LoadToPostfix("ctg(17)");
            double result = parser.Solve();
            double expected = 1 / Math.Tan(17);
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("ctg(PI/2)");
            result = parser.Solve();
            expected = 1 / Math.Tan(Math.PI / 2);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionLn()
        {
            var parser = new Parser();
            parser.LoadToPostfix("ln(7)");
            double result = parser.Solve();
            double expected = Math.Log(7);
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("ln(E^2)");
            result = parser.Solve();
            expected = 2;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionLog()
        {
            var parser = new Parser();
            parser.LoadToPostfix("log2(8)");
            double result = parser.Solve();
            double expected = 3;
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("log10(100)");
            result = parser.Solve();
            expected = 2;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionSin()
        {
            var parser = new Parser();
            parser.LoadToPostfix("sin(17)");
            double result = parser.Solve();
            double expected = Math.Sin(17);
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("sin(PI/2)");
            result = parser.Solve();
            expected = Math.Sin(Math.PI / 2);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionTan()
        {
            var parser = new Parser();
            parser.LoadToPostfix("tan(17)");
            double result = parser.Solve();
            double expected = Math.Tan(17);
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("tan(PI/2)");
            result = parser.Solve();
            expected = Math.Tan(Math.PI / 2);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorDiv()
        {
            var parser = new Parser();
            parser.LoadToPostfix("7/2");
            double result = parser.Solve();
            var expected = 3.5;
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("2.3/4.2");
            result = parser.Solve();
            expected = 2.3 / 4.2;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorFactorial()
        {
            var parser = new Parser();
            parser.LoadToPostfix("5!");
            double result = parser.Solve();
            double expected = 120;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorFraction()
        {
            var parser = new Parser();
            parser.LoadToPostfix("5_2");
            double result = parser.Solve();
            var expected = 2.5;
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("1_2/3_4");
            result = parser.Solve();
            expected = 2.0 / 3.0;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorModulo()
        {
            var parser = new Parser();
            parser.LoadToPostfix("5 # 2");
            double result = parser.Solve();
            double expected = 1;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorMul()
        {
            var parser = new Parser();
            parser.LoadToPostfix("2*3");
            double result = parser.Solve();
            double expected = 6;
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("2.5*3");
            result = parser.Solve();
            expected = 7.5;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorNegate()
        {
            var parser = new Parser();
            parser.LoadToPostfix("-3");
            double result = parser.Solve();
            double expected = -3;
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("7+-3");
            result = parser.Solve();
            expected = 4;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorParenthesis()
        {
            var parser = new Parser();
            parser.LoadToPostfix("(5)");
            double result = parser.Solve();
            double expected = 5;
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("(((5)))");
            result = parser.Solve();
            expected = 5;
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("(5*(1+(2-1)))");
            result = parser.Solve();
            expected = 10;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorPercent()
        {
            var parser = new Parser();
            parser.LoadToPostfix("7%");
            double result = parser.Solve();
            var expected = 0.07;
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("10+30%");
            result = parser.Solve();
            expected = 10.3;
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("5+5+30%");
            result = parser.Solve();
            expected = 10.3;
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("5+5+30%+5");
            result = parser.Solve();
            expected = 15.3;
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("5*50%");
            result = parser.Solve();
            expected = 2.5;
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("3+5*200%");
            result = parser.Solve();
            expected = 13;
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("10-30%");
            result = parser.Solve();
            expected = 9.7;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorPower()
        {
            var parser = new Parser();
            parser.LoadToPostfix("2^3");
            double result = parser.Solve();
            double expected = 8;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorPriority()
        {
            var parser = new Parser();
            parser.LoadToPostfix("2+2*2");
            double result = parser.Solve();
            double expected = 6;
            Assert.That(expected, Is.EqualTo(result), "sum before mul");

            parser.LoadToPostfix("2*(2+2)");
            result = parser.Solve();
            expected = 8;
            Assert.That(expected, Is.EqualTo(result), "parenthesis ignored");

            parser.LoadToPostfix("2*3^2");
            result = parser.Solve();
            expected = 18;
            Assert.That(expected, Is.EqualTo(result), "mul before power");

            parser.LoadToPostfix("3+2+50%+5");
            result = parser.Solve();
            expected = 10.5;
            Assert.That(expected, Is.EqualTo(result), "percent should only affect the 50");

            parser.LoadToPostfix("√4+5");
            result = parser.Solve();
            expected = 7;
            Assert.That(expected, Is.EqualTo(result), "sum before root");

            parser.LoadToPostfix("(5-3)√4+5");
            result = parser.Solve();
            expected = 7;
            Assert.That(expected, Is.EqualTo(result), "should solve 5-3 first to create square root");

            parser.LoadToPostfix("3+7*log3(7+2)-8");
            result = parser.Solve();
            expected = 9;
            Assert.That(expected, Is.EqualTo(result), "");
        }

        [Test]
        public void OperatorRoot()
        {
            var parser = new Parser();
            parser.LoadToPostfix("√2");
            double result = parser.Solve();
            double expected = Math.Sqrt(2);
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("3√27");
            result = parser.Solve();
            expected = 3;
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("3√(3*3*3)");
            result = parser.Solve();
            expected = 3;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorSub()
        {
            var parser = new Parser();
            parser.LoadToPostfix("4-1");
            double result = parser.Solve();
            double expected = 3;
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("7.34-2.12");
            result = parser.Solve();
            expected = 5.22;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorSum()
        {
            var parser = new Parser();
            parser.LoadToPostfix("2+2");
            double result = parser.Solve();
            double expected = 4;
            Assert.That(result, Is.EqualTo(expected));

            parser.LoadToPostfix("2.3+2");
            result = parser.Solve();
            expected = 4.3;
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}

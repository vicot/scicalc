using System;
using NUnit.Framework;
using SciCalc;

namespace SciCalc.Tests
{
    [TestFixture]
    public class ParserTests
    {

        [Test]
        public void OperatorSum()
        {
            var parser = new Parser();
            parser.Parse("2+2");
            var result = parser.Solve();
            double expected = 4;
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("2.3+2");
            result = parser.Solve();
            expected = 4.3;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorSub()
        {
            var parser = new Parser();
            parser.Parse("4-1");
            var result = parser.Solve();
            double expected = 3;
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("7.34-2.12");
            result = parser.Solve();
            expected = 5.22;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorMul()
        {
            var parser = new Parser();
            parser.Parse("2*3");
            var result = parser.Solve();
            double expected = 6;
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("2.5*3");
            result = parser.Solve();
            expected = 7.5;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorDiv()
        {
            var parser = new Parser();
            parser.Parse("7/2");
            var result = parser.Solve();
            double expected = 3.5;
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("2.3/4.2");
            result = parser.Solve();
            expected = 2.3/4.2;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorParenthesis() {
            var parser = new Parser();
            parser.Parse("(5)");
            var result = parser.Solve();
            double expected = 5;
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("(((5)))");
            result = parser.Solve();
            expected = 5;
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("(5*(1+(2-1)))");
            result = parser.Solve();
            expected = 10;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorNegate()
        {
            var parser = new Parser();
            parser.Parse("-3");
            var result = parser.Solve();
            double expected = -3;
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("7+-3");
            result = parser.Solve();
            expected = 4;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorPercent()
        {
            var parser = new Parser();
            parser.Parse("7%");
            var result = parser.Solve();
            double expected = 0.07;
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("10+30%");
            result = parser.Solve();
            expected = 13;
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("5+5+30%");
            result = parser.Solve();
            expected = 13;
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("5+5+30%+5");
            result = parser.Solve();
            expected = 19.5;
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("5*50%");
            result = parser.Solve();
            expected = 7.5;
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("3+5*200%");
            result = parser.Solve();
            expected = 13;
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("10-30%");
            result = parser.Solve();
            expected = 7;
            Assert.That(result, Is.EqualTo(expected));



        }

        [Test]
        public void OperatorPower()
        {
            var parser = new Parser();
            parser.Parse("2^3");
            var result = parser.Solve();
            double expected = 8;
            Assert.That(result, Is.EqualTo(expected));

        }

        [Test]
        public void OperatorRoot()
        {
            var parser = new Parser();
            parser.Parse("√2");
            var result = parser.Solve();
            double expected = Math.Sqrt(2);
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("3√27");
            result = parser.Solve();
            expected = 3;
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("3√(3*3*3)");
            result = parser.Solve();
            expected = 3;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorFraction()
        {
            //todo ???
            var parser = new Parser();
            Assert.Fail("test not implemented");
            //parser.Parse("5⁄3");
            //double expected = 4;
            //Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorFactorial()
        {
            var parser = new Parser();
            parser.Parse("5!");
            var result = parser.Solve();
            double expected = 120;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorModulo()
        {
            var parser = new Parser();
            parser.Parse("5mod2");
            var result = parser.Solve();
            double expected = 1;
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("5mod3.5");
            result = parser.Solve();
            expected = double.NaN;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionSin()
        {
            var parser = new Parser();
            parser.Parse("sin(17)");
            var result = parser.Solve();
            double expected = Math.Sin(17);
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("sin(pi/2)");
            result = parser.Solve();
            expected = Math.Sin(Math.PI / 2);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionCos()
        {
            var parser = new Parser();
            parser.Parse("cos(17)");
            var result = parser.Solve();
            double expected = Math.Cos(17);
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("cos(pi/2)");
            result = parser.Solve();
            expected = Math.Cos(Math.PI/2);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionTan()
        {
            var parser = new Parser();
            parser.Parse("tan(17)");
            var result = parser.Solve();
            double expected = Math.Tan(17);
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("tan(pi/2)");
            result = parser.Solve();
            expected = Math.Tan(Math.PI / 2);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionCtg()
        {
            var parser = new Parser();
            parser.Parse("ctg(17)");
            var result = parser.Solve();
            double expected = 1/Math.Tan(17);
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("ctg(pi/2)");
            result = parser.Solve();
            expected = 1 /Math.Tan(Math.PI / 2);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionLn()
        {
            var parser = new Parser();
            parser.Parse("ln(7)");
            var result = parser.Solve();
            double expected = Math.Log(7);
            Assert.That(result, Is.EqualTo(expected));
            
            parser.Parse("ln(e^2)");
            result = parser.Solve();
            expected = 2;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionLog()
        {
            var parser = new Parser();
            parser.Parse("log2(8)");
            var result = parser.Solve();
            double expected = 3;
            Assert.That(result, Is.EqualTo(expected));

            parser.Parse("log10(100)");
            result = parser.Solve();
            expected = 2;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Constants()
        {
            var parser = new Parser();
            parser.SetVariable("X", 8);
            parser.Parse("X+3-X");
            var result = parser.Solve();
            double expected = 3;
            Assert.That(result, Is.EqualTo(expected));

            parser.SetVariable("Y", 3);
            parser.Parse("2*X-Y");
            result = parser.Solve();
            expected = 13;
            Assert.That(result, Is.EqualTo(expected));

        }

        [Test]
        public void OperatorPriority()
        {
            var parser = new Parser();
            parser.Parse("2+2*2");
            var result = parser.Solve();
            double expected = 6;
            Assert.That(expected, Is.EqualTo(result), "sum before mul");

            parser.Parse("2*(2+2)");
            result = parser.Solve();
            expected = 8;
            Assert.That(expected, Is.EqualTo(result), "parenthesis ignored");

            parser.Parse("2*3^2");
            result = parser.Solve();
            expected = 18;
            Assert.That(expected, Is.EqualTo(result), "mul before power");

            parser.Parse("3+2+50%+5");
            result = parser.Solve();
            expected = 15;
            Assert.That(expected, Is.EqualTo(result), "percent should added to result of the whole block");

            parser.Parse("√4+5");
            result = parser.Solve();
            expected = 7;
            Assert.That(expected, Is.EqualTo(result), "sum before root");


        }

    }
}
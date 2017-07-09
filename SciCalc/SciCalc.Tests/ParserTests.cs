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
            var result = parser.Parse("2+2");
            double expected = 4;
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("2.3+2");
            expected = 4.3;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorSub()
        {
            var parser = new Parser();
            var result = parser.Parse("4-1");
            double expected = 3;
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("7.34-2.12");
            expected = 5.22;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorMul()
        {
            var parser = new Parser();
            var result = parser.Parse("2*3");
            double expected = 6;
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("2.5*3");
            expected = 7.5;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorDiv()
        {
            var parser = new Parser();
            var result = parser.Parse("7/2");
            double expected = 3.5;
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("2.3/4.2");
            expected = 0.547619047619048;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorParenthesis() {
            var parser = new Parser();
            var result = parser.Parse("(5)");
            double expected = 5;
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("(((5)))");
            expected = 5;
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("(5*(1+(2-1)))");
            expected = 10;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorNegate()
        {
            var parser = new Parser();
            var result = parser.Parse("-3");
            double expected = -3;
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("7+-3");
            expected = 4;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorPercent()
        {
            var parser = new Parser();
            var result = parser.Parse("7%");
            double expected = 0.07;
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("10+30%");
            expected = 13;
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("5+5+30%");
            expected = 13;
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("5+5+30%+5");
            expected = 19.5;
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("5*50%");
            expected = 7.5;
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("3+5*200%");
            expected = 13;
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("10-30%");
            expected = 7;
            Assert.That(result, Is.EqualTo(expected));



        }

        [Test]
        public void OperatorPower()
        {
            var parser = new Parser();
            var result = parser.Parse("2^3");
            double expected = 8;
            Assert.That(result, Is.EqualTo(expected));

        }

        [Test]
        public void OperatorRoot()
        {
            var parser = new Parser();
            var result = parser.Parse("√2");
            double expected = Math.Sqrt(2);
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("3√27");
            expected = 3;
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("3√(3*3*3)");
            expected = 3;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorFraction()
        {
            //todo ???
            var parser = new Parser();
            Assert.Fail("test not implemented");
            //var result = parser.Parse("5⁄3");
            //double expected = 4;
            //Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorFactorial()
        {
            var parser = new Parser();
            var result = parser.Parse("5!");
            double expected = 120;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void OperatorModulo()
        {
            var parser = new Parser();
            var result = parser.Parse("5mod2");
            double expected = 1;
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("5mod3.5");
            expected = double.NaN;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionSin()
        {
            var parser = new Parser();
            var result = parser.Parse("sin(17)");
            double expected = Math.Sin(17);
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("sin(pi/2)");
            expected = Math.Sin(Math.PI / 2);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionCos()
        {
            var parser = new Parser();
            var result = parser.Parse("cos(17)");
            double expected = Math.Cos(17);
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("cos(pi/2)");
            expected = Math.Cos(Math.PI/2);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionTan()
        {
            var parser = new Parser();
            var result = parser.Parse("tan(17)");
            double expected = Math.Tan(17);
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("tan(pi/2)");
            expected = Math.Tan(Math.PI / 2);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionCtg()
        {
            var parser = new Parser();
            var result = parser.Parse("ctg(17)");
            double expected = 1/Math.Tan(17);
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("ctg(pi/2)");
            expected = 1/Math.Tan(Math.PI / 2);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionLn()
        {
            var parser = new Parser();
            var result = parser.Parse("ln(7)");
            double expected = Math.Log(7);
            Assert.That(result, Is.EqualTo(expected));
            
            result = parser.Parse("ln(e^2)");
            expected = 2;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FunctionLog()
        {
            var parser = new Parser();
            var result = parser.Parse("log2(8)");
            double expected = 3;
            Assert.That(result, Is.EqualTo(expected));

            result = parser.Parse("log10(100)");
            expected = 2;
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Constants()
        {
            var parser = new Parser();
            parser.SetVariable("X", 8);
            var result = parser.Parse("X+3-X");
            double expected = 3;
            Assert.That(result, Is.EqualTo(expected));

            parser.SetVariable("Y", 3);
            result = parser.Parse("2*X-Y");
            expected = 13;
            Assert.That(result, Is.EqualTo(expected));

        }

        [Test]
        public void OperatorPriority()
        {
            var parser = new Parser();
            var result = parser.Parse("2+2*2");
            double expected = 6;
            Assert.That(expected, Is.EqualTo(result), "sum before mul");

            result = parser.Parse("2*(2+2)");
            expected = 8;
            Assert.That(expected, Is.EqualTo(result), "parenthesis ignored");

            result = parser.Parse("2*3^2");
            expected = 18;
            Assert.That(expected, Is.EqualTo(result), "mul before power");

            result = parser.Parse("3+2+50%+5");
            expected = 15;
            Assert.That(expected, Is.EqualTo(result), "percent should added to result of the whole block");

            result = parser.Parse("√4+5");
            expected = 7;
            Assert.That(expected, Is.EqualTo(result), "sum before root");


        }

    }
}
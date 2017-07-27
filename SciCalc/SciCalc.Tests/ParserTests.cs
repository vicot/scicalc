using System;
using System.Collections.Generic;
using NUnit.Framework;
using SciCalc.Tokens;
using SciCalc.Tokens.Operators;
using SciCalc.Tokens.Values;

namespace SciCalc.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void TokenizeExpression()
        {
            var expression = "15 + 3 - 6 * 10.3.5";

            var expected = new List<Token>
            {
                new IntegerValue(15),
                new SumOperator(),
                new IntegerValue(3),
                new SubOperator(),
                new IntegerValue(6),
                new MulOperator(),
                new DoubleValue(10.3),
                new ExcessiveDotToken(),
                new IntegerValue(5)
            };

            var p = new Parser();
            var result = p.ParseTokens(expression);

            Assert.That(result, Is.EquivalentTo(expected));
        }

        [Test]
        public void InsertMultiplyOperators()
        {

            var p = new Parser();
            p.LoadToPostfix("15(3-1)");
            var result = p.Solve();
            var expected = 30;

            Assert.AreEqual(result, expected, "Should add * between number and (");

            
        }

    }
}

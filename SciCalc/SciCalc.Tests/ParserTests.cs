using System;
using System.Collections.Generic;
using NUnit.Framework;
using SciCalc.Tokens;
using SciCalc.Tokens.Operators;

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
                new Value(15),
                new SumOperator(),
                new Value(3),
                new SubOperator(),
                new Value(6),
                new MulOperator(),
                new Value(10.3),
                new ExcessiveDotToken(),
                new Value(5)
            };

            var p = new Parser();
            var result = p.ParseTokens(expression);

            Assert.That(result, Is.EquivalentTo(expected));
        }

    }
}

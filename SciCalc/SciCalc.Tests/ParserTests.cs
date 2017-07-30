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
            var expression = "15 + 3 - 6 * 10.3.5 + (1) - -3";

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
                new IntegerValue(5),
                new SumOperator(),
                new ParentOperator(),
                new IntegerValue(1),
                new CloseParentOperator(),
                new SubOperator(),
                new NegateOperator(),
                new IntegerValue(3)
            };

            var p = new Parser();
            var result = p.ParseTokens(expression);

            Assert.That(result, Is.EquivalentTo(expected));
        }

        [Test]
        public void InsertMultiplyOperators()
        {
            /* add multiplication between:
            * - values (and constants)
            * - values and functions
            * - values and parenthesis
            * - closing and opening parenthesis
            * - left-bound unary operators and opening parenthesis
            * - closing parenthesis and values, functions
            * 
            * don't add multiplication for logartihm case: log2(...)
            */

            var p = new Parser();
            p.SetConstant("X", 2);

            p.LoadToPostfix("3!(3X)(100%(4))log2(2(2))");
            var result = p.Solve();
            var expected = 288;

            Assert.AreEqual(result, expected, "Should fix the expression to: 3! * (3*X)*(100% * (4))*log2(2*(2)) = 288");

            
        }

        

    }
}

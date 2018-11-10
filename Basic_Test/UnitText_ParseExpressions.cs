using System;
using System.Collections.Generic;
using System.Linq;
using Basic.Execute;
using Basic.Expressions;
using Basic.Parser;
using Xunit;

namespace Basic_Test
{
    public static class TestHelper
    {
        internal static void TestNumericExpression(string input, double expectedNumber)
        {
            var expression = PartsParser.ParseStringAsExpression(input);

            // Evaluate an expression, result must be numeric and a specific value
            var eu = new ExecutionContext();
            eu.Variables.Set("TEST", Value.CreateNumber(100));
            var value = expression.Evaluate(eu);

            Assert.True(value.IsNumber);
            if (Math.Abs(expectedNumber - value.NumberValue) > 0.00001)
            {
                Assert.Equal(expectedNumber, value.NumberValue);
            }
        }
        internal static void TestStringExpression(string input, string expectedValue)
        {
            var expression = PartsParser.ParseStringAsExpression(input.Replace('\'', '"'));

            var eu = new ExecutionContext();
            var value = expression.Evaluate(eu);

            Assert.True(value.IsString);
            Assert.Equal(expectedValue, value.StringValue);
        }
    }

    public class UnitText_ParseExpressions
    {
        [Fact]
        public void Parser_Addition()
        {
            TestHelper.TestNumericExpression("1 + 2", 3);
            TestHelper.TestNumericExpression("100 + 900", 1000);
            TestHelper.TestNumericExpression("1 + 2+3+4+5    +   6", 21);

            TestHelper.TestNumericExpression("100 - 12", 88);
            TestHelper.TestNumericExpression("21   -6-5-4-3-2-1    ", 0);
        }

        [Fact]
        public void Parser_Multiplication()
        {
            TestHelper.TestNumericExpression("10 * 9", 90);
            TestHelper.TestNumericExpression(" 1 * 2 * 3 * 4     *5", 120);
            TestHelper.TestNumericExpression("99/33", 3);
        }

        [Fact]
        public void Parser_Associativity()
        {
            TestHelper.TestNumericExpression("10-3-2", 5);
            TestHelper.TestNumericExpression("(10-3)-2", 5);
            TestHelper.TestNumericExpression("10-(3-2)", 9);

            TestHelper.TestNumericExpression("100/10/2", 5);
            TestHelper.TestNumericExpression("(100/10)/2", 5);
            TestHelper.TestNumericExpression("100/(10/2)", 20);
 
            TestHelper.TestNumericExpression("5^3^2", 1953125);
            TestHelper.TestNumericExpression("(5^3)^2", 15625);
            TestHelper.TestNumericExpression("5^(3^2)", 1953125); 
        }

        [Fact]
        public void Parser_Relational()
        {
            TestHelper.TestNumericExpression("1 < 2", 1);
            TestHelper.TestNumericExpression("1 <= 1", 1);
            TestHelper.TestNumericExpression("2 > 1", 1);
            TestHelper.TestNumericExpression("2 >= 2", 1);
            TestHelper.TestNumericExpression("1 = 1", 1);
            TestHelper.TestNumericExpression("1 <> 2", 1);

            TestHelper.TestNumericExpression("10 < 2", 0);
            TestHelper.TestNumericExpression("10 <= 1", 0);
            TestHelper.TestNumericExpression("2 > 10", 0);
            TestHelper.TestNumericExpression("2 >= 20", 0);
            TestHelper.TestNumericExpression("1 = 10", 0);
            TestHelper.TestNumericExpression("1 <> 1", 0);
        }

        [Fact]
        public void Parser_Parenthesis()
        {
            TestHelper.TestNumericExpression("2 * (1 + 3)", 8);
            TestHelper.TestNumericExpression("(1 + 1) * (5 - 3)", 4);
            TestHelper.TestNumericExpression("(2) * (((1) + 3))", 8);
            TestHelper.TestNumericExpression("(((1 + 1))) * ((((5) - 3)))", 4);
        }

        [Fact]
        public void Parser_Exponent()
        {
            TestHelper.TestNumericExpression("(1+2)^2", 9);
            TestHelper.TestNumericExpression("(1+2)^(3-1)", 9);
        }

        [Fact]
        public void Parser_StringExpression()
        {
            TestHelper.TestStringExpression("'abc' + 'def'", "abcdef");
            TestHelper.TestStringExpression("'abc' + 9", "abc9");
            TestHelper.TestStringExpression("100 + 'abc'", "100abc");
            TestHelper.TestStringExpression("'' + 'help'", "help");
        }
    }
}

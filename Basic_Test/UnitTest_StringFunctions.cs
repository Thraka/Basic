using System;
using Basic.Infrastructure;
using Xunit;

namespace Basic_Test
{
    public class UnitTest_StringFunctions
    {
         [Fact]
        public void Test_CHR()
        {
            TestHelper.TestStringExpression("CHR$(48)", "0");
            TestHelper.TestStringExpression("CHR$(65)", "A");
        }

        [Fact]
        public void Test_LEFT()
        {
            TestHelper.TestStringExpression("LEFT$('abcde', 2)", "ab");
            TestHelper.TestStringExpression("LEFT$('abcde', 0)", "");
            TestHelper.TestStringExpression("LEFT$('', 0)", "");
            TestHelper.TestStringExpression("LEFT$('abcde', 100)", "abcde");
            Assert.Throws<BasicRuntimeException>(() => TestHelper.TestStringExpression("LEFT$('abcde', -100)", "abcde"));
        }

        [Fact]
        public void Test_RIGHT()
        {
            TestHelper.TestStringExpression("RIGHT$('abcde', 2)", "de");
            TestHelper.TestStringExpression("RIGHT$('abcde', 0)", "");
            TestHelper.TestStringExpression("RIGHT$('', 0)", "");
            TestHelper.TestStringExpression("RIGHT$('abcde', 100)", "abcde");
            Assert.Throws<BasicRuntimeException>(() => TestHelper.TestStringExpression("RIGHT$('abcde', -100)", "abcde"));
        }
        
        [Fact]
        public void Test_MID()
        {
            TestHelper.TestStringExpression("MID$('abcde', 3)", "cde");
            TestHelper.TestStringExpression("MID$('abcde', 1)", "abcde");
            TestHelper.TestStringExpression("MID$('', 1)", "");
            TestHelper.TestStringExpression("MID$('abcde', 100)", "abcde");

            TestHelper.TestStringExpression("MID$('abcde', 3, 2)", "cd");
            TestHelper.TestStringExpression("MID$('abcde', 1, 3)", "abc");
            TestHelper.TestStringExpression("MID$('', 1, 1)", "");
            TestHelper.TestStringExpression("MID$('abcde', 100, 100)", "abcde");

            Assert.Throws<BasicRuntimeException>(() => TestHelper.TestStringExpression("mid$('abcde', 0)", "abcde"));
            Assert.Throws<BasicRuntimeException>(() => TestHelper.TestStringExpression("mid$('abcde', 1, -10)", "abcde"));
        }

        [Fact]
        public void Test_STR()
        {
            TestHelper.TestStringExpression("STR(1)", "1");
            TestHelper.TestStringExpression("STR(3.14159)", "3.14159");
            TestHelper.TestStringExpression("STR(-9)", "-9");
        }

        [Fact]
        public void Test_SPC()
        {
            TestHelper.TestStringExpression("SPC(1)", " ");
            TestHelper.TestStringExpression("SPC(10)", "          ");
        }
    }
}
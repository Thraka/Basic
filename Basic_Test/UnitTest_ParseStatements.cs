using System;
using System.Collections.Generic;
using System.Linq;
using Basic.Execute;
using Basic.Expressions;
using Basic.Parser;
using Xunit;

namespace Basic_Test
{
    public class UnitTest_ParseStatements
    {
        [Fact]
        public void Parser_Let()
        {
            var parser = new StatementParser("let a = 9");

            var statements = parser.ParseLine();

            Assert.False(parser.IsProgramLine);
            Assert.Equal(1, statements.Count);
        }

        [Fact]
        public void Parser_Run()
        {
            var parser = new StatementParser("run");
            var statements = parser.ParseLine();

            Assert.False(parser.IsProgramLine);
            Assert.Equal(1, statements.Count);
        }

        [Fact]
        public void MultiStatements()
        {
            var parser = new StatementParser("let a=1+2:let b=1:print (a+b)");
            var statements = parser.ParseLine();

            Assert.False(parser.IsProgramLine);
            Assert.Equal(3, statements.Count);
        }

        [Fact]
        public void Test_ProgramLine()
        {
            var parser = new StatementParser("20 let a=1+2:let b=1:print (a+b)");
            var statements = parser.ParseLine();

            Assert.True(parser.IsProgramLine);
            Assert.Equal(20, parser.LineNumber);
            Assert.Equal(3, statements.Count);
        }
    }
}

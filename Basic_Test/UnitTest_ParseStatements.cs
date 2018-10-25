using System;
using System.Collections.Generic;
using System.Linq;
using Basic.Execute;
using Basic.Expressions;
using Basic.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Basic_Test
{
    [TestClass]
    public class UnitTest_ParseStatements
    {
        [TestMethod]
        public void Parser_Let()
        {
            var parser = new StatementParser("let a = 9");

            var statements = parser.ParseLine();

            Assert.IsFalse(parser.IsProgramLine);
            Assert.AreEqual(statements.Count, 1);
        }

        [TestMethod]
        public void Parser_Run()
        {
            var parser = new StatementParser("run");
            var statements = parser.ParseLine();

            Assert.IsFalse(parser.IsProgramLine);
            Assert.AreEqual(statements.Count, 1);
        }

        [TestMethod]
        public void MultiStatements()
        {
            var parser = new StatementParser("let a=1+2:let b=1:print (a+b)");
            var statements = parser.ParseLine();

            Assert.IsFalse(parser.IsProgramLine);
            Assert.AreEqual(3, statements.Count);
        }

        [TestMethod]
        public void Test_ProgramLine()
        {
            var parser = new StatementParser("20 let a=1+2:let b=1:print (a+b)");
            var statements = parser.ParseLine();

            Assert.IsTrue(parser.IsProgramLine);
            Assert.AreEqual(20, parser.LineNumber);
            Assert.AreEqual(3, statements.Count);
        }
    }
}

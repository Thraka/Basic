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
    public class UnitTest_ParseProgram
    {
        [TestMethod]
        public void ParseProgram_Simple()
        {
            const string ProgramText = @"10 A=-3.141E2
                20 print A
            ";

            var isOK = ProgramList.TryLoadFromString(ProgramText, out ProgramList newProgram, out List<string> errorMessages);

            Assert.IsTrue(isOK);
            Assert.AreEqual(0, errorMessages.Count);
        }
        
        [TestMethod]
        public void ParseProgram_Exponent()
        {
            const string ProgramText = @"10 V0=100:R=2:A1=9
                20 A=-.5*R0*((V0/R)^2)+R*A1*A1
            ";

            var isOK = ProgramList.TryLoadFromString(ProgramText, out ProgramList newProgram, out List<string> errorMessages);

            Assert.IsTrue(isOK);
            Assert.AreEqual(0, errorMessages.Count);
        }
    }
}
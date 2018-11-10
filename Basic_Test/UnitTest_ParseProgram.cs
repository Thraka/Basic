using System;
using System.Collections.Generic;
using System.Linq;
using Basic.Execute;
using Basic.Expressions;
using Basic.Parser;
using Xunit;

namespace Basic_Test
{
    public class UnitTest_ParseProgram
    {
        [Fact]
        public void ParseProgram_Simple()
        {
            const string ProgramText = @"10 A=-3.141E2
                20 print A
            ";

            var isOK = ProgramList.TryLoadFromString(ProgramText, out ProgramList newProgram, out List<string> errorMessages);

            Assert.True(isOK);
            Assert.Empty(errorMessages);
        }
        
        [Fact]
        public void ParseProgram_Exponent()
        {
            const string ProgramText = @"10 V0=100:R=2:A1=9
                20 A=-.5*R0*((V0/R)^2)+R*A1*A1
            ";

            var isOK = ProgramList.TryLoadFromString(ProgramText, out ProgramList newProgram, out List<string> errorMessages);

            Assert.True(isOK);
            Assert.Empty(errorMessages);
        }
                
        [Fact]
        public void ParseProgram_NextVars()
        {
            const string ProgramText = @"10 FOR I=1 TO 10
                20 NEXT I
            ";

            var isOK = ProgramList.TryLoadFromString(ProgramText, out ProgramList newProgram, out List<string> errorMessages);

            Assert.True(isOK);
            Assert.Empty(errorMessages);
        }
                        
        [Fact]
        public void ParseProgram_Input()
        {
            const string ProgramText = @"10 INPUT ""met een prompt"",A
                20 INPUT A,B,C
            ";

            var isOK = ProgramList.TryLoadFromString(ProgramText, out ProgramList newProgram, out List<string> errorMessages);

            Assert.True(isOK);
            Assert.Empty(errorMessages);
        }
    }
}
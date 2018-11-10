using System;
using Xunit;

namespace Basic_Test
{
    public class UnitTest_NumericFunctions
    {
        [Fact]
        public void Test_Abs()
        {
            TestHelper.TestNumericExpression("ABS(9*8)", 72);
            TestHelper.TestNumericExpression("ABS(10-TEST)", 90);
            TestHelper.TestNumericExpression("ABS(10-10)", 0);
        }

        [Fact]
        public void Test_ATN()
        {
            TestHelper.TestNumericExpression("ATN(0)", 0);
            TestHelper.TestNumericExpression("ATN(1)", Math.PI / 4);
        }  

        [Fact]
        public void Test_COS()
        {
            TestHelper.TestNumericExpression("COS(0)", 1);
            TestHelper.TestNumericExpression("COS(3.14159)", -1);
        }    
  
        [Fact]
        public void Test_EXP()
        {
            TestHelper.TestNumericExpression("EXP(0)", 1);
            TestHelper.TestNumericExpression("EXP(1)", 2.718281);
            TestHelper.TestNumericExpression("EXP(2)", 7.389056);
        }  
 
        [Fact]
        public void Test_INT()
        {
            TestHelper.TestNumericExpression("INT(1.1)", 1);
            TestHelper.TestNumericExpression("INT(99.99)", 99);
            TestHelper.TestNumericExpression("INT(-9.6)", -10);
        }  
 
       [Fact]
        public void Test_LOG()
        {
            TestHelper.TestNumericExpression("LOG(2.7182818)", 1);
        }  
 
        [Fact]
        public void Test_SIN()
        {
            TestHelper.TestNumericExpression("SIN(0)", 0);
            TestHelper.TestNumericExpression("SIN(3.14159/2)", 1);
        }  
 
        [Fact]
        public void Test_SQRT()
        {
            TestHelper.TestNumericExpression("SQR(16)", 4);
            TestHelper.TestNumericExpression("SQR(144)", 12);
        }  
 
        [Fact]
        public void Test_TAN()
        {
            TestHelper.TestNumericExpression("TAN(3.14159265359)", 0);
        } 

        [Fact]
        public void Test_SGN()
        {
            TestHelper.TestNumericExpression("SGN(10 - 8)", 1);
            TestHelper.TestNumericExpression("SGN(10 - 10)", 0);
            TestHelper.TestNumericExpression("SGN(10 - 12)", -1);
        }  
        
        [Fact]
        public void Test_ASC()
        {
            TestHelper.TestNumericExpression("ASC(\"@\")", 64);
            TestHelper.TestNumericExpression("ASC(\"0\")", 48);
        }

        [Fact]
        public void Test_LEN()
        {
            TestHelper.TestNumericExpression("LEN(\"\")", 0);
            TestHelper.TestNumericExpression("LEN(\"abcde\")", 5);
            TestHelper.TestNumericExpression("LEN(\"AaBbCcDdEe\")", 10);
        }

        [Fact]
        public void Test_VAL()
        {
            TestHelper.TestNumericExpression("VAL(\"1\")", 1);
            TestHelper.TestNumericExpression("VAL(\"3.14159\")", 3.14159);
        }
    }
}

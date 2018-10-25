using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Basic_Test
{
    [TestClass]
    public class UnitTest_NumericFunctions
    {
        [TestMethod]
        public void Test_Abs()
        {
            TestHelper.TestNumericExpression("ABS(9*8)", 72);
            TestHelper.TestNumericExpression("ABS(10-TEST)", 90);
            TestHelper.TestNumericExpression("ABS(10-10)", 0);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            TPW.Data.Ball ball = new TPW.Data.Ball(0, 0, 10);
            TPW.Logic.TestClass logic = new TPW.Logic.TestClass();
            Assert.IsTrue(true);
        }
    }
}

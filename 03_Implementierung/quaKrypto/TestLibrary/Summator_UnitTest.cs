using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using quaKrypto.Models;

namespace TestLibrary
{
    [TestFixture]
    public class Summator_UnitTest
    {
        // Alexander Denner, 06.05.2023
        [Test]
        public void Add_Two_Numbers_Success()
        {
            //Arrange 
            Summator summator = new Summator();
            int x = 1;
            int y = 2;

            //Act
            int result = summator.Add(x, y);

            //Assert
            int expected_result = 3;
            Assert.AreEqual(expected_result, result);
        }
    }
}

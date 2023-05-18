using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using NUnit.Framework;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;
using Information = quaKrypto.Models.Classes.Information;

namespace TestLibrary
{
    [TestFixture]
    public class Operationen
    {
        // Alexander Denner, 06.05.2023
        [Test]
        public void HandlungsschrittDurchfuehren()
        {
            //Arrange 
            Information op1 = new Information(0, "Mende", InformationsEnum.asciiText, "Chris");
            Information op2 = new Information(0, "Mende", InformationsEnum.asciiText, "Mende");
            Handlungsschritt summator = new Handlungsschritt(OperationsEnum.bitmaskeGenerieren, op1, op2, RolleEnum.Alice );

            //Act
            summator.HandlungsschrittAusfuehren();

            //Assert
            int expected_result = 3;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quaKrypto.Models.Enums;

namespace quaKrypto.Models.Classes
{
    internal class Operationen
    {
        public Information NachrichtSenden()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information NachrichtEmpfangen()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public  Information NachrichtAbhoeren()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information BitfolgeGenerierenZahl()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information BitfolgeGenerierenAngabe()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information PolarisationsschemataGenerierenZahl()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information PolarisationsschemataGenerierenAngabe()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information PhotonenGenerieren()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information BitmaskeGenerieren()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information PolschataVergleichen()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information BitfolgenVergleichen()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information PhotonenZuBitfolge()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information TextVerschluesseln()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information TextEntschluesseln()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information BitsStreichen()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information BitsFreiBearbeiten()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }

        public Information ZugBeenden()
        {
            return new Information(2, "Mende", InformationsEnum.asciiText, "Chris", RolleEnum.Alice);
        }
    }
}

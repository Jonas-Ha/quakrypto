using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace quaKrypto.Services
{

    //Konverter der im SpielView verwendet wird, damit die Informationen nach dem Informationstyp Farbkodiert werden
    public class InformationFarbenWaehler : IValueConverter
    {
        //Dictionary anlegen, das das Colorcoding angibt
        private static Dictionary<InformationsEnum, Color> InfoZuFarbe =
            new Dictionary<InformationsEnum, Color>()
            {      
                {InformationsEnum.zahl, Colors.AliceBlue},
                {InformationsEnum.bitfolge, Colors.LightYellow},
                {InformationsEnum.photonen, Colors.LightGreen},
                {InformationsEnum.polarisationsschemata, Colors.LightCoral},
                {InformationsEnum.unscharfePhotonen, Colors.Olive},
                {InformationsEnum.asciiText, Colors.LightPink},
                {InformationsEnum.verschluesselterText, Colors.LightSalmon},
            };

        //Funktion für die View um aus einem InformationsEnum eine Farbe zu bekommen
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Prüfen ob der Eingabewert ein InformationsEnum ist
            if (value is InformationsEnum)
            {
                //Value in ein InformationsEnum wandeln
                InformationsEnum infotyp = (InformationsEnum)value;
                Color del = Colors.White;//Standardfarbe falls im Dictionary kein Eintrag für den InformationsEnum gefunden wurde
                //Versuchen eine Farbe zurückzu bekommen
                if (InfoZuFarbe.TryGetValue(infotyp, out del))
                {
                    return new SolidColorBrush(del);
                }
                else return new SolidColorBrush(del);
            }
            //Falls Value kein InformationsEnum ist nicht machen
            return Binding.DoNothing;
        }

        //Nicht implementiert wird nicht benötigt
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

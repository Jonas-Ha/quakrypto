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
    //Konverter der im AufzeichnungsView verwendet wird, damit die Einträge der Rollen Farbkodiert werden
    internal class HandlungsschrittRolleZuFarbe : IValueConverter
    {
        //Dictionary anlegen, das das Colorcoding angibt
        private static Dictionary<RolleEnum, Color> RolleZuFarbe =
            new Dictionary<RolleEnum, Color>()
            {
                {RolleEnum.Alice, Colors.LightGreen},
                {RolleEnum.Eve, Colors.Orange},
                {RolleEnum.Bob, Colors.LightPink},
            };

        //Funktion für die View aus einem RollenEnum eine Farbe zu bekommen
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Prüfen ob der Eingabewert ein RolleEnum ist
            if (value is RolleEnum)
            {
                //Value in ein RolleEnum wandeln
                RolleEnum rolletyp = (RolleEnum)value;
                Color del = Colors.White; //Standardfarbe falls im Dictionary kein Eintrag für den RolleEnum gefunden wurde
                //Versuchen eine Farbe zurückzu bekommen
                if (RolleZuFarbe.TryGetValue(rolletyp, out del))
                {
                    return new SolidColorBrush(del);
                }
                else return new SolidColorBrush(del);
            }
            //Falls Value kein RolleEnum ist nicht machen
            return Binding.DoNothing;
        }

        //Nicht implementiert wird nicht benötigt
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

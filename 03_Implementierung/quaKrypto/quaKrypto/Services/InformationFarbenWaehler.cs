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
 
    public class InformationFarbenWaehler : IValueConverter
    {
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

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is InformationsEnum)
            {
                InformationsEnum infotyp = (InformationsEnum)value;
                Color del = Colors.White;
                if (InfoZuFarbe.TryGetValue(infotyp, out del))
                {
                    return new SolidColorBrush(del);
                }
                else return new SolidColorBrush(del);
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

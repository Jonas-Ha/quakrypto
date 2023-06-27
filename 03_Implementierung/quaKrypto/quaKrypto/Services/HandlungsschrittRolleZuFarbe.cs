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
    internal class HandlungsschrittRolleZuFarbe : IValueConverter
    {
        private static Dictionary<RolleEnum, Color> RolleZuFarbe =
            new Dictionary<RolleEnum, Color>()
            {
                {RolleEnum.Alice, Colors.LightGreen},
                {RolleEnum.Eve, Colors.Orange},
                {RolleEnum.Bob, Colors.LightPink},
            };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RolleEnum)
            {
                RolleEnum rolletyp = (RolleEnum)value;
                Color del = Colors.White;
                if (RolleZuFarbe.TryGetValue(rolletyp, out del))
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

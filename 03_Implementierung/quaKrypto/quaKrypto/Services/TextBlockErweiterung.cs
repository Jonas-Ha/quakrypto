using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace quaKrypto.Services
{
    public class TextBlockErweiterung : DependencyObject
    {
        public static string GetFormattedText(DependencyObject obj)
        {
            Trace.WriteLine("GetFormattedText");
            return (string)obj.GetValue(FormattedTextProperty);
        }

        public static void SetFormattedText(DependencyObject obj, string value)
        {
            Trace.WriteLine("Setfr");
            obj.SetValue(FormattedTextProperty, value);
        }

        public static readonly DependencyProperty FormattedTextProperty =
            DependencyProperty.Register("FormattedText", typeof(string), typeof(TextBlockErweiterung),
            new PropertyMetadata("TEST", (sender, e) =>
            {
                string text = e.NewValue as string;
                var textBl = sender as TextBlock;

                Trace.WriteLine("Text Was" + text);
                if (textBl != null)
                {
                    textBl.Inlines.Clear();
                    Regex regx = new Regex(@"(http://[^\s]+)", RegexOptions.IgnoreCase);
                    var str = regx.Split(text);
                    for (int i = 0; i < str.Length; i++)
                        if (i % 2 == 0)
                            textBl.Inlines.Add(new Run { Text = str[i] });
                        else
                        {
                            Hyperlink link = new Hyperlink { NavigateUri = new Uri(str[i]), Foreground = Application.Current.Resources["PhoneAccentBrush"] as SolidColorBrush };
                            link.Inlines.Add(new Run { Text = str[i] });
                            textBl.Inlines.Add(link);
                        }
                }
            }));
    }
}

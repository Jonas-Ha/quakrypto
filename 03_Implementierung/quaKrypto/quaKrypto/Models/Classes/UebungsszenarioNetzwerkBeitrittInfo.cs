using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace quaKrypto.Models.Classes
{
    public class UebungsszenarioNetzwerkBeitrittInfo
    {
        public string Lobbyname { get; set; }
        public string Protokoll { get; set; }
        public string Variante { get; set; }
        public string Schwierigkeitsgrad { get; set; }
        public BitmapImage AliceIcon { get; set; }
        public BitmapImage BobIcon { get; set; }
        public BitmapImage EveIcon { get; set; }
    }
}

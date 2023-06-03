using quaKrypto.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace quaKrypto.Models.Classes
{
    public class UebungsszenarioNetzwerkBeitrittInfo
    {
        //TODO Wie mit den Rollen machen?
        public UebungsszenarioNetzwerkBeitrittInfo(IPAddress address, string lobbyname, string protokoll, string variante, SchwierigkeitsgradEnum schwierigkeitsgrad)
        {
            IPAddress = address;
            Lobbyname = lobbyname;
            Protokoll = protokoll;
            Variante = variante;
            Schwierigkeitsgrad = schwierigkeitsgrad;
        }
        public IPAddress IPAddress { get; set; }
        public string Lobbyname { get; set; }
        public string Protokoll { get; set; }
        public string Variante { get; set; }
        public SchwierigkeitsgradEnum Schwierigkeitsgrad { get; set; }
        public BitmapImage AliceIcon { get; set; }
        public BitmapImage BobIcon { get; set; }
        public BitmapImage EveIcon { get; set; }
    }
}

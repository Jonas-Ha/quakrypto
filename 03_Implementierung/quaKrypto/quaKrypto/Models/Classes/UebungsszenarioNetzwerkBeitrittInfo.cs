using quaKrypto.Models.Enums;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace quaKrypto.Models.Classes
{
    public class UebungsszenarioNetzwerkBeitrittInfo
    {
        public UebungsszenarioNetzwerkBeitrittInfo(IPAddress address, string lobbyname, string protokoll, string variante, SchwierigkeitsgradEnum schwierigkeitsgrad, bool aliceState, bool bobState, bool eveState)
        {
            IPAddress = address;
            Lobbyname = lobbyname;
            Protokoll = protokoll;
            Variante = variante;
            Schwierigkeitsgrad = schwierigkeitsgrad;
            AliceState = aliceState;
            BobState = bobState;
            EveState = eveState;
        }
        public IPAddress IPAddress { get; set; }
        public string Lobbyname { get; set; }
        public string Protokoll { get; set; }
        public string Variante { get; set; }
        public SchwierigkeitsgradEnum Schwierigkeitsgrad { get; set; }
        public BitmapImage AliceIcon { get { return new BitmapImage(new Uri(AliceState ? "pack://application:,,,/Icons/Spiel/Alice/Alice_128px.png" : "pack://application:,,,/Icons/Spiel/Alice/Alice_128px_grey.png")); } }
        public BitmapImage BobIcon { get { return new BitmapImage(new Uri(BobState ? "pack://application:,,,/Icons/Spiel/Bob/Bob_128px.png" : "pack://application:,,,/Icons/Spiel/Bob/Bob_128px_grey.png")); } }
        public BitmapImage EveIcon { get { return new BitmapImage(new Uri(Variante == VarianteNormalerAblauf.VariantenName ? "pack://application:,,,/Icons/Spiel/Eve/Eve_128px_grey.png"/* Durchgestrichene Eve fehlt*/ : EveState ? "pack://application:,,,/Icons/Spiel/Eve/Eve_128px.png" : "pack://application:,,,/Icons/Spiel/Eve/Eve_128px_grey.png")); } }
        public bool AliceState { get; set; }
        public bool BobState { get; set; }
        public bool EveState { get; set; }
    }
}

﻿using quaKrypto.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class UebungsszenarioNetzwerkBeitrittInfo : INotifyPropertyChanged
    {
        //Information die durch den Host gesendet wird, damit ein Client der Lobby später beitreten kann
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

        //IP des Hosts
        public IPAddress IPAddress { get; set; }
        public string Lobbyname { get; set; }
        public string Protokoll { get; set; }
        public string Variante { get; set; }

        public uint StartPhase { get; set; }
        public uint EndPhase { get; set; }
        public SchwierigkeitsgradEnum Schwierigkeitsgrad { get; set; }

        //Enthält die Icons ob eine Rolle verfügbar, besetzt oder nicht exisitiert (Variante NormalerAblauf)
        public BitmapImage AliceIcon { get { return new BitmapImage(new Uri(AliceState ? "pack://application:,,,/Icons/Spiel/Alice/Alice_128px.png" : "pack://application:,,,/Icons/Spiel/Alice/Alice_128px_grey.png")); } }
        public BitmapImage BobIcon { get { return new BitmapImage(new Uri(BobState ? "pack://application:,,,/Icons/Spiel/Bob/Bob_128px.png" : "pack://application:,,,/Icons/Spiel/Bob/Bob_128px_grey.png")); } }
        public BitmapImage EveIcon { get { return new BitmapImage(new Uri(Variante == VarianteNormalerAblauf.VariantenName ? "pack://application:,,,/Icons/Spiel/Eve/Eve_dg_128px.png" : EveState ? "pack://application:,,,/Icons/Spiel/Eve/Eve_128px.png" : "pack://application:,,,/Icons/Spiel/Eve/Eve_128px_grey.png")); } }

        //Speichert ob die Rolle besetzt ist oder nicht
        private bool aliceState, bobState, eveState;

        //Properties die anzeigen ob die Rolle besetzt ist oder nicht
        public bool AliceState { get => aliceState; set { aliceState = value; Changed(nameof(AliceIcon)); } }
        public bool BobState { get => bobState; set { bobState = value; Changed(nameof(BobIcon)); } }
        public bool EveState { get => eveState; set { eveState = value; Changed(nameof(EveIcon)); } }

        //Gibt für den Client den Port an auf welchen er sich verbinden muss
        public int HostPort { get; set; }

        //Eventhandler falls eine Rolle gewählt oder freigegeben wurde
        public event PropertyChangedEventHandler? PropertyChanged;

        //Löst das PropertyChanged Event aus
        public void Changed(string a) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(a));
    }
}

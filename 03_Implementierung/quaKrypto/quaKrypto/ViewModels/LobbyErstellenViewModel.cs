using quaKrypto.Commands;
using quaKrypto.Models.Classes;
using quaKrypto.Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using quaKrypto.Models.Interfaces;

namespace quaKrypto.ViewModels
{
    public class LobbyErstellenViewModel : BaseViewModel
    {
        public DelegateCommand HauptMenu { get; set; }
        public DelegateCommand LobbyErstellen { get; set; }
        
        public LobbyErstellenViewModel(Navigator navigator)
        {
            
            HauptMenu = new((o) =>
            {
                NetzwerkHost.BeendeZyklischesSendenVonLobbyinformation();
                navigator.aktuellesViewModel = new HauptMenuViewModel(navigator);
            }, null);
            LobbyErstellen = new((o) =>
            {
                IVariante ausgewaehlteVariante;
                if (AusgVariante == 0)
                {
                    ausgewaehlteVariante = new VarianteNormalerAblauf((uint)AusgPhaseStart);
                }
                else if (AusgVariante == 1)
                {
                    ausgewaehlteVariante = new VarianteAbhoeren((uint)AusgPhaseStart);
                }
                else
                {
                    ausgewaehlteVariante = new VarianteManInTheMiddle((uint)AusgPhaseStart);
                }

                if (NetzwerkBasiert)
                {
                    UebungsszenarioNetzwerkBeitrittInfo ErstelltesSzenarioInfo = new UebungsszenarioNetzwerkBeitrittInfo(IPAddress.Any, LobbyName, Protokoll[AusgProtokoll], VarianteAuswahl[AusgVariante], AusgSchwierigkeit == 0 ? SchwierigkeitsgradEnum.Leicht : AusgSchwierigkeit == 1 ? SchwierigkeitsgradEnum.Mittel : SchwierigkeitsgradEnum.Schwer, false, false, false);
                    ErstelltesSzenarioInfo.StartPhase = (uint)AusgPhaseStart;
                    ErstelltesSzenarioInfo.EndPhase = (uint)AusgPhaseEnd;
                    NetzwerkHost.BeginneZyklischesSendenVonLobbyinformation(ErstelltesSzenarioInfo);
                    UebungsszenarioNetzwerk uebungsszenarioNetzwerk = new UebungsszenarioNetzwerk(AusgSchwierigkeit == 0 ? SchwierigkeitsgradEnum.Leicht : AusgSchwierigkeit == 1 ? SchwierigkeitsgradEnum.Mittel : SchwierigkeitsgradEnum.Schwer, ausgewaehlteVariante, (uint)AusgPhaseStart, (uint)AusgPhaseEnd, LobbyName, true);
                    navigator.aktuellesViewModel = new LobbyScreenViewModel(navigator, uebungsszenarioNetzwerk, true);
                }
                else
                {                 
                    UebungsszenarioLokal uebungsszenarioLokal = new UebungsszenarioLokal(AusgSchwierigkeit == 0 ? SchwierigkeitsgradEnum.Leicht : AusgSchwierigkeit == 1 ? SchwierigkeitsgradEnum.Mittel : SchwierigkeitsgradEnum.Schwer, ausgewaehlteVariante, (uint)AusgPhaseStart, (uint)AusgPhaseEnd, LobbyName);
                    navigator.aktuellesViewModel = new LobbyScreenViewModel(navigator, uebungsszenarioLokal, true);
                }


            }, (o) => LobbyName != "" && AusgProtokoll != -1 && AusgSchwierigkeit != -1 && AusgVariante != -1) ;
            AusgPhaseStart = 0;
            AusgPhaseEnd = 5;
            SchwierigkeitsgradAuswahl = new ObservableCollection<string>();
            SchwierigkeitsgradAuswahl.Add(SchwierigkeitsgradEnum.Leicht.ToString());
            SchwierigkeitsgradAuswahl.Add(SchwierigkeitsgradEnum.Mittel.ToString());
            SchwierigkeitsgradAuswahl.Add(SchwierigkeitsgradEnum.Schwer.ToString());
            VarianteAuswahl = new ObservableCollection<string>();
            VarianteAuswahl.Add(VarianteNormalerAblauf.VariantenName);
            VarianteAuswahl.Add(VarianteAbhoeren.VariantenName);
            VarianteAuswahl.Add(VarianteManInTheMiddle.VariantenName);
            Protokoll = new ObservableCollection<string>();
            Protokoll.Add("BB84");
            Verbindungstyp = new ObservableCollection<string>();
            Verbindungstyp.Add("Lokal");
            Verbindungstyp.Add("Netzwerkbasiert");
        }
        private string _lobbyName = string.Empty;
        private int _ausgProtokoll = -1;
        private int _ausgSchwierigkeit = -1;
        private int _ausgVariante = -1;
        private int _ausgPhaseStart;
        private int _ausgPhaseEnde;
        private bool _netzwerkbasiert = false;
        
        public string LobbyName { get { return _lobbyName; } set { _lobbyName = value; this.EigenschaftWurdeGeändert(); this.LobbyErstellen.RaiseCanExecuteChanged(); } }
        public int AusgProtokoll { get {  return _ausgProtokoll; } set { _ausgProtokoll = value; this.EigenschaftWurdeGeändert(); this.LobbyErstellen.RaiseCanExecuteChanged(); } }
        public int AusgSchwierigkeit { get { return _ausgSchwierigkeit; } set { _ausgSchwierigkeit = value; this.EigenschaftWurdeGeändert(); this.LobbyErstellen.RaiseCanExecuteChanged(); } }
        public int AusgVariante { get { return _ausgVariante; } set { _ausgVariante = value; this.EigenschaftWurdeGeändert(); this.LobbyErstellen.RaiseCanExecuteChanged(); } }
        public int AusgPhaseStart 
        { 
            get { return _ausgPhaseStart; } 
            set {
                if (AusgPhaseEnd == value) AusgPhaseStart = value - 1;
                else _ausgPhaseStart = value; 
                EigenschaftWurdeGeändert(nameof(AusgPhaseStart));
            } 
        }
        public int AusgPhaseEnd { 
            get { return _ausgPhaseEnde; }
            set 
            {
                if (AusgPhaseStart == value) AusgPhaseEnd = value + 1;
                else _ausgPhaseEnde = value;
                EigenschaftWurdeGeändert(nameof(AusgPhaseEnd));
            } 
        }
        public ObservableCollection<string> Protokoll { get; set; }
        public ObservableCollection<string> SchwierigkeitsgradAuswahl { get; set; }
        public ObservableCollection<string> VarianteAuswahl { get; set; }
        public ObservableCollection<string> Verbindungstyp { get; set; }
        public bool NetzwerkBasiert { get { return _netzwerkbasiert; } 
            set { _netzwerkbasiert = value; this.EigenschaftWurdeGeändert();} }
    }
}

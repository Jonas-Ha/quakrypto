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
                    UebungsszenarioNetzwerkBeitrittInfo ErstelltesSzenarioInfo = new UebungsszenarioNetzwerkBeitrittInfo(IPAddress.Any, LobbyName, Protokoll[AusgProtokoll], VarianteAuswahl[AusgVariante], AusgSchwierigkeit == 0 ? SchwierigkeitsgradEnum.leicht : AusgSchwierigkeit == 1 ? SchwierigkeitsgradEnum.mittel : SchwierigkeitsgradEnum.schwer, false, false, false);
                    NetzwerkHost.BeginneZyklischesSendenVonLobbyinformation(ErstelltesSzenarioInfo);
                    UebungsszenarioNetzwerk uebungsszenarioNetzwerk = new UebungsszenarioNetzwerk(AusgSchwierigkeit == 0 ? SchwierigkeitsgradEnum.leicht : AusgSchwierigkeit == 1 ? SchwierigkeitsgradEnum.mittel : SchwierigkeitsgradEnum.schwer, ausgewaehlteVariante, (uint)AusgPhaseStart, (uint)AusgPhaseEnd, LobbyName);
                    navigator.aktuellesViewModel = new LobbyScreenViewModel(navigator, uebungsszenarioNetzwerk);
                }
                else
                {                 
                    UebungsszenarioLokal uebungsszenarioLokal = new UebungsszenarioLokal(AusgSchwierigkeit == 0 ? SchwierigkeitsgradEnum.leicht : AusgSchwierigkeit == 1 ? SchwierigkeitsgradEnum.mittel : SchwierigkeitsgradEnum.schwer, ausgewaehlteVariante, (uint)AusgPhaseStart, (uint)AusgPhaseEnd, LobbyName);
                    navigator.aktuellesViewModel = new LobbyScreenViewModel(navigator, uebungsszenarioLokal);
                }


            }, (o) => LobbyName != "" && AusgProtokoll != -1 && AusgSchwierigkeit != -1 && AusgVariante != -1) ;
            AusgPhaseStart = 0;
            AusgPhaseEnd = 5;
            SchwierigkeitsgradAuswahl = new ObservableCollection<string>();
            SchwierigkeitsgradAuswahl.Add(SchwierigkeitsgradEnum.leicht.ToString());
            SchwierigkeitsgradAuswahl.Add(SchwierigkeitsgradEnum.mittel.ToString());
            SchwierigkeitsgradAuswahl.Add(SchwierigkeitsgradEnum.schwer.ToString());
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
        public int AusgPhaseStart { get { return _ausgPhaseStart; } set { _ausgPhaseStart = value; this.EigenschaftWurdeGeändert(); } }
        public int AusgPhaseEnd { 
            get { return _ausgPhaseEnde; }
            set 
            { 
                _ausgPhaseEnde = value;
                this.EigenschaftWurdeGeändert(); 
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

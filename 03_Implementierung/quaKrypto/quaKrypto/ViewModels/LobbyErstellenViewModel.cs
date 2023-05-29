using quaKrypto.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                //navigator.aktuellesViewModel = new LobbyScreenViewModel(navigator);

            }, null);
        }
        private string _lobbyName = string.Empty;
        private int _ausgProtokoll = -1;
        private int _ausgSchwierigkeit = -1;
        private int _ausgVariante = -1;
        private int _ausgPhaseStart;
        private int _ausgPhaseEnde;
        private bool _netzwerkbasiert;
        public string LobbyName { get { return _lobbyName; } set { _lobbyName = value; this.EigenschaftWurdeGeändert(); } }
        public int AusgProtokoll { get {  return _ausgProtokoll; } set { _ausgProtokoll = value; this.EigenschaftWurdeGeändert(); } }
        public int AusgSchwierigkeit { get { return _ausgSchwierigkeit; } set { _ausgSchwierigkeit = value; this.EigenschaftWurdeGeändert(); } }
        public int AusgVariante { get { return _ausgVariante; } set { _ausgVariante = value; this.EigenschaftWurdeGeändert(); } }
        public int AusgPhaseStart { get { return _ausgPhaseStart; } set { _ausgPhaseStart = value; this.EigenschaftWurdeGeändert(); } }
        public int AusgPhaseEnd { get { return _ausgPhaseEnde; } set { _ausgPhaseEnde = value; this.EigenschaftWurdeGeändert(); } }
        public bool NetzwerkBasiert { get { return _netzwerkbasiert; } set { _netzwerkbasiert = value; this.EigenschaftWurdeGeändert();} }
    }
}

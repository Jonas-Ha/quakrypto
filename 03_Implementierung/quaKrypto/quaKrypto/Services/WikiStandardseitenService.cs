﻿using quaKrypto.Models.Enums;
using System;
using System.IO;
using quaKrypto.Models.Classes;

namespace quaKrypto.Services
{
    public static class WikiStandardseitenService
    {
        private static readonly string WIKI_STANDARDSEITEN_ORDNERNAME = Path.Combine(Environment.CurrentDirectory, "Wiki Standardseiten");

        private static readonly string wikiSeiteOperationenName = Path.Combine(WIKI_STANDARDSEITEN_ORDNERNAME, "(1) Operationen");
        private static readonly string wikiSeiteAblaufMittelName = Path.Combine(WIKI_STANDARDSEITEN_ORDNERNAME, "(2) Ablauf [Mittel]");
        private static readonly string wikiSeiteAblaufNormalEinfachName = Path.Combine(WIKI_STANDARDSEITEN_ORDNERNAME, "(3) Normaler Ablauf [Einfach]");
        private static readonly string wikiSeiteAblaufLauschenEinfachName = Path.Combine(WIKI_STANDARDSEITEN_ORDNERNAME, "(4) Ablauf Lauschangriff [Einfach]");
        private static readonly string wikiSeiteAblaufMITMEinfachName = Path.Combine(WIKI_STANDARDSEITEN_ORDNERNAME, "(5) Ablauf Man-in-the-Middle [Einfach]");

        private const string wikiSeiteOperationenInhalt = "Diese Seite bietet eine detaillierte Anleitung zu den in Ihrem Übungszenario verfügbaren Operationen, mit denen Sie Informationen erzeugen oder ändern können. Jede Operation benötigt einen eindeutigen Namen für die zu generierende Information.\r\n\r\n\r\n\r\nBitfolge erzeugen: Geben Sie eine Zahl ein, um eine zufällige Bitfolge entsprechender Länge zu generieren. Wählen Sie die Option \"Manuell eingeben\", um die Bitfolge selbst einzugeben. Die Länge kann in diesem Fall manuell angepasst werden.\r\n\r\nEnt/Verschlüsseln: Geben Sie einen Text und eine Bitfolge ein. Der Text wird mit der Bitfolge per XOR-Operation kombiniert und entsprechend ver- oder entschlüsselt.\r\n\r\nPhotonen erzeugen: Hierfür benötigen Sie ein Polarisationsschema und eine Bitfolge. Die Eingaben erzeugen entsprechende Photonen. Zum Beispiel bedeutet \"✕\"+\"1\" => \"/\", \"✕\"+\"0\" => \"\", \"✛\"+\"1\" => \"-\" , \"✛\"+\"0\" => \"|\".\r\n\r\nPolarisationsschemata erzeugen: Geben Sie eine Zahl ein, um eine entsprechend lange, zufällige Folge von Polarisationsschemata zu erzeugen. Bei der Option \"Manuell eingeben\" können Sie eine Bitfolge selbst eingeben: \"0\" entspricht \"✕\" und \"1\" entspricht \"✛\".\r\n\r\nStreichen (Bits): Hierfür sind zwei Eingaben erforderlich: \"Bitfolge zu streichende Bits\" und \"Bitfolge Muster zu streichen\". Beide Bitfolgen müssen gleich lang sein. An den Positionen, an denen das Muster eine 1 aufweist, werden die Bits in der ersten Bitfolge entfernt.\r\n\r\nVergleichen: Sie können entweder zwei Bitfolgen oder zwei Polarisationsschemata vergleichen. Gleichheit resultiert in einer 0, Ungleichheit in einer 1.\r\n\r\nZahl erzeugen: Geben Sie eine Zahl ein, die als Information generiert wird.\r\n\r\nBitmaske generieren: Geben Sie zwei Zahlen ein. Die erste definiert die Länge der zu erzeugenden Bitmaske, die zweite die Anzahl der Einsen in der Bitmaske.\r\n\r\nBitfolge negieren: Negiert die eingegebene Bitfolge.\r\n\r\nPhotonen zu Bitfolge: Geben Sie empfangene Photonen und ein Polarisationsschema ein, um eine entsprechende Bitfolge zu generieren.\r\n\r\nText generieren: Geben Sie einen Text ein, der in eine entsprechende Information umgewandelt wird.\r\n\r\nTextlänge bestimmen: Bestimmt die Anzahl an Zeichen in einer Information.\r\n\r\nBits frei bearbeiten: Geben Sie eine Bitfolge ein, die frei bearbeitet werden kann. Dies erzeugt eine neue Information.\r\n\r\nInformation umbenennen: Sie können jeder Information einen neuen Namen geben.";
        private const string wikiSeiteAblaufMittelInhalt = "Der BB44-Protokoll\r\n\r\nHinweis: Der folgende Ablauf entspricht nicht zu 100% dem Originalen Protokoll von Charles Bennett und Gilles Brassard.\r\nZur besseren Nutzung des Programms wurden einige Schritte hinzugefügt oder leicht verändert.\r\nEve ist, falls sie in der Ablaufvariante vorkommt immer nach Alice und Bob dran.\r\nDie Abfolge ist also immer Alice, Eve, Bob, Eve, Alice, Eve, ....\r\nEve kann die Nachrichten die Alice und Bob austauschen abhören und sich versuchen daraus den Schlüssel herzuleiten.\r\nSie kann aber auch versuchen sich Alice gegenüber als Bob auszugeben und sich Bob gegebüber als Alice ausgeben und so einen Man In The Middle Angirff starten.\r\n\r\nPhase 0: Schlüssellänge bestimmen\r\n\r\nAlice erzeugt eine Nachricht und misst ihre Länge.\r\nAlice wählt eine Schlüssellänge, die etwas größer ist als die Länge der Nachricht und sendet diese an Bob.\r\nBob speichert die empfangene Schlüssellänge.\r\n\r\nPhase 1: Übertragung der mit Photonen codierten Schlüssel\r\n\r\nAlice erstellt eine zufällige Bitfolge und eine zufällige Folge von Polarisationsschemata gleicher Länge.\r\nAlice erzeugt Photonen, die die Bitfolge und das Polarisationsschema codieren, und sendet sie an Bob.\r\nBob erstellt eine zufällige Folge von Polarisationsschemata gleicher Länge und liest die empfangenen Photonen, um eine Bitfolge zu erzeugen.\r\n\r\nPhase 2: Vergleich der Polarisationsschemata und Erzeugung des Schlüssels\r\n\r\nAlice sendet ihr Polarisationschema an Bob.\r\nBob vergleicht sein Polarisationsschema mit dem von Alice, um eine Bitmaske zu erstellen, und streicht die nicht übereinstimmenden Bits aus seiner Schlüsselbitfolge.\r\nBob sendet seine Bitmaske an Alice, die die nicht übereinstimmenden Bits aus ihrer Bitfolge streicht.\r\n\r\nPhase 3: Überprüfung der Schlüssel\r\n\r\nAlice erstellt eine Bitmaske zur Auswahl der Prüfbits und streicht diese Bits aus dem Schlüssel.\r\nAlice sendet die Prüfbitmaske an Bob, der die Prüfbits aus dem Schlüssel extrahiert und streicht.\r\nBob sendet die Prüfbits an Alice, die ihre Prüfbits mit den empfangenen Prüfbits vergleicht.\r\n\r\nPhase 4: Nachrichtenübertragung\r\n\r\nAlice verschlüsselt ihre Nachricht mit dem endgültigen Schlüssel und sendet sie an Bob.\r\nBob entschlüsselt die Nachricht mit seinem Schlüssel.\r\n\r\nDie Phasen 0 bis 4 repräsentieren den Ablauf des BB84-Protokolls in Ihrem Übungsszenario. Bei erfolgreicher Durchführung haben Alice und Bob einen gemeinsamen geheimen Schlüssel ausgetauscht, mit dem sie ihre Nachrichten verschlüsseln und entschlüsseln können.";
        private const string wikiSeiteAblaufNormalEinfachInhalt = "Der BB44-Protokoll Ablauf zwischen Alice und Bob.\r\n\r\nHinweis: Der folgende Ablauf entspricht nicht zu 100% dem Originalen Protokoll von Charles Bennett und Gilles Brassard.\r\nZur besseren Nutzung des Programms wurden einige Schritte hinzugefügt oder leicht verändert.\r\n\r\nPhase 0:\r\nAls erstes erzeugt Alice eine Nachricht, welche sie später verschlüsselt an Bob senden möchte.\r\nDanach misst Alice die Länge ihrer Nachricht.\r\nAbgeleitet von dieser Länge erzeugt Alice eine Schlüssellänge.\r\nDiese sollte etwas größer sein, als die Länge der Nachricht, da im Verlauf des Protokolls noch einige Bits getsrichen werden müssen.\r\nNachdem sich Alice eine angemessene Schlüssellänge gewählt hat, sendet sie diese auf einem Bitübertragungskanal an Bob.\r\nDamit ist für Alice dieser Zug beendet.\r\n\r\nBob empfängt, die von Alice generierte, Schlüssellänge und speichert sich diese ab.\r\nDamit ist auch Bobs Zug beendet und die Phase ist vorbei.\r\n\r\nPhase 1:\r\nIn dieser Phase geht es nun darum den mit Photonen codierten Schlüssel zu übertragen.\r\nDazu beginnt Alice, indem sie eine zufällige Bitfolge erstellt. Diese Bitfolge sollte die selbe Größe haben, wie die in Phase 0 ausgemachte Schlüssellänge.\r\nDanach erzeugt Alice zusätzlich noch eine zufällige Folge an Polarisationschemata, welche dieselbe Länge haben muss wie die, eben generierte, Bitfolge.\r\nMit diesen zwei Komponenten kann Alice nun Photonen erzeugen, die die Bitfolge mit dem dazugehörigen Polarisationschema codiert.\r\nDie daraus entstandenen Photonen sendet Alice über einen Photonenkanal an Bob.\r\nDanach kann Alice ihren Zug beenden.\r\n\r\nAls nächster ist Bob dran.\r\nEr beginnt in dem er eine zufällige Folge an Polarisationschemata erstellt, welche dieselbe Länge hat wie die ausgemachte Schlüssellänge.\r\nDiese Folge an Polarisationschemata nutzt Bob nun um die erhaltenen unscharfen Photonen zu lesen und in eine Bitfolge zu konvertieren.\r\nDiese Bitfolge wird später als Schlüssel genutzt.\r\nDanach beendet Bob seinen zu und die Phase endet.\r\n\r\nPhase 2:\r\nAlice beginnt diese Phase, indem sie ihr erzeugtes Polarisationschemata über den Bitübertragungskanal an Bob sendet.\r\nDamit ist Alice für diesen Zug erstmal fertig.\r\n\r\nBob nutzt nun das Empfangene Polarisationschemata und vergleicht es mit seinem eigenem.\r\nDabei entsteht eine Bitmaske, die anzeigt an welchen Stellen die Polarisationschemata übereinstimmen und an welchen Stellen sie dies nicht tun.\r\nDabei steht ein '0' in der Bitmaske für eine Übereinstimmung und eine '1' für keine Übereinstimmung!\r\nBob nutzt nun diese Bitmaske und streicht jene Bits aus seiner Schlüsselbitfolge, welche nicht übereinstimmen.\r\nZu guter letzt schickt er noch seine erzeugte Bitmaske über den Bitübertragungskanal an Alice und beendet danach seinen Zug.\r\n\r\nAlice nutzt die von Bob empfangene Bitmaske und streicht auch bei sich die Bits aus ihrer erzeugten Bitfolge, in der die Polarisationschemata nicht übereinstimmen.\r\nDamit beendet Alice auch die aktuelle Phase.\r\n\r\nPhase 3:\r\nAlice, die noch immer am Zug ist, überlegt sich nun eine Anzahl an Bits aus ihrem Schlüssel welche sie mit Bob vergleichen möchte.\r\nDazu erstellt sie eine Bitmaske mit der aktuellen Länge des Schlüssels und einer Anzahl an Bits.\r\nMit dieser Bitmaske kann sie nun diese bestimmten Bits aus dem Schlüssel extrahieren.\r\nDen endgültigen Schlüssel erhält sie, indem sie die Prüfbits aus dem Schlüssel streicht.\r\nDie Prüfbitmaske sendet Alice nun über den Bitübertragungskanal an Bob und sie beendet ihren Zug.\r\n\r\nBob empfängt die Prüfbitmaske und extrahiert die Prüfbits aus dem Schlüssel.\r\nEr streicht außerdem die Prüfbits aus seinen Schlüsselbits und erhält den endgültigen Schlüssel.\r\nDanach sender Bob die Prüfbits über den Bitübertragungskanal an Alice und er beendet seinen Zug.\r\n\r\nJetzt kann Alice ihre Prüfbits mit denen von Bob empfangenen Prüfbits vergleichen.\r\nSind die Bits gleich, so hat ihnen bei der Übertragung keiner zugehört.\r\nGibt es aber Unterschiede, so wurden sie wahrscheinlich Opfer eines Lauschangriffs.\r\nNachdem Alice die Prüfbits verglichen hat, ist diese Phase beendet.\r\n\r\nPhase 4:\r\nJetzt kann Alice ihren endgültigen Schlüssel nutzen und ihre Nachricht vom Anfang verschlüsseln.\r\nDiese verschlüsselte Nachricht sendet sie dann über den Bitübertragungskanal an Bob.\r\nDamit beendet Alice auch ihren Zug.\r\n\r\nBob empfängt nun die verschlüsselte Nachricht von Alice und kann diese mit seinem Schlüssel entschlüsseln.\r\nDie Datenübertragung hat erfolgreich stattgefunden und das Protokoll wurde durchgeführt.\r\nDamit ist auch diese Phase zu ende und das Spiel endet.\r\n";
        private const string wikiSeiteAblaufLauschenEinfachInhalt = "Der BB44-Protokoll Ablauf zwischen Alice, Bob und Eve bei einem Lauschangriff.\r\n\r\nHinweis: Der folgende Ablauf entspricht nicht zu 100% dem Originalen Protokoll von Charles Bennett und Gilles Brassard.\r\nZur besseren Nutzung des Programms wurden einige Schritte hinzugefügt oder leicht verändert.\r\nDieser ABlauf bezieht sich auf einen Lauschangriff und zielt darauf ab auch die Schritte zu erläutern die Eve einleiten kann um eine gestörte Übertragung zu erzeugen.\r\n\r\nPhase 0:\r\nAls erstes erzeugt Alice eine Nachricht, welche sie später verschlüsselt an Bob senden möchte.\r\nDanach misst Alice die Länge ihrer Nachricht.\r\nAbgeleitet von dieser Länge erzeugt Alice eine Schlüssellänge.\r\nDiese sollte etwas größer sein, als die Länge der Nachricht, da im Verlauf des Protokolls noch einige Bits getsrichen werden müssen.\r\nNachdem sich Alice eine angemessene Schlüssellänge gewählt hat, sendet sie diese auf einem Bitübertragungskanal an Bob.\r\nDamit ist für Alice dieser Zug beendet.\r\n\r\nEve kann sich die Schlüssellänge speichern und diese an Bob weiterleiten.\r\n\r\nBob empfängt, die von Alice generierte, Schlüssellänge und speichert sich diese ab.\r\nDamit ist auch Bobs Zug beendet.\r\n\r\nEve kann eigentlich nichz wirklich was tun, weshalb sie ihren Zug einfach beendet.\r\nDamit endet diese Phase.\r\n\r\nPhase 1:\r\nIn dieser Phase geht es nun darum den mit Photonen codierten Schlüssel zu übertragen.\r\nDazu beginnt Alice, indem sie eine zufällige Bitfolge erstellt. Diese Bitfolge sollte die selbe Größe haben, wie die in Phase 0 ausgemachte Schlüssellänge.\r\nDanach erzeugt Alice zusätzlich noch eine zufällige Folge an Polarisationschemata, welche dieselbe Länge haben muss wie die, eben generierte, Bitfolge.\r\nMit diesen zwei Komponenten kann Alice nun Photonen erzeugen, die die Bitfolge mit dem dazugehörigen Polarisationschema codiert.\r\nDie daraus entstandenen Photonen sendet Alice über einen Photonenkanal an Bob.\r\nDanach kann Alice ihren Zug beenden.\r\n\r\nEve kann nun auch einen Polarisationschemata in Länge des Schlüssels erstellen.\r\nDie auf dem Photonenkanal liegenden unscharfen Photonen kann Eve nun mit ihren erzeugten Polarisationschemata in eine Bitfolge konvertieren.\r\n(Werden Photonen geänder oder müssen neue erzeugt werden??)\r\nEve schickt die Photonen auf dem Photonenübertragunskanal an Bob weiter.\r\n\r\nAls nächster ist Bob dran.\r\nEr beginnt in dem er eine zufällige Folge an Polarisationschemata erstellt, welche dieselbe Länge hat wie die ausgemachte Schlüssellänge.\r\nDiese Folge an Polarisationschemata nutzt Bob nun um die erhaltenen unscharfen Photonen zu lesen und in eine Bitfolge zu konvertieren.\r\nDiese Bitfolge wird später als Schlüssel genutzt.\r\nDanach beendet Bob seinen Zug.\r\nJetzt ist Eve dran, welche wieder nichts machen kann. Deshabl beendet sie ihren Zug und die Phase endet.\r\n\r\nPhase 2:\r\nAlice beginnt diese Phase, indem sie ihr erzeugtes Polarisationschemata über den Bitübertragungskanal an Bob sendet.\r\nDamit ist Alice für diesen Zug erstmal fertig.\r\n\r\nEve empfängt Alices Polarisationschema.\r\nEve kann das Polarisationschema mit ihrem vergleichen und erhält eine Bitmaske die ihr angibt an welchen Stellen sie das selbe Schema wie Alice genutzt hat.\r\nDanach leitet sie das Polarisationschema von Alice an Bob weiter und beendet ihren Zug.\r\n\r\nBob nutzt nun das Empfangene Polarisationschemata und vergleicht es mit seinem eigenem.\r\nDabei entsteht eine Bitmaske, die anzeigt an welchen Stellen die Polarisationschemata übereinstimmen und an welchen Stellen sie dies nicht tun.\r\nDabei steht ein '0' in der Bitmaske für eine Übereinstimmung und eine '1' für keine Übereinstimmung!\r\nBob nutzt nun diese Bitmaske und streicht jene Bits aus seiner Schlüsselbitfolge, welche nicht übereinstimmen.\r\nZu guter letzt schickt er noch seine erzeugte Bitmaske über den Bitübertragungskanal an Alice und beendet danach seinen Zug.\r\n\r\nEve nutzt die Bitmaske die sie mithilfe von Alice Polaristaionschema erstellt hat und löscht die faslchen Bitstellen aus ihrem Schlüssel.\r\nDanach nutzt EVe die Bitmaske von Bob und löscht die Bits aus ihren Schlüssel, welche auch Bob gestrichen hat.\r\nDann hat sie nur noch Bits in ihrem Schlüssel, welche sowohl von ihr als auch von Bob korrekt decodiert worden.\r\nDadurch ist ihr Schlüssel eventuell kleiner als der von Alice und Bob (??)\r\nSie beendet ihren Zug\r\n\r\nAlice nutzt die von Bob empfangene Bitmaske und streicht auch bei sich die Bits aus ihrer erzeugten Bitfolge, in der die Polarisationschemata nicht übereinstimmen.\r\nDamit beendet Alice auch die aktuelle Phase.\r\n\r\nPhase 3:\r\nAlice, die noch immer am Zug ist, überlegt sich nun eine Anzahl an Bits aus ihrem Schlüssel welche sie mit Bob vergleichen möchte.\r\nDazu erstellt sie eine Bitmaske mit der aktuellen Länge des Schlüssels und einer Anzahl an Bits.\r\nMit dieser Bitmaske kann sie nun diese bestimmten Bits aus dem Schlüssel extrahieren.\r\nDen endgültigen Schlüssel erhält sie, indem sie die Prüfbits aus dem Schlüssel streicht.\r\nDie Prüfbitmaske sendet Alice nun über den Bitübertragungskanal an Bob und sie beendet ihren Zug.\r\n\r\nEve empfängt die Prüfbitmaske und extrahiert die Prüfbits aus ihrem Schlüssel.\r\nDanach beendet sie ihren Zug.\r\n\r\nBob empfängt die Prüfbitmaske und extrahiert die Prüfbits aus dem Schlüssel.\r\nEr streicht außerdem die Prüfbits aus seinen Schlüsselbits und erhält den endgültigen Schlüssel.\r\nDanach sendet Bob die Prüfbits über den Bitübertragungskanal an Alice und er beendet seinen Zug.\r\n\r\nEve empfängt die Prüfbits von Bob und überprüft diese mit ihren extrahierten Prüfbits.\r\nSind diese unterschiedlich so weiß Eve, dass sie erwischt wurde.\r\nDanach beendet sie ihren Zug.\r\n\r\nJetzt kann Alice ihre Prüfbits mit denen von Bob empfangenen Prüfbits vergleichen.\r\nSind die Bits gleich, so hat ihnen bei der Übertragung keiner zugehört.\r\nGibt es aber Unterschiede, so wurden sie wahrscheinlich Opfer eines Lauschangriffs.\r\nNachdem Alice die Prüfbits verglichen hat, ist diese Phase beendet.\r\n\r\nPhase 4:\r\nNormalerweise würde Alice erkenne, das sie Opfer eines Lauschangriffes wurden.\r\nZu Lehrzwecken, kann man hier aber trotzdem noch eine verschlüsselte Naachricht versenden und sehen, was dies für Auswirkungen hat.\r\nJetzt kann Alice ihren endgültigen Schlüssel nutzen und ihre Nachricht vom Anfang verschlüsseln.\r\nDiese verschlüsselte Nachricht sendet sie dann über den Bitübertragungskanal an Bob.\r\nDamit beendet Alice auch ihren Zug.\r\n\r\nEve empfängt die verschlüsselte Nachricht und versucht sie mit ihrem Schlüssel zu entschlüsseln.\r\n\r\nBob empfängt nun die verschlüsselte Nachricht von Alice und kann diese mit seinem Schlüssel entschlüsseln.\r\nDie Datenübertragung hat erfolgreich stattgefunden und das Protokoll wurde durchgeführt.\r\nDamit ist auch diese Phase zu ende und das Spiel endet.\r\n";
        private const string wikiSeiteAblaufMITMEinfachInhalt = "Der BB44-Protokoll Ablauf zwischen Alice, Bob und Eve bei einem Man in The Middle Angriff.\r\n\r\nHinweis: Der folgende Ablauf entspricht nicht zu 100% dem Originalen Protokoll von Charles Bennett und Gilles Brassard.\r\nZur besseren Nutzung des Programms wurden einige Schritte hinzugefügt oder leicht verändert.\r\nDieser Ablauf bezieht sich auf einen Man in The Middle Angriff und zielt darauf ab auch die Schritte zu erläutern die Eve einleiten kann um die Nachricht zwischen Alice und Bob zu verändern.\r\nAchtung: Insbesonders für Eve ist es in diesem Modus sehr wichtig die Informationen aussagekräftig zu bennen um nicht durcheinander zu kommen! \r\n\r\nPhase 0:\r\nAls erstes erzeugt Alice eine Nachricht, welche sie später verschlüsselt an Bob senden möchte.\r\nDanach misst Alice die Länge ihrer Nachricht.\r\nAbgeleitet von dieser Länge erzeugt Alice eine Schlüssellänge.\r\nDiese sollte etwas größer sein, als die Länge der Nachricht, da im Verlauf des Protokolls noch einige Bits gestrichen werden müssen.\r\nNachdem sich Alice eine angemessene Schlüssellänge gewählt hat, sendet sie diese auf einem Bitübertragungskanal an Bob.\r\nDamit ist für Alice dieser Zug beendet.\r\n\r\nEve kann sich die Schlüssellänge speichern \r\nSie erzeugt für ihre Nachricht an Bob eine entsprechende Schlüssellänge\r\nDiese sollte etwas größer sein, als die Länge der Nachricht, da im Verlauf des Protokolls noch einige Bits gestrichen werden müssen.\r\nDiese neue Schlüssellänge schickt sie auf dem Bitübertragungskanal an Bob\r\n\r\nBob empfängt, die von „Alice“ generierte, Schlüssellänge und speichert sich diese ab.\r\nDamit ist auch Bobs Zug beendet.\r\n\r\nEve kann nicht wirklich etwas tun, weshalb sie ihren Zug einfach beendet.\r\nDamit endet diese Phase.\r\n\r\nPhase 1:\r\nIn dieser Phase geht es nun darum den mit Photonen codierten Schlüssel zu übertragen.\r\nDazu beginnt Alice, indem sie eine zufällige Bitfolge erstellt. Diese Bitfolge sollte die selbe Größe haben, wie die in Phase 0 ausgemachte Schlüssellänge.\r\nDanach erzeugt Alice zusätzlich noch eine zufällige Folge an Polarisationschemata, welche dieselbe Länge haben muss wie die, eben generierte, Bitfolge.\r\nMit diesen zwei Komponenten kann Alice nun Photonen erzeugen, die die Bitfolge mit dem dazugehörigen Polarisationschema codiert.\r\nDie daraus entstandenen Photonen sendet Alice über einen Photonenkanal an Bob.\r\nDanach kann Alice ihren Zug beenden.\r\n\r\nEve kann nun auch einen Polarisationschemata in Länge des Schlüssels erstellen.\r\nDie auf dem Photonenkanal liegenden unscharfen Photonen kann Eve nun mit ihren erzeugten Polarisationschemata in eine Bitfolge konvertieren.\r\nMit Bob muss Eve auch eine Bitfolge ausmachen, indem sie eine zufällige Bitfolge erstellt. Diese Bitfolge sollte die selbe Größe haben, wie die in Phase 0 ausgemachte Schlüssellänge.\r\nDanach erzeugt Eve zusätzlich noch eine zufällige Folge an Polarisationschemata, welche dieselbe Länge haben muss wie die, eben generierte, Bitfolge.\r\nMit diesen zwei Komponenten kann Eve nun Photonen erzeugen, die die Bitfolge mit dem dazugehörigen Polarisationschema codiert.\r\nDie daraus entstandenen Photonen sendet Eve  über einen Photonenkanal an Bob.\r\nDanach kann Eve ihren Zug beenden.\r\n\r\nAls nächster ist Bob dran.\r\nEr beginnt in dem er eine zufällige Folge an Polarisationschemata erstellt, welche dieselbe Länge hat wie die ausgemachte Schlüssellänge.\r\nDiese Folge an Polarisationschemata nutzt Bob nun um die erhaltenen unscharfen Photonen zu lesen und in eine Bitfolge zu konvertieren.\r\nDiese Bitfolge wird später als Schlüssel genutzt.\r\nDanach beendet Bob seinen Zug.\r\nJetzt ist Eve dran, welche wieder nichts machen kann. Deshalb beendet sie ihren Zug und die Phase endet.\r\n\r\nPhase 2:\r\nAlice beginnt diese Phase, indem sie ihr erzeugtes Polarisationschemata über den Bitübertragungskanal an Bob sendet.\r\nDamit ist Alice für diesen Zug erstmal fertig.\r\n\r\n\r\nEve empfängt Alices Polarisationschema.\r\nEve kann das Polarisationschema mit ihrem vergleichen und erhält eine Bitmaske die ihr angibt an welchen Stellen sie dasselbe Schema wie Alice genutzt hat.\r\nDiese Bitmaske Schickt sie an Alice über den Bitübertragungskanal.\r\nDanach leitet sie das Polarisationschema welches sie für Bob benutzt hat an ihn weiter und beendet ihren Zug.\r\n\r\nBob nutzt nun das Empfangene Polarisationschemata und vergleicht es mit seinem eigenem.\r\nDabei entsteht eine Bitmaske, die anzeigt an welchen Stellen die Polarisationschemata übereinstimmen und an welchen Stellen sie dies nicht tun.\r\nDabei steht ein '0' in der Bitmaske für eine Übereinstimmung und eine '1' für keine Übereinstimmung!\r\nBob nutzt nun diese Bitmaske und streicht jene Bits aus seiner Schlüsselbitfolge, welche nicht übereinstimmen.\r\nZu guter letzt schickt er noch seine erzeugte Bitmaske über den Bitübertragungskanal an Alice und beendet danach seinen Zug.\r\n\r\nEve nutzt die Bitmaske von Bob und löscht die Bits aus ihren Schlüssel, welche auch Bob gestrichen hat.\r\nDann hat sie nur noch Bits in ihrem Schlüssel, welche sowohl von ihr als auch von Bob korrekt decodiert worden sind.\r\nSie beendet ihren Zug\r\n\r\nAlice nutzt die von „Bob“ empfangene Bitmaske und streicht auch bei sich die Bits aus ihrer erzeugten Bitfolge, in der die Polarisationschemata nicht übereinstimmen.\r\nDamit beendet Alice auch die aktuelle Phase.\r\n\r\nPhase 3:\r\nAlice, die noch immer am Zug ist, überlegt sich nun eine Anzahl an Bits aus ihrem Schlüssel welche sie mit Bob vergleichen möchte.\r\nDazu erstellt sie eine Bitmaske mit der aktuellen Länge des Schlüssels und einer Anzahl an Bits.\r\nMit dieser Bitmaske kann sie nun diese bestimmten Bits aus dem Schlüssel extrahieren.\r\nDen endgültigen Schlüssel erhält sie, indem sie die Prüfbits aus dem Schlüssel streicht.\r\nDie Prüfbitmaske sendet Alice nun über den Bitübertragungskanal an Bob und sie beendet ihren Zug.\r\n\r\nEve empfängt die Prüfbitmaske und extrahiert die Prüfbits aus ihrem Schlüssel.\r\nSie überlegt sich nun eine Anzahl an Bits aus ihrem Schlüssel welche sie mit Bob vergleichen möchte.\r\nDazu erstellt sie eine Bitmaske mit der aktuellen Länge des Schlüssels und einer Anzahl an Bits.\r\nMit dieser Bitmaske kann sie nun diese bestimmten Bits aus dem Schlüssel extrahieren.\r\nDen endgültigen Schlüssel erhält sie, indem sie die Prüfbits aus dem Schlüssel streicht.\r\nDie Prüfbitmaske sendet Eve nun über den Bitübertragungskanal an Bob und sie beendet ihren Zug.\r\nDanach beendet sie ihren Zug.\r\n\r\nBob empfängt die Prüfbitmaske und extrahiert die Prüfbits aus dem Schlüssel.\r\nEr streicht außerdem die Prüfbits aus seinen Schlüsselbits und erhält den endgültigen Schlüssel.\r\nDanach sendet Bob die Prüfbits über den Bitübertragungskanal an Alice und er beendet seinen Zug.\r\n\r\nEve empfängt die Prüfbits von Bob und überprüft diese mit ihren extrahierten Prüfbits.\r\nSind diese unterschiedlich so weiß Eve, dass sie erwischt wurde.\r\nDanach beendet sie ihren Zug.\r\n\r\nJetzt kann Alice ihre Prüfbits mit denen von Bob empfangenen Prüfbits vergleichen.\r\nSind die Bits gleich, so hat ihnen bei der Übertragung keiner zugehört.\r\nSie weiß aber nicht das sie einen Schlüssel mit eve ausgemacht hat statt mit Bob\r\nNachdem Alice die Prüfbits verglichen hat, ist diese Phase beendet.\r\n\r\nPhase 4:\r\nJetzt kann Alice ihren endgültigen Schlüssel nutzen und ihre Nachricht vom Anfang verschlüsseln.\r\nDiese verschlüsselte Nachricht sendet sie dann über den Bitübertragungskanal an Bob.\r\nDamit beendet Alice auch ihren Zug.\r\n\r\nEve empfängt die verschlüsselte Nachricht und kann sie mit ihrem Schlüssel für Alice Entschlüsseln.\r\nSie kann nun ihre Nachricht an Bob mit dem entsprechenden Schlüssel verschlüsseln und losschicken.\r\n\r\nBob empfängt nun die verschlüsselte Nachricht von Alice und kann diese mit seinem Schlüssel entschlüsseln.\r\nDie Datenübertragung hat erfolgreich stattgefunden und das Protokoll wurde durchgeführt.\r\nDamit ist auch diese Phase zu ende und das Spiel endet.";


        public static void ErzeugeAlleStandardWikiseiten()
        {
            if (!Directory.Exists(WIKI_STANDARDSEITEN_ORDNERNAME)) _ = Directory.CreateDirectory(WIKI_STANDARDSEITEN_ORDNERNAME);
            File.WriteAllText(wikiSeiteOperationenName, wikiSeiteOperationenInhalt);
            File.WriteAllText(wikiSeiteAblaufMittelName, wikiSeiteAblaufMittelInhalt);
            File.WriteAllText(wikiSeiteAblaufNormalEinfachName, wikiSeiteAblaufNormalEinfachInhalt);
            File.WriteAllText(wikiSeiteAblaufLauschenEinfachName, wikiSeiteAblaufLauschenEinfachInhalt);
            File.WriteAllText(wikiSeiteAblaufMITMEinfachName, wikiSeiteAblaufMITMEinfachInhalt);
        }

        public static void LadeAlleWikiSeitenMitSchwierigkeit(SchwierigkeitsgradEnum schwierigkeitsgrad)
        {
            Wiki.WikiSeiten.Clear();
            Wiki.WikiSeiten.Add(new WikiSeite(Path.GetFileName(wikiSeiteOperationenName).Split(") ")[1], wikiSeiteOperationenInhalt, 1));
            if (schwierigkeitsgrad.Equals(SchwierigkeitsgradEnum.Mittel)) Wiki.WikiSeiten.Add(new WikiSeite(Path.GetFileName(wikiSeiteAblaufMittelName).Split(") ")[1], wikiSeiteAblaufMittelInhalt, 2));
            else if (schwierigkeitsgrad.Equals(SchwierigkeitsgradEnum.Leicht))
            {
                Wiki.WikiSeiten.Add(new WikiSeite(Path.GetFileName(wikiSeiteAblaufNormalEinfachName).Split(") ")[1], wikiSeiteAblaufNormalEinfachInhalt, 3));
                Wiki.WikiSeiten.Add(new WikiSeite(Path.GetFileName(wikiSeiteAblaufLauschenEinfachName).Split(") ")[1], wikiSeiteAblaufLauschenEinfachInhalt, 4));
                Wiki.WikiSeiten.Add(new WikiSeite(Path.GetFileName(wikiSeiteAblaufMITMEinfachName).Split(") ")[1], wikiSeiteAblaufMITMEinfachInhalt, 5));
            }
            WikiSeite.StandardSeitenGeladen();
        }
    }
}

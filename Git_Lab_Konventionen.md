# Git Konventionen

## Namenskonvention für Branches
Diese Konvention gilt für alle Branches und ist für die Übersichtlichkeit innerhalb des Projekts zwingend einzuhalten!

- `bezeichner` / `<issue_id>` _ `beschreibung`
<br>Beispiel: `feature` / `123` _ `created_class_user`</br>

<br>**Unterscheidung eines Branch-Bezeichners in ...**</br>
<br>- `feature` eine neue Funktionalität, Klasse, ... wurde implementiert oder geändert</br>
<br>- `test` ein Test wurde implementiert</br>
<br>- `doku` eine Dokumentation (Protokoll, Analyse-, Entwurfsdokument, ...) wurde erstellt oder geändert</br>
<br> - `bugfix` ein Bug wurde behoben </br>
<br> - `hotfix` </br>

## Kleine Git-Anleitung
- Clone des Repositories erzeugen
```
$ cd /path/to/project

$ git clone https://git.oth-aw.de/swp/sose2023/team_c/quakrypto.git
```
- Anzeigen aller Branches
```
$ git branch -a
```
erzeugt zum Beispiel die Ausgabe...
```
  development
* doku/39_codierrichtlinien_festlegen
  main
  remotes/origin/HEAD -> origin/main
  remotes/origin/development
  remotes/origin/doku/29_Systemarchitektur_erstellen
  remotes/origin/doku/31_Teststrategie
  remotes/origin/doku/32_UI_Mockup
  remotes/origin/doku/33_UI_Mockup_Überarbeiten
  remotes/origin/doku/39_codierrichtlinien_festlegen
  remotes/origin/main
```
Der aktuelle Branch, der momentan "ausgecheckt" ist, wird mit dem `*` gekenntzeichnet.

### Wie erzeuge ich meinen eigenen Branch zum Arbeiten?
- Auschecken des `development`-Branches
```
$ git checkout development
```
- Aktualisieren der lokalen Branch-Informationen
```
$ git fetch origin
```
`origin` ist nichts anderes als `https://git.oth-aw.de/swp/sose2023/team_c/quakrypto`!

- Eventuell müssen noch lokale Veränderungen commited und gepusht werden! Ziemlich sicher wird es so sein, dass man nun einen sogenannten `pull` durchführen muss:
```
$ git pull
```
- Der Branch ist nun auf dem aktuellsten Stand, der Remote verfügbar ist. Nun kann vom `development`-Branch ein Arbeitsbranch erzeugt werden: Hierzu sind die Namenskonventionen zu beachten und zwingend einzuhalten! Eine Suche bei vielen Arbeitsbranches wird so erleichtert und außerdem schaut das so sauberer aus.
```
$ git checkout -b feature/123_my_magic_feature
```
- Der Branch kann nun nicht gepusht werden, er ist ja nur lokal vorhanden. Hierzu entweder in die Konsole schauen oder folgenden Befehl eingeben:
```
$ git push --set-upstream origin feature/123_my_magic_feature
```
- Nun kann fleißig entwickelt werden! :smiley:
- Um Änderungen im Branch abzuspeichern müssen diese auch einem `commit` hinzugefügt werden. Dazu folgende Befehle:
```
$ git add <file1><file2>

$ git add . (für mehrere Files, übernimmt alle Files im aktuellen Verzeichnis)
```
- Danach eine aussagekräfige `commit`-Message hinzufügen:
```
$ git commit -m "ein tolles neues feature"

$ git push
```

### Wie mergen?
- mindestens ein Reviewer muss eingetragen werden: Vier-Augen-Prinzip
- nach Bestätigung des Reviewers kann der Merge erfolgen
- Regeln:
    - im aktuellen Sprint immer vom `development`-Branch abzweigen (`main` ist nur für das Ende der Iteration gedacht)
    - wenn Tasks voneinander abhängen, kann auch vom jeweiligen Branch abgezweigt werden - das sollte allerdings vermieden werden
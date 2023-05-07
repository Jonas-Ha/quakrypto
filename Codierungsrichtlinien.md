# Codierungsrichtlinien
für das Projekt ``quakrypto``.

## Generelles zu Programmierrichtlinien
Ein guter Programmierstil trägt wesentlich zur Lesbarkeit und Wartbarkeit von Programmen bei. Folgende Richtlinien müssen daher im Projekt ``quakrypto`` eingehalten werden: 

https://learn.microsoft.com/de-de/dotnet/csharp/fundamentals/coding-style/coding-conventions

## Namensgebung
Namen für | beginnen mit Wort... | beginnen mit Zeichen... | Beispiele 
--- | --- | --- | ---
Konstanten und Variablen | Substantiv oder Adjektiv | Kleinbuchstabe | version, nummer, temp
Typen (Klassen) | Substantiv | Großbuchstabe | Lobby, Benutzergruppe
Funktionen und Methoden | Verb | Kleinbuchstabe | summiereZahlen
Namespace | Substantiv | Kleinbuchstabe | quakrypto

## Namenslänge
- lokale, temporäre Variablen: eher kurz
- globale, persistente Variablen: aussagekräftig, so kurz wie möglich

## Sprache
- Englisch oder Deutsch??

## Kommentare
- kurze, aussagekräftige Kommentare
- kommentieren, was nicht im Programm steht
- englische oder deutsche Kommentare??
- "Kopfkommentar" eines Files
```
// **********************************************************
// File: <filename>.<type>
// Autor: Max Mustermann
// erstellt am: 01.01.2023
// Projekt: quakrypto
// ********************************************************** 
```

## Allgemeines
- Einrückungstiefe: Tabulator
- zusammengehörige Anweisungen in die gleiche Zeile, falls es die Lesbarkeit erlaubt
```
if ((divisor != 0) && (dividend / divisor > 0))
{
    Console.WriteLine("Quotient: {0}", dividend / divisor);
}
else
{
    Console.WriteLine("Attempted division by 0 ends up here");
}
```
- kurze, zusammengesetzte Anweisungen in eine Zeile:
```
if (condition) state1; else state2;
```

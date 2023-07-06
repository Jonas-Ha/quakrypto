// **********************************************************
// File: InformationsEnum.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;

// Enum für Datentypen der Spieloberfläche
namespace quaKrypto.Models.Enums
{
    [Serializable]
    public enum InformationsEnum
    {
		zahl,
		bitfolge,
		photonen,
		polarisationsschemata,
		unscharfePhotonen,
		asciiText,
		verschluesselterText,
		keinInhalt
	}
}

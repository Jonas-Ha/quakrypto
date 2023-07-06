// **********************************************************
// File: OperationsEnum.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;

// Enum aller durch die Rolle ausführbaren Operationen
namespace quaKrypto.Models.Enums
{
    [Serializable]
    public enum OperationsEnum
    {
		nachrichtSenden,
		nachrichtEmpfangen,
		nachrichtAbhoeren,
		bitfolgeGenerierenZahl,
		bitfolgeGenerierenAngabe,
		polarisationsschemataGenerierenZahl,
		polarisationsschemataGenerierenAngabe,
		photonenGenerieren,
		bitmaskeGenerieren,
		polschataVergleichen,
		bitfolgenVergleichen,
		bitfolgeNegieren,
		photonenZuBitfolge,
        textGenerieren,
		textLaengeBestimmen,
        textVerschluesseln,
		textEntschluesseln,
		bitsStreichen,
		bitsFreiBearbeiten,
		zahlGenerieren,
        informationUmbenennen,
        zugBeenden,
	}
}

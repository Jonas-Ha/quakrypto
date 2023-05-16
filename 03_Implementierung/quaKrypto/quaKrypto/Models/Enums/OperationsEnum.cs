// **********************************************************
// File: OperationsEnum.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.Models.Enums
{
    internal enum OperationsEnum
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
		photonenZuBitfolge,
		textVerschluesseln,
		textEntschluesseln,
		bitsStreichen,
		bitsFreiBearbeiten,
		zugBeenden
	}
}

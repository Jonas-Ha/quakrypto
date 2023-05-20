﻿// **********************************************************
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
		photonenZuBitfolge,
        textGenerieren,
        textVerschluesseln,
		textEntschluesseln,
		bitsStreichen,
		bitsFreiBearbeiten,
		zahlGenerieren,
		zugBeenden
	}
}

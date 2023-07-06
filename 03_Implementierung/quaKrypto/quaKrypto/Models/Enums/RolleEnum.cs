// **********************************************************
// File: RolleEnum.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System;

// Enum, das alle möglichen Rollen des BB84-Protokolls beinhaltet
namespace quaKrypto.Models.Enums
{
    [Serializable]
    public enum RolleEnum
    {
        Alice,
        Bob,
        Eve
    }
}

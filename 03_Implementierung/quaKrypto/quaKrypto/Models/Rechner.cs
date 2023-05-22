//**************************
// File: Rechner.cs
// Autor: TestTeam
// erstellt am: 06.05.2023
// Projekt quaKrypto
//**************************

using System;

namespace quaKrypto.Models
{
    public class Rechner
    {

        public int Addieren(int x, int y)

        {
            if (x == int.MaxValue && y != 0 || y == int.MaxValue && x != 0) throw new OverflowException();
            return x + y;
        }
    }
}
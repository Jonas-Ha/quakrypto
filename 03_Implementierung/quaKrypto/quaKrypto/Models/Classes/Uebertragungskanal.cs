// **********************************************************
// File: Uebertragungskanal.cs
// Autor: Alexander Denner, Leopold Bialek, Jonas Hammer
// erstellt am: 16.05.2023
// Projekt: quakrypto
// ********************************************************** 

using System.Collections.Generic;

namespace quaKrypto.Models.Classes
{
    public class Uebertragungskanal
    {
        private List<Information> photonenKanal;
        private List<Information> bitKanal;

        public Uebertragungskanal()
        {
            photonenKanal = new List<Information>();
            bitKanal = new List<Information>();
        }

        public List<Information> PhotonenKanal
        {
            get { return photonenKanal; }
        }

        public List<Information> BitKanal
        {
            get { return bitKanal; }
        }

        // legt eine Information auf dem entsprechenden Übertragungskanal ab
        // informationsTyp --> Photonen: photonenkanal
        // informationsTyp !=  Photonen: bitkanal
        public void SpeicherNachrichtAb(Information information)
        {
            if(information.InformationsTyp == Enums.InformationsEnum.unscharfePhotonen)
            {
                photonenKanal.Add(information);
            }
            else if(information.InformationsTyp != Enums.InformationsEnum.photonen)
            {
                bitKanal.Add(information);
            }
        }

        // gibt eine Liste aus Informationen aus den entsprechenden Kanälen zurück
        public List<Information>? LeseKanalAus(Enums.KanalEnum kanal)
        {
            if(kanal == Enums.KanalEnum.photonenKanal)
            {
                return photonenKanal;
            }
            else if(kanal == Enums.KanalEnum.bitKanal)
            {
                return bitKanal;
            }
            else return null;
        }

        // entfernt eine Nachricht aus dem Übertragungskanal
        public void LoescheNachricht(Enums.KanalEnum kanal, int informationsID)
        {
            if(kanal == Enums.KanalEnum.photonenKanal)
            {
                for(int i = 0; i < photonenKanal.Count; i++)
                {
                    if (photonenKanal[i].InformationsID == informationsID)
                    {
                        photonenKanal.RemoveAt(i);
                    }
                }
            }
            else if(kanal == Enums.KanalEnum.bitKanal)
            {
                for (int i = 0; i < bitKanal.Count; i++)
                {
                    if (bitKanal[i].InformationsID == informationsID)
                    {
                        bitKanal.RemoveAt(i);
                    }
                }
            }
        }
    }
}

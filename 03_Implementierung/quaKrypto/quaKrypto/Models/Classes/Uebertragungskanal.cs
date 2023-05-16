using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quaKrypto.Models.Classes
{
    internal class Uebertragungskanal
    {
        //Eventuell noch zu einer ObservableCollection machen
        private List<Information> photonenKanal;
        private List<Information> bitKanal;

        Uebertragungskanal()
        {
            photonenKanal = new List<Information>();
            bitKanal = new List<Information>();
        }

        void SpeicherNachrichtAb(Information information)
        {
            if(information.informationsTyp == Enums.InformationsEnum.unscharfePhotonen)
            {
                photonenKanal.Append(information);
            }
            else if(information.informationsTyp != Enums.InformationsEnum.photonen)
            {
                bitKanal.Append(information);
            }
        }

        List<Information>? LeseKanalAus(Enums.KanalEnum kanal)
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

        void LoescheNachricht(Enums.KanalEnum kanal, uint informationsID)
        {
            if(kanal == Enums.KanalEnum.photonenKanal)
            {
                for(int i = 0; i < photonenKanal.Count; i++)
                {
                    if (photonenKanal[i].informationsID == informationsID)
                    {
                        photonenKanal.RemoveAt(i);
                    }
                }
            }
            else if(kanal == Enums.KanalEnum.bitKanal)
            {
                for (int i = 0; i < bitKanal.Count; i++)
                {
                    if (bitKanal[i].informationsID == informationsID)
                    {
                        bitKanal.RemoveAt(i);
                    }
                }
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bar_length_calculator
{
    class Ulozenie
    {
        public static int _ileKonstrukcji;

        List<Element> uklad;
        private float reszta;
        public float Reszta() { return reszta; }
        private int ileSztabek = 0;

        public int Sztabek { get => ileSztabek; set => ileSztabek = value; }

        //public int Sztabek()
        //{
        //    return ileSztabek;
        //}

        public string ToString(ShowMode sm)
        {
            string s="";
            if(sm==ShowMode.Old)
            {
                s += "[";
                foreach (var el in uklad)
                {
                    // el.dlugosc_cm + "cm x " +
                    s += el.quantity + ", ";
                }
                s += "r = " + reszta + "] x " + ileSztabek;
            }
            else if(sm==ShowMode.Default || sm==ShowMode.All)
            {
                foreach (var e in uklad)
                {
                    if (e.quantity > 0 || sm == ShowMode.All)
                    {
                        if(e.quantity==1)
                            s += "    " + e.quantity + " element o długości ";
                        else if(e.quantity<5)
                            s += "    " + e.quantity + " elementy o długości ";
                        else
                            s += "    " + e.quantity + " elementów o długości ";
                    
                        if (e.length == Math.Floor(e.length))
                        {
                            int temp = (int)e.length;
                            s += temp.ToString();
                        }
                        else
                        {
                            s += e.length.ToString();
                        }
                        s += " cm\n";
                    }
                   
                }
            }
            else
            {
                foreach (var e in uklad)
                {
                    if (e.quantity > 0 || sm == ShowMode.AllShort)
                    {
                        s += "    " + e.quantity + " x ";
                        if (e.length == Math.Floor(e.length))
                        {
                            int temp = (int)e.length;
                            s += temp.ToString();
                        }
                        else
                        {
                            s += e.length.ToString();
                        }
                        s += " cm\n";
                    }

                       
                }
            }
            
            return s;
        }

        public override string ToString()
        {
            return ToString(ShowMode.Old);
        }

        public Ulozenie(float dlugosc_pelnej_sztabki, List<Element> profil_typow)
        {
            uklad = new List<Element>();
            uklad.AddRange(profil_typow);

            // przepisuje liste elementow z profilu_typow ale z iloscia 0
            Element e = new Element();
            e.quantity = 0;
            for (int i = 0; i < uklad.Count(); i++)
            {
                //e.ile = 0 zawsze
                e.length = uklad[i].length;
                uklad[i] = e;
            }

            reszta = -1;
            //elemy = new List<int>();
            _ileKonstrukcji++;
        }

        public Ulozenie(float dlugosc_pelnej_sztabki, List<Element> profil_typow, float reszta)
        {

            uklad = new List<Element>();
            uklad.AddRange(profil_typow);

            // przepisuje liste elementow ale z iloscia 1
            Element e = new Element();
            e.quantity = 0;
            for (int i = 0; i < uklad.Count(); i++)
            {
                e.length = uklad[i].length;
                uklad[i] = e;
            }

            this.reszta = reszta;
            _ileKonstrukcji++;
        }

        public float AnalizaResztek(float dl_sztabki)
        {
            float s = 0;
            foreach (var el in uklad)
            {
                s += el.quantity * el.length;
            }
            reszta = dl_sztabki - s;
            return reszta;
        }

        public List<Element> OdczytajElement()
        {
            return uklad;
        }

        //public int IleElemntow(int id)

        /// <summary>
        /// Sprawdza czy podany element da się odciąć od pozostałej sztabki. Jeśli tak to odcina, w przeciwnym razie sztabka zostaje nienaruszona
        /// </summary>
        /// <param name="el">typ elementu</param>
        /// <returns>Prawde jeśli udało się odciąć, fałsz gdy jest to niemożliwe.</returns>
        public bool SprobojOdciac(Element el)
        {
            if (reszta - el.length > 0)
            {
                ++el.quantity;
                reszta -= el.length;
                return true;
            }
            else return false;
        }

        public void DodajElement(int numer_elementu)
        {
            Element e = new Element();
            e.quantity = uklad[numer_elementu].quantity + 1;
            e.length = uklad[numer_elementu].length;
            uklad[numer_elementu] = e;
        }
    }
}

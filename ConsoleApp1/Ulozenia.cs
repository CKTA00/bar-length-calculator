using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rdzen
{
    struct Elementy
    {
        /// <summary>
        /// Długość elementu w cm.
        /// </summary>
        public float dlugosc_cm;
        /// <summary>
        /// Ilosc elementów o tej długości.
        /// </summary>
        public int ile;
        /// <summary>
        /// Zmienna porządkowa; 0 - el. najwiekszy. Identyfikuje element
        /// </summary>
        //public byte p;
    }

    /// <summary>
    /// Opisuje ulozenie elementów na sztabce. Przechowuje również ilość sztabek.
    /// </summary>
    class Ulozenie
    {
        public static int _ileKonstrukcji;

        List<Elementy> uklad;
        private float reszta;
        public float Reszta() { return reszta; }
        private int ileSztabek = 0;

        public int Sztabek { get => ileSztabek; set => ileSztabek = value; }


        public override string ToString()
        {
            string r = "[";
            foreach (var el in uklad)
            {
                // el.dlugosc_cm + "cm x " +
                r += el.ile + ", ";
            }
            r += "r = " + reszta + "] x " + ileSztabek;
            return r;
        }

        public Ulozenie(float dlugosc_pelnej_sztabki, List<Elementy> profil_typow)
        {
            uklad = new List<Elementy>();
            uklad.AddRange(profil_typow);

            // przepisuje liste elementow z profilu_typow ale z iloscia 0
            Elementy e;
            e.ile = 0;
            for (int i = 0; i < uklad.Count(); i++)
            {
                //e.ile = 0 zawsze
                e.dlugosc_cm = uklad[i].dlugosc_cm;
                uklad[i] = e;
            }

            reszta = -1;
            //elemy = new List<int>();
            _ileKonstrukcji++;
        }

        public Ulozenie(float dlugosc_pelnej_sztabki, List<Elementy> profil_typow, float reszta)
        {

            uklad = new List<Elementy>();
            uklad.AddRange(profil_typow);

            // przepisuje liste elementow ale z iloscia 1
            Elementy e;
            e.ile = 0;
            for (int i = 0; i < uklad.Count(); i++)
            {
                e.dlugosc_cm = uklad[i].dlugosc_cm;
                uklad[i] = e;
            }

            this.reszta = reszta;
            _ileKonstrukcji++;
        }

        //delegat
        public static int porownajReszty(Ulozenie u1, Ulozenie u2)
        {
            if (u1.reszta > u2.reszta) return 1;
            else if (u1.reszta < u2.reszta) return -1;
            else return 0;
        }

        public float AnalizaResztek(float dl_sztabki)
        {
            float s = 0;
            foreach (var el in uklad)
            {
                s += el.ile * el.dlugosc_cm;
            }
            reszta = dl_sztabki - s;
            return reszta;
        }

        public List<Elementy> OdczytajElement()
        {
            return uklad;
        }

        //public int IleElemntow(int id)

        /// <summary>
        /// Sprawdza czy podany element da się odciąć od pozostałej sztabki. Jeśli tak to odcina, w przeciwnym razie sztabka zostaje nienaruszona
        /// </summary>
        /// <param name="el">typ elementu</param>
        /// <returns>Prawde jeśli udało się odciąć, fałsz gdy jest to niemożliwe.</returns>
        public bool SprobojOdciac(Elementy el)
        {
            if (reszta - el.dlugosc_cm > 0)
            {
                ++el.ile;
                reszta -= el.dlugosc_cm;
                return true;
            }
            else return false;
        }

        public void DodajElement(int numer_elementu)
        {
            Elementy e = new Elementy();
            e.ile = uklad[numer_elementu].ile + 1;
            e.dlugosc_cm = uklad[numer_elementu].dlugosc_cm;
            uklad[numer_elementu] = e;
        }
    }
}

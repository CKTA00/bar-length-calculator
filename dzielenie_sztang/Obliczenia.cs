using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
//using System.Windows.Data;

namespace dzielenie_sztang
{
    class Obliczenia
    {
        const int N = 256;
        /// <summary>
        /// Długość całej sztabki w cm.
        /// </summary>
        public float dlugosc_sztabki_cm { get; set; }

        public Profil profil { get; set; }

        /// <summary>
        /// lista wszystkich zasobów (rodzaju elementów i ich ilości) dodanych przez użytkownika
        /// </summary>
        public List<Elementy> zasoby;
        public ObservableCollection<ElementyObiekt> Zasoby { get; set; }
        

        /// <summary>
        /// Tutaj wgrane i przechowane zostaną wszystkie rodzaje ułożeń elementów na sztabce
        /// </summary>
        private List<Ulozenie> wszystkie_uklady;

        //public DebugOutput dout = DebugOutput.NONE;

        public void _pokazUlozenia()
        {
            Console.Write("[");
            foreach (var e in zasoby)
            {
                Console.Write(e.dlugosc_cm + "cm, ");
            }
            Console.WriteLine(" reszta]");
            foreach (var u in wszystkie_uklady)
            {
                Console.WriteLine(u.ToString());
            }
        }

        public void _pokazZasoby()
        {
            foreach (var z in zasoby)
            {
                Console.WriteLine("element dlugosci " + z.dlugosc_cm + " razy " + z.ile);
            }
        }

        public override string ToString()
        {
            return profil.nazwa_profilu + ": " + dlugosc_sztabki_cm + " cm";
        }

       

        public Obliczenia(string profil)
        {
            //ile_typow = 0;
            zasoby = new List<Elementy>();
            Zasoby = new ObservableCollection<ElementyObiekt>();
            wszystkie_uklady = new List<Ulozenie>();
            this.profil = new Profil(profil);
        }

        public Obliczenia(Profil profil)
        {
            //ile_typow = 0;
            zasoby = new List<Elementy>();
            Zasoby = new ObservableCollection<ElementyObiekt>();
            wszystkie_uklady = new List<Ulozenie>();
            this.profil = profil;
        }

        /// <summary>
        /// Dodaje rodzaj elementu i zwraca prawde, jesli el. o tej dlugosci jeszcze nie istnieje. Zwraca fałsz i nie dodaje jeśli istniał.
        /// </summary>
        /// <param name="e">Element do dodania</param>
        /// <returns>Czy element został dodany?</returns>
        public bool DodajZasob(ElementyObiekt e)
        {
            foreach (var z in Zasoby)
            {
                if (z.dlugosc_cm == e.dlugosc_cm) return false; //element juz istnieje na tej liscie!
            }
            Zasoby.Add(e);
            return true;
        }

        /// <summary>
        /// Dodaje elementy o tej samej długości do siebie.
        /// </summary>
        /// <param name="oDlugosci">Wyszukaj element o tej długości...</param>
        /// <param name="oTyle">...i powiększ ich ilosć o tą wartość.</param>
        public void PowiekszZasob(float oDlugosci,int oTyle)
        {
            for(int i = 0; i<Zasoby.Count; i++)
            {
                if (Zasoby[i].dlugosc_cm == oDlugosci)
                {
                    //el.ile += oTyle;
                    ElementyObiekt newEl = new ElementyObiekt(Zasoby[i].dlugosc_cm, Zasoby[i].ile + oTyle);
                    Zasoby[i] = newEl;
                    return;
                }
            }
            MessageBox.Show("Lista \"Zasoby\" nie posiada elementu danej długości.\nFunkcja PowiekszZasob(float,int) nie wykonała się."
                , "Dziwny błąd. Powiedz koniecznie Przemkowi!", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        /// <summary>
        /// Konweruje listę obiektów wykorzystywaną przez DataGrid na liste struktur wykorzystywaną przez algorytm. (nie zwraca, wewnątrz obiektu)
        /// </summary>
        void KonwertujZasoby()
        {
            zasoby.Clear();
            Elementy el = new Elementy();
            foreach (var zas in Zasoby)
            {
                el.dlugosc_cm = zas.dlugosc_cm;
                el.ile = zas.ile;
                zasoby.Add(el);
            }
        }

        /// <summary>
        /// Sortuje elementy w liscie zasoby.
        /// </summary>
        private void SortujTypyElementow()
        {
            // bubble sort
            Elementy e;
            int ile_z = zasoby.Count();
            ile_z--;
            for (int p = 0; p < ile_z; p++)
            {
                for (int i = 0; i < ile_z; i++)
                {
                    if (zasoby[i].dlugosc_cm < zasoby[i + 1].dlugosc_cm)
                    {
                        e = zasoby[i];
                        zasoby[i] = zasoby[i + 1];
                        zasoby[i + 1] = e;
                    }
                }
            }
        }

        /// <summary>
        /// Wyznacza wszystkie możliwe ułożenia wraz z resztami
        /// </summary>
        private void WyznaczWszystkieMozliweUlozenia()
        {
            //KonwertujZasoby();
            wszystkie_uklady.Clear();
            wszystkie_uklady.AddRange(WyznaczPodulozenie(dlugosc_sztabki_cm, dlugosc_sztabki_cm));
            wszystkie_uklady.Remove(wszystkie_uklady.Last<Ulozenie>());
            foreach (var u in wszystkie_uklady) u.AnalizaResztek(dlugosc_sztabki_cm);
            //usuń argument formalny i zastąp zmienną statyczną wgrywaną w konstruktorze
        }

        private List<Ulozenie> WyznaczPodulozenie(float dl_sztabeczki, float max_dl_elementu)
        {
            List<Ulozenie> lista = new List<Ulozenie>();
            float roz;
            for (int i = 0; i < zasoby.Count; ++i)// i to numer zasobu
            {
                if (zasoby[i].dlugosc_cm <= max_dl_elementu)
                {
                    roz = dl_sztabeczki - zasoby[i].dlugosc_cm;
                    if (roz >= 0 && zasoby[i].ile > 0)
                    {
                        Elementy e = zasoby[i];
                        --e.ile;
                        zasoby[i] = e;

                        List<Ulozenie> l = new List<Ulozenie>();
                        l = WyznaczPodulozenie(roz, zasoby[i].dlugosc_cm);

                        foreach (var u in l)
                        {
                            u.DodajElement(i);//dodaje element o numerze zasobu i
                        }
                        lista.AddRange(l);
                        ++e.ile;
                        zasoby[i] = e;
                    }
                }//else nic nie rób   
            }

            lista.Add(new Ulozenie(dlugosc_sztabki_cm, zasoby));//dodaje element [0,0,...,0]
            
            return lista;
        }

        /// <summary>
        /// Wylicza ile potrzeba jakich sztabek (Ułożeń) aby pokryć zasoby.
        /// </summary>
        private void WybierzWymaganeSztabki()
        {
            //List<Elementy> zasoby = new List<Elementy>(this.zasoby);
            List<Ulozenie> ret = new List<Ulozenie>();
            Elementy temp = new Elementy();
            bool dodaj;
            int dodaj_ile = 0;
            int k = 0;
            while (k < wszystkie_uklady.Count)
            {
                List<Elementy> uklad = wszystkie_uklady[k].OdczytajElement();
                int i = 0;
                dodaj = true;

                for (; i < zasoby.Count; i++) if (uklad[i].ile > zasoby[i].ile) dodaj = false;
                // jesli choc o jeden element za duzo ustaw na false

                if (dodaj)
                {
                    i = 0;
                    for (; i < zasoby.Count; i++)
                    {
                        temp = zasoby[i];
                        temp.ile -= uklad[i].ile;
                        zasoby[i] = temp;
                    }

                    dodaj = false;
                    dodaj_ile++;
                }
                else
                {
                    ret.Add(wszystkie_uklady[k]);
                    wszystkie_uklady[k].Sztabek = dodaj_ile;
                    ret.Last<Ulozenie>().Sztabek = dodaj_ile;
                    dodaj_ile = 0;
                    k++;
                }
            }
        }


        public List<Ulozenie> WykonajObliczenia()
        {
            //Console.WriteLine("TEST");
            //Console.ReadKey();
            KonwertujZasoby();
            SortujTypyElementow();
            WyznaczWszystkieMozliweUlozenia();
            WybierzWymaganeSztabki();
            return wszystkie_uklady;
        }
    }

    
}

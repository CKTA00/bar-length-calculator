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

namespace bar_length_calculator
{
    class Obliczenia
    {
        const int N = 256;
        /// <summary>
        /// Długość całej sztabki w cm.
        /// </summary>
        public float dlugosc_sztabki_cm { get; set; }

        public BarProfile profil { get; set; }

        /// <summary>
        /// lista wszystkich zasobów (rodzaju elementów i ich ilości) dodanych przez użytkownika
        /// </summary>
        public List<Element> zasoby;
        public ObservableCollection<ElementObject> Zasoby { get; set; }
        

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
                Console.Write(e.length + "cm, ");
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
                Console.WriteLine("element dlugosci " + z.length + " razy " + z.quantity);
            }
        }

        public override string ToString()
        {
            return profil.profileName + ": " + dlugosc_sztabki_cm + " cm";
        }

       

        public Obliczenia(string profil)
        {
            //ile_typow = 0;
            zasoby = new List<Element>();
            Zasoby = new ObservableCollection<ElementObject>();
            wszystkie_uklady = new List<Ulozenie>();
            this.profil = new BarProfile(profil);
        }

        public Obliczenia(BarProfile profil)
        {
            //ile_typow = 0;
            zasoby = new List<Element>();
            Zasoby = new ObservableCollection<ElementObject>();
            wszystkie_uklady = new List<Ulozenie>();
            this.profil = profil;
        }

        /// <summary>
        /// Dodaje rodzaj elementu i zwraca prawde, jesli el. o tej dlugosci jeszcze nie istnieje. Zwraca fałsz i nie dodaje jeśli istniał.
        /// </summary>
        /// <param name="e">Element do dodania</param>
        /// <returns>Czy element został dodany?</returns>
        public bool DodajZasob(ElementObject e)
        {
            foreach (var z in Zasoby)
            {
                if (z.length == e.length) return false; //element juz istnieje na tej liscie!
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
                if (Zasoby[i].length == oDlugosci)
                {
                    //el.ile += oTyle;
                    ElementObject newEl = new ElementObject(Zasoby[i].length, Zasoby[i].quantity + oTyle);
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
            Element el = new Element();
            foreach (var zas in Zasoby)
            {
                el.length = zas.length;
                el.quantity = zas.quantity;
                zasoby.Add(el);
            }
        }

        /// <summary>
        /// Sortuje elementy w liscie zasoby.
        /// </summary>
        private void SortujTypyElementow()
        {
            // bubble sort
            Element e;
            int ile_z = zasoby.Count();
            ile_z--;
            for (int p = 0; p < ile_z; p++)
            {
                for (int i = 0; i < ile_z; i++)
                {
                    if (zasoby[i].length < zasoby[i + 1].length)
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
                if (zasoby[i].length <= max_dl_elementu)
                {
                    roz = dl_sztabeczki - zasoby[i].length;
                    if (roz >= 0 && zasoby[i].quantity > 0)
                    {
                        Element e = zasoby[i];
                        --e.quantity;
                        zasoby[i] = e;

                        List<Ulozenie> l = new List<Ulozenie>();
                        l = WyznaczPodulozenie(roz, zasoby[i].length);

                        foreach (var u in l)
                        {
                            u.DodajElement(i);//dodaje element o numerze zasobu i
                        }
                        lista.AddRange(l);
                        ++e.quantity;
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
            Element temp = new Element();
            bool dodaj;
            int dodaj_ile = 0;
            int k = 0;
            while (k < wszystkie_uklady.Count)
            {
                List<Element> uklad = wszystkie_uklady[k].OdczytajElement();
                int i = 0;
                dodaj = true;

                for (; i < zasoby.Count; i++) if (uklad[i].quantity > zasoby[i].quantity) dodaj = false;
                // jesli choc o jeden element za duzo ustaw na false

                if (dodaj)
                {
                    i = 0;
                    for (; i < zasoby.Count; i++)
                    {
                        temp = zasoby[i];
                        temp.quantity -= uklad[i].quantity;
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

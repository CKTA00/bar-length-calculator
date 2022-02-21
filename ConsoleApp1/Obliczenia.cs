using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rdzen
{

    /// <summary>
    /// Jakie dodatkowe informacje wypisać w konsoli. Dla zmiennej >= BASIC wypisane zostanie zarówno dla ALL jak i BASIC.  
    /// </summary>
    //enum DebugOutput : short
    //{
    //    NONE, FANCY, BASIC, ALL
    //}

    class Obliczenia
    {
        const int N = 256;
        /// <summary>
        /// Długość całej sztabki w cm.
        /// </summary>
        private float dlugosc_sztabki_cm { get; set; }
        

        /// <summary>
        /// Najkrótszy element.
        /// </summary>
        //private float nk;
        /// <summary>
        /// lista wszystkich zasobów (rodzaju elementów i ich ilości) dodanych przez użytkownika
        /// </summary>
        private List<Elementy> zasoby;
        /// <summary>
        /// Tutaj wgrane i przechowane zostaną wszystkie rodzaje ułożeń elementów na sztabce
        /// </summary>
        private List<Ulozenie> wszystkie_uklady;

        private List<Ulozenie> wynik;
        public List<Ulozenie> Wynik { get => wynik; }

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
                Console.WriteLine("element dlugosci "+z.dlugosc_cm + " razy " + z.ile);
            }
        }

        public Obliczenia()
        {
            //ile_typow = 0;
            zasoby = new List<Elementy>();
            wszystkie_uklady = new List<Ulozenie>();
        }

        /// <summary>
        /// Dodaje rodzaj elementu i zwraca prawde, jesli el. o tej dlugosci jeszcze nie istnieje. Zwraca fałsz i nie dodaje jeśli istniał.
        /// </summary>
        /// <param name="e">Element do dodania</param>
        /// <returns>Czy element został dodany?</returns>
        public bool DodajTypElementu(Elementy e)
        {
            foreach (var z in zasoby)
            {
                if (z.dlugosc_cm == e.dlugosc_cm) return false; //element juz istnieje na tej liscie!
            }
            //if(ile_typow>255)
            //{
            //    Console.WriteLine("Za duzo elementow w tablicy typy_elementow");
            //    return false;
            //}
            //typy_elementow[ile_typow] = e;
            zasoby.Add(e);
            //++ile_typow;
            return true;
        }

        // nic
        ///// <summary>
        ///// Najkrótszy element zostanie zapisany do zmiennej 'nk'.
        ///// </summary>
        //public void WyznaczNajkrotszyElement()
        //{
        //    nk = dlugosc_sztabki_cm;
        //    foreach (var z in zasoby)
        //    {
        //        if (nk > z.dlugosc_cm)
        //            nk = z.dlugosc_cm;
        //    }
        //    Console.WriteLine(nk);
        //}

        public void SortujTypyElementow()
        {
            // bubble sort
            Elementy e;
            int ile_z = zasoby.Count();
            ile_z--;
            for(int p = 0; p < ile_z; p++)
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
        public void WyznaczWszystkieMozliweUlozenia()
        {
            wszystkie_uklady.AddRange(WyznaczPodulozenie(dlugosc_sztabki_cm,dlugosc_sztabki_cm));
            wszystkie_uklady.Remove(wszystkie_uklady.Last<Ulozenie>());
            foreach (var u in wszystkie_uklady) u.AnalizaResztek(dlugosc_sztabki_cm); 
            //usuń argument formalny i zastąp zmienną statyczną wgrywaną w konstruktorze
        }

        public List<Ulozenie> WyznaczPodulozenie(float dl_sztabeczki,float max_dl_elementu)
        {
            //if (dout == DebugOutput.ALL) Console.WriteLine("FUNCT: sztab = "+dl_sztabeczki+", max_dl = "+max_dl_elementu);
            List<Ulozenie> lista = new List<Ulozenie>();
            float roz;
            for (int i = 0; i < zasoby.Count; ++i)// i to numer zasobu
            {
                if(zasoby[i].dlugosc_cm<=max_dl_elementu)
                {
                    roz = dl_sztabeczki - zasoby[i].dlugosc_cm;
                    if (roz >= 0 && zasoby[i].ile > 0)
                    {
                        //if (dout == DebugOutput.ALL) Console.WriteLine("  rekurencja \\/");
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

            lista.Add(new Ulozenie(dlugosc_sztabki_cm,zasoby));//dodaje element [0,0,...,0]
            //if (dout == DebugOutput.ALL)
            //{
            //    Console.WriteLine("return /\\");
            //    foreach (var ll in lista)
            //    {
            //        Console.WriteLine(ll.ToString());
            //    }
            //} 
            return lista;
        }

        /// <summary>
        /// Sortuje ułożenia wg. reszty (od najmniejszej do największej)
        /// </summary>
        public void SortujWgReszty()
        {
            // mial byc merge sort ale w sumie chuj
            wszystkie_uklady.Sort(Ulozenie.porownajReszty);
        }

   
        /// <summary>
        /// Wylicza ile potrzeba jakich sztabek (Ułożeń) aby pokryć zasoby.
        /// </summary>
        public void WybierzWymaganeSztabki()
        {
            List<Ulozenie> ret= new List<Ulozenie>();
            Elementy temp;
            bool dodaj;
            int dodaj_ile=0;
            int k = 0;
            while(k<wszystkie_uklady.Count)
            {
                List<Elementy> uklad = wszystkie_uklady[k].OdczytajElement();
                int i = 0;
                dodaj = true;
                
                for (; i < zasoby.Count; i++) if (uklad[i].ile > zasoby[i].ile) dodaj = false;
                // jesli choc o jeden element zad uzo ustaw na false

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

        static void Main(string[] args)
        {
            Obliczenia o = new Obliczenia();
            Console.WriteLine("MAIN");
            Console.WriteLine("Wprowadź długość sztabki: ");
            o.dlugosc_sztabki_cm = float.Parse(Console.ReadLine());
            Console.WriteLine("Wprowadź długości elementów. Wpisz x aby zakończyć wprowadzanie. ");
            string menu = "0";
            float dana;
            int ile_dana;
            Elementy el = new Elementy();
            while (menu != "x" && menu != "d")
            {
                Console.Write("\tWprowadź długość elementu: ");
                menu = Console.ReadLine();
                if (float.TryParse(menu, out dana))
                {
                    el.dlugosc_cm = dana;
                    Console.Write("\tIle: ");
                    menu = Console.ReadLine();
                    if (int.TryParse(menu, out ile_dana))
                    {
                        el.ile = ile_dana;
                        o.DodajTypElementu(el);
                    }
                }
            }
            Console.WriteLine("Zakończono podawanie danych");
            //if (menu == "d") o.dout = DebugOutput.ALL;
            //else o.dout = DebugOutput.BASIC;
            o.SortujTypyElementow();
            //if (o.dout == DebugOutput.ALL) o._pokazZasoby();


            //    o.WyznaczWszystkieMozliweUlozenia();
            //    //if (o.dout >= DebugOutput.BASIC) o._pokazUlozenia();
            //    //Console.WriteLine("Liczba Ułożeń to {0}\n", Ulozenie._ileKonstrukcji-1);

            //    o.SortujWgReszty();

            //    //List<Ulozenie> wynik = o.WybierzWymaganeSztabki();
            //    o.WybierzWymaganeSztabki();
            //    foreach (var u in o.wszystkie_uklady)
            //    {
            //        Console.WriteLine(u.ToString());
            //    }



            //    Console.ReadKey();
            }
        }
    }



// kappa


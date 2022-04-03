using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace bar_length_calculator
{
    class Calculations
    {
        const int N = 256;
        /// <summary>
        /// Długość całej sztabki w cm.
        /// </summary>
        public float mainBarLength { get; set; }

        /// <summary>
        /// BarProfile associated with resources
        /// </summary>
        public BarProfile profile { get; set; }

        /// <summary>
        /// List of all elements with their amounts added by the user.
        /// Lista wszystkich zasobów (rodzaju elementów i ich ilości) dodanych przez użytkownika.
        /// </summary>
        public List<Element> resources;

        /// <summary>
        /// List of all elements with their amounts added by the user, that is updated by the UI.
        /// </summary>
        public ObservableCollection<ElementObject> Resources { get; set; }
        
        /// <summary>
        /// List of all arrangements that will be calculated by this object.
        /// Tutaj wgrane i przechowane zostaną wszystkie rodzaje ułożeń elementów na sztabce
        /// </summary>
        private List<Arrangement> allArrangements;

        public Calculations(string profil)
        {
            //ile_typow = 0;
            resources = new List<Element>();
            Resources = new ObservableCollection<ElementObject>();
            allArrangements = new List<Arrangement>();
            this.profile = new BarProfile(profil);
        }

        public Calculations(BarProfile profil)
        {
            //ile_typow = 0;
            resources = new List<Element>();
            Resources = new ObservableCollection<ElementObject>();
            allArrangements = new List<Arrangement>();
            this.profile = profil;
        }

        /// <summary>
        /// Adds kind of element and returns true if one hadn't existed yet. Do not add if given kind already had existed.
        /// Dodaje rodzaj elementu i zwraca prawde, jesli el. o tej dlugosci jeszcze nie istnieje. Zwraca fałsz i nie dodaje jeśli istniał.
        /// </summary>
        /// <param name="e">Element witch kind will be added</param>
        /// <returns>True on success</returns>
        public bool AddResource(ElementObject e)
        {
            foreach (var z in Resources)
            {
                if (z.length == e.length) return false; //element juz istnieje na tej liscie!
            }
            Resources.Add(e);
            return true;
        }

        /// <summary>
        /// Adds amount to existing kind defined by length
        /// Dodaje elementy o tej samej długości do siebie.
        /// </summary>
        /// <param name="resorceLength">Find element with maching length</param>
        /// <param name="increaseAmount">Increase by this value</param>
        public void IncreaseExistingResource(float resorceLength, int increaseAmount)
        {
            for (int i = 0; i < Resources.Count; i++)
            {
                if (Resources[i].length == resorceLength)
                {
                    //el.ile += oTyle;
                    ElementObject newEl = new ElementObject(Resources[i].length, Resources[i].quantity + increaseAmount);
                    Resources[i] = newEl;
                    return;
                }
            }
            MessageBox.Show("Lista \"Zasoby\" nie posiada elementu danej długości.\nFunkcja PowiekszZasob(float,int) nie wykonała się."
                , "Dziwny błąd. Powiedz koniecznie Przemkowi!", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        /// <summary>
        /// Run all functions necessary to perform calculations, which results will be returned
        /// </summary>
        /// <returns>Result of calculations</returns>
        public List<Arrangement> RunCalculations()
        {
            ConvertResources();
            SortElementByLength();
            CalculateAllPossibleArrangements();
            SelectRequiredBars();
            return allArrangements;
        }

        //deprecated
        void _debug_PrintArrangaments()
        {
            Console.Write("[");
            foreach (var e in resources)
            {
                Console.Write(e.length + "cm, ");
            }
            Console.WriteLine(" reszta]");
            foreach (var u in allArrangements)
            {
                Console.WriteLine(u.ToString());
            }
        }

        //deprecated
        void _debug_PrintResources()
        {
            foreach (var z in resources)
            {
                Console.WriteLine("element dlugosci " + z.length + " razy " + z.quantity);
            }
        }

        public override string ToString()
        {
            return profile.profileName + ": " + mainBarLength + " cm";
        }

        /// <summary>
        /// Konweruje listę obiektów wykorzystywaną przez DataGrid na liste struktur wykorzystywaną przez algorytm. (nie zwraca, wewnątrz obiektu)
        /// </summary>
        void ConvertResources()
        {
            resources.Clear();
            Element el = new Element();
            foreach (var zas in Resources)
            {
                el.length = zas.length;
                el.quantity = zas.quantity;
                resources.Add(el);
            }
        }

        /// <summary>
        /// Sorts elements of resources list with custom algorithm.
        /// Sortuje elementy w liscie zasoby.
        /// </summary>
        private void SortElementByLength()
        {
            // TODO: Maybe use C# sorting instead?
            // bubble sort
            Element e;
            int resCount = resources.Count();
            resCount--;
            for (int p = 0; p < resCount; p++)
            {
                for (int i = 0; i < resCount; i++)
                {
                    if (resources[i].length < resources[i + 1].length)
                    {
                        e = resources[i];
                        resources[i] = resources[i + 1];
                        resources[i + 1] = e;
                    }
                }
            }
        }

        /// <summary>
        /// Calculates all possible arrangements with proper waste calculated.
        /// Wyznacza wszystkie możliwe ułożenia wraz z resztami
        /// </summary>
        private void CalculateAllPossibleArrangements()
        {
            //KonwertujZasoby();
            allArrangements.Clear();
            allArrangements.AddRange(CalculateSubtask(mainBarLength, mainBarLength));
            allArrangements.Remove(allArrangements.Last<Arrangement>());
            foreach (var u in allArrangements) u.CalculateRemnants(mainBarLength);
        }

        private List<Arrangement> CalculateSubtask(float dl_sztabeczki, float max_dl_elementu)
        {
            List<Arrangement> lista = new List<Arrangement>();
            float roz;
            for (int i = 0; i < resources.Count; ++i)// i to numer zasobu
            {
                if (resources[i].length <= max_dl_elementu)
                {
                    roz = dl_sztabeczki - resources[i].length;
                    if (roz >= 0 && resources[i].quantity > 0)
                    {
                        Element e = resources[i];
                        --e.quantity;
                        resources[i] = e;

                        List<Arrangement> l = new List<Arrangement>();
                        l = CalculateSubtask(roz, resources[i].length);

                        foreach (var u in l)
                        {
                            u.AddElement(i); //add resource element number i
                        }
                        lista.AddRange(l);
                        ++e.quantity;
                        resources[i] = e;
                    }
                }
            }

            lista.Add(new Arrangement(mainBarLength, resources));//add elements [0,0,...,0]
            
            return lista;
        }

        /// <summary>
        /// Calculates which bars in what amount are needed to cover resources.
        /// Wylicza ile potrzeba jakich sztabek (Ułożeń) aby pokryć zasoby.
        /// </summary>
        private void SelectRequiredBars()
        {
            List<Arrangement> ret = new List<Arrangement>();
            Element temp = new Element();
            bool shouldAdd;
            int toAdd = 0;
            int k = 0;
            while (k < allArrangements.Count)
            {
                List<Element> uklad = allArrangements[k].GetLayout();
                int i = 0;
                shouldAdd = true;

                for (; i < resources.Count; i++) if (uklad[i].quantity > resources[i].quantity) shouldAdd = false;
                // if just one element too much set it to false

                if (shouldAdd)
                {
                    i = 0;
                    for (; i < resources.Count; i++)
                    {
                        temp = resources[i];
                        temp.quantity -= uklad[i].quantity;
                        resources[i] = temp;
                    }

                    shouldAdd = false;
                    toAdd++;
                }
                else
                {
                    ret.Add(allArrangements[k]);
                    allArrangements[k].BarAmount = toAdd;
                    ret.Last<Arrangement>().BarAmount = toAdd;
                    toAdd = 0;
                    k++;
                }
            }
        }
    }

    
}

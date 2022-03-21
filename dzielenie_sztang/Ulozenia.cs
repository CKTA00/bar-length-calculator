using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bar_length_calculator
{
    enum ShowMode
    {
        Default,
        Short,
        All,
        AllShort,
        Old
    }

    //struct Elementy
    //{
    //    //int id;
    //    /// <summary>
    //    /// Długość elementu w cm.
    //    /// </summary>
    //    public float dlugosc_cm;
    //    /// <summary>
    //    /// Ilosc elementów o tej długości.
    //    /// </summary>
    //    public int ile;
    //    /// <summary>
    //    /// Zmienna porządkowa; 0 - el. najwiekszy. Identyfikuje element
    //    /// </summary>
    //    //public byte p;
    //}

    struct Elementy : INotifyPropertyChanged
    {
        //int id;
        /// <summary>
        /// Długość elementu w cm.
        /// </summary>
        public float dlugosc_cm { get; set; }
        /// <summary>
        /// Ilosc elementów o tej długości.
        /// </summary>
        public int ile { get; set; }
        /// <summary>
        /// Zmienna porządkowa; 0 - el. najwiekszy. Identyfikuje element
        /// </summary>
        //public byte p;
        public float DlugoscCm
        {
            get { return dlugosc_cm; }
            set
            {
                dlugosc_cm = value;
                NotifyPropertyChanged("DlugoscCm");
            }
        }

        public int Ile
        {
            get { return ile; }
            set
            {
                ile = value;
                NotifyPropertyChanged("Ile");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            
        }
    }

    class ElementyObiekt : INotifyPropertyChanged
    {
        /// <summary>
        /// Długość elementu w cm.
        /// </summary>
        public float dlugosc_cm { get; set; }
        /// <summary>
        /// Ilosc elementów o tej długości.
        /// </summary>
        public int ile { get; set; }

        public float DlugoscCm
        {
            get { return dlugosc_cm; }
            set
            {
                dlugosc_cm = value;
                NotifyPropertyChanged("DlugoscCm");
            }
        }

        public int Ile
        {
            get { return ile; }
            set
            {
                ile = value;
                NotifyPropertyChanged("Ile");
            }
        }

        public ElementyObiekt(float dlugosc=0,int ilosc=0)
        {
            DlugoscCm = dlugosc;
            Ile = ilosc;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));

        }
    }

    class Ulozenie
    {
        public static int _ileKonstrukcji;

        List<Elementy> uklad;
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
                    s += el.ile + ", ";
                }
                s += "r = " + reszta + "] x " + ileSztabek;
            }
            else if(sm==ShowMode.Default || sm==ShowMode.All)
            {
                foreach (var e in uklad)
                {
                    if (e.ile > 0 || sm == ShowMode.All)
                    {
                        if(e.ile==1)
                            s += "    " + e.ile + " element o długości ";
                        else if(e.ile<5)
                            s += "    " + e.ile + " elementy o długości ";
                        else
                            s += "    " + e.ile + " elementów o długości ";
                    
                        if (e.dlugosc_cm == Math.Floor(e.dlugosc_cm))
                        {
                            int temp = (int)e.dlugosc_cm;
                            s += temp.ToString();
                        }
                        else
                        {
                            s += e.dlugosc_cm.ToString();
                        }
                        s += " cm\n";
                    }
                   
                }
            }
            else
            {
                foreach (var e in uklad)
                {
                    if (e.ile > 0 || sm == ShowMode.AllShort)
                    {
                        s += "    " + e.ile + " x ";
                        if (e.dlugosc_cm == Math.Floor(e.dlugosc_cm))
                        {
                            int temp = (int)e.dlugosc_cm;
                            s += temp.ToString();
                        }
                        else
                        {
                            s += e.dlugosc_cm.ToString();
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

        public Ulozenie(float dlugosc_pelnej_sztabki, List<Elementy> profil_typow)
        {
            uklad = new List<Elementy>();
            uklad.AddRange(profil_typow);

            // przepisuje liste elementow z profilu_typow ale z iloscia 0
            Elementy e = new Elementy();
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
            Elementy e = new Elementy();
            e.ile = 0;
            for (int i = 0; i < uklad.Count(); i++)
            {
                e.dlugosc_cm = uklad[i].dlugosc_cm;
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

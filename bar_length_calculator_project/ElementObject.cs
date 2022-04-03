using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bar_length_calculator
{
    class ElementObject : INotifyPropertyChanged // TODO: replace with proxy pattern or some sort of adapter to struct Element (or replace struct by object or remove entirely or fundamentaly change architecture)
    {
        /// <summary>
        /// Element length in cm.
        /// Długość elementu w cm.
        /// </summary>
        public float length { get; set; }
        /// <summary>
        /// Amount of elements represented by ElementObject object.
        /// Ilosc elementów o tej długości.
        /// </summary>
        public int quantity { get; set; }

        public float Length
        {
            get { return length; }
            set
            {
                length = value;
                NotifyPropertyChanged("Length");
            }
        }

        public int Quantity
        {
            get { return quantity; }
            set
            {
                quantity = value;
                NotifyPropertyChanged("Quantity");
            }
        }

        public ElementObject(float dlugosc = 0, int ilosc = 0)
        {
            Length = dlugosc;
            Quantity = ilosc;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}

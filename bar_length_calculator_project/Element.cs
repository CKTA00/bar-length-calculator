using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bar_length_calculator
{
    struct Element : INotifyPropertyChanged
    {
        /// <summary>
        /// Element length in cm.
        /// Długość elementu w cm.
        /// </summary>
        public float length { get; set; }
        /// <summary>
        /// Amount of elements represented by Element object.
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));

        }
    }
}

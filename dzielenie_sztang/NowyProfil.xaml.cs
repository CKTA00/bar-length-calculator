using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace bar_length_calculator
{
    /// <summary>
    /// Logika interakcji dla klasy NowyProfil.xaml
    /// </summary>
    public partial class NowyProfil : Window
    {
        public ObservableCollection<BarProfile> lista_profili { get; set; }

        public BarProfile WybranyProfil {
            get
            {
                if ((bool)ZLlistyRB.IsChecked)
                {
                    return ProfilZListyCBX.SelectedItem as BarProfile;
                }
                else
                {
                    return new BarProfile(DowolnyTBX.Text);
                }
            }
        }

        public NowyProfil()
        {
            bool brak_pliku = false;
            lista_profili = new ObservableCollection<BarProfile>();
            try
            {
                string[] lines = System.IO.File.ReadAllLines(Environment.CurrentDirectory + "\\profile.txt");
                if (lines != null)
                {
                    foreach (var line in lines)
                    {
                        lista_profili.Add(new BarProfile(line));
                    }
                }
            }
            catch
            {
                brak_pliku = true;
            }
           
            if(lista_profili.Count<=0)
            {
                brak_pliku = true;
            }

            InitializeComponent();
            ProfilZListyCBX.ItemsSource = lista_profili;
            ProfilZListyCBX.Items.Refresh();
            DowolnyRB.IsChecked = brak_pliku;
            ZLlistyRB.IsEnabled = !brak_pliku;
            if(!brak_pliku)
            {
                ProfilZListyCBX.SelectedItem = lista_profili.First();
            }
        }

        private void DodajBT_Click(object sender, RoutedEventArgs e)
        {
            if((bool)DowolnyRB.IsChecked && (bool)ZapiszProfilCB.IsChecked)
            {
                using (StreamWriter file =
                new StreamWriter(Environment.CurrentDirectory + "\\profile.txt", true))
                {
                    file.WriteLine(DowolnyTBX.Text);
                }
            }



            this.DialogResult = true;
        }

    }
}

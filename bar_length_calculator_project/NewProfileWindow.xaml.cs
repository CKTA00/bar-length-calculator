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
    /// Logic behind interaction with New Profile Window
    /// </summary>
    public partial class NewProfileWindow : Window
    {
        public ObservableCollection<BarProfile> profilesList { get; set; }

        public BarProfile SelectedProfile {
            get
            {
                if ((bool)FromListRB.IsChecked)
                {
                    return ProfileOfListCBX.SelectedItem as BarProfile;
                }
                else
                {
                    return new BarProfile(CustomTBX.Text);
                }
            }
        }

        public NewProfileWindow()
        {
            bool noFile = false;
            profilesList = new ObservableCollection<BarProfile>();
            try
            {
                string[] lines = System.IO.File.ReadAllLines(Environment.CurrentDirectory + "\\profile.txt");
                if (lines != null)
                {
                    foreach (var line in lines)
                    {
                        profilesList.Add(new BarProfile(line));
                    }
                }
            }
            catch
            {
                noFile = true;
            }
           
            if(profilesList.Count<=0)
            {
                noFile = true;
            }

            InitializeComponent();
            ProfileOfListCBX.ItemsSource = profilesList;
            ProfileOfListCBX.Items.Refresh();
            CustomRB.IsChecked = noFile;
            FromListRB.IsEnabled = !noFile;
            if(!noFile)
            {
                ProfileOfListCBX.SelectedItem = profilesList.First();
            }
        }

        private void AddBT_Click(object sender, RoutedEventArgs e)
        {
            if((bool)CustomRB.IsChecked && (bool)SaveProfileCB.IsChecked)
            {
                using (StreamWriter file =
                new StreamWriter(Environment.CurrentDirectory + "\\profile.txt", true))
                {
                    file.WriteLine(CustomTBX.Text);
                }
            }



            this.DialogResult = true;
        }

    }
}

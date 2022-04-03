using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace bar_length_calculator
{
    

    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string version = "2.1";
        /// <summary>
        /// Length of currently selected bar.
        /// </summary>
        private float barLength = 0;
        /// <summary>
        /// Does adding of same length bars option is selected by user?
        /// </summary>
        private bool userDecidedToAdd = false;
        /// <summary>
        /// Results of calculations of one currently selected profile that will be printed in <see cref="ResultTB"/>.
        /// </summary>
        private List<Arrangement> result;  //TODO: Replace with direct read from Calculations object or proxy if needed
        /// <summary>
        /// List of added profiles, that can be manipulated in <see cref="ProfileCB"/>
        /// </summary>
        private List<Calculations> profiles;
        /// <summary>
        /// Currently selected profile from <see cref="ProfileCB"/>.
        /// </summary>
        private Calculations currentProfile { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ElementsTable.AutoGenerateColumns = false;
            profiles = new List<Calculations>();
            ProfileCB.ItemsSource = profiles;
            BlockModification();
            BottomStrip.Text = "Witaj " + Environment.UserName + " w programie Kalkulator Sztabek v." + version;
        }

        /// <summary>
        /// Blocks (or unblocks when param true) GUI elements, which should be disabled when there is no profiles.
        /// </summary>
        /// <param name="unlock"> true == unlock, false(default) == block</param>
        private void BlockModification(bool unlock = false)
        {
            MiniProfilePanel.IsEnabled = unlock;
            MiniBarPanel.IsEnabled = unlock;
            DeleteProfileB.IsEnabled = unlock;
            CalculateB.IsEnabled = unlock;
            AddElemenPanel.IsEnabled = unlock;
            ElementToolsPanel.IsEnabled = unlock;
            ElementsTable.IsEnabled = unlock;
            if (!unlock)
            {
                barTB.Text = "";
            }
        }

        /// <summary>
        /// Updates sources of data and UI to new profile
        /// </summary>
        /// <param name="selectedProfile">New profile (cannot be NULL!)</param>
        private void ChangeProfile(Calculations selectedProfile)
        {
            ProfileCB.SelectedItem = selectedProfile;
            currentProfile = selectedProfile;
            BottomStrip.Text = "Edytujesz: " + currentProfile.ToString();
            ElementsTable.ItemsSource = currentProfile.Resources;
            ElementsTable.Items.Refresh();

            barLength = currentProfile.mainBarLength;
            if (barLength != 0) barTB.Text = barLength.ToString();
            else barTB.Text = "";
            ValidateBar();
        }
        
        /// <summary>
        /// Validates length of bar. Can display error messages.
        /// </summary>
        private void ValidateBar()
        {
            if (currentProfile != null)
            {
                string errorMessage;
                float tempLength = 0;
                if (ValidateAndCheckUserInput(barTB.Text, out tempLength, out errorMessage))
                {
                    MiniBarPanel.ToolTip = "Długość całej sztabki"; // TODO: make language dependent
                    barTB.Foreground = Brushes.Black;
                    barTB.ClearValue(BorderBrushProperty);
                    barTB.ClearValue(BackgroundProperty);
                }
                else
                {
                    MiniBarPanel.ToolTip = errorMessage;
                    barTB.Foreground = Brushes.DarkRed;
                    barTB.BorderBrush = Brushes.Red;
                    barTB.Background = Brushes.LightPink;
                }
                currentProfile.mainBarLength = tempLength;
            }
            ValidateElementsLength();
            BottomStrip.Text = "Edytujesz: " + currentProfile.ToString();  // TODO: make language dependent
        }

        private void ValidateElementsLength()
        {
            float test;
            if (String.IsNullOrWhiteSpace(DlugoscTB.Text))
            {
                DlugoscTB.ClearValue(ForegroundProperty);
                DlugoscTB.ClearValue(BorderBrushProperty);
                DlugoscTB.ClearValue(BackgroundProperty);
                DlugoscTB.ToolTip = "Długość elementu wyrażona w centymetrach.";  // TODO: make language dependent
                return;
            }
            if (!float.TryParse(DlugoscTB.Text,out test))
            {
                DlugoscTB.Foreground = Brushes.DarkRed;
                DlugoscTB.BorderBrush = Brushes.Red;
                DlugoscTB.Background = Brushes.LightPink;
                DlugoscTB.ToolTip = "Wprowadź poprawną liczbę.";  // TODO: make language dependent
            }
            else
            {
                if(test<=0)
                {
                    DlugoscTB.Foreground = Brushes.DarkRed;
                    DlugoscTB.BorderBrush = Brushes.Red;
                    DlugoscTB.Background = Brushes.LightPink;
                    DlugoscTB.ToolTip = "Długość musi być dodatnia."; // TODO: make language dependent
                }
                else if(test>currentProfile.mainBarLength && currentProfile.mainBarLength!=0)
                {

                    DlugoscTB.Foreground = Brushes.DarkRed;
                    DlugoscTB.BorderBrush = Brushes.Red;
                    DlugoscTB.Background = Brushes.LightPink;
                    DlugoscTB.ToolTip = "Długość elementu nie może być większa od długośći sztabki."; // TODO: make language dependent
                }
                else
                {
                    DlugoscTB.ClearValue(ForegroundProperty);
                    DlugoscTB.ClearValue(BorderBrushProperty);
                    DlugoscTB.ClearValue(BackgroundProperty);
                    DlugoscTB.ToolTip = "Długość elementu wyrażona w centymetrach."; // TODO: make language dependent
                }
            }
        }

        /// <summary>
        /// Parses string text to float barLength. Returns false in case of error, writes error message to msg.
        /// </summary>
        /// <param name="text">Parsed string.</param>
        /// <param name="barLength">Parsing result bar length.</param>
        /// <param name="msg">User-friendly error message.</param>
        /// <returns></returns>
        private bool ValidateAndCheckUserInput(string text, out float barLength, out string msg)
        {
            if (float.TryParse(text, out barLength))
            {
                if (barLength <= 0)
                {
                    msg = "Długość sztabki musi być dodatnia."; // TODO: make language dependent
                    barLength = 0;
                    return false;
                }
            }
            else if (barTB.Text != "")
            {
                msg = "Pole długości sztabki może składać się wyłącznie z cyfr i znkau \",\""; // TODO: make language dependent
                this.barLength = 0;
                return false;
            }
            else
            {
                msg = "Wprowadź długość sztabki!"; // TODO: make language dependent
                this.barLength = 0;
                return false;
            }

            msg = text;
            return true;
        }

        /// <summary>
        /// Returns false in case of error, writes error message to msg.
        /// </summary>
        /// <param name="value">Parsed string.</param>
        /// <param name="msg">User-friendly error message.</param>
        /// <returns></returns>
        private bool CheckUserInput(float value, out string msg)
        {

            if (value <= 0)
            {
                msg = "Długość sztabki musi być dodatnia."; // TODO: make language dependent
                value = 0;
                return false;
            }
            else if (value == 0)
            {
                msg = "Wprowadź długość sztabki!";  // TODO: make language dependent
                barLength = 0;
                return false;
            }

            msg = value.ToString();
            return true;
        }

        /// <summary>
        /// Ask user if they want to correct data by merging same length counts
        /// </summary>
        /// <returns> True if user decided to correct data. </returns>
        private bool AskUserToCorrectData()
        {
            if (!userDecidedToAdd)
            {
                MessageBoxResult r =
                   MessageBox.Show("Długości wprowadzonych elementów powtarzają się.\nCzy dodać do siebie podane ilości elementów o identycznych długościach?",
                   "Powtarzające się dane", MessageBoxButton.OKCancel, MessageBoxImage.Question);  // TODO: make language dependent
                if (r == MessageBoxResult.Cancel || r == MessageBoxResult.None) return false;
                else userDecidedToAdd = true;
            }
            return true;
        }

        /// <summary>
        /// Wykonuje obliczenia dla "profil". Zwraca fałsz w prypadku błędu.
        /// Starts calculations for profile. Returns false in case of error.
        /// </summary>
        /// <param name="profile">Calculate for which profile.</param>
        /// <param name="continue">Continue if more profiles to show.</param>
        /// <returns></returns>
        private bool CalculateForProfile(Calculations profile, bool @continue) // TODO: make language dependent
        {

            foreach (var dana in profile.Resources)
            {
                string errorMessage;
                if (!CheckUserInput(profile.mainBarLength, out errorMessage))
                {
                    errorMessage += "\nBłąd wystąpił w profilu " + profile.profile.profileName + ".\nSprawdź pole zaznaczone na czerwono.";
                    MessageBox.Show(errorMessage, "Błędne dane", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return false;
                }

                if (dana.length > profile.mainBarLength)
                {
                    MessageBox.Show("Długość każdego elementu musi być\nmniejsza od długości sztabki.\nBłąd wystąpił w profilu " + profile.profile.profileName,
                        "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Asterisk); 
                    return false;
                }
                if (dana.length <= 0)
                {
                    MessageBox.Show("Długość każdego elementu musi być dodatnia.\nBłąd wystąpił w profilu " + profile.profile.profileName,
                       "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return false;
                }
                if (dana.quantity <= 0)
                {
                    MessageBox.Show("Ilość każdego elementu musi być dodatnia.\nBłąd wystąpił w profilu " + profile.profile.profileName,
                       "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return false;
                }
            }

            // TODO: Create thread
            result = profile.RunCalculations();

            userDecidedToAdd = false;

            if (!@continue)
                ResultTB.Clear();
            if(profile.profile.presentationName!="")
                ResultTB.Text += "PROFIL " + profile.profile.presentationName + ":\n"; // TODO: make language dependent
            ResultTB.Text+=ShowResults();
            return true;
        }

        /// <summary>
        /// Shows results to user.
        /// </summary>
        private string ShowResults() // TODO: make language dependent
        {
            String t = "";
            float wasteSum = 0;
            if (!ShortCB.IsChecked.Value)
            {
                t += "Algorytm zakończył działanie!\n\n";
                foreach (var u in result)
                {
                    if (u.BarAmount > 0)
                    {
                        if (u.BarAmount > 4)
                            t += u.BarAmount.ToString() + " sztabek o układzie elementów:\n";
                        else if (u.BarAmount > 1)
                            t += u.BarAmount.ToString() + " sztabki o układzie elementów:\n";
                        else if (u.BarAmount == 1)
                            t += u.BarAmount.ToString() + " sztabka o układzie elementów:\n";
                        else
                            t += u.BarAmount.ToString() + " sztabek o układzie elementów:\n";

                        t += u.ToString(ShowMode.Default);
                        t += "Pozostałe resztki: " + u.Remnant().ToString() + " cm\n\n";
                        wasteSum += u.Remnant() * u.BarAmount;

                    }
                }
                t += "Suma wszystkich odpadów to: " + wasteSum;

            }
            else
            {
                foreach (var u in result)
                {
                    if (u.BarAmount > 0)
                    {
                        t += u.BarAmount.ToString() + " x sztanga:\n";
                        t += u.ToString(ShowMode.Short);
                        t += "    Odpad: " + u.Remnant().ToString() + " cm\n";
                        wasteSum += u.Remnant() * u.BarAmount;
                    }
                }
                t += "Suma odpadów: " + wasteSum + " cm";
            }

            
            return t + "\n\n";
            
        }

        // HANDILNG BUTTONS AND OTHER INTERACTIVE UI ELEMENTS
            // Upper panel:

        private void ProfileCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProfileCB.SelectedItem != null)
                ChangeProfile(ProfileCB.SelectedItem as Calculations);
            else
                BottomStrip.Text = "Witaj " + Environment.UserName + " w programie Kalkulator Sztabek v." + version;
        }

        private void BarTB_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateBar();
        }

        private void NewProfileB_Click(object sender, RoutedEventArgs e)
        {
            NewProfileWindow dialog = new NewProfileWindow();
            if (dialog.ShowDialog() == true)
            {
                Calculations nowyProfil = new Calculations(dialog.SelectedProfile);

                profiles.Add(nowyProfil);
                ChangeProfile(nowyProfil);
                barTB.Text = dialog.DlugoscTBX.Text;
                ValidateBar();
                ProfileCB.Items.Refresh();
                BlockModification(true);
            }
        }

        private void DeleteProfileB_Click(object sender, RoutedEventArgs e)
        {
            // TODO: make language dependent
            if (MessageBox.Show("Czy na pewno chcesz usunąć profil "+currentProfile.ToString()+" z obliczeń?","Potwierdź",MessageBoxButton.YesNo,MessageBoxImage.Question,MessageBoxResult.No)==MessageBoxResult.Yes)
            {
                if (profiles.Count == 1)
                {
                    ProfileCB.SelectedItem = null;
                    profiles.Clear();
                    BlockModification();
                    currentProfile.Resources.Clear();
                }
                else
                {
                    profiles.Remove(currentProfile);
                    ProfileCB.SelectedItem = profiles.Last();
                }
                ProfileCB.Items.Refresh();
                ElementsTable.Items.Refresh();
            }
        }

        private void CalculateB_Click(object sender, RoutedEventArgs e)
        {
            if (profiles.Count > 0)
            {
                ResultTB.Text = "";
                foreach (var p in profiles)
                    if (p.Resources.Count > 0)
                    {
                        
                        CalculateForProfile(p, true);
                    }
                        
            }
            else MessageBox.Show("Brak kluczowych danych dotyczących proflili.", "Brak profili", MessageBoxButton.OK, MessageBoxImage.Warning); // TODO: make language dependent
            BottomStrip.Text = "Algorytm zakończył działanie! Edytujesz: " + currentProfile.ToString(); // TODO: make language dependent
        }

            // Lower panel:

        private void AddB_Click(object sender, RoutedEventArgs e) // TODO: make language dependent
        {
            if (currentProfile != null)
            {
                float probaFloat;
                int probaInt;
                if (float.TryParse(DlugoscTB.Text, out probaFloat))
                {
                    if (probaFloat <= 0)
                    {
                        MessageBox.Show("Długości elementu musi być dodatnia.", "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        //DlugoscTB.Text = "";
                        DlugoscTB.Focus();
                        return;
                    }
                    else if(probaFloat>currentProfile.mainBarLength && currentProfile.mainBarLength!=0)
                    {
                        MessageBox.Show("Długości elementu nie może być większa od długości sztabki.", "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        DlugoscTB.Focus();
                        return;
                    }

                    if (int.TryParse(IleTB.Text, out probaInt))
                    {
                        if (probaInt <= 0)
                        {
                            MessageBox.Show("Ilość elementów musi być dodatnia.", "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            IleTB.Focus();
                        }
                        else
                        {
                            ElementObject ell = new ElementObject(probaFloat, probaInt);

                            if (!currentProfile.AddResource(ell))
                            {

                                if (MessageBox.Show("Element o takiej długości już został dodany.\nCzy dodać wprowadzoną ilość (" + ell.quantity + ") do istniejącej?",
                                    "Powtarzające się dane", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    currentProfile.IncreaseExistingResource(ell.length, ell.quantity);
                                }
                            }
                            ElementsTable.Items.Refresh();
                            IleTB.Text = "";
                            DlugoscTB.Text = "";
                            DlugoscTB.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Pole ilości elementów może składać się\nwyłącznie z cyfr i znkau \",\"", "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        IleTB.Focus();
                    }
                }
                else
                {
                    DlugoscTB.Focus();
                }
            }
            else
            {
                MessageBox.Show("Najpierw musisz dodać profil do obliczeń.", "Brak profili", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        private void DeleteB_Click(object sender, RoutedEventArgs e)
        {

            List<ElementObject> lista = new List<ElementObject>();
            ElementObject temp_e;
            foreach (var item in ElementsTable.SelectedCells)
            {
                temp_e = (ElementObject)item.Item;
                lista.Add(temp_e);
            }
            foreach (var item in lista)
            {
                currentProfile.Resources.Remove(item);
            }
            ElementsTable.Items.Refresh();
        }

        private void ClearB_Click(object sender, RoutedEventArgs e)
        {
            currentProfile.Resources.Clear();

            ElementsTable.Items.Refresh();
        }

        private void LengthTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateElementsLength();
        }

        private void AmountTB_TextChanged(object sender, TextChangedEventArgs e) // TODO: make language dependent
        {
            int test;
            if(String.IsNullOrWhiteSpace(IleTB.Text))
            {
                IleTB.ClearValue(ForegroundProperty);
                IleTB.ClearValue(BorderBrushProperty);
                IleTB.ClearValue(BackgroundProperty);
                IleTB.ToolTip = "Ilość elementów o danej długości.";
                return;
            }
            if (!int.TryParse(IleTB.Text, out test))
            {
                IleTB.Foreground = Brushes.DarkRed;
                IleTB.BorderBrush = Brushes.Red;
                IleTB.Background = Brushes.LightPink;
                IleTB.ToolTip = "Wprowadź poprawną liczbę naturalną.";
            }
            else
            {
                if (test <= 0)
                {
                    IleTB.Foreground = Brushes.DarkRed;
                    IleTB.BorderBrush = Brushes.Red;
                    IleTB.Background = Brushes.LightPink;
                    IleTB.ToolTip = "Ilość musi być dodatnia.";
                }
                else
                {
                    IleTB.ClearValue(ForegroundProperty);
                    IleTB.ClearValue(BorderBrushProperty);
                    IleTB.ClearValue(BackgroundProperty);
                    IleTB.ToolTip = "Ilość elementów o danej długości.";
                }
            }
        }
    }
}
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
        static string wersja = "2.1";
        /// <summary>
        /// Długość sztabki obecnie wybranego profilu. Zastępuje Binding ponieważ jest to jedna z pierwszych zmiennych, które wprowadziłem, zanim zrozumiałem Binding. (bruh)
        /// </summary>
        private float dlugoscSztabki = 0;
        /// <summary>
        /// Czy użytkownik wybrał opcje dodawania podobnych danych elementów?
        /// </summary>
        private bool zdecydowalZeDodaje = false;
        /// <summary>
        /// Wyliczone ułożenie elementów dla jednego profilu akurat dodawanego do rezultatu tekstowego w <see cref="WynikTB"/>. Prawdopodobnie powinna być zastąpiona bezpośrednim odczytem z obiektu Obliczenia. (legacy: ostateczny wynik)
        /// </summary>
        private List<Ulozenie> wynik;
        /// <summary>
        /// Lista profili dodanych do edycji i możliwych do wybrania w <see cref="profilCB"/>.
        /// </summary>
        private List<Obliczenia> profile;
        /// <summary>
        /// Aktualnie wybrany profil z <see cref="profilCB"/>.
        /// </summary>
        private Obliczenia edytowany_profil { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            tabelaElementow.AutoGenerateColumns = false;
            profile = new List<Obliczenia>();
            profilCB.ItemsSource = profile;
            BlokadaEdycji();
            PasekInformacji.Text = "Witaj " + Environment.UserName + " w programie Kalkulator Sztabek v." + wersja;
        }

        /// <summary>
        /// Blokuje (Odblokowuje gdy true) elementy GUI, które powinny być wyłączone gdy nie ma żadnych profili. 
        /// </summary>
        /// <param name="odblokuj"> true == odblokuj, false(domyślnie) == zablokuj</param>
        private void BlokadaEdycji(bool odblokuj = false)
        {
            MiniPanelProfil.IsEnabled = odblokuj;
            MiniPanelSztabka.IsEnabled = odblokuj;
            UsunProfilB.IsEnabled = odblokuj;
            ObliczB.IsEnabled = odblokuj;
            PanelDodawaniaElementu.IsEnabled = odblokuj;
            PanelPrzyciskowElementu.IsEnabled = odblokuj;
            tabelaElementow.IsEnabled = odblokuj;
            if (!odblokuj)
            {
                sztabkaTB.Text = "";
            }
        }

        /// <summary>
        /// Aktualizuje źródła danych i GUI na nowy profil.
        /// </summary>
        /// <param name="zaznaczony_profil">Nowy profil (NIE NULL-ODPORNE!)</param>
        private void ZmianaProfilu(Obliczenia zaznaczony_profil)
        {
            profilCB.SelectedItem = zaznaczony_profil;
            edytowany_profil = zaznaczony_profil;
            PasekInformacji.Text = "Edytujesz: " + edytowany_profil.ToString();
            tabelaElementow.ItemsSource = edytowany_profil.Zasoby;
            tabelaElementow.Items.Refresh();

            dlugoscSztabki = edytowany_profil.dlugosc_sztabki_cm;
            if (dlugoscSztabki != 0) sztabkaTB.Text = dlugoscSztabki.ToString();
            else sztabkaTB.Text = "";
            UstawSztabke();
        }
        
        /// <summary>
        /// Obsługuje sprawdzanie poprawności długości sztabki i wyświetlenie komunikatu o błędzie.
        /// </summary>
        private void UstawSztabke()
        {
            if (edytowany_profil != null)
            {
                string errorMessage;
                float temp_dlugoscSztabki = 0;
                if (WprowadzonoPoprawnieSztabke(sztabkaTB.Text, out temp_dlugoscSztabki, out errorMessage))
                {
                    MiniPanelSztabka.ToolTip = "Długość całej sztabki";
                    sztabkaTB.Foreground = Brushes.Black;
                    sztabkaTB.ClearValue(BorderBrushProperty);
                    sztabkaTB.ClearValue(BackgroundProperty);
                }
                else
                {
                    MiniPanelSztabka.ToolTip = errorMessage;
                    sztabkaTB.Foreground = Brushes.DarkRed;
                    sztabkaTB.BorderBrush = Brushes.Red;
                    sztabkaTB.Background = Brushes.LightPink;
                }
                edytowany_profil.dlugosc_sztabki_cm = temp_dlugoscSztabki;
            }
            SprawdzDlugoscElementu();
            PasekInformacji.Text = "Edytujesz: " + edytowany_profil.ToString();
        }

        private void SprawdzDlugoscElementu()
        {
            float test;
            if (String.IsNullOrWhiteSpace(DlugoscTB.Text))
            {
                DlugoscTB.ClearValue(ForegroundProperty);
                DlugoscTB.ClearValue(BorderBrushProperty);
                DlugoscTB.ClearValue(BackgroundProperty);
                DlugoscTB.ToolTip = "Długość elementu wyrażona w centymetrach.";
                return;
            }
            if (!float.TryParse(DlugoscTB.Text,out test))
            {
                DlugoscTB.Foreground = Brushes.DarkRed;
                DlugoscTB.BorderBrush = Brushes.Red;
                DlugoscTB.Background = Brushes.LightPink;
                DlugoscTB.ToolTip = "Wprowadź poprawną liczbę.";
            }
            else
            {
                if(test<=0)
                {
                    DlugoscTB.Foreground = Brushes.DarkRed;
                    DlugoscTB.BorderBrush = Brushes.Red;
                    DlugoscTB.Background = Brushes.LightPink;
                    DlugoscTB.ToolTip = "Długość musi być dodatnia.";
                }
                else if(test>edytowany_profil.dlugosc_sztabki_cm && edytowany_profil.dlugosc_sztabki_cm!=0)
                {

                    DlugoscTB.Foreground = Brushes.DarkRed;
                    DlugoscTB.BorderBrush = Brushes.Red;
                    DlugoscTB.Background = Brushes.LightPink;
                    DlugoscTB.ToolTip = "Długość elementu nie może być większa od długośći sztabki.";
                }
                else
                {
                    DlugoscTB.ClearValue(ForegroundProperty);
                    DlugoscTB.ClearValue(BorderBrushProperty);
                    DlugoscTB.ClearValue(BackgroundProperty);
                    DlugoscTB.ToolTip = "Długość elementu wyrażona w centymetrach.";
                }
            }
        }

        /// <summary>
        /// Konwertuje string text na float dlugosc_sztabki. Kiedy natrafi na błąd zwraca false, a jego treść znajdzie się w string rezultat.
        /// </summary>
        /// <param name="text">String do próby przekonwertowania na poprawną długość sztabki.</param>
        /// <param name="dlugosc_sztabki">Zwracana długość sztabki.</param>
        /// <param name="rezultat">Komunikat błędu (user-friendly).</param>
        /// <returns></returns>
        private bool WprowadzonoPoprawnieSztabke(string text, out float dlugosc_sztabki, out string rezultat)
        {
            if (float.TryParse(text, out dlugosc_sztabki))
            {
                if (dlugosc_sztabki <= 0)
                {
                    rezultat = "Długość sztabki musi być dodatnia.";
                    dlugosc_sztabki = 0;
                    return false;
                }
            }
            else if (sztabkaTB.Text != "")
            {
                rezultat = "Pole długości sztabki może składać się wyłącznie z cyfr i znkau \",\"";
                dlugoscSztabki = 0;
                return false;
            }
            else
            {
                rezultat = "Wprowadź długość sztabki!";
                dlugoscSztabki = 0;
                return false;
            }

            rezultat = text;
            return true;
        }

        /// <summary>
        /// Sprawdza czy jest problem z konwersją textu na poprawną długość sztabki. Gdy błąd, zwraca false oraz treść błędu w string rezultat.
        /// </summary>
        /// <param name="text">String do próby przekonwertowania na poprawną długość sztabki.</param>
        /// <param name="rezultat">Komunikat błędu (user-friendly).</param>
        /// <returns></returns>
        private bool WprowadzonoPoprawnieSztabke(float wartosc, out string rezultat)
        {

            if (wartosc <= 0)
            {
                rezultat = "Długość sztabki musi być dodatnia.";
                wartosc = 0;
                return false;
            }
            else if (wartosc == 0)
            {
                rezultat = "Wprowadź długość sztabki!";
                dlugoscSztabki = 0;
                return false;
            }

            rezultat = wartosc.ToString();
            return true;
            //float dummy;
            //return WprowadzonoPoprawnieSztabke(text, out dummy, out rezultat);
        }

        /// <summary>
        /// Zapytaj użytkownika, czy chce poprawić powtarzające się dane.
        /// </summary>
        /// <returns> Prawde jeśli nie chce poprawić a dodać dane do siebie. </returns>
        private bool DodawajDane()
        {
            if (!zdecydowalZeDodaje)
            {
                MessageBoxResult r =
                   MessageBox.Show("Długości wprowadzonych elementów powtarzają się.\nCzy dodać do siebie podane ilości elementów o identycznych długościach?",
                   "Powtarzające się dane", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (r == MessageBoxResult.Cancel || r == MessageBoxResult.None) return false;
                else zdecydowalZeDodaje = true;
            }
            return true;
        }

        /// <summary>
        /// Wykonuje obliczenia dla "profil". Zwraca fałsz w prypadku błędu.
        /// </summary>
        /// <param name="profil">Na jakim profilu ma wykonać.</param>
        /// <param name="kontynuuj">Czy będzie wiele profilów do wyświetlenia.</param>
        /// <returns></returns>
        private bool ObliczProfil(Obliczenia profil, bool kontynuuj)
        {

            foreach (var dana in profil.Zasoby)
            {
                string errorMessage;
                if (!WprowadzonoPoprawnieSztabke(profil.dlugosc_sztabki_cm, out errorMessage))
                {
                    errorMessage += "\nBłąd wystąpił w profilu " + profil.profil.profileName + ".\nSprawdź pole zaznaczone na czerwono.";
                    MessageBox.Show(errorMessage, "Błędne dane", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return false;
                }

                if (dana.dlugosc_cm > profil.dlugosc_sztabki_cm)
                {
                    MessageBox.Show("Długość każdego elementu musi być\nmniejsza od długości sztabki.\nBłąd wystąpił w profilu " + profil.profil.profileName,
                        "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return false;
                }
                if (dana.dlugosc_cm <= 0)
                {
                    MessageBox.Show("Długość każdego elementu musi być dodatnia.\nBłąd wystąpił w profilu " + profil.profil.profileName,
                       "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return false;
                }
                if (dana.ile <= 0)
                {
                    MessageBox.Show("Ilość każdego elementu musi być dodatnia.\nBłąd wystąpił w profilu " + profil.profil.profileName,
                       "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return false;
                }
            }

            try
            {
                //BackgroundWorker worker = new BackgroundWorker();
                //worker.WorkerReportsProgress = true;
                //worker.DoWork += worker_DoWork;
                //worker.ProgressChanged += worker_ProgressChanged;
                //worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                //worker.RunWorkerAsync(profil);
                wynik = profil.WykonajObliczenia();
            }
            catch
            {
                //MessageBox.Show("Fatalny błąd związany z obliczeniami. Profil: "+profil.ToString(), "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                // return false;
            }
            zdecydowalZeDodaje = false;

            if (!kontynuuj)
                WynikTB.Clear();
            if(profil.profil.presentationName!="")
                WynikTB.Text += "PROFIL " + profil.profil.presentationName + ":\n";
            WynikTB.Text+=WyswietlWyniki();
            return true;
        }

        /// <summary>
        /// Wyświtela wynik obliczeń w TextBox.
        /// </summary>
        /// <param name="dodajDoWyniku">Czy ma zaktualizować wynik? Gdy false to podmienia</param>
        private string WyswietlWyniki()
        {
            String t = "";
            float sumaResztek = 0;
            if (!ShortCB.IsChecked.Value)
            {
                t += "Algorytm zakończył działanie!\n\n";
                foreach (var u in wynik)
                {
                    if (u.Sztabek > 0)
                    {
                        if (u.Sztabek > 4)
                            t += u.Sztabek.ToString() + " sztabek o układzie elementów:\n";
                        else if (u.Sztabek > 1)
                            t += u.Sztabek.ToString() + " sztabki o układzie elementów:\n";
                        else if (u.Sztabek == 1)
                            t += u.Sztabek.ToString() + " sztabka o układzie elementów:\n";
                        else
                            t += u.Sztabek.ToString() + " sztabek o układzie elementów:\n";

                        t += u.ToString(ShowMode.Default);
                        t += "Pozostałe resztki: " + u.Reszta().ToString() + " cm\n\n";
                        sumaResztek += u.Reszta() * u.Sztabek;

                    }
                }
                t += "Suma wszystkich odpadów to: " + sumaResztek;

            }
            else
            {
                foreach (var u in wynik)
                {
                    if (u.Sztabek > 0)
                    {
                        t += u.Sztabek.ToString() + " x sztanga:\n";
                        t += u.ToString(ShowMode.Short);
                        t += "    Odpad: " + u.Reszta().ToString() + " cm\n";
                        sumaResztek += u.Reszta() * u.Sztabek;
                    }
                }
                t += "Suma odpadów: " + sumaResztek + " cm";
            }

            
            return t + "\n\n";
            
        }

        // OBSŁUGA PRZYCISKÓW I INNYCH CONTROLSÓW:
            // Górny panel:

        private void ProfilCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (profilCB.SelectedItem != null)
                ZmianaProfilu(profilCB.SelectedItem as Obliczenia);
            else
                PasekInformacji.Text = "Witaj " + Environment.UserName + " w programie Kalkulator Sztabek v." + wersja;
        }

        private void SztabkaTB_LostFocus(object sender, RoutedEventArgs e)
        {
            UstawSztabke();
        }

        private void NowyProfilB_Click(object sender, RoutedEventArgs e)
        {
            NowyProfil okienko = new NowyProfil();
            if (okienko.ShowDialog() == true)
            {
                Obliczenia nowyProfil = new Obliczenia(okienko.WybranyProfil);

                profile.Add(nowyProfil);
                ZmianaProfilu(nowyProfil);
                sztabkaTB.Text = okienko.DlugoscTBX.Text;
                UstawSztabke();
                profilCB.Items.Refresh();
                BlokadaEdycji(true);
            }
        }

        private void UsunProfilB_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Czy na pewno chcesz usunąć profil "+edytowany_profil.ToString()+" z obliczeń?","Potwierdź",MessageBoxButton.YesNo,MessageBoxImage.Question,MessageBoxResult.No)==MessageBoxResult.Yes)
            {
                if (profile.Count == 1)
                {
                    profilCB.SelectedItem = null;
                    profile.Clear();
                    BlokadaEdycji();
                    edytowany_profil.Zasoby.Clear();
                }
                else
                {
                    profile.Remove(edytowany_profil);
                    profilCB.SelectedItem = profile.Last();
                }
                profilCB.Items.Refresh();
                tabelaElementow.Items.Refresh();
            }
        }

        private void ObliczB_Click(object sender, RoutedEventArgs e)
        {
           
            

            if (profile.Count > 0)
            {
                WynikTB.Text = "";
                foreach (var p in profile)
                    if (p.Zasoby.Count > 0)
                    {
                        
                        ObliczProfil(p, true);
                    }
                        
            }
            else MessageBox.Show("Z pustego i Salomon nie naleje.", "Brak profili", MessageBoxButton.OK, MessageBoxImage.Warning);
            PasekInformacji.Text = "Algorytm zakończył działanie! Edytujesz: " + edytowany_profil.ToString();
        }

        /*
        void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			
		}

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            PasekInformacji.Text = "Obliczanie... " + Convert.ToInt32(e.ProgressPercentage/100.0*profile.Count()) + "%";
        }
        */

        // Dolny Panel:

        private void DodajB_Click(object sender, RoutedEventArgs e)
        {
            if (edytowany_profil != null)
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
                    else if(probaFloat>edytowany_profil.dlugosc_sztabki_cm && edytowany_profil.dlugosc_sztabki_cm!=0)
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
                            //IleTB.Text = "";
                            IleTB.Focus();
                        }
                        else
                        {
                            ElementyObiekt ell = new ElementyObiekt(probaFloat, probaInt);

                            if (!edytowany_profil.DodajZasob(ell))
                            {

                                if (MessageBox.Show("Element o takiej długości już został dodany.\nCzy dodać wprowadzoną ilość (" + ell.ile + ") do istniejącej?",
                                    "Powtarzające się dane", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    edytowany_profil.PowiekszZasob(ell.dlugosc_cm, ell.ile);
                                }
                            }
                            tabelaElementow.Items.Refresh();
                            IleTB.Text = "";
                            DlugoscTB.Text = "";
                            DlugoscTB.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Pole ilości elementów może składać się\nwyłącznie z cyfr i znkau \",\"", "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        //IleTB.Text = "";
                        IleTB.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Pole długości sztabki może składać się\nwyłącznie z cyfr i znkau \",\"", "Nieprawidłowe dane", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    //DlugoscTB.Text = "";
                    DlugoscTB.Focus();
                }
            }
            else
            {
                MessageBox.Show("Najpierw musisz dodać profil do obliczeń.", "Brak profili", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        private void UsunB_Click(object sender, RoutedEventArgs e)
        {

            List<ElementyObiekt> lista = new List<ElementyObiekt>();
            ElementyObiekt temp_e;
            foreach (var item in tabelaElementow.SelectedCells)
            {
                temp_e = (ElementyObiekt)item.Item;
                lista.Add(temp_e);

                //edytowany_profil.Zasoby.Remove(item.Item as ElementyObiekt); z jakiegoś powodu nie dizała poprawnie,
                // zapewne jeden foreach wykonuje się 4 razy przy 2 zaznaczeniach bo są 4 komórki
            }
            foreach (var item in lista)
            {
                edytowany_profil.Zasoby.Remove(item);
            }
            tabelaElementow.Items.Refresh();
        }

        private void Wyczysc_Click(object sender, RoutedEventArgs e)
        {
            edytowany_profil.Zasoby.Clear();

            tabelaElementow.Items.Refresh();
        }

        private void DlugoscTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            SprawdzDlugoscElementu();
        }

        private void IleTB_TextChanged(object sender, TextChangedEventArgs e)
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
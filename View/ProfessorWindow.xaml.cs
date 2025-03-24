using PrviProjektniZadatakHCI;
using PrviProjektniZadatakHCI.DataAccess;
using PrviProjektniZadatakHCI.Resources;
using PrviProjektniZadatakHCI.View;
using PrviProjektniZadatakHCI.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ASystem
{
    public partial class ProfessorWindow : Window
    {
        public static RoutedCommand LogoutCommand = new RoutedCommand();

        private Profesor profesor;
        private PredmetViewModel predmetViewModel;
        public Stack<Action> undoStack = new Stack<Action>();
        private Stack<Action> redoStack = new Stack<Action>();
        ObservableCollection<Student> studenti = new ObservableCollection<Student>();

      
        public ProfessorWindow(Profesor profesor)
        {

            InitializeComponent();
            this.profesor = profesor;
            this.WindowState = WindowState.Maximized;
            ObservableCollection<String> predmeti = new ObservableCollection<String>();
            ObservableCollection<Predmet> listaPredmeta = PredmetDataAccess.izlistajPredmete(profesor);
            ObservableCollection<Student> listaStudenata = StudentDataAccess.GetStudents();
            predmeti = PredmetDataAccess.GetPredmeti(ProfessorDataAccess.GetProfesorId(profesor));
            homeworkDataGrid.ItemsSource = DomaciZadatakDataAccess.pregledDomacegPoProfesoru(profesor);
            predmetViewModel = new PredmetViewModel(profesor);
            this.DataContext = predmetViewModel;

            cmbPredmeti.ItemsSource = predmetViewModel.Predmeti;

            cmbStudenti.ItemsSource = listaStudenata;
            cmbStudentsGrade.ItemsSource = listaStudenata;
            cmbSubjectsForGrade.ItemsSource = listaPredmeta;
  

            lvDataBinding.ItemsSource= predmetViewModel.Predmeti; 
            cmbSubjectsForGrade.ItemsSource= predmetViewModel.Predmeti;
            cmbSubjects.ItemsSource = predmetViewModel.Predmeti;


            this.DataContext = listaPredmeta;
            cmbPredmet.ItemsSource = listaPredmeta;
            cmbPredmetOcjena.ItemsSource = listaPredmeta;

            CommandBinding undoBinding = new CommandBinding(ApplicationCommands.Undo, Undo_Executed);
            this.CommandBindings.Add(undoBinding);
            CommandBinding logoutBinding = new CommandBinding(LogoutCommand, LogoutButton_Click);
            this.CommandBindings.Add(logoutBinding);

        }

        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Undo_Click(sender, e);
        }
        private void LightThemeClick(object sender, RoutedEventArgs e)
        {
            ChangerThemes.ChangeTheme("Light", profesor.username);
        }

        private void DarkThemeClick(object sender, RoutedEventArgs e)
        {
            ChangerThemes.ChangeTheme("Dark", profesor.username);
        }

        private void GreenThemeClick(object sender, RoutedEventArgs e)
        {
            ChangerThemes.ChangeTheme("Green", profesor.username);
        }
        private void cmbPredmet_SelectionChangedOcjene(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPredmetOcjena.SelectedItem is Predmet selectedPredmet)
            {
               
                studenti = PredmetDataAccess.studentiSlusaju(selectedPredmet);


                if (studenti != null && studenti.Count > 0)
                {
                    cmbStudents.ItemsSource = studenti;
                }
                else
                {
                    MessageBox.Show("Nema studenata za odabrani predmet.");
                    cmbStudents.ItemsSource = null;
                }
            }
        }

        private void cmbSubject_SelectionChanged_Grade(object sender, SelectionChangedEventArgs e)
        {

            if (cmbStudentsGrade.SelectedItem is Student student)
            {
                if (cmbSubjectsForGrade.SelectedItem is Predmet predmet)
                {
                
                    ListGrade.ItemsSource = IspitDataAccess.pregledIspita(predmet, student).Select(ispit => new IspitViewModel(ispit)).ToList();

                }
                else
                {
                    new WarningWindow("Izaberite predmet!").ShowDialog();
                   // MessageBox.Show("Molimo izaberite predmet.");
                }
            }
            else
            {
                new WarningWindow("Izaberite studenta").ShowDialog();
              //  MessageBox.Show("Molimo izaberite studenta.");
            }
        }
        private void cmbStudent_SelectionChanged_Grade(object sender, SelectionChangedEventArgs e)
        {
           
            if (cmbStudentsGrade.SelectedItem is Student student)
            {
                if (cmbSubjectsForGrade.SelectedItem is Predmet predmet)
                {
                    ListGrade.ItemsSource = IspitDataAccess.pregledIspita(predmet, student);
                }
                else
                {       
                    MessageBox.Show("Molimo izaberite predmet.");
                }
            }
            else
            {
                MessageBox.Show("Molimo izaberite studenta.");
            }
        }


        private void cmbPredmet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPredmet.SelectedItem is Predmet selectedPredmet)
            {
                studenti = PredmetDataAccess.studentiSlusaju(selectedPredmet);
                if (studenti != null && studenti.Count > 0)
                {
                    AttendanceDataGrid.ItemsSource = studenti;
                }
                else
                {
                    new WarningWindow("Nema studenta za odabrani predmet").ShowDialog();
                 //   MessageBox.Show("Nema studenata za odabrani predmet.");
                    AttendanceDataGrid.ItemsSource = null;
                }
            }
        }
   private void cmbSubject_SelectionChanged(object sender, SelectionChangedEventArgs e)
           {

               if (cmbStudenti.SelectedItem is Student selectedStudent && cmbSubjects.SelectedItem is Predmet selectedPredmet)
               {
                   Attendance.ItemsSource = PrisustvoDataAccess.PregledPrisustva(selectedStudent, selectedPredmet, profesor);
               }

           }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (undoStack.Count > 0)
            {
                var undoAction = undoStack.Pop();
                undoAction.Invoke();
                homeworkDataGrid.ItemsSource = DomaciZadatakDataAccess.pregledDomacegPoProfesoru(profesor);
                redoStack.Push(undoAction);
            }
            else
            {
               new WarningWindow("Nema dostupnih operacija za poništavanje.").ShowDialog();
            }
        }

        private void SaveAttendance(object sender, RoutedEventArgs e)
        {
            Predmet predmet = cmbPredmet.SelectedItem as Predmet;
            DateTime? vrijeme = AttendanceDate.SelectedDate;

            if (predmet == null || !vrijeme.HasValue)
            {
                new WarningWindow("Molimo vas da izaberete predmet i datum.").ShowDialog();
                return;
            }

            ObservableCollection<Student> selektovaniStudenti = new ObservableCollection<Student>();

            foreach (var item in AttendanceDataGrid.Items)
            {
                var dataRow = item as Student;
                if (dataRow != null)
                {
                   
                    bool uspjeh = PrisustvoDataAccess.UnesiPrisustvo(vrijeme.Value, predmet, dataRow);

                    if (uspjeh /*&& dataRow.IsChecked*/)
                    {
                        selektovaniStudenti.Add(dataRow);
                    }
                }
            }

            if (selektovaniStudenti.Count > 0)
            {
                new SuccessWindow($"Prisustvo evidentirano za {selektovaniStudenti.Count} studenata na predmetu {predmet.Naziv} na datum {vrijeme.Value:dd.MM.yyyy}.").ShowDialog();
            }
            else
            {
                new WarningWindow("Nijedan student nije selektovan").ShowDialog();
               // MessageBox.Show("Nijedan student nije selektovan.");
            }
            foreach (var item in AttendanceDataGrid.Items)
            {
                var dataRow = item as Student;
                if (dataRow != null)
                {
                   
                }
            }

            AttendanceDataGrid.Items.Refresh();

            undoStack.Push(() =>
            {
                foreach (var student in selektovaniStudenti)
                {
                    PrisustvoDataAccess.izbrisiPrisustvo(vrijeme.Value, predmet, student);
                    new SuccessWindow($"Prisustvo za {student.ime} {student.prezime} je uklonjeno.").ShowDialog();
                }
            });

        }


        private void AzurirajPodatke(object sender, RoutedEventArgs e)
        {   var selektovaniPredmet = lvDataBinding.SelectedItem as Predmet;

            if (selektovaniPredmet != null)
            {
                new SuccessWindow($"Ažuriranje podataka").ShowDialog();
            }
            else
            {
               
            new WarningWindow("Nema selektovanog predmeta za ažuriranje.").ShowDialog();
            }
        }



        private void PregledStudenata(object sender, RoutedEventArgs e)
        {

            if (lvDataBinding.SelectedItem is Predmet selektovaniPredmet)
            {
                ObservableCollection<Student> listaStudenata = PredmetDataAccess.studentiSlusaju(selektovaniPredmet);
                StudentView studentView = new StudentView(listaStudenata);
                studentView.ShowDialog();
            }
            else
            {
                new WarningWindow("Nema selektovanog predmeta.");
            }
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
           
            CheckBox checkBox = sender as CheckBox;
          
        }
        private void SaveGrade(object sender, RoutedEventArgs e)
        {
         
            if (cmbPredmetOcjena.SelectedItem is not Predmet predmet)
            {
                new WarningWindow("Izaberite predmet").ShowDialog();
                //MessageBox.Show("Molimo izaberite predmet.");
                return;
            }
            if (ExamDate.SelectedDate is not DateTime datum)
            {
                new WarningWindow("Izaberite predmet").ShowDialog();
               // MessageBox.Show("Molimo izaberite datum ispita.");
                return;
            }
            if (!double.TryParse(txtBodovi.Text, out double bodovi) || bodovi < 0 || bodovi > 100)
            {
                new WarningWindow("Molimo unesite validan broj bodova (0-100).").ShowDialog();
               // MessageBox.Show("Molimo unesite validan broj bodova (0-100).");
                return;
            }
            if (!int.TryParse(txtOcjena.Text, out int ocjena) || ocjena < 5 || ocjena > 10)
            {
                new WarningWindow("Unesite validnu ocjenu (5-10)").ShowDialog();
              //  MessageBox.Show("Molimo unesite validnu ocjenu (5-10).");
                return;
            }
            if (cmbStudents.SelectedItem is not Student student)
            {
                new WarningWindow("Izaberite studenta").ShowDialog();
                //MessageBox.Show("Molimo izaberite studenta.");
                return;
            }
            try
            {
                
                IspitDataAccess.sacuvajIspit(bodovi, ocjena, datum, predmet, student);
                cmbPredmetOcjena.SelectedIndex=-1;
                ExamDate.SelectedDate = null;
                cmbStudents.SelectedIndex = -1;
                txtOcjena.Clear();
                txtBodovi.Clear();
                txtOcjena.Clear();
                new SuccessWindow("Ispit je uspješno sačuvan!").ShowDialog();
            }
            catch (Exception ex)
            {
                new WrongWindow("Greška prilikom čuvanja ispita {ex.Message}").ShowDialog();
              //  MessageBox.Show($"Došlo je do greške prilikom čuvanja ispita: {ex.Message}");
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

            CheckBox checkBox = sender as CheckBox;

        }
        private void AddTask(object sender, RoutedEventArgs e)
        {

            Predmet predmet = cmbPredmeti.SelectedItem as Predmet;

            if (predmet == null)
            {
                new WarningWindow("Predmet nije odabran").ShowDialog();
                return; 
            }
            string predmetNaziv = predmet.Naziv;

            string tipZadatka = TaskTypeSelector.SelectedItem is ComboBoxItem tipItem ? tipItem.Content.ToString() : "Nije odabran";
            string naziv = ime.Text;
            string opis = taskDescription.Text;
            string sifra = taskCode.Text;
            DateTime? rok = TaskDeadline.SelectedDate;
            string maksimalniBrojBodova = MaxPoints.Text;


            if (string.IsNullOrEmpty(naziv) || string.IsNullOrEmpty(opis) || string.IsNullOrEmpty(sifra) || !rok.HasValue || string.IsNullOrEmpty(maksimalniBrojBodova))
            {
                string message = SharedResource.FillField;
                new WarningWindow(message).Show();
                return; 
            }

            ObservableCollection<Student> studenti = PredmetDataAccess.studentiSlusaju(predmet);


            if (DomaciZadatakDataAccess.dodajDZadatak(naziv, opis, (DateTime)rok, sifra, predmet, profesor))
            {

                cmbPredmeti.SelectedItem = null;
                TaskTypeSelector.SelectedItem = null;
                ime.Text = string.Empty;
                taskDescription.Text = string.Empty;
                taskCode.Text = string.Empty;
                MaxPoints.Text = string.Empty;
                TaskDeadline.SelectedDate = null;

                homeworkDataGrid.ItemsSource = DomaciZadatakDataAccess.pregledDomacegPoProfesoru(profesor);

                string message = SharedResource.TaskAdd;
                new SuccessWindow(message).ShowDialog();

            }
            else
            {
                string message = SharedResource.TaskErrorMessage;
                new WrongWindow(message).ShowDialog();
            }    
            undoStack.Push(() =>
            {
                DomaciZadatakDataAccess.obrisiDZadatak(sifra);
                homeworkDataGrid.ItemsSource = DomaciZadatakDataAccess.pregledDomacegPoProfesoru(profesor);
                string message = SharedResource.TaskDelete;
                new SuccessWindow(message).ShowDialog();
            });
            redoStack.Push(() =>
            {
            DomaciZadatakDataAccess.dodajDZadatak(naziv, opis, (DateTime)rok, sifra, predmet, profesor);
                string message = SharedResource.TaskAgainAdd;
                new SuccessWindow(message).ShowDialog();
        });
        }


        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (homeworkDataGrid.SelectedItem != null)
            {
               
                var selectedDomaciZD = (DomaciZadatak)homeworkDataGrid.SelectedItem;
             
                undoStack.Push(() =>
                {
                    DomaciZadatakDataAccess.ponovoDodajZadatak(selectedDomaciZD,profesor);
                    new SuccessWindow("Zadatak je uspješno vraćen!");

                });
               
                if (DomaciZadatakDataAccess.obrisiZadatak(selectedDomaciZD))
                {
                    new SuccessWindow("Zadatak je uspješno obrisan!");
                }
              
            }
        }

     

        private void dgAktivnosti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (homeworkDataGrid.SelectedItem is DomaciZadatak selectedDomaciZD)
            { 
                ZadatakWindow prozor = new ZadatakWindow(selectedDomaciZD, profesor, undoStack);

                if (prozor.ShowDialog() == true)  
                {
                    // Osvježi DataGrid nakon brisanja ili ažuriranja
                    homeworkDataGrid.ItemsSource = null;
                    homeworkDataGrid.ItemsSource = DomaciZadatakDataAccess.pregledDomacegPoProfesoru(profesor);
                }
            }
        }


        private void Password_Click(object sender, RoutedEventArgs e)
        {
            PromjenaLozinkeView prozor = new PromjenaLozinkeView(profesor);
            prozor.ShowDialog();
        }

        private void ShowDetailWindow(object dataContext)
        {
            DetailWindow detailWindow = new DetailWindow
            {
                DataContext = dataContext
            };
            detailWindow.ShowDialog();
            homeworkDataGrid.SelectedItem = null;
        }


        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show(); 
            this.Close();
        }

    }
}
using PrviProjektniZadatakHCI;
using PrviProjektniZadatakHCI.DataAccess;
using PrviProjektniZadatakHCI.Resources;
using PrviProjektniZadatakHCI.View;
using PrviProjektniZadatakHCI.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

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
            TaskDeadline.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1)));
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

            var students = StudentDataAccess.GetStudents();
            var studentViewModels = students.Select(s => new StudentViewModel(s)).ToList();

            cmbStudenti.ItemsSource = studentViewModels;
            cmbStudentsGrade.ItemsSource = studentViewModels;


          //  cmbStudenti.ItemsSource = listaStudenata;
         //   cmbStudentsGrade.ItemsSource = listaStudenata;
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
 
      
        private List<DataGrid> FindAllDataGrids(DependencyObject parent)
        {
            var dataGrids = new List<DataGrid>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is DataGrid dataGrid)
                {
                    dataGrids.Add(dataGrid);
                }
                else
                {
                    dataGrids.AddRange(FindAllDataGrids(child));
                }
            }
            return dataGrids;
        }


        public void RefreshAllDataGrids()
        {
            var allDataGrids = FindAllDataGrids(this); 
            foreach (var dataGrid in allDataGrids)
            {
                dataGrid.InvalidateVisual();
                dataGrid.Items.Refresh(); // Опционо: Освјежи податке
            }
        }

       
        private void ChangeLanguageToEnglish(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).ChangeLanguage("en");
        }

        private void ChangeLanguageToSerbian(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).ChangeLanguage("sr");
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
                var studentModels = PredmetDataAccess.studentiSlusaju(selectedPredmet);

                if (studentModels != null && studentModels.Count > 0)
                {

                    var studentViewModels = studentModels
                        .Select(s => new StudentViewModel(s))
                        .ToList();

                    cmbStudents.ItemsSource = studentViewModels;
                }
                else
                {
                    string message = (string)Application.Current.Resources["NoStudentsForCourse"];
                    new WarningWindow(message).ShowDialog();
                    cmbStudents.ItemsSource = null;
                }
            }
        }

       private void cmbSubject_SelectionChanged_Grade(object sender, SelectionChangedEventArgs e)
           {
               if (cmbStudentsGrade.SelectedItem is StudentViewModel studentVM)
               {
                   if (cmbSubjectsForGrade.SelectedItem is Predmet predmet)
                   {
                       var student = studentVM.StudentModel;
                       ListGrade.ItemsSource = IspitDataAccess.pregledIspita(predmet, student)
                           .Select(ispit => new IspitViewModel(ispit))
                           .ToList();
                   }
                   else
                   {
                    string message = (string)Application.Current.Resources["SelectSubjectText"];
                    new WarningWindow(message).ShowDialog();
                   }
               }
               else
               {
                string message = (string)Application.Current.Resources["SelectStudentText"];
                new WarningWindow(message).ShowDialog();
              
               }
           }

        private void cmbStudent_SelectionChanged_Grade(object sender, SelectionChangedEventArgs e)
        {
            if (cmbStudentsGrade.SelectedItem is StudentViewModel studentVM)
            {
                if (cmbSubjectsForGrade.SelectedItem is Predmet predmet)
                {
                    Student student = studentVM.StudentModel;
                    ListGrade.ItemsSource = IspitDataAccess.pregledIspita(predmet, student);
                }
                else
                {
                    string message = (string)Application.Current.Resources["SelectSubjectText"];
                    new WarningWindow(message).ShowDialog();
                }
            }
            else
            {
                string message = (string)Application.Current.Resources["SelectStudentText"];
                new WarningWindow(message).ShowDialog();
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
                    string message = (string)Application.Current.Resources["NoStudentsForCourse"];
                    new WarningWindow(message).ShowDialog();            
                    AttendanceDataGrid.ItemsSource = null;
                }
            }
        }
        private void cmbSubject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (cmbStudenti.SelectedItem is StudentViewModel selectedStudentVM && cmbSubjects.SelectedItem is Predmet selectedPredmet)
            {
                Attendance.ItemsSource = PrisustvoDataAccess.PregledPrisustva(selectedStudentVM.StudentModel, selectedPredmet, profesor);
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
                string message = (string)Application.Current.Resources["UndoMessage"];
                new WarningWindow(message).ShowDialog();
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
                string message = (string)Application.Current.Resources["NoSubjectSelected"];
                new WarningWindow(message).ShowDialog();
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
                return;
            }

            if (ExamDate.SelectedDate is not DateTime datum)
            {
                new WarningWindow("Izaberite datum ispita").ShowDialog(); 
                return;
            }
            if (!double.TryParse(txtBodovi.Text, out double bodovi) || bodovi < 0 || bodovi > 100)
            {
                string message = (string)Application.Current.Resources["PointsValid"];
                new WarningWindow(message).ShowDialog();
                return;
            }
            if (!int.TryParse(txtOcjena.Text, out int ocjena) || ocjena < 5 || ocjena > 10)
            {
                string message = (string)Application.Current.Resources["ValidGrade"];
                new WarningWindow(message).ShowDialog();
                return;
            }

            if (cmbStudents.SelectedItem is not StudentViewModel studentVM)
            {
                new WarningWindow("Izaberite studenta").ShowDialog();
                return;
            }

            try
            {
                Student student = studentVM.StudentModel;
                IspitDataAccess.sacuvajIspit(bodovi, ocjena, datum, predmet, student);
                cmbPredmetOcjena.SelectedIndex = -1;
                ExamDate.SelectedDate = null;
                cmbStudents.SelectedIndex = -1;
                txtOcjena.Clear();
                txtBodovi.Clear();
                string message = (string)Application.Current.Resources["ExamAdd"];
                new SuccessWindow(message).ShowDialog();           
                undoStack.Push(() =>
                {
                    IspitDataAccess.obrisiOcjenu(bodovi, ocjena, datum, predmet, student);
                    string message = SharedResource.SuccessfullyDelete;
                    new SuccessWindow(message).ShowDialog();
                    ListGrade.ItemsSource = IspitDataAccess.pregledIspita(predmet, student)
                        .Select(ispit => new IspitViewModel(ispit))
                        .ToList();
                });
            }
            catch (Exception ex)
            {
                new WrongWindow($"Greška prilikom čuvanja ispita: {ex.Message}").ShowDialog();
            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

            CheckBox checkBox = sender as CheckBox;

        }
        private void AddTask(object sender, RoutedEventArgs e)
        {
            TaskDeadline.DisplayDateStart = DateTime.Now;

            Predmet predmet = cmbPredmeti.SelectedItem as Predmet;

            if (predmet == null)
            {
                string message = (string)Application.Current.Resources["NoSubjectSelected"];
                new WarningWindow(message).ShowDialog();
                return; 
            }
            string predmetNaziv = predmet.Naziv;
            string naziv = ime.Text;
            string opis = taskDescription.Text;
            string sifra = taskCode.Text;
            DateTime? rok = TaskDeadline.SelectedDate;



            if (!rok.HasValue)
            {
                new WarningWindow("Datum roka mora biti odabran.").ShowDialog();
                return;
            }

            DateTime selectedDate = rok.Value.Date;
            DateTime today = DateTime.Today;

            if (selectedDate < today)
            {
                string message = (string)Application.Current.Resources["DateInPast"];
                new WarningWindow(message).ShowDialog();
                return;
            }

            if (string.IsNullOrEmpty(naziv) || string.IsNullOrEmpty(opis) || string.IsNullOrEmpty(sifra) || !rok.HasValue )
            {
                string message = SharedResource.FillField;
                new WarningWindow(message).Show();
                return; 
            }

            ObservableCollection<Student> studenti = PredmetDataAccess.studentiSlusaju(predmet);


            if (DomaciZadatakDataAccess.dodajDZadatak(naziv, opis, (DateTime)rok, sifra, predmet, profesor))
            {
                string message = (string)Application.Current.Resources["TaskAdd"];
                new SuccessWindow(message).ShowDialog();
                cmbPredmeti.SelectedItem = null;
                ime.Text = string.Empty;
                taskDescription.Text = string.Empty;
                taskCode.Text = string.Empty;
                TaskDeadline.SelectedDate = null;
                homeworkDataGrid.ItemsSource = DomaciZadatakDataAccess.pregledDomacegPoProfesoru(profesor);
            }
            else
            {
                string message = (string)Application.Current.Resources["TaskErrorMessage"];
                new WrongWindow(message).ShowDialog();
            }    
            undoStack.Push(() =>
            {
                DomaciZadatakDataAccess.obrisiDZadatak(sifra);
                homeworkDataGrid.ItemsSource = DomaciZadatakDataAccess.pregledDomacegPoProfesoru(profesor);
                string message = (string)Application.Current.Resources["TaskDelete"];
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
                    string message = (string)Application.Current.Resources["SuccessfullyUndo"];
                    new SuccessWindow(message).ShowDialog();

                });
               
                if (DomaciZadatakDataAccess.obrisiZadatak(selectedDomaciZD))
                {
                    string message = (string)Application.Current.Resources["SuccessfullyDelete"];
                    new SuccessWindow(message).ShowDialog();
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

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {    
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show(); 
            this.Close();
        }

    }
}
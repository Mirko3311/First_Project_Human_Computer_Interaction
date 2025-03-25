using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using PrviProjektniZadatakHCI.DataAccess;
using PrviProjektniZadatakHCI.Resources;
using PrviProjektniZadatakHCI.View;
using PrviProjektniZadatakHCI.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
using PrviProjektniZadatakHCI.Resources;
using PrviProjektniZadatakHCI;

namespace ASystem
{

    public partial class AdminWindow : Window
    {
       
        public static RoutedCommand CancelCommand = new RoutedCommand();
        public static RoutedCommand LogoutCommand = new RoutedCommand();
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MySql_hci"].ConnectionString;

        private Stack<Action> undoStack = new Stack<Action>();
        private Stack<Action> redoStack = new Stack<Action>();


        ObservableCollection<Predmet> predmeti = PredmetDataAccess.pregledPredmeta();
        ObservableCollection<Student> studenti = StudentDataAccess.GetStudents();
        ObservableCollection<Profesor> profesori = ProfessorDataAccess.GetProfessors();
        List<Korisnik> korisnici = KorisnikDataAccess.GetKorisnici();
        private Profesor trenutniProfesor = null;
        private Student trenutniStudent = null;
        private Predmet trenutniPredmet = null;
        public AdminWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            this.DataContext = this;
            cmbProfesori.ItemsSource = ProfessorDataAccess.GetProfessors();
            cmbProfessors.ItemsSource = ProfessorDataAccess.GetProfessors();
            cmbPredmeti.ItemsSource = predmeti;
            CommandBinding undoBinding = new CommandBinding(ApplicationCommands.Undo, Undo_Executed);
            this.CommandBindings.Add(undoBinding);

            CommandBinding cancelBinding = new CommandBinding(CancelCommand, Cancel_Click);
            this.CommandBindings.Add(cancelBinding);

            CommandBinding logoutBinding = new CommandBinding(LogoutCommand, LogoutButton_Click);
            this.CommandBindings.Add(logoutBinding);
        }
        private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Cancel_Click(sender, e); 
        }
        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Undo_Click(sender, e);
        }

        private void SubjectChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProfessors.SelectedItem is Profesor selectedProfessor)
            {
                cmbSubjects.ItemsSource = ProfessorDataAccess.profesorPredaje(selectedProfessor);

            }
        }

        private void AddProfessor_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProfessorName.Text) ||
                string.IsNullOrWhiteSpace(txtProfessorSurname.Text) ||
                string.IsNullOrWhiteSpace(email.Text) ||
                string.IsNullOrWhiteSpace(username.Text) ||
                string.IsNullOrWhiteSpace(password.Text) ||
                string.IsNullOrWhiteSpace(titule.Text))
            {

                string message = SharedResource.FillField;
                new WarningWindow(message).ShowDialog();
                return;
            }

            if (!int.TryParse(id.Text.Trim(), out int parsedId))
            {
                string message = SharedResource.IdIntegerValue;
                new WarningWindow(message).ShowDialog();
                return;
            }
            if (korisnici.Any(p => p.idKorisnika == parsedId))
            {
                string message = SharedResource.IdExist;
                new WarningWindow(message).ShowDialog();
                return;
            }

            Profesor profesor = new Profesor(
       idKorisnik: parsedId,
       ime: txtProfessorName.Text,
       prezime: txtProfessorSurname.Text,
       email: email.Text,
       username: username.Text,
       password: password.Text,
       tipKorisnika: "profesor",
       zvanje: titule.Text
   );

            if (ProfessorDataAccess.dodajProfesora(profesor))
            {
                string message = SharedResource.MessageProfAdd;
                cmbProfesori.ItemsSource = ProfessorDataAccess.GetProfessors();
                cmbProfessors.ItemsSource = ProfessorDataAccess.GetProfessors();
                lstDeleteItems.ItemsSource = ProfessorDataAccess.GetProfessors();
                cmbUpdate.ItemsSource = ProfessorDataAccess.GetProfessors();
                new SuccessWindow(message).ShowDialog();
 
            }
            else
            {
               new WarningWindow("Profesor nije uspješno dodat!").ShowDialog();
            }
            undoStack.Push(() =>
            {
                ProfessorDataAccess.obrisiProfesora(profesor);
                cmbProfesori.ItemsSource = ProfessorDataAccess.GetProfessors();
                cmbProfessors.ItemsSource = ProfessorDataAccess.GetProfessors();
                lstDeleteItems.ItemsSource = ProfessorDataAccess.GetProfessors();
                cmbUpdate.ItemsSource = ProfessorDataAccess.GetProfessors();
                string message = SharedResource.SuccessfullyDelete;
                new SuccessWindow(message).ShowDialog();
            });

            redoStack.Push(() =>
            {
                ProfessorDataAccess.dodajProfesora(profesor);

                new SuccessWindow(string.Format(SharedResource.ProfessorReadded, profesor.ime, profesor.prezime)).ShowDialog();
            });
            redoStack.Clear();
            RefreshProfessors();
            txtProfessorName.Text = "";
            txtProfessorSurname.Text = "";
            email.Text = "";
            username.Text = "";
            password.Text = "";
            titule.Text = "";
            id.Text = "";
 

        }

        private void cmbAddChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbAddChoice.SelectedItem is ComboBoxItem selectedItem)
            {
                string choice = selectedItem.Tag?.ToString();
                addProfessorForm.Visibility = Visibility.Collapsed;
                addStudentForm.Visibility = Visibility.Collapsed;
                addSubjectForm.Visibility = Visibility.Collapsed;

                switch (choice)
                {
                    case "Professor":
                        addProfessorForm.Visibility = Visibility.Visible;
                        break;
                    case "Student":
                        addStudentForm.Visibility = Visibility.Visible;
                        break;
                    case "Subject":
                        addSubjectForm.Visibility = Visibility.Visible;
                        break;
                }
            }
        }


        private void RefreshProfessors()
        {
            profesori.Clear(); 
            var refreshedProfessors = ProfessorDataAccess.GetProfessors(); 
            foreach (var professor in refreshedProfessors)
            {
                profesori.Add(professor); 
            }
            lstDeleteItems.ItemsSource = ProfessorDataAccess.GetProfessors();
            cmbProfessors.ItemsSource = ProfessorDataAccess.GetProfessors();
        }

        public void RefreshStudents()
        {
            studenti.Clear();
            lstDeleteItems.ItemsSource = StudentDataAccess.GetStudents();
            var refreshedStudents = StudentDataAccess.GetStudents();         
           foreach (var student in refreshedStudents)
           {
                studenti.Add(student);
           }
        }
        Korisnik korisnik;
        private void Password_Click(object sender, RoutedEventArgs e)
        {
            PromjenaLozinkeView prozor = new PromjenaLozinkeView(korisnik);
            prozor.ShowDialog();
        }

        private object selectedItem = null;

        private void cmbDeleteChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbDeleteChoice.SelectedItem is ComboBoxItem selectedItem)
            {
                string choice = selectedItem.Tag?.ToString();

                lstDeleteItems.ItemsSource = null; 

                DataTemplate itemTemplate = new DataTemplate();

                switch (choice)
                {
                    case "Professor":
                        lstDeleteItems.ItemsSource = ProfessorDataAccess.GetProfessors();
                        itemTemplate = CreateDataTemplate("ime", "prezime", "Zvanje");
                        break;

                    case "Student":
                        lstDeleteItems.ItemsSource = StudentDataAccess.GetStudents();
                        itemTemplate = CreateDataTemplate("ime", "prezime", "BrojIndeksa");
                        break;

                    case "Subject":
                        lstDeleteItems.ItemsSource = PredmetDataAccess.pregledPredmeta(); 
                   
                        itemTemplate = CreateDataTemplate("Naziv");
                        break;
                }

               
                lstDeleteItems.ItemTemplate = itemTemplate;
            }
        }

       
        private void cmbUpdateE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbUpdate.SelectedItem != null)
            {
               
                var selektovaniEntitet = cmbUpdate.SelectedItem;

               
                contentPanel.Content = selektovaniEntitet;
                if (selektovaniEntitet is Profesor profesor)
                {
                    contentPanel.DataContext = profesor;
                    trenutniProfesor = profesor.DeepCopy();
                }
                else if (selektovaniEntitet is Student student)
                {
                    contentPanel.DataContext = student;
                    trenutniStudent = student.DeepCopy();
                }
                else if (selektovaniEntitet is Predmet predmet)
                {
                      contentPanel.DataContext = predmet;
                       trenutniPredmet = predmet.DeepCopy();
                }
            }
        }

        private void CmbUpdateChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (cmbUpdateChoice.SelectedItem is ComboBoxItem selectedItem)
            {
                contentPanel.Content = null;
                cmbUpdate.SelectedItem = null;
                cmbUpdate.ItemsSource = null;

                string choice = selectedItem.Tag?.ToString();
                switch (choice)
                {
                    case "Professor":
                        contentPanel.ContentTemplate = (DataTemplate)Resources["ProfesorTemplate"];
                        cmbUpdate.ItemsSource = ProfessorDataAccess.GetProfessors();

                        break;

                    case "Student":
                        contentPanel.ContentTemplate = (DataTemplate)Resources["StudentTemplate"];
                        cmbUpdate.ItemsSource = StudentDataAccess.GetStudents();
                        break;

                    case "Subject":
                        contentPanel.ContentTemplate = (DataTemplate)Resources["SubjectTemplate"];

                        cmbUpdate.ItemsSource = PredmetDataAccess.pregledPredmeta();
                        break;
                }
            }
        }
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (lstDeleteItems.SelectedItem != null)
            {
                string choice = ((ComboBoxItem)cmbDeleteChoice.SelectedItem).Content.ToString();


                switch (choice)
                {
                    case var _ when choice == SharedResource.ProfessorText:
                        var selectedProfesor = (Profesor)lstDeleteItems.SelectedItem;
                        undoStack.Push(() =>
                        {
                            ProfessorDataAccess.dodajProfesora(selectedProfesor);
                            lstDeleteItems.ItemsSource = ProfessorDataAccess.GetProfessors();
                            cmbProfesori.ItemsSource = ProfessorDataAccess.GetProfessors();
                            cmbUpdate.ItemsSource = ProfessorDataAccess.GetProfessors();
                         
                            cmbProfessors.ItemsSource = ProfessorDataAccess.GetProfessors();
                            RefreshProfessors();
                            new SuccessWindow(string.Format(SharedResource.UndoProfessorRestored, selectedProfesor.ime, selectedProfesor.prezime)).ShowDialog();
                        });
                        redoStack.Push(() =>
                        {
                            ProfessorDataAccess.obrisiProfesora(selectedProfesor);
                           lstDeleteItems.ItemsSource = ProfessorDataAccess.GetProfessors();
                           cmbProfessors.ItemsSource = ProfessorDataAccess.GetProfessors();
                           cmbProfesori.ItemsSource = ProfessorDataAccess.GetProfessors();
                            RefreshProfessors();
                            new SuccessWindow(string.Format(SharedResource.ProfessorDeleted, selectedProfesor.ime, selectedProfesor.prezime)).ShowDialog();
                        });
                        if (ProfessorDataAccess.obrisiProfesora(selectedProfesor))
                        {
                            lstDeleteItems.ItemsSource = ProfessorDataAccess.GetProfessors();
                            cmbUpdate.ItemsSource = ProfessorDataAccess.GetProfessors();
                            cmbProfesori.ItemsSource = ProfessorDataAccess.GetProfessors();
                      
                            cmbProfessors.ItemsSource = ProfessorDataAccess.GetProfessors();
                            new SuccessWindow(SharedResource.SuccessfullyDelete).ShowDialog();
                            RefreshProfessors();
                        }
                        break;

                    case var _ when choice == SharedResource.StudentText:
                        var selectedStudent = (Student)lstDeleteItems.SelectedItem;
                        undoStack.Push(() =>
                        {
                            StudentDataAccess.dodajStudenta(selectedStudent);
                            lstDeleteItems.ItemsSource = StudentDataAccess.GetStudents();
                            cmbUpdate.ItemsSource = StudentDataAccess.GetStudents();
                            RefreshStudents();
                            new SuccessWindow(string.Format(SharedResource.StudentRestored, selectedStudent.ime, selectedStudent.prezime)).ShowDialog();
                        });
                        redoStack.Push(() =>
                        {
                            StudentDataAccess.obrisiStudenta(selectedStudent.idKorisnika);
                            RefreshStudents();
                            lstDeleteItems.ItemsSource = StudentDataAccess.GetStudents();
                            new SuccessWindow(string.Format(SharedResource.StudentDeleted, selectedStudent.ime, selectedStudent.prezime)).ShowDialog();
                        });
                        if (StudentDataAccess.obrisiStudenta(selectedStudent.idKorisnika))
                        {
                            RefreshStudents();
                            cmbUpdate.ItemsSource = StudentDataAccess.GetStudents();
                            new SuccessWindow(string.Format(SharedResource.StudentDeleted, selectedStudent.ime, selectedStudent.prezime)).ShowDialog();
                        }
                        break;

                    case var _ when choice == SharedResource.SubjectText:
                        var selectedPredmet = (Predmet)lstDeleteItems.SelectedItem;
                        undoStack.Push(() =>
                        {
                            PredmetDataAccess.dodajPredmet(selectedPredmet);
                            lstDeleteItems.ItemsSource = PredmetDataAccess.pregledPredmeta();
                            cmbUpdate.ItemsSource = PredmetDataAccess.pregledPredmeta();
                            new SuccessWindow($"{selectedPredmet.Naziv} {SharedResource.SaveButton}.").ShowDialog();
                        });
                        redoStack.Push(() =>
                        {
                            PredmetDataAccess.ObrisiPredmet(selectedPredmet.IdPredmeta);
                            lstDeleteItems.ItemsSource = PredmetDataAccess.pregledPredmeta();
                            cmbUpdate.ItemsSource = PredmetDataAccess.pregledPredmeta();
                            new SuccessWindow($"{selectedPredmet.Naziv} {SharedResource.DeleteText}.").ShowDialog();
                        });
                        if (PredmetDataAccess.ObrisiPredmet(selectedPredmet.IdPredmeta))
                        {
                            predmeti.Remove(selectedPredmet);
                            lstDeleteItems.ItemsSource = PredmetDataAccess.pregledPredmeta();
                            cmbUpdate.ItemsSource = PredmetDataAccess.pregledPredmeta();
                            lstDeleteItems.Items.Refresh();
                        }
                        break;
                }
            }
        }

       private DataTemplate CreateDataTemplate(params string[] propertyNames)
        {
            var template = new DataTemplate();
            var stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
            stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            foreach (var propertyName in propertyNames)
            {
                var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding(propertyName));
                textBlockFactory.SetValue(TextBlock.MarginProperty, new Thickness(5, 0, 0, 0));
                stackPanelFactory.AppendChild(textBlockFactory);
            }
   
            template.VisualTree = stackPanelFactory;

            return template;
        }


        private void lstDeleteItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = lstDeleteItems.SelectedItem;
            btnDelete.IsEnabled = selectedItem != null;
        }


        private void txtStudentName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtStudentName.Text.Length < 3)
            {
                txtStudentName.BorderBrush = Brushes.Red;
            }
            else
            {
                txtStudentName.BorderBrush = Brushes.Green;
            }
        }



        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtStudentName.Text) ||
                string.IsNullOrWhiteSpace(txtStudentSurname.Text) ||
                string.IsNullOrWhiteSpace(emailS.Text) ||
                string.IsNullOrWhiteSpace(usernameS.Text) ||
                string.IsNullOrWhiteSpace(passwordS.Text) ||
                string.IsNullOrWhiteSpace(grade.Text) ||
                string.IsNullOrWhiteSpace(idS.Text) ||
                string.IsNullOrWhiteSpace(index.Text))
            {

                string message = SharedResource.FillField;
                new WarningWindow(message!).ShowDialog();
                //new WarningWindow("Popunite sva polja").ShowDialog();
                return;
            }
    
             if (!int.TryParse(grade.Text.Trim(), out int parseGrade))
            {
                string message = SharedResource.AcademicIntegerValue;
                new WarningWindow(message).ShowDialog();
                // new WarningWindow("Akademska godine mora da bude cjelobrojna vrijednost!").ShowDialog();  
                return;
            }
            if (!int.TryParse(idS.Text.Trim(), out int parseIds))
            {
                string message = SharedResource.IdIntegerValue;
                new WarningWindow(message).ShowDialog();
              //  new WarningWindow("ID studenta mora da bude cjelobrojna vrijednost!").ShowDialog();
                return;
            }
          
            if (korisnici.Any(p => p.idKorisnika == parseIds))
            {
                //new WarningWindow("Identifikator zauzet!").ShowDialog();
                string message = SharedResource.TakenId;
                new WarningWindow(message).ShowDialog();

                return;
            }
            int.TryParse(grade.Text, out parseGrade);
            int.TryParse(idS.Text, out parseIds);

            if (parseIds <= 0 || parseGrade <= 0)
            {
                string message = SharedResource.Invalid_ID_AY;
                new WrongWindow(message).ShowDialog();
              //  new WrongWindow("Nevalidan unos za ID ili godinu studija").ShowDialog();
                return;
            }
            Student student = new Student(
                parseIds,
                txtStudentName.Text,
                txtStudentSurname.Text,
                emailS.Text,
                usernameS.Text,
                passwordS.Text,
                "student",
                index.Text,
                parseGrade
            );
            if (StudentDataAccess.dodajStudenta(student))
            {
                string message = SharedResource.SuccessAddStudent;
                new SuccessWindow(message).ShowDialog();
                //new SuccessWindow("Student je uspješno dodat!").ShowDialog();
                studenti = StudentDataAccess.GetStudents();
               // cmbPredmeti.ItemsSource = StudentDataAccess.GetStudents();
                lstDeleteItems.ItemsSource = StudentDataAccess.GetStudents();
            }
            else
            {
                string message = SharedResource.UnsuccessfulStudent;
                new WrongWindow(message).ShowDialog();
                //  new WrongWindow("Neuspješno dodavanje studenta!").ShowDialog();
            }
                undoStack.Push(() =>
                {
                  StudentDataAccess.obrisiStudenta(student.idKorisnika);
                    new SuccessWindow(string.Format(SharedResource.StudentRemoved_, student.ime, student.prezime)).ShowDialog();

                    /*  new SuccessWindow($"{student.ime} {student.prezime} je uklonjen.").ShowDialog();*/
                });
                redoStack.Clear();


            
            txtStudentName.Clear();
            txtStudentSurname.Clear();
            emailS.Clear();
            usernameS.Clear();
            passwordS.Clear();
            idS.Clear();
            grade.Clear();
            lstStudentSubjects.UnselectAll();
            index.Clear();
        }

        private void LightThemeClick(object sender, RoutedEventArgs e)
        {
            ChangerThemes.ChangeTheme("Light","admin");
        }

        private void DarkThemeClick(object sender, RoutedEventArgs e)
        {
            ChangerThemes.ChangeTheme("Dark","admin");
        }
        private void GreenThemeClick(object sender, RoutedEventArgs e)
        {
            ChangerThemes.ChangeTheme("Green", "admin");
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (undoStack.Count > 0)
            {
                var undoAction = undoStack.Pop();
                undoAction.Invoke();
                redoStack.Push(undoAction);
            }
            else
            {
                string message = SharedResource.UndoMessage;
                new WarningWindow(message).ShowDialog();
                //     new WarningWindow("Nema dostupnih operacija za poništavanje").ShowDialog();
                // Content="{x:Static resources:SharedResource.UpdateButton}"
                //  new SuccessWindow(x: Static resources: SharedResource.UpdateButton).ShowDialog();
                //new SuccessWindow(Resources.UpdateSuccessMessage).ShowDialog();


            }
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (redoStack.Count > 0)
            {
                var redoAction = redoStack.Pop();
                redoAction.Invoke();
                undoStack.Push(redoAction);
            }
            else
            {
                string message = SharedResource.UndoMessage;
                new WarningWindow(message).ShowDialog();
            }
        }

        private void AddSubject_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSubjectName.Text) ||
                string.IsNullOrWhiteSpace(characteristic.Text) ||
                string.IsNullOrWhiteSpace(ects.Text) ||
                string.IsNullOrWhiteSpace(identifikator.Text))
            {
                string message = SharedResource.FillField;
                new WarningWindow(message).ShowDialog();
                return;
            }

            if (!int.TryParse(identifikator.Text.Trim(), out int id))
            {
                string message = SharedResource.IdIntegerValue;
                new WarningWindow(message).ShowDialog();
                return;
            }


            if (!int.TryParse(ects.Text.Trim(), out int ECTS))
            {
                string message = SharedResource.ECTSIntegerValue;
                new WarningWindow(message).ShowDialog();
                return;
            }
    
            if (predmeti.Any(p => p.IdPredmeta == id))
            {
                string message = SharedResource.IdIntegerValue;
                new WarningWindow(message).ShowDialog();
                return; 
            }
            Predmet predmet = new Predmet
            {


                Naziv = txtSubjectName.Text,
                Opis = characteristic.Text,
                ECTS = ECTS,
                IdPredmeta = id
            };

            if(PredmetDataAccess.dodajPredmet(predmet))
            {
                predmeti = PredmetDataAccess.pregledPredmeta();
                cmbPredmeti.ItemsSource = PredmetDataAccess.pregledPredmeta();
                lstDeleteItems.ItemsSource = PredmetDataAccess.pregledPredmeta();
                string message = SharedResource.SuccessfulAddSubject;
                new SuccessWindow(message).ShowDialog();
                txtSubjectName.Clear();
                characteristic.Clear();
                ects.Clear();
                identifikator.Clear();
            }
            else
            {
                string message = SharedResource.ErrorAdding;
                new WarningWindow(message).ShowDialog();
            }
            undoStack.Push(() =>
            {
                PredmetDataAccess.ObrisiPredmet(predmet.IdPredmeta);
                string message = SharedResource.SuccessfullyDelete;
                new SuccessWindow(message).ShowDialog();
            });

            redoStack.Push(() =>
            {
                PredmetDataAccess.dodajPredmet(predmet);
            });
       
            txtSubjectName.Clear();
            characteristic.Clear();
            ects.Clear();
            identifikator.Clear();
        }

        private void UpdateProfessor_Click(object sender, RoutedEventArgs e)
        {
            Profesor selektovaniProfesor = contentPanel.DataContext as Profesor;
            if (selektovaniProfesor != null && trenutniProfesor != null)
            {
             
                undoStack.Push(() =>
                {
                  
                    ProfessorDataAccess.AzurirajProfesora(trenutniProfesor);
                    cmbProfesori.ItemsSource = ProfessorDataAccess.GetProfessors();
                    cmbProfessors.ItemsSource = ProfessorDataAccess.GetProfessors();
                    string message = string.Format(SharedResource.ProfessorReadded, trenutniProfesor.ime,trenutniProfesor.prezime);

                    new SuccessWindow(message).ShowDialog();


                });

                bool success = ProfessorDataAccess.AzurirajProfesora(selektovaniProfesor);
                if (success)
                {
                    string message = SharedResource.ProfUpdateMessage;
                    new SuccessWindow(message).ShowDialog();
                    cmbProfesori.ItemsSource = ProfessorDataAccess.GetProfessors();
                    cmbProfessors.ItemsSource = ProfessorDataAccess.GetProfessors();
                    cmbUpdate.ItemsSource = ProfessorDataAccess.GetProfessors();
                    txtProfessorName.Clear();
                    txtProfessorSurname.Clear();
                    email.Clear();
                    titule.Clear();
                }
                else
                {
                    string message = SharedResource.ErrorUpdate;
                    new WrongWindow(message).ShowDialog();
                }
            }
        }
        private void UpdateStudent_Click(object sender, RoutedEventArgs e)
        {
           
            Student selektovaniStudent = contentPanel.DataContext as Student;
        
            if (selektovaniStudent != null)
            {

                undoStack.Push(() =>
                {

                    StudentDataAccess.AzurirajStudenta(trenutniStudent);
                    string message = SharedResource.SuccessAddStudent;
                    new SuccessWindow(message).ShowDialog();

                });



                if (string.IsNullOrWhiteSpace(selektovaniStudent.ime) ||
                    string.IsNullOrWhiteSpace(selektovaniStudent.prezime) ||
                    string.IsNullOrWhiteSpace(selektovaniStudent.email) ||
                    string.IsNullOrWhiteSpace(selektovaniStudent.BrojIndeksa) ||
                    selektovaniStudent.GodinaStudija <= 0) 
                {
                    string message = SharedResource.FillField;
                    new WarningWindow(message).ShowDialog();             
                    return;
                }

                bool success = StudentDataAccess.AzurirajStudenta(selektovaniStudent);
                if (success)
                {
                    txtStudentName.Clear();
                    txtStudentSurname.Clear();
                    emailS.Clear();            
                    grade.Clear();                  
                    index.Clear();
                    string message = SharedResource.SuccessAddStudent;
                    new SuccessWindow(message).ShowDialog();
                    txtStudentName.Clear();
                    txtStudentSurname.Clear();
                    emailS.Clear();
                    index.Clear();
                    grade.Clear();                                
                }
                else
                {
                    string message = SharedResource.ErrorUpdate;
                    new WrongWindow(message).ShowDialog();
                }
            }
        }

        private void RestartField()
        {

            if (contentPanel.DataContext is Student student)
            {
                student.ime = string.Empty;
                student.prezime = string.Empty;
                student.email = string.Empty;
                student.GodinaStudija = 0;
                student.BrojIndeksa = string.Empty;
            }


            contentPanel.DataContext = null;
            contentPanel.DataContext = new StudentView();
        }

        private void UpdateSubject_Click(object sender, RoutedEventArgs e)
        {
            Predmet selektovaniPredmet = contentPanel.DataContext as Predmet;
            if (selektovaniPredmet != null)
            {

                undoStack.Push(() =>
                {

                    PredmetDataAccess.AzurirajPredmet(trenutniPredmet);
                    string message = SharedResource.SubjectUndo;
                    new SuccessWindow(message).ShowDialog();

                });
                bool success = PredmetDataAccess.AzurirajPredmet(selektovaniPredmet);
                if (success)
                {
                    string message = SharedResource.SubjectSuccessfullyUpdated;
                    new SuccessWindow(message).ShowDialog();
                }
                else
                {
                    string message = SharedResource.ErrorUpdate;
                    new WrongWindow(message).ShowDialog();
                }
            }
        }


        private void InsertProfSub(object sender, RoutedEventArgs e)
        {
        
            var selektovaniProfesor = cmbProfesori.SelectedItem as Profesor; 
            var selektovaniPredmet = cmbPredmeti.SelectedItem as Predmet;
           

            if (selektovaniProfesor != null && selektovaniPredmet != null)
            {
                PredmetDataAccess.profesorPredmet(selektovaniProfesor, selektovaniPredmet);
                string message = SharedResource.SuccessProfSubject;
                new SuccessWindow(message).ShowDialog();
                cmbPredmeti.SelectedIndex = -1;
                cmbProfesori.SelectedIndex = -1;

            }
            else
            {
                string message = SharedResource.ErrorProfSubject;
                new WrongWindow(message).ShowDialog();

            }


            undoStack.Push(() =>
            {
                PredmetDataAccess.razduzi(selektovaniProfesor, selektovaniPredmet);
                string message = SharedResource.UnassignCourse;
                new SuccessWindow(message).ShowDialog();
            });
        }

        private void DeleteProfSub(object sender, RoutedEventArgs e)
        {
          
            var selektovaniProfesor = cmbProfessors.SelectedItem as Profesor;
            var selektovaniPredmet = cmbSubjects.SelectedItem as Predmet;

            if (selektovaniProfesor == null)
            {
                string message = SharedResource.NoSelectedProfesoor;
                new WarningWindow(message).ShowDialog();
            }
            if (selektovaniPredmet == null)
            {
                string message = SharedResource.NoSubjectSelected;
                new WarningWindow(message).ShowDialog();
            }

            if (selektovaniProfesor != null && selektovaniPredmet != null)
            {
                if (PredmetDataAccess.razduzi(selektovaniProfesor, selektovaniPredmet))
                {
                    cmbSubjects.SelectedIndex = -1;
                    cmbProfessors.SelectedIndex = -1;
                    predmeti.Remove(selektovaniPredmet);
                    string message = SharedResource.UnassignMessage;
                    new SuccessWindow(message).ShowDialog();

                }
                else
                {
                    string message = SharedResource.ErrorUnassign;
                    new WrongWindow(message).ShowDialog();

                }
              
            }
            undoStack.Push(() =>
            {
                PredmetDataAccess.profesorPredmet(selektovaniProfesor, selektovaniPredmet);
                string message = SharedResource.AssignMessage;
                new SuccessWindow(message).ShowDialog();
            });
        }


        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {

            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            username.Clear();
            password.Clear();
            id.Clear();
            txtProfessorName.Clear();
            txtProfessorSurname.Clear();
            titule.Clear();
            txtStudentName.Clear();
            txtStudentSurname.Clear();
            txtSubjectName.Clear();
            emailS.Clear();
            usernameS.Clear();
            passwordS.Clear();
            idS.Clear();
            index.Clear();
            grade.Clear();
            txtSubjectName.Clear();
            characteristic.Clear();
            ects.Clear();
            identifikator.Clear();
            email.Clear();
        }
    }
}
   




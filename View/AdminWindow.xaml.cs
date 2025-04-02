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
using PrviProjektniZadatakHCI;
using System.Globalization;
using PrviProjektniZadatakHCI.Util;
using static MaterialDesignThemes.Wpf.Theme;

namespace ASystem
{

    public partial class AdminWindow : Window
    {
       
        public static readonly RoutedCommand CancelCommand = new RoutedCommand();
        public static readonly RoutedCommand LogoutCommand = new RoutedCommand();
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MySql_hci"].ConnectionString;

        private Stack<Action> undoStack = new Stack<Action>();
        ObservableCollection<Predmet> predmeti = PredmetDataAccess.pregledPredmeta();
        ObservableCollection<Student> studenti = StudentDataAccess.GetStudents();
        ObservableCollection<Profesor> profesori = ProfessorDataAccess.GetProfessors();
        List<Korisnik> korisnici = KorisnikDataAccess.GetKorisnici();
        private Profesor? trenutniProfesor = null;
        private Student? trenutniStudent = null;
        private Predmet? trenutniPredmet = null;
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

            cmbUpdate.IsEnabled = false; 
            cmbUpdateChoice.SelectionChanged += CmbUpdateChoice_SelectionChanged;
            cmbUpdate.PreviewMouseDown += CmbUpdate_PreviewMouseDown;

        }
       private void email_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox emailTextBox)
            {
               
                if (string.IsNullOrWhiteSpace(emailTextBox.Text))
                {
                    if (emailTextBox.Name == "email") 
                    {
                        emailError.Visibility = Visibility.Collapsed;
                    }
                    else if (emailTextBox.Name == "emailS") 
                    {
                        emailStudentError.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    bool isValidEmail = IsValidEmail(emailTextBox.Text);

                    if (emailTextBox.Name == "email") 
                    {
                        emailError.Visibility = isValidEmail ? Visibility.Collapsed : Visibility.Visible;
                    }
                    else if (emailTextBox.Name == "emailS") 
                    {
                        emailStudentError.Visibility = isValidEmail ? Visibility.Collapsed : Visibility.Visible;
                    }
                }
            }
        }


        private void id_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox idTextBox)
            {
          
                if (string.IsNullOrWhiteSpace(idTextBox.Text))
                {
                    if (idTextBox.Name == "id")
                    {
                        idProfError.Visibility = Visibility.Collapsed;
                    }
                    else if (idTextBox.Name == "idS") 
                    {
                        idStudentError.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    bool isValidId = int.TryParse(idTextBox.Text, out _);

                    if (idTextBox.Name == "id") 
                    {
                        idProfError.Visibility = isValidId ? Visibility.Collapsed : Visibility.Visible;
                    }
                    else if (idTextBox.Name == "idS") 
                    {
                        idStudentError.Visibility = isValidId ? Visibility.Collapsed : Visibility.Visible;
                    }
                }
            }
        }


        private bool isStudentPasswordVisible = false;

        private void ToggleSButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isStudentPasswordVisible)
            {
                textSBox.Text = passwordSBox.Password;
                textSBox.Visibility = Visibility.Visible;
                passwordSBox.Visibility = Visibility.Collapsed;
                toggleSIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.EyeOff;
                isStudentPasswordVisible = true;
            }
            else
            {
                passwordSBox.Password = textSBox.Text;
                passwordSBox.Visibility = Visibility.Visible;
                textSBox.Visibility = Visibility.Collapsed;
                toggleSIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Eye;
                isStudentPasswordVisible = false;
            }
        }

        private void PasswordSBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Opcionalno: ažurirajte textSBox da ostane sinhronizovan, ako je potrebno
            if (!isStudentPasswordVisible)
            {
                textSBox.Text = passwordSBox.Password;
            }
        }

        private void TextSBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Opcionalno: ažurirajte passwordSBox da ostane sinhronizovan, ako je potrebno
            if (isStudentPasswordVisible)
            {
                passwordSBox.Password = textSBox.Text;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@]+@[^@]+\.[^@]+$");
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }


        private bool isPasswordVisible = false;

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                // Prikazujemo lozinku u TextBox-u
                textBox.Text = passwordBox.Password;
                textBox.Visibility = Visibility.Visible;
                passwordBox.Visibility = Visibility.Collapsed;
                toggleButton.Content = "🔒"; 
            }
            else
            {
  
                passwordBox.Password = textBox.Text;
                passwordBox.Visibility = Visibility.Visible;
                textBox.Visibility = Visibility.Collapsed;
                toggleButton.Content = "👁"; 
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!isPasswordVisible)
            {
                textBox.Text = passwordBox.Password;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isPasswordVisible)
            {
                passwordBox.Password = textBox.Text;
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
                var subjects = ProfessorDataAccess.profesorPredaje(selectedProfessor);
               cmbSubjects.ItemsSource = subjects;
                cmbSubjects.IsEnabled = (subjects?.Count > 0);
                if (!cmbSubjects.IsEnabled)
                {
                    string message = (string)Application.Current.Resources["NoDataProfSubject"];
                    new WarningWindow(message).ShowDialog();

                }
            }
        }

        private void AddProfessor_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProfessorName.Text) ||
                string.IsNullOrWhiteSpace(txtProfessorSurname.Text) ||
                string.IsNullOrWhiteSpace(email.Text) ||
                string.IsNullOrWhiteSpace(username.Text) ||
                string.IsNullOrWhiteSpace(textBox.Text) ||
                string.IsNullOrWhiteSpace(titule.Text))
            {
                string message = (string)Application.Current.Resources["FillField"];
                new WarningWindow(message).ShowDialog();      
                return;
            }


            if (!IsValidEmail(email.Text))
            {
                
                string message = (string)Application.Current.Resources["InvalidEmailFormat"];
                new WarningWindow(message).ShowDialog();
                return;
            }
            else
            {
                emailError.Visibility = Visibility.Collapsed; 
            }
            if (!int.TryParse(id.Text.Trim(), out int parsedId))
            {
                string message = (string)Application.Current.Resources["IdIntegerValue"];
                new WarningWindow(message).ShowDialog();
                return;
            }
            if (korisnici.Any(p => p.idKorisnika == parsedId))
            {
               
                string message = (string)Application.Current.Resources["IdExist"];
                new WarningWindow(message).ShowDialog(); 
                return;
            }
            if (korisnici.Any(p => p.username.Equals(username.Text, StringComparison.OrdinalIgnoreCase)))
            {
                string message = (string)Application.Current.Resources["UsernameExist"];
                new WarningWindow(message).ShowDialog();
                return;
            }
            Profesor profesor = new Profesor(
       idKorisnik: parsedId,
       ime: txtProfessorName.Text,
       prezime: txtProfessorSurname.Text,
       email: email.Text,
       username: username.Text,
       password: textBox.Text,
       tipKorisnika: "profesor",
       zvanje: titule.Text
   );

            if (ProfessorDataAccess.dodajProfesora(profesor))
            {
                cmbProfesori.ItemsSource = ProfessorDataAccess.GetProfessors();
                cmbProfessors.ItemsSource = ProfessorDataAccess.GetProfessors();
                lstDeleteItems.ItemsSource = ProfessorDataAccess.GetProfessors();
                cmbUpdate.ItemsSource = ProfessorDataAccess.GetProfessors();
                string message = (string)Application.Current.Resources["MessageProfAdd"];
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
                string message = (string)Application.Current.Resources["SuccessfullyDelete"];
                new SuccessWindow(message).ShowDialog();
            });

          
            RefreshProfessors();
            txtProfessorName.Text = "";
            txtProfessorSurname.Text = "";
            email.Text = "";
            username.Text = "";
            textBox.Text = "";
            titule.Text = "";
            id.Text = "";
            passwordBox.Clear(); 
            textBox.Clear(); 



        }

        private void cmbChoice_Loaded(object sender, RoutedEventArgs e)
        {
            cmbAddChoice.SelectedIndex = 0;
        }
        private void cmbUpdateChoice_Loaded(object sender, RoutedEventArgs e)
        {
            cmbUpdateChoice.SelectedIndex = 0;
            cmbUpdate.ItemsSource = ProfessorDataAccess.GetProfessors();
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
                    cmbUpdate.ItemsSource = null;
                    cmbUpdate.IsEnabled = false;
                string choice = selectedItem.Tag?.ToString();
                if (choice == null)
                    return;
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
                cmbUpdate.IsEnabled = true;
            }
        }
        private void CmbUpdate_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!cmbUpdateChoice.IsEnabled)
            {
                MessageBox.Show("Molimo vas da prvo selektujete stavku u prvom ComboBox-u.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Handled = true; // Sprečava otvaranje dropdown-a drugog ComboBox-a
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (lstDeleteItems.SelectedItem != null)
            {
                string choice = ((ComboBoxItem)cmbDeleteChoice.SelectedItem).Content.ToString();


                switch (choice)
                {
                       
                    case var _ when choice == (string)Application.Current.Resources["ProfessorText"]:
                        var selectedProfesor = (Profesor)lstDeleteItems.SelectedItem;
                        undoStack.Push(() =>
                        {
                            ProfessorDataAccess.dodajProfesora(selectedProfesor);
                            lstDeleteItems.ItemsSource = ProfessorDataAccess.GetProfessors();
                            cmbProfesori.ItemsSource = ProfessorDataAccess.GetProfessors();
                            cmbUpdate.ItemsSource = ProfessorDataAccess.GetProfessors();
                            cmbProfessors.ItemsSource = ProfessorDataAccess.GetProfessors();
                            RefreshProfessors();
                            string message = (string)Application.Current.Resources["SuccessfullyUndo"];                        
                            new SuccessWindow(message).ShowDialog();
                         
                        });
                      
                        if (ProfessorDataAccess.obrisiProfesora(selectedProfesor))
                        {
                            lstDeleteItems.ItemsSource = ProfessorDataAccess.GetProfessors();
                            cmbUpdate.ItemsSource = ProfessorDataAccess.GetProfessors();
                            cmbProfesori.ItemsSource = ProfessorDataAccess.GetProfessors();
                            cmbProfessors.ItemsSource = ProfessorDataAccess.GetProfessors();
                            string message = (string)Application.Current.Resources["SuccessfullyDelete"];
                            new SuccessWindow(message).ShowDialog();
                            RefreshProfessors();
                        }
                        break;

                    case var _ when choice == (string)Application.Current.Resources["StudentText"]:
                        var selectedStudent = (Student)lstDeleteItems.SelectedItem;
                        undoStack.Push(() =>
                        {
                            StudentDataAccess.dodajStudenta(selectedStudent);
                            lstDeleteItems.ItemsSource = StudentDataAccess.GetStudents();
                            cmbUpdate.ItemsSource = StudentDataAccess.GetStudents();
                            RefreshStudents();
                            string message = (string)Application.Current.Resources["SuccessfullyUndo"];
                            new SuccessWindow(message).ShowDialog();
                        });
                     
                        if (StudentDataAccess.obrisiStudenta(selectedStudent.idKorisnika))
                        {
                            RefreshStudents();
                            cmbUpdate.ItemsSource = StudentDataAccess.GetStudents();
                            string message = (string)Application.Current.Resources["SuccessfullyDelete"];
                            new SuccessWindow(message).ShowDialog();


                        }
                        break;

                    case var _ when choice == (string)Application.Current.Resources["SubjectText"]:
                        var selectedPredmet = (Predmet)lstDeleteItems.SelectedItem;
                        undoStack.Push(() =>
                        {
                            PredmetDataAccess.dodajPredmet(selectedPredmet);
                            lstDeleteItems.ItemsSource = PredmetDataAccess.pregledPredmeta();
                            cmbUpdate.ItemsSource = PredmetDataAccess.pregledPredmeta();
                            string message = (string)Application.Current.Resources["SuccessfullyUndo"];
                            new SuccessWindow(message).ShowDialog();
                        });
                        if (PredmetDataAccess.ObrisiPredmet(selectedPredmet.IdPredmeta))
                        {
                            predmeti.Remove(selectedPredmet);
                            lstDeleteItems.ItemsSource = PredmetDataAccess.pregledPredmeta();
                            cmbUpdate.ItemsSource = PredmetDataAccess.pregledPredmeta();
                            lstDeleteItems.Items.Refresh();
                            string message = (string)Application.Current.Resources["SuccessfullyDelete"];
                            new SuccessWindow(message).ShowDialog();
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
                string.IsNullOrWhiteSpace(passwordSBox.Visibility == Visibility.Visible ? passwordSBox.Password : textSBox.Text) ||
                string.IsNullOrWhiteSpace(grade.Text) ||
                string.IsNullOrWhiteSpace(idS.Text) ||
                string.IsNullOrWhiteSpace(index.Text))
            {
                string message = (string)Application.Current.Resources["FillField"];
                new SuccessWindow(message).ShowDialog();
                return;
            }
            if (!IsValidEmail(emailS.Text))
            {

                string message = (string)Application.Current.Resources["InvalidEmailFormat"];
                new WarningWindow(message).ShowDialog();
                return;
            }
            else
            {
                emailError.Visibility = Visibility.Collapsed; 
            }

            if (!int.TryParse(grade.Text.Trim(), out int parseGrade))
            {         
                string message = (string)Application.Current.Resources["AcademicIntegerValue"];
                new WarningWindow(message).ShowDialog();
                return;
            }
            if (!int.TryParse(idS.Text.Trim(), out int parseIds))
            {
                string message = (string)Application.Current.Resources["IdIntegerValue"];
                new WarningWindow(message).ShowDialog();
                return;
            }
          
            if (korisnici.Any(p => p.idKorisnika == parseIds))
            {
                string message = (string)Application.Current.Resources["TakenId"];
                new WarningWindow(message).ShowDialog();
                return;
            }
            int.TryParse(grade.Text, out parseGrade);
            if (parseGrade <= 0 || parseGrade >= 7)
            {
                string message = (string)Application.Current.Resources["AcademicYear"];
                new WarningWindow(message).ShowDialog();
                return;
            }

            int.TryParse(idS.Text, out parseIds);

            if (parseIds <= 0 || parseGrade <= 0)
            {
                string message = (string)Application.Current.Resources["Invalid_ID_AY"];
                new WrongWindow(message).ShowDialog();
                return;
            }
            string passwordValue = passwordSBox.Visibility == Visibility.Visible ? passwordSBox.Password : textSBox.Text;
            Student student = new Student(
                parseIds,
                txtStudentName.Text,
                txtStudentSurname.Text,
                emailS.Text,
                usernameS.Text,
                passwordValue,
                "student",
                index.Text,
                parseGrade
            );
            if (StudentDataAccess.dodajStudenta(student))
            {
      
                string message = (string)Application.Current.Resources["SuccessAddStudent"];
                new SuccessWindow(message).ShowDialog();
                studenti = StudentDataAccess.GetStudents();
                lstDeleteItems.ItemsSource = StudentDataAccess.GetStudents();
            }
            else
            {
                string message = (string)Application.Current.Resources["UnsuccessfulStudent"];
                new WrongWindow(message).ShowDialog();
            }
                undoStack.Push(() =>
                {
                    StudentDataAccess.obrisiStudenta(student.idKorisnika);
                    lstDeleteItems.ItemsSource = StudentDataAccess.GetStudents(); 
                    string message = (string)Application.Current.Resources["StudentDeleted"];
                    new SuccessWindow(message).ShowDialog();
                });
                       
            txtStudentName.Clear();
            txtStudentSurname.Clear();
            emailS.Clear();
            usernameS.Clear();
            passwordSBox.Clear();
            textSBox.Clear();
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
             
            }
            else
            {
                string message = (string)Application.Current.Resources["UndoMessage"];
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

                string message = (string)Application.Current.Resources["FillField"];
                new WarningWindow(message).ShowDialog();
                return;
            }

            if (!int.TryParse(identifikator.Text.Trim(), out int id))
            {          
                string message = (string)Application.Current.Resources["IdIntegerValue"];
                new WarningWindow(message).ShowDialog();
                return;
            }


            if (!int.TryParse(ects.Text.Trim(), out int ECTS))
            {
                string message = (string)Application.Current.Resources["ECTSIntegerValue"];
                new WarningWindow(message).ShowDialog();
                return;
            }
            else if (ECTS <= 0 || ECTS >= 9)
            {
                string message = (string)Application.Current.Resources["ECTSRange"];
                new WarningWindow(message).ShowDialog();
                return;
            }
            if (predmeti.Any(p => p.IdPredmeta == id))
            {
                string message = (string)Application.Current.Resources["IdIntegerValue"];
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
                string message = (string)Application.Current.Resources["SuccessfulAddSubject"];
                new SuccessWindow(message).ShowDialog();
                txtSubjectName.Clear();
                characteristic.Clear();
                ects.Clear();
                identifikator.Clear();
            }
            else
            {
                string message = (string)Application.Current.Resources["ErrorAdding"];
                new SuccessWindow(message).ShowDialog();
            }
            undoStack.Push(() =>
            {
                PredmetDataAccess.ObrisiPredmet(predmet.IdPredmeta);
                string message = (string)Application.Current.Resources["SuccessfullyDelete"];
                new SuccessWindow(message).ShowDialog();
            });
       
            txtSubjectName.Clear();
            characteristic.Clear();
            ects.Clear();
            identifikator.Clear();
        }

        private void UpdateProfessor_Click(object sender, RoutedEventArgs e)
        {
            Profesor selektovaniProfesor = contentPanel.DataContext as Profesor;

           
            if (!IsValidEmail(selektovaniProfesor.email))
            {
                string message = (string)Application.Current.Resources["InvalidEmailFormat"];
                new WarningWindow(message).ShowDialog();
                return;
            }

            if (selektovaniProfesor != null && trenutniProfesor != null )
            {
                undoStack.Push(() =>
                {
                  
                    ProfessorDataAccess.AzurirajProfesora(trenutniProfesor);
                    cmbProfesori.ItemsSource = ProfessorDataAccess.GetProfessors();
                    cmbProfessors.ItemsSource = ProfessorDataAccess.GetProfessors();
                    string message = string.Format((string)Application.Current.Resources["InvalidEmailFormat"],trenutniProfesor.ime, trenutniProfesor.prezime);
                    new SuccessWindow(message).ShowDialog();

                });
                bool success = ProfessorDataAccess.AzurirajProfesora(selektovaniProfesor);
                if (success)
                {
                    string message = (string)Application.Current.Resources["ProfUpdateMessage"];
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
                    string message = (string)Application.Current.Resources["ErrorUpdate"];
                    new WrongWindow(message).ShowDialog();
                }
            }
        }
      
        private void UpdateStudent_Click(object sender, RoutedEventArgs e)
        {     
            Student selektovaniStudent = contentPanel.DataContext as Student;
        
            if (selektovaniStudent != null)
            {
                var yearValidation = new AcademicYearValidationRule { MinValue = 1, MaxValue = 5 };
                var yearResult = yearValidation.Validate(selektovaniStudent.GodinaStudija, CultureInfo.CurrentCulture);
                if (!yearResult.IsValid)
                {
                    string message = (string)Application.Current.Resources["AcademicYearRange"];
                    new WarningWindow(message).ShowDialog();
                    return;
                }
                if (!IsValidEmail(selektovaniStudent.email))
                {
                    string message = (string)Application.Current.Resources["InvalidEmailFormat"];
                    new WarningWindow(message).ShowDialog();
                    return;
                }
                undoStack.Push(() =>
                {
                    StudentDataAccess.AzurirajStudenta(trenutniStudent);
                    string message = (string)Application.Current.Resources["SuccessAddStudent"];
                    new SuccessWindow(message).ShowDialog();

                });
                if (string.IsNullOrWhiteSpace(selektovaniStudent.ime) ||
                    string.IsNullOrWhiteSpace(selektovaniStudent.prezime) ||
                    string.IsNullOrWhiteSpace(selektovaniStudent.email) ||
                    string.IsNullOrWhiteSpace(selektovaniStudent.BrojIndeksa) ||
                    selektovaniStudent.GodinaStudija <= 0) 
                {
                    string message = (string)Application.Current.Resources["FillField"];
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
                    string message = (string)Application.Current.Resources["UpdateStudent"];
                    new SuccessWindow(message).ShowDialog();
                    txtStudentName.Clear();
                    txtStudentSurname.Clear();
                    emailS.Clear();
                    index.Clear();
                    grade.Clear();                                
                }
                else
                {
                    string message = (string)Application.Current.Resources["ErrorUpdate"];
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

                if (selektovaniPredmet.ECTS < 1 || selektovaniPredmet.ECTS > 8)
                {
                    string message = (string)Application.Current.Resources["ECTSRange"];
                    new WarningWindow(message).Show();
                    return;
                }
                undoStack.Push(() =>
                {

                    PredmetDataAccess.AzurirajPredmet(trenutniPredmet);

                    string message = (string)Application.Current.Resources["SubjectUndo"];
                    new SuccessWindow(message).ShowDialog();

                });
                bool success = PredmetDataAccess.AzurirajPredmet(selektovaniPredmet);
                if (success)
                {
                    string message = (string)Application.Current.Resources["SubjectSuccessfullyUpdated"];
                    new SuccessWindow(message).ShowDialog();
                }
                else
                {
                    string message = (string)Application.Current.Resources["ErrorUpdate"];
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
                if (PredmetDataAccess.profesorPredmet(selektovaniProfesor, selektovaniPredmet))
                {
                    string message = (string)Application.Current.Resources["SuccessProfSubject"];
                    new SuccessWindow(message).ShowDialog();
                    cmbPredmeti.SelectedIndex = -1;
                    cmbProfesori.SelectedIndex = -1;
                }      
            }
            else
            {
                string message = (string)Application.Current.Resources["ErrorProfSubject"];
                new SuccessWindow(message).ShowDialog();
            }


            undoStack.Push(() =>
            {
                PredmetDataAccess.razduzi(selektovaniProfesor, selektovaniPredmet);
                string message = (string)Application.Current.Resources["UnassignCourse"];
                new SuccessWindow(message).ShowDialog();
            });
        }

        private void DeleteProfSub(object sender, RoutedEventArgs e)
        {
          
            var selektovaniProfesor = cmbProfessors.SelectedItem as Profesor;
            var selektovaniPredmet = cmbSubjects.SelectedItem as Predmet;

            if (selektovaniProfesor == null)
            {

                string message = (string)Application.Current.Resources["NoSelectedProfesor"];
                new WarningWindow(message).ShowDialog();
            }
            if (selektovaniPredmet == null)
            {
                string message = (string)Application.Current.Resources["NoSubjectSelected"];
                new WarningWindow(message).ShowDialog();
            }

            if (selektovaniProfesor != null && selektovaniPredmet != null)
            {
                if (PredmetDataAccess.razduzi(selektovaniProfesor, selektovaniPredmet))
                {
                    cmbSubjects.SelectedIndex = -1;
                    cmbProfessors.SelectedIndex = -1;
                    predmeti.Remove(selektovaniPredmet);
                    string message = (string)Application.Current.Resources["UnassignMessage"];
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
            textBox.Clear();
            id.Clear();
            txtProfessorName.Clear();
            txtProfessorSurname.Clear();
            titule.Clear();
            txtStudentName.Clear();
            txtStudentSurname.Clear();
            txtSubjectName.Clear();
            emailS.Clear();
            usernameS.Clear();
            passwordSBox.Clear();
            textSBox.Clear();
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
   




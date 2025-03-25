using PrviProjektniZadatakHCI;
using PrviProjektniZadatakHCI.DataAccess;
using PrviProjektniZadatakHCI.Resources;
using PrviProjektniZadatakHCI.View;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ASystem
{

    public partial class MainWindow : Window
        {



            public MainWindow()
            {
                try
                {
                    InitializeComponent();
      
                this.WindowState = WindowState.Maximized;
            }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error during MainWindow initialization: {ex.Message}", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw; 
                }
            }

        private void ChangeLanguageToEnglish(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).SetLanguage("en");
            MainWindow newWindow = new MainWindow();
            newWindow.Show();
            this.Close();
        }

        private void ChangeLanguageToSerbian(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).SetLanguage("sr");
            MainWindow newWindow = new MainWindow();
            newWindow.Show();
            this.Close();
        }


        private void btnLogin_Click(object sender, RoutedEventArgs e)
            {
                try
                {
                    var userType = KorisnikDataAccess.VerifyUser(txtUsername.Text, txtPassword.Password);

                if (userType == "student")
                {
                    Student trenutniStudent = StudentDataAccess.GetStudent(txtUsername.Text);
                    if (trenutniStudent != null)
                    {
                      
                        StudentWindow studentWindow = new StudentWindow(trenutniStudent);
                        studentWindow.Show();
                        ChangerThemes.ApplyUserTheme(txtUsername.Text);
                        this.Close(); 
                    }
                }
                else if (userType == "profesor")
                {
                    Profesor trenutniProfesor = ProfessorDataAccess.GetProfesor(txtUsername.Text);
                    


                    if (trenutniProfesor != null)
                    {
                        Console.WriteLine(trenutniProfesor.ime);
                        ProfessorWindow profesorWindow = new ProfessorWindow(trenutniProfesor);
                        ChangerThemes.ApplyUserTheme(txtUsername.Text);
                        profesorWindow.Show();
                     
                        this.Close(); 
                    }
                }

                else if (txtUsername.Text == "admin" && txtPassword.Password == "admin")
                {

                    AdminWindow adminWindow = new AdminWindow();
                    adminWindow.Show();
                    ChangerThemes.ApplyUserTheme(txtUsername.Text);
                        this.Close(); 
                    
                }
                else
                {
                    string message = SharedResource.UsernamePasswordInCorrect;
                    new WrongWindow(message).ShowDialog();
                
                }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Greška pri logovanju: " + ex.Message);
                    System.Windows.MessageBox.Show("Greška pri logovanju: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


            
            private void Border_MouseDown(object sender, MouseButtonEventArgs e)
            {
                DragMove();
            }

            private void btnExit_Click(object sender, RoutedEventArgs e)
            {
                System.Windows.Application.Current.Shutdown();
            }



            private void LoginButton_Click(object sender, RoutedEventArgs e)
            {


            }
        }

    }


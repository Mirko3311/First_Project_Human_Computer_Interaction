using ASystem;
using MySql.Data.MySqlClient;
using PrviProjektniZadatakHCI.DataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PrviProjektniZadatakHCI
{

    public class ChangerThemes
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MySql_hci"].ConnectionString;
        public static void ApplyUserTheme(string username)
        {
            string theme = KorisnikDataAccess.GetTheme(username);


            if (theme == "Light")
            {
                App.ChangeTheme(new Uri("Themes/BlueTheme.xaml", UriKind.Relative));
            }
            else if (theme == "Dark")
            {
                App.ChangeTheme(new Uri("Themes/RedTheme.xaml", UriKind.Relative));
            }
            else if (theme == "Green")
            {
                App.ChangeTheme(new Uri("Themes/GreenTheme.xaml", UriKind.Relative));
            }
        }


        public static void ChangeTheme(string theme, string username)
        {
            try
            {
                if (theme == "Light")
                {
                    App.ChangeTheme(new Uri("Themes/BlueTheme.xaml", UriKind.Relative));
                }
                else if (theme == "Dark")
                {
                    App.ChangeTheme(new Uri("Themes/RedTheme.xaml", UriKind.Relative));
                }
                else if (theme == "Green")
                {

                    App.ChangeTheme(new Uri("Themes/GreenTheme.xaml", UriKind.Relative));
                }


                string query = "UPDATE Korisnik SET Tema = @tema WHERE Username =@parametar";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@tema", theme);
                        command.Parameters.AddWithValue("@parametar", username);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Greška prilikom promene teme: " + ex.Message);
            }
        }
    }
}

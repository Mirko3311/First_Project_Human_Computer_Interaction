using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASystem;
using MySql.Data.MySqlClient;
namespace PrviProjektniZadatakHCI.DataAccess
{
    class ProfessorDataAccess
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MySql_hci"].ConnectionString;

        public static bool dodajProfesora(Profesor profesor)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand commandKorisnik = conn.CreateCommand();
                commandKorisnik.CommandText = @"
            INSERT INTO Korisnik (Ime, Prezime, Email, Username, Password, Tip_Korisnika,idKorisnik) 
            VALUES (@Ime, @Prezime, @Email, @Username, @Password, @TipKorisnika,@IdKorisnika);";

                commandKorisnik.Parameters.AddWithValue("@Ime", profesor.ime);
                commandKorisnik.Parameters.AddWithValue("@Prezime", profesor.prezime);
                commandKorisnik.Parameters.AddWithValue("@Email", profesor.email);
                commandKorisnik.Parameters.AddWithValue("@Username", profesor.username);
                commandKorisnik.Parameters.AddWithValue("@Password", profesor.password);
                commandKorisnik.Parameters.AddWithValue("@TipKorisnika", "profesor");
                commandKorisnik.Parameters.AddWithValue("@IdKorisnika", profesor.idKorisnika);
                int korisnikId = Convert.ToInt32(commandKorisnik.ExecuteScalar());
                MySqlCommand commandStudent = conn.CreateCommand();
                commandStudent.CommandText = @"
                INSERT INTO Profesor (Korisnik_idKorisnik, Zvanje)
                  VALUES (@KorisnikId, @Zvanje)";

                commandStudent.Parameters.AddWithValue("@KorisnikId", profesor.idKorisnika);
                commandStudent.Parameters.AddWithValue("@Zvanje", profesor.Zvanje);

                int rowsAffected = commandStudent.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Došlo je do greške: " + ex.Message);
                return false;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public static int GetProfesorId(Profesor profesor)
        {

            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();

                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "SELECT idKorisnik FROM Korisnik WHERE username = @Email AND Tip_Korisnika = 'profesor'";
                command.Parameters.AddWithValue("@Email", profesor.username);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Došlo je do greške: " + ex.Message);
                return -1;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        public static bool AzurirajProfesora(Profesor profesor)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                using (MySqlCommand command = conn.CreateCommand())
                {
                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            command.Transaction = transaction;
                            command.CommandText = @"
                        UPDATE Korisnik 
                        SET Ime = @Ime, Prezime = @Prezime, Username= @Username, Email = @Email, Tip_korisnika = @Tip 
                        WHERE idKorisnik = @ProfesorId";
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@Ime", profesor.ime);
                            command.Parameters.AddWithValue("@Username", profesor.username);
                            command.Parameters.AddWithValue("@Prezime", profesor.prezime);
                            command.Parameters.AddWithValue("@Email", profesor.email);
                            command.Parameters.AddWithValue("@Tip", profesor.tipKorisnika);
                            command.Parameters.AddWithValue("@ProfesorId", profesor.idKorisnika);

                            command.ExecuteNonQuery();
                            command.CommandText = @"
                        UPDATE Profesor 
                        SET Zvanje = @Zvanje 
                        WHERE Korisnik_idKorisnik = @ProfesorId";
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@Zvanje", profesor.Zvanje);

                            command.Parameters.AddWithValue("@ProfesorId", profesor.idKorisnika);

                            command.ExecuteNonQuery();
                            transaction.Commit();
                            return true;

                        }
                        catch (Exception ex)
                        {

                            transaction.Rollback();
                            Console.WriteLine("Došlo je do greške prilikom ažuriranja profesora: " + ex.Message);
                            return false;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Došlo je do greške pri povezivanju sa bazom: " + ex.Message);
                return false;
            }
            finally
            {

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public static ObservableCollection<Predmet> profesorPredaje(Profesor profesor)
        {
            ObservableCollection<Predmet> predmeti = new ObservableCollection<Predmet>();
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();

                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "select idPredmeta, Naziv, Opis, ECTS from Predmet p inner join Predmet_Profesor pp on p.idPredmeta = pp.Predmet_idPredmeta where Profesor_Korisnik_idKorisnik = @identifikator";
                command.Parameters.AddWithValue("@identifikator", profesor.idKorisnika);

                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Predmet predmet = new Predmet
                    {
                        IdPredmeta = reader.GetInt32("idPredmeta"),
                        Naziv = reader.GetString("Naziv"),
                        Opis = reader.GetString("Opis"),
                        ECTS = reader.GetInt32("ECTS")
                    };
                    predmeti.Add(predmet);
                }
                reader.Close();
            }
            catch (Exception ex)
            {

                Console.WriteLine("Greška: " + ex.Message);
            }
            finally
            {

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return predmeti;
        }


        public static bool obrisiProfesora(Profesor profesor)
        {

            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                try
                {

                    command.CommandText = "DELETE FROM DomaciZadatak WHERE Profesor_Korisnik_idKorisnik = @ProfesorId";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@ProfesorId", GetProfesorId(profesor));
                    command.ExecuteNonQuery();



                    command.CommandText = "DELETE FROM Predmet_Profesor WHERE Profesor_Korisnik_idKorisnik = @ProfesorId";
                    command.ExecuteNonQuery();


                    command.CommandText = "DELETE FROM Profesor WHERE Korisnik_idKorisnik = @ProfesorId";
                    command.ExecuteNonQuery();

                    command.CommandText = "DELETE FROM Korisnik WHERE idKorisnik = @ProfesorId";
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Došlo je do greške: " + ex.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Došlo je do greške pri povezivanju sa bazom: " + ex.Message);
                return false;
            }
            finally
            {

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public static Profesor GetProfesor(string username)
        {
            Profesor profesor = null;
            MySqlConnection conn = new MySqlConnection(connectionString);

            try
            {
                conn.Open();
                MySqlCommand commandKorisnik = conn.CreateCommand();
                commandKorisnik.CommandText = @"
            SELECT k.idKorisnik, k.Ime, k.Prezime, k.Username, k.Email, k.Tip_korisnika, k.Password,p.Zvanje
            FROM korisnik k
            INNER JOIN profesor p ON p.Korisnik_idKorisnik = k.idKorisnik
            WHERE k.Username = @Username";
                commandKorisnik.Parameters.AddWithValue("@Username", username);

                using (MySqlDataReader reader = commandKorisnik.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int idKorisnik = reader["idKorisnik"] != DBNull.Value ? Convert.ToInt32(reader["idKorisnik"]) : 0;
                        profesor = new Profesor(
                    idKorisnik,
                    reader["Ime"].ToString(),
                    reader["Prezime"].ToString(),
                    reader["Email"].ToString(),
                    reader["Username"].ToString(),
                    reader["Password"].ToString(),
                    reader["Tip_korisnika"].ToString(),
                    reader["Zvanje"].ToString()
                );
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("MySQL greška: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return profesor;
        }


        public static ObservableCollection<Profesor> GetProfessors()
        {
            ObservableCollection<Profesor> profesori = new ObservableCollection<Profesor>();
            string query = @"
        SELECT k.idKorisnik, k.Ime, k.Prezime, k.Username, k.Email, k.Tip_korisnika, k.Password, p.Zvanje
        FROM korisnik k
        INNER JOIN profesor p ON p.Korisnik_idKorisnik = k.idKorisnik
        WHERE k.Tip_Korisnika = 'profesor'";


            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    using (MySqlCommand commandKorisnik = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = commandKorisnik.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idKorisnik = reader["idKorisnik"] != DBNull.Value ? Convert.ToInt32(reader["idKorisnik"]) : 0;
                            string ime = reader["Ime"]?.ToString() ?? string.Empty;
                            string prezime = reader["Prezime"]?.ToString() ?? string.Empty;
                            string email = reader["Email"]?.ToString() ?? string.Empty;
                            string username = reader["Username"]?.ToString() ?? string.Empty;
                            string password = reader["Password"]?.ToString() ?? string.Empty;
                            string tipKorisnika = reader["Tip_korisnika"]?.ToString() ?? string.Empty;
                            string zvanje = reader["Zvanje"]?.ToString() ?? string.Empty;
                            Profesor profesor = new Profesor(
                                idKorisnik, ime, prezime, email, username, password, tipKorisnika, zvanje);
                            profesori.Add(profesor);
                        }
                    }
                }
                catch (MySqlException ex)
                {

                    Console.WriteLine("MySQL greška: " + ex.Message);


                }
            }
            return profesori;
        }
    }


}

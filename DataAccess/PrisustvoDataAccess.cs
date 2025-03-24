using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using ASystem;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows;

namespace PrviProjektniZadatakHCI.DataAccess
{
    class PrisustvoDataAccess
    {

        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MySql_hci"].ConnectionString;
        public static ObservableCollection<Prisustvo> PregledPrisustva(Student student, Predmet predmet, Profesor profesor)
        {
            ObservableCollection<Prisustvo> listaPrisustva = new ObservableCollection<Prisustvo>();
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT 
    k.Ime, 
    k.Prezime, 
    p.Status , 
    p.Datum
FROM 
    mydb.Korisnik k
INNER JOIN 
    mydb.Student s ON k.idKorisnik = s.Korisnik_idKorisnik
INNER JOIN 
    mydb.Prisustvo p ON s.Korisnik_idKorisnik = p.Student_Korisnik_idKorisnik
INNER JOIN 
    mydb.Predmet pr ON p.Predmet_idPredmeta = pr.idPredmeta
INNER JOIN 
    mydb.Profesor prof ON prof.Korisnik_idKorisnik = @idProfesora
WHERE 
    p.Student_Korisnik_idKorisnik = @idKorisnika 
    AND p.Predmet_idPredmeta = @idPredmeta 
    AND k.Tip_korisnika = 'student';
";
                cmd.Parameters.AddWithValue("@idProfesora", profesor.idKorisnika);
                cmd.Parameters.AddWithValue("@idKorisnika", student.idKorisnika);
                cmd.Parameters.AddWithValue("@idPredmeta", predmet.IdPredmeta);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Prisustvo prisustvo = new Prisustvo(
                        student,
                        reader.GetDateTime("Datum"),
                        reader.GetString("Status"),
                        predmet
                    );
                    listaPrisustva.Add(prisustvo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Došlo je do greške: " + ex.Message);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return listaPrisustva;


        }

        public static ObservableCollection<Prisustvo> pregledPrisustvaList(int studentId, int predmetId)
        {
            ObservableCollection<Prisustvo> prisustvoLista = new ObservableCollection<Prisustvo>();
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"
            SELECT Datum, Status
            FROM Prisustvo
            WHERE Student_Korisnik_idKorisnik = @studentId
            AND Predmet_idPredmeta = @predmetId";

                cmd.Parameters.AddWithValue("@studentId", studentId);
                cmd.Parameters.AddWithValue("@predmetId", predmetId);

                MySqlDataReader reader = cmd.ExecuteReader();


                if (!reader.HasRows)
                {
                    MessageBox.Show("Upit nije vratio rezultate za date parametre.");
                }


                while (reader.Read())
                {

                    Prisustvo prisustvo = new Prisustvo(
                        reader.GetDateTime("Datum"),
                        reader.GetString("Status"),
                        predmetId,
                        studentId
                    );
                    prisustvoLista.Add(prisustvo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Došlo je do greške: " + ex.Message);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }


            return prisustvoLista;
        }


        public static bool UnesiPrisustvo(DateTime datum, Predmet predmet, Student student)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);

            try
            {
                conn.Open();
                MySqlCommand checkCmd = conn.CreateCommand();
                checkCmd.CommandText = @"
            SELECT COUNT(*) 
            FROM Prisustvo 
            WHERE Datum = @Datum AND Predmet_idPredmeta = @PredmetId AND Student_Korisnik_idKorisnik = @StudentId";

                checkCmd.Parameters.AddWithValue("@Datum", datum);
                checkCmd.Parameters.AddWithValue("@PredmetId", predmet.IdPredmeta);
                checkCmd.Parameters.AddWithValue("@StudentId", student.idKorisnika);

                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    return false;
                }
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"
            INSERT INTO Prisustvo (Datum, Status, Predmet_idPredmeta, Student_Korisnik_idKorisnik)
            VALUES (@Datum, @Status, @PredmetId, @StudentId)";

                cmd.Parameters.AddWithValue("@Datum", datum);
                cmd.Parameters.AddWithValue("@Status", "prisutan");
                cmd.Parameters.AddWithValue("@PredmetId", predmet.IdPredmeta);
                cmd.Parameters.AddWithValue("@StudentId", student.idKorisnika);

                int rowsAffected = cmd.ExecuteNonQuery();
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

        public static bool izbrisiPrisustvo(DateTime datum, Predmet predmet, Student student)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"delete from Prisustvo prisustvo where prisustvo.Predmet_idPredmeta=@idPremdeta and prisustvo.Datum=@datum and prisustvo.Student_Korisnik_idKorisnik=@idKorisnika";
                cmd.Parameters.AddWithValue("@idPremdeta", predmet.IdPredmeta);
                cmd.Parameters.AddWithValue("@datum", datum);
                cmd.Parameters.AddWithValue("@idKorisnika", student.idKorisnika);
                int rowsAffected = cmd.ExecuteNonQuery();
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


        public static bool AžurirajPrisustvo(Prisustvo prisustvo)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);

            try
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"
            UPDATE Prisustvo
            SET Status = @Status
            WHERE Datum = @Datum
            AND Predmet_idPredmeta = @PredmetId
            AND Student_Korisnik_idKorisnik = @StudentId";

                cmd.Parameters.AddWithValue("@Status", prisustvo.Status);
                cmd.Parameters.AddWithValue("@Datum", prisustvo.Datum);
                cmd.Parameters.AddWithValue("@PredmetId", prisustvo.Predmet_idPredmeta);
                cmd.Parameters.AddWithValue("@StudentId", prisustvo.Student_Korisnik_idKorisnik);
                int rowsAffected = cmd.ExecuteNonQuery();
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
    }
}


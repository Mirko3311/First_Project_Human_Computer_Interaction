using ASystem;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PrviProjektniZadatakHCI.DataAccess
{
    class DomaciZadatakDataAccess
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MySql_hci"].ConnectionString;

        public static ObservableCollection<DomaciZadatak> pregledDomacegZadatka()
        {
            ObservableCollection<DomaciZadatak> zadaci = new ObservableCollection<DomaciZadatak>();
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = @"select  p.Naziv, p.Opis, p.Rok from domacizadatak p;";

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DomaciZadatak dm = new DomaciZadatak
                    {

                        naziv = reader.GetString("Naziv"),
                        opis = reader.GetString("Opis"),
                        rok = reader.GetDateTime("Rok")
                    };

                    zadaci.Add(dm);
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
            return zadaci;
        }

        public static ObservableCollection<DomaciZadatak> pregledDomacegZadatkaPoPredmetu(Predmet predmet)
        {
            ObservableCollection<DomaciZadatak> zadaci = new ObservableCollection<DomaciZadatak>();
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"
            SELECT p.Naziv, p.Opis, p.Rok 
            FROM domacizadatak p
            JOIN predmet pr ON p.Predmet_idPredmeta = pr.idPredmeta
            WHERE pr.Naziv = @nazivPredmeta;";

                cmd.Parameters.AddWithValue("@nazivPredmeta", predmet.Naziv);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DomaciZadatak dm = new DomaciZadatak
                    {
                        naziv = reader.GetString("Naziv"),
                        opis = reader.GetString("Opis"),
                        rok = reader.GetDateTime("Rok")
                    };
                    zadaci.Add(dm);
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

            return zadaci;
        }

        public static ObservableCollection<DomaciZadatak> pregledDomacegPoProfesoru(Profesor profesor)
        {
            ObservableCollection<DomaciZadatak> zadaci = new ObservableCollection<DomaciZadatak>();
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"
            SELECT 
                DZ.idDomaciZadatak, 
                DZ.Naziv, 
                DZ.Opis, 
                DZ.Rok, 
                DZ.Predmet_idPredmeta,
                DZ.Profesor_Korisnik_idKorisnik
            FROM 
                DomaciZadatak DZ
            JOIN 
                Predmet P ON DZ.Predmet_idPredmeta = P.idPredmeta
            WHERE 
                DZ.Profesor_Korisnik_idKorisnik = @idProfesora;";

                cmd.Parameters.AddWithValue("@idProfesora", profesor.idKorisnika);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DomaciZadatak dm = new DomaciZadatak
                        {
                            idDomaciZadatak = reader.GetString("idDomaciZadatak"),
                            naziv = reader.GetString("Naziv"),
                            opis = reader.IsDBNull(reader.GetOrdinal("Opis")) ? null : reader.GetString("Opis"),
                            rok = reader.GetDateTime("Rok"),
                            idProfesora = reader.GetInt32("Profesor_Korisnik_idKorisnik"),
                            idPredmeta = reader.GetInt32("Predmet_idPredmeta")
                            
                        };
                        dm.predmet = PredmetDataAccess.dohvatiPredmetById(dm.idPredmeta);
                
                        zadaci.Add(dm);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Došlo je do greške: " + ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return zadaci;
        }

        public static ObservableCollection<DomaciZadatak> pregledDomacegZadatka(int idPredmeta)
        {
            ObservableCollection<DomaciZadatak> zadaci = new ObservableCollection<DomaciZadatak>();
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"
            SELECT p.Naziv, p.Opis, p.Rok 
            FROM domacizadatak p
            JOIN predmet pr ON p.Predmet_idPredmeta = pr.idPredmeta
            WHERE p.Predmet_idPredmeta = @idPredmeta;";

                cmd.Parameters.AddWithValue("@idPredmeta", idPredmeta);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DomaciZadatak dm = new DomaciZadatak
                    {
                        naziv = reader.GetString("Naziv"),
                        opis = reader.GetString("Opis"),
                        rok = reader.GetDateTime("Rok")
                    };

                    zadaci.Add(dm);
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
            return zadaci;
        }



        public static ObservableCollection<DomaciZadatak> pregledNeuradjenihDZ()
        {
            ObservableCollection<DomaciZadatak> domaciZadaci = new ObservableCollection<DomaciZadatak>();
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = " SELECT d.Naziv AS NazivZadatka, d.Opis AS OpisZadatka, d.Rok AS RokZadatka" +
                    " FROM  DomaciZadatak d WHERE  d.Predmet_idPredmeta = 2222 " +
                    "  AND d.idDomaciZadatak NOT IN (SELECT DISTINCT ds.DomaciZadatak_IdDomaciZadatak FROM DomaciZadatak_Student ds " +
                    " WHERE ds.Student_Korisnik_idKorisnik = 1);";
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DomaciZadatak dm = new DomaciZadatak
                    {
                        naziv = reader.GetString("NazivZadatka"),
                        opis = reader.GetString("OpisZadatka"),
                        rok = reader.GetDateTime("RokZadatka")

                    };
                    domaciZadaci.Add(dm);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Doslo je do  greske:" + ex.Message);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return domaciZadaci;
        }
        public static bool dodajDZadatak(String naziv, String opis, DateTime rok, String idDomaciZadatak, Predmet predmet, Profesor profesor)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "INSERT into DomaciZadatak(Naziv,Opis,Rok,idDomaciZadatak,Predmet_idPredmeta,Profesor_Korisnik_idKorisnik) values (@Naziv,@Opis,@Rok,@Id, @IdPredmeta, @IdProfesora)";
                command.Parameters.AddWithValue("@Naziv", naziv);
                command.Parameters.AddWithValue("@Opis", opis);
                command.Parameters.AddWithValue("@Rok", rok);
                command.Parameters.AddWithValue("@Id", idDomaciZadatak);
                command.Parameters.AddWithValue("@IdPredmeta", predmet.IdPredmeta);
                command.Parameters.AddWithValue("@IdProfesora", ProfessorDataAccess.GetProfesorId(profesor));
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;

            }
            catch (Exception ex)
            {
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

        public static bool obrisiZadatak(DomaciZadatak domaciZadatak)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = @"DELETE FROM DomaciZadatak WHERE idDomaciZadatak = @parametar";
                command.Parameters.AddWithValue("@parametar", domaciZadatak.idDomaciZadatak);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }


        public static bool obrisiDZadatak(string idDomaciZadatak)
        {

            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = @"DELETE FROM DomaciZadatak WHERE idDomaciZadatak = @parametar";
                command.Parameters.AddWithValue("@parametar", idDomaciZadatak);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public static bool azurirajDZadatak(DomaciZadatak zadatak, Profesor profesor)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "UPDATE DomaciZadatak SET Naziv = @Naziv, Opis = @Opis, Rok = @Rok, " +
                                      "Predmet_idPredmeta = @IdPredmeta, Profesor_Korisnik_idKorisnik = @IdProfesora " +
                                      "WHERE idDomaciZadatak = @Id";
                command.Parameters.AddWithValue("@Naziv", zadatak.naziv);
                command.Parameters.AddWithValue("@Opis", zadatak.opis);
                command.Parameters.AddWithValue("@Rok", zadatak.rok);
                command.Parameters.AddWithValue("@Id", zadatak.idDomaciZadatak);
                command.Parameters.AddWithValue("@IdPredmeta", zadatak.idPredmeta);
                command.Parameters.AddWithValue("@IdProfesora",profesor.idKorisnika);


                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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


        public static bool ponovoDodajZadatak(DomaciZadatak zadatak, Profesor profesor)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "INSERT into DomaciZadatak(Naziv,Opis,Rok,idDomaciZadatak,Predmet_idPredmeta,Profesor_Korisnik_idKorisnik) values (@Naziv,@Opis,@Rok,@Id, @IdPredmeta, @IdProfesora)";
                command.Parameters.AddWithValue("@Naziv", zadatak.naziv);
                command.Parameters.AddWithValue("@Opis", zadatak.opis);
                command.Parameters.AddWithValue("@Rok", zadatak.rok);
                command.Parameters.AddWithValue("@Id", zadatak.idDomaciZadatak);
                command.Parameters.AddWithValue("@IdPredmeta", zadatak.idPredmeta);
                command.Parameters.AddWithValue("@IdProfesora", ProfessorDataAccess.GetProfesorId(profesor));
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;

            }
            catch (Exception ex)
            {
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

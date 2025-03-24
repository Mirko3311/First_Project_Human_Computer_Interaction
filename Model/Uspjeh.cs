using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrviProjektniZadatakHCI
{
    public class Uspjeh
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MySql_hci"].ConnectionString;
        public string Predmet { get; set; }
        public decimal BodoviIspit { get; set; }
        public int OcjenaIspit { get; set; }
        public decimal BodoviDomaci { get; set; } 
  
    
    public static List<Uspjeh> DohvatiUspjeh(int studentId)
        {
            List<Uspjeh> uspjesi = new List<Uspjeh>();

            string query = @"
        SELECT 
            p.Naziv AS Predmet,
            COALESCE(SUM(i.Bodovi), 0) AS BodoviIspit,
            COALESCE(AVG(i.Ocjena), 0) AS OcjenaIspit,
            COALESCE(SUM(d.Bodovi), 0) AS BodoviDomaci
        FROM Predmet p
        LEFT JOIN Ispit_Student i 
            ON p.idPredmeta = i.Predmet_idPredmeta AND i.Student_Korisnik_idKorisnik = @studentId
        LEFT JOIN DomaciZadatak_Student d
            ON p.idPredmeta = d.DomaciZadatak_IdDomaciZadatak AND d.Student_Korisnik_idKorisnik = @studentId
        GROUP BY p.Naziv";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@studentId", studentId);

                try
                {
                    conn.Open();
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        uspjesi.Add(new Uspjeh
                        {
                            Predmet = reader.GetString("Predmet"),
                            BodoviIspit = reader.GetDecimal("BodoviIspit"),
                            OcjenaIspit = reader.GetInt32("OcjenaIspit"),
                            BodoviDomaci = reader.GetDecimal("BodoviDomaci")
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Greška prilikom dohvaćanja podataka: " + ex.Message);
                }
            }

            return uspjesi;
        } 
    }
}

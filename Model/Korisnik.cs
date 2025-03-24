using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Compiler;
using MySqlX.XDevAPI.Common;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Drawing;
using System.Windows;

namespace ASystem

{
public class Korisnik
    {
        public int idKorisnika { get; set; }
        public string ime { get; set; }
        public string prezime { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string tipKorisnika { get; set; }

        public string DisplayText => $"{ime} {prezime}";

        public Korisnik(int idKorisnik, string ime, string prezime)
        {
            this.idKorisnika = idKorisnik;
            this.ime = ime;
            this.prezime = prezime;
        }

   
    //   private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MySql_hci"]?.ConnectionString;


    // Konstruktor
    public Korisnik(int idKorisnik, string ime, string prezime, string email, string username, string password, string tipKorisnika)
        {
            idKorisnika = idKorisnik;
            this.ime = ime;
            this.prezime = prezime;
            
            this.email = email;
            this.username = username;
            this.password = password;
            this.tipKorisnika = tipKorisnika;
        }
    

    }

}


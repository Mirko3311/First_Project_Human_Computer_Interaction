using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
namespace ASystem
{
    public class Prisustvo
    {
       
        public DateTime Datum { get; set; }

        public string Status { get; set; }
        public int Predmet_idPredmeta { get; set; }
        public int Student_Korisnik_idKorisnik { get; set; }
        public Student Student { get; set; }

        public Prisustvo(DateTime datum, string status, int predmetId, int studentId)
        {
            Datum = datum;
            Status = status;
            Predmet_idPredmeta = predmetId;
            Student_Korisnik_idKorisnik = studentId;
        }


        public Prisustvo(Student student, DateTime datum, string status, Predmet predmet)
        {
            Student = student;
            Datum = datum;
            Status = status;
            Predmet_idPredmeta = predmet.IdPredmeta;

        }

    }

}

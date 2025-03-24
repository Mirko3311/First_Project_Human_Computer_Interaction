using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Controls;
using System.Windows;
using System.Security.Permissions;
using System.Collections.ObjectModel;

namespace ASystem
{
    public class Ispit 
    {
       
        public double bodovi { get; set; }
        public int ocjena { get; set; }
        public DateTime datumIspita { get; set; }
        public Student Student { get; set; }
        public int studentId { get; set; }
        public int predmetId { get; set; }

        public Ispit(Student student, double bodovii, int ocjena, DateTime datum)
        {
            Student = student;
            bodovi = bodovii;
            datumIspita = datum;
        }
        public Ispit() { }


    }
}
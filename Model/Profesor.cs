using MySql.Data.MySqlClient;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using Mysqlx.Crud;
using System.Collections.ObjectModel;

namespace ASystem
{
    public class Profesor : Korisnik
    {
     
        //  public string Zvanje { get; set; }
        private string _zvanje;
        public string Zvanje
        {
            get { return _zvanje; }
            set
            {
                if (_zvanje != value)
                {
                    _zvanje = value;
               
                }
            }
        }



        public Profesor() : base(0, "", "", "", "", "", "")
        {
            Zvanje = "";
        }

        public Profesor(int idKorisnik, string ime, string prezime, string email, string username, string password, string tipKorisnika, string zvanje)
       : base(idKorisnik, ime, prezime, email, username, password, tipKorisnika)
        {
            this.Zvanje = zvanje;


        }

        public string ToString()
        {
            return DisplayText;
        }
        public Profesor DeepCopy()
    {
        return (Profesor)this.MemberwiseClone();
    }
        public int GetIdKorisnika()
        {
            return this.idKorisnika;
        }

    }
}


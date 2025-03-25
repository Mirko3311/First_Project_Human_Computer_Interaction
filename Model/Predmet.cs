using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Org.BouncyCastle.Crypto.Signers;


namespace ASystem
{
        public class Predmet : INotifyPropertyChanged
        {
            private int idPredmeta;
            private string naziv;
            private string opis;
            private int eCTS;

            public event PropertyChangedEventHandler PropertyChanged;

            public int IdPredmeta
            {
                get => idPredmeta;
                set
                {
                    if (idPredmeta != value)
                    {
                        idPredmeta = value;
                        OnPropertyChanged(nameof(IdPredmeta));
                    }
                }
            }

            public string Naziv
            {
                get => naziv;
                set
                {
                    if (naziv != value)
                    {
                        naziv = value;
                        OnPropertyChanged(nameof(Naziv));
                    }
                }
            }

            public string Opis
            {
                get => opis;
                set
                {
                    if (opis != value)
                    {
                        opis = value;
                        OnPropertyChanged(nameof(Opis));
                    }
                }
            }

            public int ECTS
            {
                get => eCTS;
                set
                {
                    if (eCTS != value)
                    {
                        eCTS = value;
                        OnPropertyChanged(nameof(ECTS));
                    }
                }
            }

            public string DisplayText => Naziv;
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }


        public Predmet DeepCopy()
        {
            return (Predmet)this.MemberwiseClone();
        }
    }
}




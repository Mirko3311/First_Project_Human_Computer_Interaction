using ASystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrviProjektniZadatakHCI.ViewModel
{
    public class KorisnikViewModel : INotifyPropertyChanged
    {
        private Korisnik _korisnik;
        public KorisnikViewModel(Korisnik korisnik)
        {
            _korisnik = korisnik;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int IdKorisnika
        {
            get => _korisnik.idKorisnika;
            set
            {
                if (_korisnik.idKorisnika != value)
                {
                    _korisnik.idKorisnika = value;
                    OnPropertyChanged(nameof(IdKorisnika));
                }
            }
        }

        public string Ime
        {
            get => _korisnik.ime;
            set
            {
                if (_korisnik.ime != value)
                {
                    _korisnik.ime = value;
                    OnPropertyChanged(nameof(Ime));
                    OnPropertyChanged(nameof(DisplayText));  
                }
            }
        }

        public string Prezime
        {
            get => _korisnik.prezime;
            set
            {
                if (_korisnik.prezime != value)
                {
                    _korisnik.prezime = value;
                    OnPropertyChanged(nameof(Prezime));
                    OnPropertyChanged(nameof(DisplayText));  
                }
            }
        }
   
     
        public string Email
        {
            get => _korisnik.email;
            set
            {
                if (_korisnik.email != value)
                {
                    _korisnik.email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

 
        public string Username
        {
            get => _korisnik.username;
            set
            {
                if (_korisnik.username != value)
                {
                    _korisnik.username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        public string Password
        {
            get => _korisnik.password;
            set
            {
                if (_korisnik.password != value)
                {
                    _korisnik.password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public string TipKorisnika
        {
            get => _korisnik.tipKorisnika;
            set
            {
                if (_korisnik.tipKorisnika != value)
                {
                    _korisnik.tipKorisnika = value;
                    OnPropertyChanged(nameof(TipKorisnika));
                }
            }
        }

     
        public string DisplayText => $"{Ime} {Prezime}";


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

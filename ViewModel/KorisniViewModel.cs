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

        // Konstruktor za inicijalizaciju Korisnika u ViewModel
        public KorisnikViewModel(Korisnik korisnik)
        {
            _korisnik = korisnik;
        }

        // Event za obaveštavanje UI-a o promenama
        public event PropertyChangedEventHandler PropertyChanged;

        // Svojstvo za idKorisnika
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

        // Svojstvo za ime
        public string Ime
        {
            get => _korisnik.ime;
            set
            {
                if (_korisnik.ime != value)
                {
                    _korisnik.ime = value;
                    OnPropertyChanged(nameof(Ime));
                    OnPropertyChanged(nameof(DisplayText));  // Ažuriranje DisplayText kada se ime promeni
                }
            }
        }

        // Svojstvo za prezime
        public string Prezime
        {
            get => _korisnik.prezime;
            set
            {
                if (_korisnik.prezime != value)
                {
                    _korisnik.prezime = value;
                    OnPropertyChanged(nameof(Prezime));
                    OnPropertyChanged(nameof(DisplayText));  // Ažuriranje DisplayText kada se prezime promeni
                }
            }
        }

        // Svojstvo za email
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

        // Svojstvo za username
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

        // Svojstvo za password
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

        // Svojstvo za tip korisnika
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

        // Svojstvo za prikaz imena i prezimena
        public string DisplayText => $"{Ime} {Prezime}";

        // Metoda za podizanje PropertyChanged event-a
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

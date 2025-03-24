using ASystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PrviProjektniZadatakHCI.ViewModel
{
    public class StudentViewModel : INotifyPropertyChanged
    {
        private Student _student;

        // Konstruktor za StudentViewModel
        public StudentViewModel(Student student)
        {
            _student = student;
            // Možeš dodati logiku za inicijalizaciju ili dodatne akcije ovde, ako je potrebno.
        }

        // Event za obaveštavanje UI-a o promenama
        public event PropertyChangedEventHandler PropertyChanged;

        // Svojstvo za studentovo ime i prezime
        public string NazivStudenta => _student.NazivStudenta;

        public string ImeStudenta => _student.ime;

        public string PrezimeStudenta => _student.prezime;
        // Svojstvo za broj indeksa
        public string BrojIndeksa => _student.BrojIndeksa;

        // Svojstvo za godinu studija
        public int GodinaStudija => _student.GodinaStudija;

        // Svojstvo za prisustvo
        public bool IsPrisutanChecked
        {
            get => _student.IsPrisutanChecked;
            set
            {
                if (_student.IsPrisutanChecked != value)
                {
                    _student.IsPrisutanChecked = value;
                    OnPropertyChanged(nameof(IsPrisutanChecked));
                }
            }
        }

        // Svojstvo za odsustvo
        public bool IsOdsutanChecked
        {
            get => _student.IsOdsutanChecked;
            set
            {
                if (_student.IsOdsutanChecked != value)
                {
                    _student.IsOdsutanChecked = value;
                    OnPropertyChanged(nameof(IsOdsutanChecked));
                }
            }
        }

        // Metoda koja podiže PropertyChanged event
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


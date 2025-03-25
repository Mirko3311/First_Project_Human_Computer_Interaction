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
        public StudentViewModel(Student student)
        {
            _student = student;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string NazivStudenta => _student.NazivStudenta;

        public string ImeStudenta => _student.ime;

        public string PrezimeStudenta => _student.prezime;

        public string BrojIndeksa => _student.BrojIndeksa;

        public int GodinaStudija => _student.GodinaStudija;

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

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


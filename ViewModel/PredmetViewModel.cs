using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASystem;
using System.Collections.ObjectModel;
using System.ComponentModel;
using PrviProjektniZadatakHCI.DataAccess;
namespace PrviProjektniZadatakHCI.ViewModel
{


    public class PredmetViewModel : INotifyPropertyChanged
    {
        private Profesor _profesor;
        public ObservableCollection<Predmet> Predmeti { get; set; }
        private Predmet _selectedPredmet;

        public Predmet SelectedPredmet
        {
            get { return _selectedPredmet; }
            set
            {
                if (_selectedPredmet != value)
                {
                    _selectedPredmet = value;
                    OnPropertyChanged(nameof(SelectedPredmet));
                }
            }
        }

        public PredmetViewModel(Profesor profesor)
        {
            _profesor = profesor;
            Predmeti = new ObservableCollection<Predmet>(PredmetDataAccess.izlistajPredmete(_profesor));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _naziv;
        public string Naziv
        {
            get => _naziv;
            set
            {
                if (_naziv != value)
                {
                    _naziv = value;
                    OnPropertyChanged(nameof(Naziv));
                    OnPropertyChanged(nameof(IsValid)); // Provjeravamo validnost
                }
            }
        }

        private string _opis;
        public string Opis
        {
            get => _opis;
            set
            {
                if (_opis != value)
                {
                    _opis = value;
                    OnPropertyChanged(nameof(Opis));
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        private string _ects;
        public string ECTS
        {
            get => _ects;
            set
            {
                if (_ects != value)
                {
                    _ects = value;
                    OnPropertyChanged(nameof(ECTS));
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(Naziv) &&
                         !string.IsNullOrWhiteSpace(Opis) &&
                         !string.IsNullOrWhiteSpace(ECTS);

        public PredmetViewModel(Predmet predmet)
        {
            Naziv = predmet.Naziv;
            Opis = predmet.Opis;
            ECTS = predmet.ECTS.ToString();
        }

    }

}

using ASystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrviProjektniZadatakHCI.ViewModel
{
  
        public class IspitViewModel : INotifyPropertyChanged
        {
            private Ispit _ispit;

            public Ispit Ispit
            {
                get { return _ispit; }
                set
                {
                    _ispit = value;
                    OnPropertyChanged(nameof(Ispit));
                }
            }

            public double Bodovi => Ispit.bodovi;
            public int Ocjena => Ispit.ocjena;
            public DateTime DatumIspita => Ispit.datumIspita;
            public string StudentIme => Ispit.Student.ime;
            public string StudentPrezime => Ispit.Student.prezime;

            public IspitViewModel(Ispit ispit)
            {
                Ispit = ispit;
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    
}

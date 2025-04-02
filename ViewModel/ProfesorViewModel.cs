using ASystem;
using PrviProjektniZadatakHCI.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace PrviProjektniZadatakHCI.ViewModel
{
    namespace ASystem
    {

        public class ProfesorViewModel : INotifyPropertyChanged
        {
            private Profesor _selektovaniProfesor;

            public Profesor SelektovaniProfesor
            {
                get { return _selektovaniProfesor; }
                set
                {
                    _selektovaniProfesor = value;
                    OnPropertyChanged();
                }
            }

            public ObservableCollection<Profesor> Profesori { get; set; }

            public ICommand AzurirajProfesoraCommand { get; }

            public ProfesorViewModel()
            {
                Profesori = new ObservableCollection<Profesor>();
                SelektovaniProfesor = new Profesor(); 
                AzurirajProfesoraCommand = new RelayCommand(AzurirajProfesora);
            }

            private void AzurirajProfesora()
            {
                bool success = ProfessorDataAccess.AzurirajProfesora(SelektovaniProfesor);

                if (success)
                {
                    MessageBox.Show("Profesor je uspješno ažuriran!");
                    Reset();
                }
                else
                {
                    MessageBox.Show("Došlo je do greške prilikom ažuriranja profesora!");
                }
            }

            public void Reset()
            {
                SelektovaniProfesor = new Profesor();
                OnPropertyChanged(nameof(SelektovaniProfesor));
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

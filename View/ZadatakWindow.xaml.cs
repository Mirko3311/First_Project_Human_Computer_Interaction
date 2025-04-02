using ASystem;
using PrviProjektniZadatakHCI.DataAccess;
using PrviProjektniZadatakHCI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PrviProjektniZadatakHCI.View
{
  
    public partial class ZadatakWindow : Window
    {
        private DomaciZadatak _zadatak;
        private Profesor _profesor;  
       
        private DomaciZadatak domaciZadatak;
        private DomaciZadatak _originalniZadatak;
        private Stack<Action> _undoStack;
        public ZadatakWindow(DomaciZadatak zadatak, Profesor profesor, Stack<Action> undoStack)
        {
         

            InitializeComponent();
            dpRok.BlackoutDates.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1)));
            _zadatak = zadatak;
            _profesor = profesor;
            _undoStack = undoStack;

            _originalniZadatak = new DomaciZadatak
            {
                idDomaciZadatak = zadatak.idDomaciZadatak,
                naziv = zadatak.naziv,
                opis = zadatak.opis,
                rok = zadatak.rok,
                idPredmeta = zadatak.idPredmeta,
                idProfesora = zadatak.idProfesora,
             
            };
            txtNaziv.Text = _zadatak.naziv;
            txtOpis.Text = _zadatak.opis;
           //dpRok.SelectedDate = _zadatak.rok;

        }

        private void BtnObrisi_Click(object sender, RoutedEventArgs e)
        {
            if (DomaciZadatakDataAccess.obrisiZadatak(_zadatak))
            {
                string message = (String)Application.Current.Resources["SuccessfullyDelete"];
                new SuccessWindow(message).ShowDialog();
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                string message = SharedResource.ErrorDeletingTask;
                new WrongWindow(message);
            }

            _undoStack.Push(() =>
            {
                DomaciZadatakDataAccess.ponovoDodajZadatak(_zadatak, _profesor);
                string message = (String)Application.Current.Resources["SuccessfullyUndo"];
                new SuccessWindow(message).ShowDialog();
            });
        }

        private void BtnAzuriraj_Click(object sender, RoutedEventArgs e)
        {
          

            _undoStack.Push(() =>
            {
                _zadatak.naziv = _originalniZadatak.naziv;
                _zadatak.opis = _originalniZadatak.opis;
                _zadatak.rok = _originalniZadatak.rok;  
                DomaciZadatakDataAccess.azurirajDZadatak(_zadatak, _profesor);
                string message = (string)Application.Current.Resources["SuccessfullyUndo"];
                new SuccessWindow(message).ShowDialog();
            });
            _zadatak.naziv = txtNaziv.Text;
            _zadatak.opis = txtOpis.Text;
            _zadatak.rok = dpRok.SelectedDate ?? _zadatak.rok;
           

            if (DomaciZadatakDataAccess.azurirajDZadatak(_zadatak, _profesor))
            {
                string message = (string)Application.Current.Resources["TaskSuccessfullyUpdate"];
                new SuccessWindow(message).ShowDialog();
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                string message = (string)Application.Current.Resources["ErrorUpdate"];
                new WrongWindow(message).ShowDialog();
            }
        }


        private List<DataGrid> FindAllDataGrids(DependencyObject parent)
        {
            var dataGrids = new List<DataGrid>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is DataGrid dataGrid)
                {
                    dataGrids.Add(dataGrid);
                }
                else
                {
                    dataGrids.AddRange(FindAllDataGrids(child));
                }
            }
            return dataGrids;
        }
        public void RefreshAllDataGrids()
        {
            var allDataGrids = FindAllDataGrids(this); // this = ProfessorWindow
            foreach (var dataGrid in allDataGrids)
            {
                dataGrid.InvalidateVisual();
                dataGrid.Items.Refresh(); // Опционо: Освјежи податке
            }
        }
    }
}

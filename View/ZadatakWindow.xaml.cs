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
            dpRok.SelectedDate = _zadatak.rok;

        }

        private void BtnObrisi_Click(object sender, RoutedEventArgs e)
        {
            if (DomaciZadatakDataAccess.obrisiZadatak(_zadatak))
            {
                string message = SharedResource.SuccessfullyDelete;
                new SuccessWindow(message);
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
                string message = SharedResource.ChangesRetreved;
                new SuccessWindow(message);
            });
            _zadatak.naziv = txtNaziv.Text;
            _zadatak.opis = txtOpis.Text;
            _zadatak.rok = dpRok.SelectedDate ?? _zadatak.rok;
           

            if (DomaciZadatakDataAccess.azurirajDZadatak(_zadatak, _profesor))
            {
                string message = SharedResource.TaskSuccessfullyUpdate;
                new SuccessWindow(message).ShowDialog();
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                string message = SharedResource.ErrorUpdate;
                new WrongWindow(message).ShowDialog();
            }
        }

    }
}

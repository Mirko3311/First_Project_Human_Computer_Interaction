﻿using ASystem;
using PrviProjektniZadatakHCI.DataAccess;
using PrviProjektniZadatakHCI.Resources;
using PrviProjektniZadatakHCI.ViewModel;
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
  
    public partial class PromjenaLozinkeView : Window
    {
        private PromjenaLozinkeViewModel viewModel;
        private Korisnik korisnik;

        public PromjenaLozinkeView(Korisnik korisnik)
        {
            InitializeComponent();
            this.korisnik = korisnik;
            viewModel = new PromjenaLozinkeViewModel();
            DataContext = viewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender == CurrentPasswordBox)
            {
                viewModel.CurrentPassword = CurrentPasswordBox.Password;
            }
            else if (sender == NewPasswordBox)
            {
                viewModel.NewPassword = NewPasswordBox.Password;
            }
            else if (sender == ConfirmPasswordBox)
            {
                viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
            }
        }

        private void SaveNewPassword_Click(object sender, RoutedEventArgs e)
        {
            
            string currentPassword = viewModel.CurrentPassword;
            string newPassword = viewModel.NewPassword;
            string confirmPassword = viewModel.ConfirmPassword;

            if (newPassword != confirmPassword)
            {
                string message = SharedResource.PassConfirmation;
                new WrongWindow(message).ShowDialog();
            
                return;
            }

    
            if (!KorisnikDataAccess.provjeriLozinku(korisnik, currentPassword))
            {
                string message = SharedResource.CurrentPassMessage;
                new WrongWindow(message).ShowDialog();
                return;
            }

   
            bool success = KorisnikDataAccess.azurirajLozinku(korisnik, newPassword);
            if (success)
            {

                string message = SharedResource.PasswordChangeMessage;
                new SuccessWindow(message).ShowDialog();           
                this.Close();
            }
            else
            {
                string message = SharedResource.ErrorChangePassword;
                new WrongWindow(message).ShowDialog();
             
            }
        }
    }


}



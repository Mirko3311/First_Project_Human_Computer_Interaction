using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrviProjektniZadatakHCI.ViewModel
{
    using GalaSoft.MvvmLight.Command;
    using MySql.Data.MySqlClient;
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Security;
    using System.Windows;
    using System.Windows.Input;

    public class PromjenaLozinkeViewModel : INotifyPropertyChanged
    {
        private string currentPassword;
        private string newPassword;
        private string confirmPassword;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string CurrentPassword
        {
            get { return currentPassword; }
            set
            {
                if (currentPassword != value)
                {
                    currentPassword = value;
                    OnPropertyChanged(nameof(CurrentPassword));
                }
            }
        }

        public string NewPassword
        {
            get { return newPassword; }
            set
            {
                if (newPassword != value)
                {
                    newPassword = value;
                    OnPropertyChanged(nameof(NewPassword));
                }
            }
        }

        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set
            {
                if (confirmPassword != value)
                {
                    confirmPassword = value;
                    OnPropertyChanged(nameof(ConfirmPassword));
                }
            }
        }
    }
}

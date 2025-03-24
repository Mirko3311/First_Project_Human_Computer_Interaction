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
    /// <summary>
    /// Interaction logic for SuccessAdd.xaml
    /// </summary>
    public partial class SuccessWindow : Window
    {
        public SuccessWindow(string message)
        {
            InitializeComponent();
            MessageText.Text = message; // Postavi dinamičku poruku
        }

        private void CloseMessageBox(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
                this.Close();
            
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

     
   

    }
}

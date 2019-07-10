using RMAInforme.DataAccessLayer;
using System;
using System.Linq;
using System.Windows.Controls;

namespace RMAInforme
{
    /// <summary>
    /// Interaction logic for Password.xaml
    /// </summary>
    public partial class PasswordWindow : UserControl
    {
        public PasswordWindow()
        {
            InitializeComponent();
            pbPassword.Focus();
        }

        private void BtnAceptar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            btnAceptar.CommandParameter = pbPassword.Password;
        }


    }
}
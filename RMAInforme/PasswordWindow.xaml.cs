using System.Windows;
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
            Pass.Focus();
        }

        public string ResponseText
        {
            get { return ObsText.Text; }
            set { ObsText.Text = value; }
        }

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Pass.Password == "3X0")
            {
                OKButton.CommandParameter = true;
                return;
            }
            else
            {
                MessageBox.Show("Contraseña Incorrecta!");
                return;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            return;
        }
    }
}
using System.Windows;

namespace RMAInforme
{
    /// <summary>
    /// Interaction logic for Password.xaml
    /// </summary>
    public partial class PasswordWindow : Window
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
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Contraseña Incorrecta!");
                return;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
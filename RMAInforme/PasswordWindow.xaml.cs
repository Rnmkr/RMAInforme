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
        private Cambio cambioSeleccionado;
        private string estadoCambioSeleccionado;
        public PasswordWindow(Cambio _cambioSeleccionado)
        {
            InitializeComponent();
            pbPassword.Focus();
            cambioSeleccionado = _cambioSeleccionado;
            estadoCambioSeleccionado = _cambioSeleccionado.EstadoCambio;
        }

        private void BtnAceptar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (pbPassword.Password == "EXO1010_")
            {
                CambiarEstado();
                btnAceptar.CommandParameter = true;
            }
            else
            {
                return;
            }

        }

        private void CambiarEstado()
        {
            PRDB context = new PRDB();
            Cambio cambioModificado = context.Cambio.Where(w => w.ID == cambioSeleccionado.ID).Single();
            if (estadoCambioSeleccionado == "CANCELADO")
            {
                cambioModificado.EstadoCambio = "APROBADO";
            }
            else
            {
                cambioModificado.EstadoCambio = "CANCELADO";
            }

            context.SaveChanges();
        }
    }
}
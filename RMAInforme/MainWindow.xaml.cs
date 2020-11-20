using ClosedXML.Excel;
using LiveCharts;
using LiveCharts.Wpf;
using MaterialDesignThemes.Wpf;
using RMAInforme.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RMAInforme
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IQueryable<Cambio> ListaResultadoBusqueda;
        private string campoSeleccionado;
        private string precisionSeleccionada;
        private string origenSeleccionado;
        private DateTime? periodoInicialSeleccionado;
        private DateTime? periodoFinalSeleccionado;
        private string sectorSeleccionado;
        private readonly string nombreServidor = "BUBBA";
        private int keywordINT;
        private string stringBusqueda;
        private string estadoSeleccionado;
        private Cambio cambioSeleccionado;
        private int cantidadResultadoBusqueda;
        private object parametroCambioEstado;
        private readonly string ContraseñaCambio = "EXO1010_";
        private PRDB context;
        private int cantidadTotalItem;
        private int cantidadTodosFechaBusqueda;
        private int cantidadTotalTodos;
        private string campoChart3;
        private string periodoEstadisticas;
        private readonly SnapshotBusqueda[] SnapShotArray = new SnapshotBusqueda[6];
        private int currentIndex = -1;
        private int arrayItemsCount;
        SeriesCollection Chart3Serie;

        public MainWindow()
        {
            InitializeComponent();

            mainWindow.Title = "INFORME RMA" + " / " + Assembly.GetExecutingAssembly().GetName().Version;

            CompletarCombos();
        }

        private void CompletarCombos()
        {
            List<string> ListaCampos = new List<string> { "ARTICULO", "CATEGORIA", "CODIGO DE FALLA", "DESCRIPCION DE FALLA", "DESCRIPCION DE ITEM", "ID DE CAMBIO", "LEGAJO", "MODELO", "NUMERO DE PEDIDO", "OBSERVACIONES", "PRODUCTO", "TECNICO", "VERSION" };
            List<string> ListaPresicion = new List<string> { "EXACTA", "SIMILAR" };
            List<string> ListaOrigenDatos = new List<string> { "BASE DE DATOS", "LISTA ACTUAL" };
            List<string> ListaPeriodo = new List<string> { "HOY", "COMPLETO", "ESPECIFICAR" };
            List<string> ListaSectores = new List<string> { "TODOS", "PRODUCCION", "SERVICIO TECNICO" };
            List<string> ListaEstados = new List<string> { "TODOS", "APROBADO", "CANCELADO" };

            int numeroMes = Convert.ToInt32(DateTime.Now.ToString("MM"));
            for (int i = 1; i < numeroMes + 1; i++)
            {
                ListaPeriodo.Add(DateTimeFormatInfo.CurrentInfo.GetMonthName(i).ToUpper());
            }

            cbCampo.ItemsSource = ListaCampos;
            cbPresicion.ItemsSource = ListaPresicion;
            cbOrigenDatos.ItemsSource = ListaOrigenDatos;
            cbPeriodo.ItemsSource = ListaPeriodo;
            cbSector.ItemsSource = ListaSectores;
            cbEstado.ItemsSource = ListaEstados;
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            pbar.IsIndeterminate = true;

            IniciarBusqueda();

        }

        private void TbSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                IniciarBusqueda();
            }
        }

        private void IniciarBusqueda()
        {
            if (ComprobarOpciones())
            {
                using (new WaitCursor())
                {
                    string keyword = tbKeyword.Text;
                    campoSeleccionado = cbCampo.SelectedValue.ToString();
                    periodoInicialSeleccionado = dpInicial.SelectedDate;
                    periodoFinalSeleccionado = dpFinal.SelectedDate;
                    periodoFinalSeleccionado = periodoFinalSeleccionado.Value.AddDays(1);
                    sectorSeleccionado = cbSector.SelectedValue.ToString();
                    precisionSeleccionada = cbPresicion.SelectedValue.ToString();
                    origenSeleccionado = cbOrigenDatos.SelectedValue.ToString();
                    estadoSeleccionado = cbEstado.SelectedValue.ToString();

                    if (origenSeleccionado == "BASE DE DATOS")
                    {
                        if (PingServer(nombreServidor))
                        {
                            try
                            {
                                BuscarBaseDatos();
                            }
                            catch (System.ArgumentNullException)
                            {

                                throw;
                            }

                        }
                    }
                    else
                    {
                        BuscarLocal();
                    }

                    if (keyword != "*")
                    {
                        FiltrarKeyword();
                    }

                    if (ListaResultadoBusqueda != null)
                    {
                        FiltrarSector();
                    }
                    else
                    {
                        var MessageDialog = new MessageDialog
                        {
                            Titulo = { Text = "Oops!" },
                            Mensaje = { Text = "No hay resultados con ese keyword." }
                        };
                        DialogHost.Show(MessageDialog, "mainDialogHost");

                    }

                    if (ListaResultadoBusqueda != null)
                    {
                        FiltrarEstado();
                    }
                    else
                    {


                        var MessageDialog = new MessageDialog
                        {
                            Titulo = { Text = "Oops!" },
                            Mensaje = { Text = "No hay resultados con ese sector." }
                        };
                        DialogHost.Show(MessageDialog, "mainDialogHost");
                        return;

                    }

                    if (ListaResultadoBusqueda == null)
                    {


                        var MessageDialog = new MessageDialog
                        {
                            Titulo = { Text = "Oops!" },
                            Mensaje = { Text = "No hay resultados con ese estado." }
                        };
                        DialogHost.Show(MessageDialog, "mainDialogHost");
                        return;

                    }

                    cantidadResultadoBusqueda = ListaResultadoBusqueda.Count();

                    if (cantidadResultadoBusqueda > 0)
                    {
                        CrearNuevoSnapshot();
                        AsignarLista();
                    }
                    else
                    {


                        var MessageDialog = new MessageDialog
                        {
                            Titulo = { Text = "Oops!" },
                            Mensaje = { Text = "No se encontraron registros." }
                        };

                        tbStatusBarText.Text = "0 registros encontrados.";

                        DialogHost.Show(MessageDialog, "mainDialogHost");


                        return;

                    }
                }
            }


        }

        private bool ComprobarOpciones()
        {
            if (cbCampo.SelectedValue == null)
            {

                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "Seleccione un campo para la búsqueda." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;

            }

            if (cbPresicion.SelectedValue == null)
            {

                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "Seleccione la presición para la búsqueda." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;

            }

            if (cbOrigenDatos.SelectedValue == null)
            {

                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "Seleccione origen de datos para la búsqueda." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;

            }

            if (dpInicial.SelectedDate == null)
            {

                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "Seleccione periodo para la búsqueda." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;

            }

            if (dpFinal.SelectedDate == null)
            {

                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "Seleccione periodo para la búsqueda." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;

            }

            if (dpInicial.SelectedDate > dpFinal.SelectedDate)
            {

                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "La fecha inicial no puede ser mayor a la final." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;

            }


            if (cbSector.SelectedValue == null)
            {

                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "Seleccione un sector para la búsqueda." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;

            }

            if (cbEstado.SelectedValue == null)
            {

                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "Seleccione estado para la búsqueda." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;

            }

            //=/
            string keyword = tbKeyword.Text;
            if (string.IsNullOrWhiteSpace(keyword))
            {

                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "Ingrese un valor o 'keyword' para la búsqueda." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;

            }


            tbKeyword.Text = tbKeyword.Text.ToUpper();


            if (cbCampo.SelectedValue.ToString() == "ID DE CAMBIO")
            {
                try
                {
                    keywordINT = Convert.ToInt32(tbKeyword.Text);

                }
                catch (FormatException)
                {

                    var MessageDialog = new MessageDialog
                    {
                        Titulo = { Text = "Oops!" },
                        Mensaje = { Text = "El ID de cambio debe ser en formato numérico." }
                    };
                    DialogHost.Show(MessageDialog, "mainDialogHost");
                    return false;

                }
            }


            if (cbCampo.SelectedValue.ToString() == "CODIGO DE FALLA")
            {
                try
                {
                    Convert.ToInt32(tbKeyword.Text);

                }
                catch (FormatException)
                {

                    var MessageDialog = new MessageDialog
                    {
                        Titulo = { Text = "Oops!" },
                        Mensaje = { Text = "El código de falla debe ser en formato numérico." }
                    };
                    DialogHost.Show(MessageDialog, "mainDialogHost");
                    return false;

                }
            }


            if (cbCampo.SelectedValue.ToString() == "LEGAJO")
            {
                try
                {
                    Convert.ToInt32(tbKeyword.Text);

                }
                catch (FormatException)
                {

                    var MessageDialog = new MessageDialog
                    {
                        Titulo = { Text = "Oops!" },
                        Mensaje = { Text = "El legajo debe ser en formato numérico." }
                    };
                    DialogHost.Show(MessageDialog, "mainDialogHost");
                    return false;

                }
            }

            return true;
        }

        private void BuscarBaseDatos()
        {
            context = new PRDB();
            ListaResultadoBusqueda = context.Cambio.Where(w => w.FechaCambio >= periodoInicialSeleccionado && w.FechaCambio <= periodoFinalSeleccionado).Select(s => s);
            cantidadTodosFechaBusqueda = ListaResultadoBusqueda.Count();
        }

        private void BuscarLocal()
        {
            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.FechaCambio >= periodoInicialSeleccionado && w.FechaCambio <= periodoFinalSeleccionado).Select(s => s);
            cantidadTodosFechaBusqueda = ListaResultadoBusqueda.Count();
        }

        private void FiltrarKeyword()
        {
            string keyword = tbKeyword.Text;

            try
            {
                switch (campoSeleccionado)
                {

                    case "ARTICULO":
                        if (precisionSeleccionada == "EXACTA")
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.ArticuloItem == keyword).Select(s => s);
                        }
                        else
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.ArticuloItem.Contains(keyword)).Select(s => s);
                        }
                        break;

                    case "CATEGORIA":
                        if (precisionSeleccionada == "EXACTA")
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.CategoriaItem == keyword).Select(s => s);
                        }
                        else
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.CategoriaItem.Contains(keyword)).Select(s => s);
                        }
                        break;

                    case "CODIGO DE FALLA":
                        if (precisionSeleccionada == "EXACTA")
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.CodigoFalla == keyword).Select(s => s);
                        }
                        else
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.CodigoFalla.Contains(keyword)).Select(s => s);
                        }
                        break;

                    case "DESCRIPCION DE FALLA":
                        if (precisionSeleccionada == "EXACTA")
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.DescripcionFalla == keyword).Select(s => s);
                        }
                        else
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.DescripcionFalla.Contains(keyword)).Select(s => s);
                        }
                        break;

                    case "DESCRIPCION DE ITEM":
                        if (precisionSeleccionada == "EXACTA")
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.DescripcionItem == keyword).Select(s => s);
                        }
                        else
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.DescripcionItem.Contains(keyword)).Select(s => s);
                        }
                        break;

                    case "ESTADO DE CAMBIO":
                        if (precisionSeleccionada == "EXACTA")
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.EstadoCambio == keyword).Select(s => s);
                        }
                        else
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.EstadoCambio.Contains(keyword)).Select(s => s);
                        }
                        break;

                    case "ID DE CAMBIO":
                        ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.IdCambio == keywordINT).Select(s => s);
                        break;

                    case "LEGAJO":
                        if (precisionSeleccionada == "EXACTA")
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.Legajo == keyword).Select(s => s);
                        }
                        else
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.Legajo.Contains(keyword)).Select(s => s);
                        }
                        break;

                    case "MODELO":
                        if (precisionSeleccionada == "EXACTA")
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.Modelo == keyword).Select(s => s);
                        }
                        else
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.Modelo.Contains(keyword)).Select(s => s);
                        }
                        break;

                    case "NUMERO DE PEDIDO":
                        if (precisionSeleccionada == "EXACTA")
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.NumeroPedido == keyword).Select(s => s);
                        }
                        else
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.NumeroPedido.Contains(keyword)).Select(s => s);
                        }
                        break;

                    case "OBSERVACIONES":
                        if (precisionSeleccionada == "EXACTA")
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.Observaciones == keyword).Select(s => s);
                        }
                        else
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.Observaciones.Contains(keyword)).Select(s => s);
                        }
                        break;

                    case "PRODUCTO":
                        if (precisionSeleccionada == "EXACTA")
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.Producto == keyword).Select(s => s);
                        }
                        else
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.Producto.Contains(keyword)).Select(s => s);
                        }
                        break;

                    case "TECNICO":
                        if (precisionSeleccionada == "EXACTA")
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.Tecnico == keyword).Select(s => s);
                        }
                        else
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.Tecnico.Contains(keyword)).Select(s => s);
                        }
                        break;

                    case "VERSION":
                        if (precisionSeleccionada == "EXACTA")
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.VersionItem == keyword).Select(s => s);
                        }
                        else
                        {
                            ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.VersionItem.Contains(keyword)).Select(s => s);
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (System.ArgumentNullException)
            {

                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "No se pudo conectar con el servidor." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");

            }
        }

        private void FiltrarSector()
        {
            switch (sectorSeleccionado)
            {
                case "PRODUCCION":
                    ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.SectorCambio == "PRODUCCION").Select(s => s);
                    break;

                case "SERVICIO TECNICO":
                    ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.SectorCambio == "SERVICIO TECNICO").Select(s => s);
                    break;

                default:
                    break;
            }
        }

        private void FiltrarEstado()
        {
            switch (estadoSeleccionado)
            {
                case "APROBADO":
                    ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.EstadoCambio == "APROBADO").Select(s => s);
                    break;

                case "CANCELADO":
                    ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.EstadoCambio == "CANCELADO").Select(s => s);
                    break;

                default:
                    break;
            }
        }

        private void AsignarLista()
        {
            cantidadResultadoBusqueda = ListaResultadoBusqueda.Count(); //
            dgListaCambios.ItemsSource = ListaResultadoBusqueda.ToList();
            stringBusqueda = tbKeyword.Text;

            if (cantidadResultadoBusqueda == 1)
            {

                tbStatusBarText.Text = cantidadResultadoBusqueda + " registro encontrado.";

            }
            else
            {

                tbStatusBarText.Text = cantidadResultadoBusqueda + " registros encontrados.";

            }

            pbar.Visibility = Visibility.Hidden;

        }

        private void CrearNuevoSnapshot()
        {
            using (new WaitCursor())
            {
                if (currentIndex >= 5)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        SnapShotArray[i] = SnapShotArray[i + 1];
                    }

                    currentIndex = 5;
                    SnapShotArray[5] = GetSnapShot();
                }
                else
                {
                    currentIndex += 1;
                    SnapShotArray[currentIndex] = GetSnapShot();

                    arrayItemsCount = SnapShotArray.Count(c => c != null) - 1;

                    if (currentIndex < arrayItemsCount)
                    {
                        for (int i = currentIndex; i < 5; i++)
                        {
                            SnapShotArray[i + 1] = null;
                        }
                    }

                    arrayItemsCount = SnapShotArray.Count(c => c != null) - 1;
                }

                SetBackForwardButtonsStatus();

            }
        }

        private SnapshotBusqueda GetSnapShot()
        {
            var snapshot = new SnapshotBusqueda
            {
                Keyword = tbKeyword.Text,
                Campo = cbCampo.SelectedValue,
                Estado = cbEstado.SelectedValue,
                FechaInicial = dpInicial.SelectedDate,
                FechaFinal = dpFinal.SelectedDate,
                Periodo = cbPeriodo.SelectedValue,
                Presicion = cbPresicion.SelectedValue,
                Origen = cbOrigenDatos.SelectedValue,
                Sector = cbSector.SelectedValue,
                ResultadoBusqueda = ListaResultadoBusqueda
            };

            return snapshot;
        }

        private void SetSnapShot(int index)
        {
            try
            {
                SnapshotBusqueda sba = SnapShotArray[index];
                tbKeyword.Text = sba.Keyword;
                cbCampo.SelectedValue = sba.Campo;
                cbOrigenDatos.SelectedValue = sba.Origen;
                cbEstado.SelectedValue = sba.Estado;
                cbPeriodo.SelectedValue = sba.Periodo;
                cbPresicion.SelectedValue = sba.Presicion;
                cbSector.SelectedValue = sba.Sector;
                dpInicial.SelectedDate = sba.FechaInicial;
                dpFinal.SelectedDate = sba.FechaFinal;
                ListaResultadoBusqueda = sba.ResultadoBusqueda;
            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString() + " index: " + index.ToString());
            }

            AsignarLista();
            SetBackForwardButtonsStatus();
        }

        private void CbPeriodo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var valorSeleccionado = cbPeriodo.SelectedValue.ToString();

            if (valorSeleccionado == "ESPECIFICAR")
            {
                dpInicial.IsEnabled = true;
                dpFinal.IsEnabled = true;
                return;
            }
            else
            {
                dpInicial.IsEnabled = false;
                dpFinal.IsEnabled = false;
            }

            switch (valorSeleccionado)
            {
                case "HOY":
                    dpInicial.SelectedDate = DateTime.Today;
                    dpFinal.SelectedDate = DateTime.Today;
                    break;

                case "COMPLETO":
                    dpInicial.SelectedDate = Convert.ToDateTime("01/01/2018");
                    dpFinal.SelectedDate = DateTime.Today;
                    break;

                case "ESPECIFICAR":
                    break;

                default:
                    int numeroAño = DateTime.Now.Year;
                    int cantidadDiasMes = DateTime.DaysInMonth(numeroAño, (DateTime.ParseExact(valorSeleccionado, "MMMM", CultureInfo.CurrentCulture).Month));
                    int numeroMes = DateTime.ParseExact(valorSeleccionado, "MMMM", CultureInfo.CurrentCulture).Month;
                    dpInicial.SelectedDate = new DateTime(numeroAño, numeroMes, 1);
                    dpFinal.SelectedDate = new DateTime(numeroAño, numeroMes, cantidadDiasMes);
                    break;
            }
        }

        public bool PingServer(string ServerHostName)
        {
            try
            {
                IPAddress[] ip = Dns.GetHostAddresses(ServerHostName);

                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(ip[0]);

                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }

                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "No se pudo contactar al servidor." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;
            }
            catch (Exception)
            {
                //System.Net.Sockets.SocketException
                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "No se pudo contactar al servidor." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;
            }
        }

        private void CbOrigenDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbOrigenDatos.SelectedValue.ToString() == "LISTA ACTUAL")
            {
                if (ListaResultadoBusqueda == null)
                {
                    var MessageDialog = new MessageDialog
                    {
                        Titulo = { Text = "Oops!" },
                        Mensaje = { Text = "Realice primero una búsqueda en la base de datos." }
                    };
                    DialogHost.Show(MessageDialog, "mainDialogHost");

                    cbOrigenDatos.SelectedValue = null;
                }
            }
        }

        private async void BtnCancelarCambioAsync_Click(object sender, RoutedEventArgs e)
        {
            if (cambioSeleccionado == null)
            {
                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "Seleccione un cambio primero." }
                };
                await DialogHost.Show(MessageDialog);
                return;
            }
            else
            {
                parametroCambioEstado = await DialogHost.Show(new PasswordWindow());
            }
            CambiarEstado(parametroCambioEstado);
        }

        private void CambiarEstado(object result)
        {
            if ((string)result == ContraseñaCambio)
            {
                Cambio cambioModificado = context.Cambio.Where(w => w.ID == cambioSeleccionado.ID).Single();

                if (cambioSeleccionado.EstadoCambio == "APROBADO")
                {
                    cambioModificado.EstadoCambio = "CANCELADO";
                }
                else
                {
                    cambioModificado.EstadoCambio = "APROBADO";
                }

                cambioModificado.FechaModificacion = DateTime.Now;
                cambioModificado.SupervisorModificacion = "Sanchez Sebastian";
                context.SaveChanges();

                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "El estado se cambió correctamente." }
                };
                DialogHost.Show(MessageDialog);
                BuscarBaseDatos();
            }
            else
            {
                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "Contraseña incorrecta. No se realizaron cambios." }
                };
                DialogHost.Show(MessageDialog);
            }
        }

        private void DgListaCambios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cambioSeleccionado = (Cambio)dgListaCambios.SelectedItem;
        }

        private void BtnExportarPlanilla_Click(object sender, RoutedEventArgs e)
        {
            if (ListaResultadoBusqueda == null)
            {
                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "Realice una búsqueda primero." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
            }
            else
            {
                ExportarLista();
            }
        }

        private void ExportarLista()
        {

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Resultados de Búsqueda",
                DefaultExt = ".xslx",
                Filter = "Documentos Excel (.xlsx)|*.xlsx"
            };

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;

                try
                {
                    ExportDataSet(filename);

                    var MessageDialog = new MessageDialog
                    {
                        Titulo = { Text = "Perfecto!" },
                        Mensaje = { Text = "El archivo se guardó correctamente." }
                    };
                    DialogHost.Show(MessageDialog, "mainDialogHost");

                }
                catch (Exception e)
                {
                    var MessageDialog = new MessageDialog
                    {
                        Titulo = { Text = "Oops!" },
                        Mensaje = { Text = "Ocurrió un error intentando guardar el archivo." + " " + e.ToString() }
                    };
                    DialogHost.Show(MessageDialog, "mainDialogHost");
                }
            }
        }

        private void ExportDataSet(string destination)
        {
            var workbook = new XLWorkbook();
            DataTable dt;
            dt = LINQToDataTable(ListaResultadoBusqueda);

            var worksheet = workbook.Worksheets.Add("Registros de cambios");
            worksheet.Cell(1, 1).InsertTable(dt);
            worksheet.Columns().AdjustToContents();

            workbook.SaveAs(destination);
            workbook.Dispose();
        }

        private DataTable LINQToDataTable<T>(IQueryable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }
                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    //dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue(rec, null);
                    dr[pi.Name] = pi.GetValue(rec, null) ?? DBNull.Value;
                }

                dtReturn.Rows.Add(dr);
            }

            return dtReturn;
        }

        private void BtnEstadisticas_Click(object sender, RoutedEventArgs e)
        {
            if (ListaResultadoBusqueda == null)
            {
                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "Realice una búsqueda primero." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return;
            }

            string periodo = cbPeriodo.SelectedValue.ToString();
            switch (periodo)
            {

                case "HOY":
                    periodoEstadisticas = "DIA DE HOY";
                    break;

                case "COMPLETO":
                    periodoEstadisticas = "DESDE INICIO DE BASE DE DATOS (01/01/2018)";
                    break;

                case "ESPECIFICAR":
                    periodoEstadisticas = "ENTRE " + periodoInicialSeleccionado.Value.ToShortDateString() + " Y " + periodoFinalSeleccionado.Value.AddDays(-1).ToShortDateString();
                    break;


                default:
                    periodoEstadisticas = "MES DE " + periodo + " DE " + DateTime.Today.Year;
                    break;
            }

            if (tbKeyword.Text == "*")
            {
                CargarValoresBusquedaConWildcard();
                return;
            }

            switch (cbCampo.SelectedValue.ToString())
            {
                case "ARTICULO":
                    using (new WaitCursor())
                    {
                        CargarValoresArticulo();
                    }
                    MostrarEstadisticas();
                    break;

                case "CATEGORIA":
                    using (new WaitCursor())
                    {
                        CargarValoresCategoria();
                    }
                    MostrarEstadisticas();
                    break;

                case "LEGAJO":
                    using (new WaitCursor())
                    {
                        CargarValoresLegajo("0");
                    }
                    MostrarEstadisticas();
                    break;

                case "MODELO":
                    using (new WaitCursor())
                    {
                        CargarValoresModelo();
                    }
                    MostrarEstadisticas();
                    break;

                case "PRODUCTO":
                    using (new WaitCursor())
                    {
                        CargarValoresProducto();
                    }
                    MostrarEstadisticas();
                    break;

                case "TECNICO":
                    using (new WaitCursor())
                    {
                        CargarValoresTecnico();
                    }
                    MostrarEstadisticas();
                    break;

                default:
                    var MessageDialog = new MessageDialog
                    {
                        Titulo = { Text = "Oops!" },
                        Mensaje = { Text = "Por el momento, la aplicación no muestra estadísticas para el campo seleccionado." + Environment.NewLine + Environment.NewLine + "Los campos habilitados son:" + Environment.NewLine + "ARTICULO, CATEGORIA, MODELO, PRODUCTO, LEGAJO/TECNICO." }
                    };
                    DialogHost.Show(MessageDialog, "mainDialogHost");
                    break;
            }
        }

        private void CargarValoresBusquedaConWildcard()
        {
            if (ListaResultadoBusqueda == null || ListaResultadoBusqueda.Count() < 1)
            {
                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "Datos insuficientes para mostrar estadísticas." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return;
            }

            PointLabelCart = chartPoint => string.Format("{0}", chartPoint.Y);

            IQueryable<Cambio> listaFiltrada;
            listaFiltrada = ListaResultadoBusqueda.Where(w => w.EstadoCambio == "APROBADO");
            listaFiltrada = listaFiltrada.Where(w => w.SectorCambio == "PRODUCCION");

            List<string> relevantes1 = listaFiltrada
                    .GroupBy(g => g.ArticuloItem)
                    .OrderByDescending(o => o.Count())
                    .Take(10)
                    .Select(s => s.Key).ToList();
            SeriesCollection Chart1WildSerie = new SeriesCollection();
            foreach (var item in relevantes1)
            {
                int valor = listaFiltrada.Where(w => w.ArticuloItem == item).Count();
                IQueryable<Cambio> itemIQ = listaFiltrada.Where(w => w.ArticuloItem == item).Select(s => s).Take(1);
                Cambio itemS = itemIQ.Single();
                string itemCat = itemS.CategoriaItem;

                Chart1WildSerie.Add(new ColumnSeries
                {
                    Title = itemCat + " " + item,
                    Values = new ChartValues<double> { valor },
                    LabelPoint = PointLabelCart,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 10,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                });
            }


            List<string> relevantes2 = listaFiltrada
                    .GroupBy(g => g.Modelo)
                    .OrderByDescending(o => o.Count())
                    .Take(10)
                    .Select(s => s.Key).ToList();
            SeriesCollection Chart2WildSerie = new SeriesCollection();
            foreach (var item in relevantes2)
            {
                int valor = listaFiltrada.Where(w => w.Modelo == item).Count();
                string itemCut = item;
                if (item.Length > 12)
                {
                    itemCut = itemCut.Remove(12);
                }

                Chart2WildSerie.Add(new ColumnSeries
                {
                    Title = itemCut,
                    Values = new ChartValues<double> { valor },
                    LabelPoint = PointLabelCart,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 10,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                });
            }

            List<string> relevantes3 = listaFiltrada
                    .GroupBy(g => g.Producto)
                    .OrderByDescending(o => o.Count())
                    .Take(10)
                    .Select(s => s.Key).ToList();
            SeriesCollection Chart3WildSerie = new SeriesCollection();
            foreach (var item in relevantes3)
            {
                int valor = listaFiltrada.Where(w => w.Producto == item).Count();
                string itemCut = item;
                if (item.Length > 12)
                {
                    itemCut = itemCut.Remove(12);
                }

                Chart3WildSerie.Add(new ColumnSeries
                {
                    Title = itemCut,
                    Values = new ChartValues<double> { valor },
                    LabelPoint = PointLabelCart,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 10,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                });
            }

            DialogHost.Show(new StatsWildcard(Chart1WildSerie, Chart2WildSerie, Chart3WildSerie, periodoEstadisticas));
        }

        private void CargarValoresArticulo()
        {
            IQueryable<Cambio> listaMismo = context.Cambio.Where(w => w.ArticuloItem == stringBusqueda);
            listaMismo = listaMismo.Where(w => w.EstadoCambio == "APROBADO");
            listaMismo = listaMismo.Where(w => w.SectorCambio == "PRODUCCION");
            cantidadTotalItem = listaMismo.Count();
            cantidadTotalTodos = context.Cambio.Select(s => s).Count();

            List<string> relevantes = listaMismo
                    .GroupBy(g => g.DescripcionFalla)
                    .OrderByDescending(o => o.Count())
                    .Take(10)
                    .Select(s => s.Key).ToList();

            PointLabelCart = chartPoint => string.Format("{0}", chartPoint.Y);
            Chart3Serie = new SeriesCollection();

            foreach (var item in relevantes)
            {
                int valor = listaMismo.Where(w => w.DescripcionFalla == item).Count();

                string itemCut = item;

                if (item.Length > 12)
                {
                    itemCut = item.Remove(12);
                }

                Chart3Serie.Add(new ColumnSeries
                {
                    Title = itemCut,
                    Values = new ChartValues<double> { valor },
                    LabelPoint = PointLabelCart,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 10,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                });
            }

            campoChart3 = "FALLAS";
        }

        private void CargarValoresCategoria()
        {
            IQueryable<Cambio> listaMismo = context.Cambio.Where(w => w.CategoriaItem == stringBusqueda);
            listaMismo = listaMismo.Where(w => w.EstadoCambio == "APROBADO");
            listaMismo = listaMismo.Where(w => w.SectorCambio == "PRODUCCION");
            cantidadTotalItem = listaMismo.Count();
            cantidadTotalTodos = context.Cambio.Select(s => s).Count();

            List<string> relevantes = listaMismo
                    .GroupBy(g => g.Modelo)
                    .OrderByDescending(o => o.Count())
                    .Take(10)
                    .Select(s => s.Key).ToList();

            PointLabelCart = chartPoint => string.Format("{0}", chartPoint.Y);
            Chart3Serie = new SeriesCollection();

            foreach (var item in relevantes)
            {
                int valor = listaMismo.Where(w => w.Modelo == item).Count();

                string itemCut = item;

                if (item.Length > 12)
                {
                    itemCut = item.Remove(12);
                }

                Chart3Serie.Add(new ColumnSeries
                {
                    Title = itemCut,
                    Values = new ChartValues<double> { valor },
                    LabelPoint = PointLabelCart,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 10,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                });
            }

            campoChart3 = "MODELOS";
        }

        private void CargarValoresModelo()
        {
            IQueryable<Cambio> listaMismo = context.Cambio.Where(w => w.Modelo == stringBusqueda);
            listaMismo = listaMismo.Where(w => w.EstadoCambio == "APROBADO");
            listaMismo = listaMismo.Where(w => w.SectorCambio == "PRODUCCION");
            cantidadTotalItem = listaMismo.Count();
            cantidadTotalTodos = context.Cambio.Select(s => s).Count();

            List<string> relevantes = listaMismo
                    .GroupBy(g => g.CategoriaItem)
                    .OrderByDescending(o => o.Count())
                    .Take(10)
                    .Select(s => s.Key).ToList();

            PointLabelCart = chartPoint => string.Format("{0}", chartPoint.Y);
            Chart3Serie = new SeriesCollection();

            foreach (var item in relevantes)
            {
                int valor = listaMismo.Where(w => w.CategoriaItem == item).Count();

                string itemCut = item;

                if (item.Length > 12)
                {
                    itemCut = item.Remove(12);
                }

                Chart3Serie.Add(new ColumnSeries
                {
                    Title = itemCut,
                    Values = new ChartValues<double> { valor },
                    LabelPoint = PointLabelCart,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 10,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                });
            }

            campoChart3 = "CATEGORIAS";
        }

        private void CargarValoresProducto()
        {
            IQueryable<Cambio> listaMismo = context.Cambio.Where(w => w.Producto == stringBusqueda);
            listaMismo = listaMismo.Where(w => w.EstadoCambio == "APROBADO");
            listaMismo = listaMismo.Where(w => w.SectorCambio == "PRODUCCION");
            cantidadTotalItem = listaMismo.Count();
            cantidadTotalTodos = context.Cambio.Select(s => s).Count();

            List<string> relevantes = listaMismo
                    .GroupBy(g => g.Modelo)
                    .OrderByDescending(o => o.Count())
                    .Take(10)
                    .Select(s => s.Key).ToList();

            PointLabelCart = chartPoint => string.Format("{0}", chartPoint.Y);
            Chart3Serie = new SeriesCollection();

            foreach (var item in relevantes)
            {
                int valor = listaMismo.Where(w => w.Modelo == item).Count();

                string itemCut = item;

                if (item.Length > 12)
                {
                    itemCut = item.Remove(12);
                }

                Chart3Serie.Add(new ColumnSeries
                {
                    Title = itemCut,
                    Values = new ChartValues<double> { valor },
                    LabelPoint = PointLabelCart,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 10,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                });
            }

            campoChart3 = "MODELOS";
        }

        private void CargarValoresLegajo(string leg)
        {

            if (leg == "0")
            {
                leg = stringBusqueda;
            }

            IQueryable<Cambio> listaMismo = context.Cambio.Where(w => w.Legajo == leg);
            listaMismo = listaMismo.Where(w => w.EstadoCambio == "APROBADO");
            listaMismo = listaMismo.Where(w => w.SectorCambio == "PRODUCCION");
            cantidadTotalItem = listaMismo.Count();
            cantidadTotalTodos = context.Cambio.Select(s => s).Count();

            List<string> relevantes = listaMismo
                    .GroupBy(g => g.Producto)
                    .OrderByDescending(o => o.Count())
                    .Take(10)
                    .Select(s => s.Key).ToList();

            PointLabelCart = chartPoint => string.Format("{0}", chartPoint.Y);
            Chart3Serie = new SeriesCollection();

            foreach (var item in relevantes)
            {
                int valor = listaMismo.Where(w => w.Producto == item).Count();

                string itemCut = item;

                if (item.Length > 12)
                {
                    itemCut = item.Remove(12);
                }

                Chart3Serie.Add(new ColumnSeries
                {
                    Title = itemCut,
                    Values = new ChartValues<double> { valor },
                    LabelPoint = PointLabelCart,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 10,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                });
            }

            campoChart3 = "PRODUCTOS";
        }

        private void CargarValoresTecnico()
        {
            try
            {
                string legajoTecnico = ListaResultadoBusqueda.Select(s => s.Legajo).Distinct().SingleOrDefault();
                CargarValoresLegajo(legajoTecnico);
            }
            catch (InvalidOperationException)
            {
                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "Hay mas de un técnico en la lista." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
            }

        }

        private void MostrarEstadisticas()
        {
            if (ListaResultadoBusqueda == null || ListaResultadoBusqueda.Count() < 1)
            {
                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "Datos insuficientes para mostrar estadísticas." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return;
            }

            DialogHost.Show(new StatsWindow(stringBusqueda, cantidadResultadoBusqueda, cantidadTotalItem, cantidadTodosFechaBusqueda, cantidadTotalTodos, Chart3Serie, campoChart3, periodoEstadisticas));
        }

        private void SetBackForwardButtonsStatus()
        {
            if (currentIndex == 0)
            {
                btnBack.IsEnabled = false;
            }
            else
            {
                btnBack.IsEnabled = true;
            }

            if (currentIndex == arrayItemsCount)
            {
                btnForward.IsEnabled = false;
            }
            else
            {
                btnForward.IsEnabled = true;
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            using (new WaitCursor())
            {
                currentIndex -= 1;

                SetSnapShot(currentIndex);

                SetBackForwardButtonsStatus();
            }
        }

        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            using (new WaitCursor())
            {
                currentIndex += 1;

                SetSnapShot(currentIndex);

                SetBackForwardButtonsStatus();
            }
        }

        internal class SnapshotBusqueda
        {
            public string Keyword { get; set; }

            public object Campo { get; set; }

            public object Presicion { get; set; }

            public object Origen { get; set; }

            public object Estado { get; set; }

            public object Periodo { get; set; }

            public object Sector { get; set; }

            public DateTime? FechaInicial { get; set; }

            public DateTime? FechaFinal { get; set; }

            public IQueryable<Cambio> ResultadoBusqueda { get; set; }

        }

        public Func<ChartPoint, string> PointLabelCart { get; set; }

        private async void btnVerLogs_Click(object sender, RoutedEventArgs e)
        {
            string selected = null;
            if (dgListaCambios.SelectedItem != null)
            {
                var i = (Cambio)dgListaCambios.SelectedItem;
                selected = i.NumeroPedido.Substring(0, 8);

            }
            await DialogHost.Show(new LogsUserControl(selected));
        }
    }
}


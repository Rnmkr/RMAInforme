using System.Windows;
using RMAInforme.DataAccessLayer;
using System.Linq;
using System.Windows.Input;
using System;
using System.Net.NetworkInformation;
using System.Windows.Controls;
using System.Net;
using ClosedXML.Excel;
using System.Data;
using System.Reflection;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Globalization;

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
        private string keyword;
        private string nombreServidor = "DESKTOP";
        private int keywordINT;
        private string estadoSeleccionado;
        private Cambio cambioSeleccionado;
        private int cantidadResultadoBusqueda;
        private bool parametroReturn;

        public MainWindow()
        {
            InitializeComponent();

            mainWindow.Title = "INFORME RMA" + " / " + Assembly.GetExecutingAssembly().GetName().Version;

            CompletarCombos();
        }

        private void CompletarCombos()
        {
            List<string> ListaCampos = new List<string> { "ARTICULO", "CATEGORIA", "CODIGO DE FALLA", "DESCRIPCION DE FALLA", "DESCRIPCION DE ITEM", "ESTADO DE CAMBIO", "ID DE CAMBIO", "LEGAJO", "MODELO", "NUMERO DE PEDIDO", "OBSERVACIONES", "PRODUCTO", "TECNICO", "VERSION" };
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
            IniciarBusqueda();
        }

        private void TbSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                IniciarBusqueda();
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

            tbSearchBox.Text = tbSearchBox.Text.ToUpper();
            keyword = tbSearchBox.Text;
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

            if (cbCampo.SelectedValue.ToString() == "ID DE CAMBIO")
            {
                try
                {
                    keywordINT = Convert.ToInt32(tbSearchBox.Text);

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
                    Convert.ToInt32(tbSearchBox.Text);

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
                    Convert.ToInt32(tbSearchBox.Text);

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

        private void IniciarBusqueda()
        {
            if (ComprobarOpciones())
            {
                using (new WaitCursor())
                {
                    if (PingServer(nombreServidor))
                    {
                        keyword = tbSearchBox.Text;
                        campoSeleccionado = cbCampo.SelectedValue.ToString();
                        periodoInicialSeleccionado = dpInicial.SelectedDate;
                        periodoFinalSeleccionado = dpFinal.SelectedDate;
                        sectorSeleccionado = cbSector.SelectedValue.ToString();
                        precisionSeleccionada = cbPresicion.SelectedValue.ToString();
                        origenSeleccionado = cbOrigenDatos.SelectedValue.ToString();
                        estadoSeleccionado = cbEstado.SelectedValue.ToString();

                        Buscar();

                    }
                }
            }
        }

        private void Buscar() //falta contemplar un resultado de busqueda NULL
        {
            periodoFinalSeleccionado = periodoFinalSeleccionado.Value.AddDays(1);

            if (origenSeleccionado == "BASE DE DATOS")
            {
                PRDB context = new PRDB();
                ListaResultadoBusqueda = context.Cambio.Where(w => w.FechaCambio >= periodoInicialSeleccionado && w.FechaCambio <= periodoFinalSeleccionado).Select(s => s);
            }
            else
            {
                ListaResultadoBusqueda = ListaResultadoBusqueda.Where(w => w.FechaCambio >= periodoInicialSeleccionado && w.FechaCambio <= periodoFinalSeleccionado).Select(s => s);
            }

            if (keyword != "*")
            {
                BuscarKeyword();
            }

            FiltrarSector();

            FiltrarEstado();

            cantidadResultadoBusqueda = ListaResultadoBusqueda.Count();
            dgListaCambios.ItemsSource = ListaResultadoBusqueda.ToList();

            if (cantidadResultadoBusqueda == 1)
            {
                tbStatusBarText.Text = cantidadResultadoBusqueda + " registro encontrado.";
            }
            else
            {
                tbStatusBarText.Text = cantidadResultadoBusqueda + " registros encontrados.";
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

        private void BuscarKeyword()
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

        public class WaitCursor : IDisposable
        {
            private Cursor _previousCursor;

            public WaitCursor()
            {
                _previousCursor = Mouse.OverrideCursor;

                Mouse.OverrideCursor = Cursors.Wait;
            }

            public void Dispose()
            {
                Mouse.OverrideCursor = _previousCursor;
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

        private void BtnCancelarCambio_Click(object sender, RoutedEventArgs e)
        {
            if (cambioSeleccionado == null)
            {
                var MessageDialog = new MessageDialog
                {
                    Titulo = { Text = "Oops!" },
                    Mensaje = { Text = "Seleccione un cambio primero." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return;
            }
            else
            {
                DialogHost.Show(new PasswordWindow(cambioSeleccionado), "mainDialogHost", null, OnDialogCancelarCambioClosing);
            }
        }

        private void OnDialogCancelarCambioClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            parametroReturn = (bool?)eventArgs.Parameter ?? false;
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

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Resultados de Búsqueda";
            dlg.DefaultExt = ".xslx";
            dlg.Filter = "Documentos Excel (.xlsx)|*.xlsx";

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
                catch (Exception)
                {
                    var MessageDialog = new MessageDialog
                    {
                        Titulo = { Text = "Oops!" },
                        Mensaje = { Text = "Ocurrió un error intentando guardar el archivo." }
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
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue(rec, null);
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
            }

            switch (campoSeleccionado)
            {
                case "ARTICULO":
                    CargarValoresArticulo();
                    break;

                case "CATEGORIA":
                    CargarValoresCategoria();
                    break;

                case "MODELO":
                    CargarValoresModelo();
                    break;

                case "PRODUCTO":
                    CargarValoresProducto();
                    break;

                default:
                    var MessageDialog = new MessageDialog
                    {
                        Titulo = { Text = "Oops!" },
                        Mensaje = { Text = "Por el momento, la aplicación no muestra estadísticas para el campo seleccionado." + Environment.NewLine + Environment.NewLine + "Los campos habilitados son:" + Environment.NewLine + "ARTICULO, CATEGORIA, MODELO y PRODUCTO." }
                    };
                    DialogHost.Show(MessageDialog, "mainDialogHost");
                    break;
            }
        }

        private void CargarValoresProducto()
        {

            MostrarEstadisticas();
        }

        private void CargarValoresModelo()
        {

            MostrarEstadisticas();
        }

        private void CargarValoresCategoria()
        {

            MostrarEstadisticas();
        }

        private void CargarValoresArticulo()
        {

            MostrarEstadisticas();
        }

        private void MostrarEstadisticas()
        {
            //DialogHost.Show(new StatsWindow {valor1a, valor1b, valor2a, valor2b, valor3a, valor3b, valor3c, valorT, valorP, valorS }, "mainDialogHost");
        }
    }
}


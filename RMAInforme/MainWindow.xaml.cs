using System.Windows;
using RMAInforme.DataAccessLayer;
using System.Linq;
using System.Windows.Input;
using System;
using System.Windows.Media;
using System.Net.NetworkInformation;
using System.Windows.Controls;
using readconfig;
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
        private PRDB context = new PRDB();
        private IQueryable<Cambio> ListaResultadoBusqueda;
        private string campoSeleccionado;
        private bool? buscarValorExacto;
        private bool? origenEsBaseDatos;
        private string periodoInicialSeleccionado;
        private string periodoFinalSeleccionado;
        private string sectorSeleccionado;
        private string keyword;

        public MainWindow()
        {
            InitializeComponent();

            mainWindow.Title = "INFORME RMA" + " / " + Assembly.GetExecutingAssembly().GetName().Version;

            CompletarCombos();
        }

        private void CompletarCombos()
        {
            List<string> ListaCampos = new List<string> { "ARTICULO", "NUMERO DE PEDIDO", "CATEGORIA", "MODELO", "PRODUCTO", "VERSION", "DESCRIPCION DE ITEM", "SECTOR CAMBIO", "LEGAJO", "TECNICO", "CODIGO DE FALLA", "DESCRIPCION DE FALLA", "OBSERVACIONES", "ESTADO DE CAMBIO", "ID DE CAMBIO" };
            List<string> ListaPresicion = new List<string> { "EXACTA", "SIMILAR" };
            List<string> ListaOrigenDatos = new List<string> { "BASE DE DATOS", "LISTA ACTUAL" };
            List<string> ListaPeriodo = new List<string> { "HOY", "COMPLETO", "ESPECIFICAR" };
            List<string> ListaSectores = new List<string> { "TODOS", "PRODUCCION", "SERVICIO TECNICO" };

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
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            Buscar();
        }

        private void TbSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Buscar();
            }
        }

        private bool ComprobarOpciones()
        {
            if (string.IsNullOrWhiteSpace(campoSeleccionado))
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "Seleccione un campo para la búsqueda." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;
            }

            if (buscarValorExacto == null)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "Seleccione la presición para la búsqueda." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;
            }

            if (origenEsBaseDatos == null)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "Seleccione origen de datos para la búsqueda." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;
            }

            if (string.IsNullOrWhiteSpace(periodoInicialSeleccionado))
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "Seleccione periodo para la búsqueda." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;
            }

            if (string.IsNullOrWhiteSpace(periodoFinalSeleccionado))
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "Seleccione periodo para la búsqueda." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;
            }

            if (dpInicial.SelectedDate > dpFinal.SelectedDate)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "La fecha inicial no puede ser mayor a la final." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;
            }


            if (string.IsNullOrWhiteSpace(sectorSeleccionado))
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "Seleccione un sector para la búsqueda." }
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
                    Message = { Text = "Ingrese un valor o 'keyword' para la búsqueda." }
                };
                DialogHost.Show(MessageDialog, "mainDialogHost");
                return false;
            }

            return true;
        }

        private IQueryable<Cambio> Buscar()
        {
            if (ComprobarOpciones())
            {
                using (new WaitCursor())
                {
                    ListaResultadoBusqueda = context.Cambio.Select(s => s);
                    dgListaCambios.ItemsSource = ListaResultadoBusqueda.ToList();
                }
            }

            IQueryable<Cambio> ResultadoBusqueda = null;

            return ResultadoBusqueda;
        }

        private void CbCampo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            campoSeleccionado = cbCampo.SelectedValue.ToString();
        }

        private void CbPresicion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var valorSeleccionado = cbPresicion.SelectedValue.ToString();
            switch (valorSeleccionado)
            {
                case "EXACTA":
                    buscarValorExacto = true;
                    break;

                default:
                    buscarValorExacto = false;
                    break;
            }
        }

        private void CbOrigenDatos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var valorSeleccionado = cbOrigenDatos.SelectedValue.ToString();
            switch (valorSeleccionado)
            {
                case "BASE DE DATOS":
                    origenEsBaseDatos = true;
                    break;

                default:
                    origenEsBaseDatos = false;
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

        private void CbSector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sectorSeleccionado = cbSector.SelectedValue.ToString();
        }

        private void DpInicial_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            periodoInicialSeleccionado = dpInicial.SelectedDate.ToString();
        }

        private void DpFinal_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            periodoFinalSeleccionado = dpFinal.SelectedDate.ToString();
        }

        public class WaitCursor : IDisposable
        {
            private Cursor _previousCursor;

            public WaitCursor()
            {
                _previousCursor = Mouse.OverrideCursor;

                Mouse.OverrideCursor = Cursors.Wait;
            }

            #region IDisposable Members

            public void Dispose()
            {
                Mouse.OverrideCursor = _previousCursor;
            }
        }


        #endregion










        //private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        //{
        //    using (new WaitCursor())
        //    {
        //        ComboBoxSector.SelectedIndex = 0;

        //        if (SimplePing() == false)
        //        {
        //            var MessageDialog = new MessageDialog
        //            {
        //                Message = { Text = "No se encontró el servidor." + Environment.NewLine + "Revise la conexión con la Base de Datos y reintente." }
        //            };
        //            DialogHost.Show(MessageDialog, "openup");
        //            return;
        //        }

        //        if ((bool)RadioLocal.IsChecked && List == null)
        //        {
        //            var MessageDialog = new MessageDialog
        //            {
        //                Message = { Text = "Realice una búsqueda en la Base de Datos primero!" }
        //            };
        //            DialogHost.Show(MessageDialog, "openup");

        //            RadioGlobal.IsChecked = true;
        //            return;
        //        }

        //        if (DateInit.SelectedDate == null && DateEnd.SelectedDate != null)
        //        {
        //            var MessageDialog = new MessageDialog
        //            {
        //                Message = { Text = "Ingrese Fecha de Inicio" }
        //            };
        //            DialogHost.Show(MessageDialog, "openup");
        //            return;
        //        }

        //        if (DateInit.SelectedDate != null && DateEnd.SelectedDate == null)
        //        {
        //            var MessageDialog = new MessageDialog
        //            {
        //                Message = { Text = "Ingrese Fecha Final" }
        //            };
        //            DialogHost.Show(MessageDialog, "openup");
        //            return;
        //        }

        //        if (DateInit.SelectedDate > DateEnd.SelectedDate)
        //        {
        //            var MessageDialog = new MessageDialog
        //            {
        //                Message = { Text = "La fecha de inicio no puede ser mayor a la final!" }
        //            };
        //            DialogHost.Show(MessageDialog, "openup");
        //            return;
        //        }


        //        Keyword = TextBoxSearchString.Text;
        //        if (Keyword == "Buscar...")
        //        {
        //            Keyword = null;
        //        }
        //        Table = ComboBoxTable.SelectedValue.ToString();
        //        InitialDate = DateInit.SelectedDate ?? Convert.ToDateTime("01/01/2018");
        //        EndDate = DateEnd.SelectedDate ?? Convert.ToDateTime(DateTime.Today);

        //        if (EndDate != null)
        //        {
        //            EndDate = EndDate.Value.AddDays(1);
        //        }

        //        if (string.IsNullOrWhiteSpace(Keyword))
        //        {
        //            if (InitialDate == null && EndDate == null)
        //            {
        //                if ((bool)RadioGlobal.IsChecked)
        //                {
        //                    if (MessageBox.Show("Esta a punto de buscar TODOS los registros de la Base de Datos, esto puede llevar un tiempo prolongado." + Environment.NewLine + Environment.NewLine + "Esta seguro?", "Buscar...", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
        //                    {
        //                        BuscarTodo();
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                Search(InitialDate, EndDate);
        //            }
        //        }
        //        else
        //        {
        //            if (InitialDate == null && EndDate == null)
        //            {
        //                Search(Keyword, Table);
        //            }
        //            else
        //            {
        //                Search(Keyword, Table, InitialDate, EndDate);
        //            }
        //        }
        //        try
        //        {
        //            if (List.FirstOrDefault() == null)
        //            {
        //                Export.IsEnabled = false;
        //                Stats.IsEnabled = false;

        //                var MessageDialog = new MessageDialog
        //                {
        //                    Message = { Text = "La búsqueda no obtuvo resultados!" + Environment.NewLine + Environment.NewLine + "Sugerencia: intente desmarcando la opción de 'Búsqueda Exacta' o ampliando el rango de fechas." }
        //                };
        //                DialogHost.Show(MessageDialog, "openup");
        //                return;
        //            }
        //            else
        //            {
        //                DataGrid.ItemsSource = List.ToList();
        //                Export.IsEnabled = true;
        //                Stats.IsEnabled = true;
        //                ComboBoxSector.IsEnabled = true;

        //            }
        //        }
        //        catch (ArgumentNullException)
        //        {
        //            return;
        //        }
        //    }

        //    DataGrid.Columns[0].Visibility = Visibility.Collapsed;
        //    DataGrid.Columns[17].Visibility = Visibility.Collapsed;
        //    DataGrid.Columns[18].Visibility = Visibility.Collapsed;
        //}

        //private void Search(string keyword, string table, DateTime? init, DateTime? end)
        //{
        //    if ((bool)RadioLocal.IsChecked)
        //    {
        //        TList = List.Where(w => w.FechaCambio >= init && w.FechaCambio <= end);
        //    }
        //    else
        //    {
        //        PRDB context = new PRDB();
        //        TList = context.Cambio.Where(w => w.FechaCambio >= init && w.FechaCambio <= end);
        //    }

        //    switch (table)
        //    {
        //        case "ARTICULO":
        //            if (CheckEqual.IsChecked == true)
        //            {
        //                List = TList.Where(w => w.ArticuloItem == keyword).Select(s => s);
        //                distintos = TList.Where(w => w.ArticuloItem == keyword).Select(s => s.ArticuloItem).Distinct().ToList();
        //            }
        //            else
        //            {
        //                List = TList.Where(w => w.ArticuloItem.Contains(keyword)).Select(s => s);
        //                distintos = TList.Where(w => w.ArticuloItem.Contains(keyword)).Select(s => s.ArticuloItem).Distinct().ToList();
        //            }
        //            break;

        //        case "NUMERO DE PEDIDO":
        //            if (CheckEqual.IsChecked == true)
        //            {
        //                List = TList.Where(w => w.NumeroPedido == keyword).Select(s => s);
        //                distintos = TList.Where(w => w.NumeroPedido == keyword).Select(s => s.NumeroPedido).Distinct().ToList();
        //            }
        //            else
        //            {
        //                List = TList.Where(w => w.NumeroPedido.Contains(keyword)).Select(s => s);
        //                distintos = TList.Where(w => w.NumeroPedido.Contains(keyword)).Select(s => s.NumeroPedido).Distinct().ToList();
        //            }
        //            break;

        //        case "CATEGORIA":
        //            if (CheckEqual.IsChecked == true)
        //            {
        //                List = TList.Where(w => w.CategoriaItem == keyword).Select(s => s);
        //                distintos = TList.Where(w => w.CategoriaItem == keyword).Select(s => s.CategoriaItem).Distinct().ToList();
        //            }
        //            else
        //            {
        //                List = TList.Where(w => w.CategoriaItem.Contains(keyword)).Select(s => s);
        //                distintos = TList.Where(w => w.CategoriaItem.Contains(keyword)).Select(s => s.CategoriaItem).Distinct().ToList();
        //            }
        //            break;

        //        case "MODELO":
        //            if (CheckEqual.IsChecked == true)
        //            {
        //                List = TList.Where(w => w.Modelo == keyword).Select(s => s);
        //                distintos = TList.Where(w => w.Modelo == keyword).Select(s => s.Modelo).Distinct().ToList();
        //            }
        //            else
        //            {
        //                List = TList.Where(w => w.Modelo.Contains(keyword)).Select(s => s);
        //                distintos = TList.Where(w => w.Modelo.Contains(keyword)).Select(s => s.Modelo).Distinct().ToList();
        //            }
        //            break;

        //        case "PRODUCTO":
        //            if (CheckEqual.IsChecked == true)
        //            {
        //                List = TList.Where(w => w.Producto == keyword).Select(s => s);
        //                distintos = TList.Where(w => w.Producto == keyword).Select(s => s.Producto).Distinct().ToList();
        //            }
        //            else
        //            {
        //                List = TList.Where(w => w.Producto.Contains(keyword)).Select(s => s);
        //                distintos = TList.Where(w => w.Producto.Contains(keyword)).Select(s => s.Producto).Distinct().ToList();
        //            }
        //            break;

        //        case "VERSION":
        //            if (CheckEqual.IsChecked == true)
        //            {
        //                List = TList.Where(w => w.VersionItem == keyword).Select(s => s);
        //                distintos = TList.Where(w => w.VersionItem == keyword).Select(s => s.VersionItem).Distinct().ToList();
        //            }
        //            else
        //            {
        //                List = TList.Where(w => w.VersionItem.Contains(keyword)).Select(s => s);
        //                distintos = TList.Where(w => w.VersionItem.Contains(keyword)).Select(s => s.VersionItem).Distinct().ToList();
        //            }
        //            break;

        //        case "DESCRIPCION DE ITEM":
        //            if (CheckEqual.IsChecked == true)
        //            {
        //                List = TList.Where(w => w.DescripcionItem == keyword).Select(s => s);
        //                distintos = TList.Where(w => w.DescripcionItem == keyword).Select(s => s.DescripcionItem).Distinct().ToList();
        //            }
        //            else
        //            {
        //                List = TList.Where(w => w.DescripcionItem.Contains(keyword)).Select(s => s);
        //                distintos = TList.Where(w => w.DescripcionItem.Contains(keyword)).Select(s => s.DescripcionItem).Distinct().ToList();
        //            }
        //            break;

        //        case "SECTOR CAMBIO":
        //            if (CheckEqual.IsChecked == true)
        //            {
        //                List = TList.Where(w => w.SectorCambio == keyword).Select(s => s);
        //                distintos = TList.Where(w => w.SectorCambio == keyword).Select(s => s.SectorCambio).Distinct().ToList();
        //            }
        //            else
        //            {
        //                List = TList.Where(w => w.SectorCambio.Contains(keyword)).Select(s => s);
        //                distintos = TList.Where(w => w.SectorCambio.Contains(keyword)).Select(s => s.SectorCambio).Distinct().ToList();
        //            }
        //            break;

        //        case "LEGAJO":
        //            if (CheckEqual.IsChecked == true)
        //            {
        //                List = TList.Where(w => w.Legajo == keyword).Select(s => s);
        //                distintos = TList.Where(w => w.Legajo == keyword).Select(s => s.Legajo).Distinct().ToList();
        //            }
        //            else
        //            {
        //                List = TList.Where(w => w.Legajo.Contains(keyword)).Select(s => s);
        //                distintos = TList.Where(w => w.Legajo.Contains(keyword)).Select(s => s.Legajo).Distinct().ToList();
        //            }
        //            break;

        //        case "TECNICO":
        //            if (CheckEqual.IsChecked == true)
        //            {
        //                List = TList.Where(w => w.Tecnico == keyword).Select(s => s);
        //                distintos = TList.Where(w => w.Tecnico == keyword).Select(s => s.Tecnico).Distinct().ToList();
        //            }
        //            else
        //            {
        //                List = TList.Where(w => w.Tecnico.Contains(keyword)).Select(s => s);
        //                distintos = TList.Where(w => w.Tecnico.Contains(keyword)).Select(s => s.Tecnico).Distinct().ToList();
        //            }
        //            break;

        //        case "CODIGO DE FALLA":
        //            if (CheckEqual.IsChecked == true)
        //            {
        //                List = TList.Where(w => w.CodigoFalla == keyword).Select(s => s);
        //                distintos = TList.Where(w => w.CodigoFalla == keyword).Select(s => s.CodigoFalla).Distinct().ToList();
        //            }
        //            else
        //            {
        //                List = TList.Where(w => w.CodigoFalla.Contains(keyword)).Select(s => s);
        //                distintos = TList.Where(w => w.CodigoFalla.Contains(keyword)).Select(s => s.CodigoFalla).Distinct().ToList();
        //            }
        //            break;

        //        case "DESCRIPCION DE FALLA":
        //            if (CheckEqual.IsChecked == true)
        //            {
        //                List = TList.Where(w => w.DescripcionFalla == keyword).Select(s => s);
        //                distintos = TList.Where(w => w.DescripcionFalla == keyword).Select(s => s.DescripcionFalla).Distinct().ToList();
        //            }
        //            else
        //            {
        //                List = TList.Where(w => w.DescripcionFalla.Contains(keyword)).Select(s => s);
        //                distintos = TList.Where(w => w.DescripcionFalla.Contains(keyword)).Select(s => s.DescripcionFalla).Distinct().ToList();
        //            }
        //            break;

        //        case "OBSERVACIONES":
        //            if (CheckEqual.IsChecked == true)
        //            {
        //                List = TList.Where(w => w.Observaciones == keyword).Select(s => s);
        //                distintos = TList.Where(w => w.Observaciones == keyword).Select(s => s.Observaciones).Distinct().ToList();
        //            }
        //            else
        //            {
        //                List = TList.Where(w => w.Observaciones.Contains(keyword)).Select(s => s);
        //                distintos = TList.Where(w => w.Observaciones.Contains(keyword)).Select(s => s.Observaciones).Distinct().ToList();
        //            }
        //            break;

        //        case "ESTADO DE CAMBIO":
        //            if (CheckEqual.IsChecked == true)
        //            {
        //                List = TList.Where(w => w.EstadoCambio == keyword).Select(s => s);
        //                distintos = TList.Where(w => w.EstadoCambio == keyword).Select(s => s.EstadoCambio).Distinct().ToList();
        //            }
        //            else
        //            {
        //                List = TList.Where(w => w.EstadoCambio.Contains(keyword)).Select(s => s);
        //                distintos = TList.Where(w => w.EstadoCambio.Contains(keyword)).Select(s => s.EstadoCambio).Distinct().ToList();
        //            }
        //            break;

        //        case "ID DE CAMBIO":
        //            int id;
        //            try
        //            {
        //                id = Int32.Parse(keyword);
        //            }
        //            catch (FormatException)
        //            {
        //                var MessageDialog1 = new MessageDialog
        //                {
        //                    Message = { Text = "Ingrese un número como parametro de búsqueda" }
        //                };
        //                DialogHost.Show(MessageDialog1, "openup");
        //                return;
        //            }

        //            List = TList.Where(w => w.IdCambio == id).Select(s => s);
        //            distintos = TList.Where(w => w.IdCambio == id).Select(s => s.IdCambio.ToString()).Distinct().ToList();
        //            break;

        //        default:
        //            var MessageDialog = new MessageDialog
        //            {
        //                Message = { Text = "Error: Los parametros de la búsqueda son erróneos, avise al administrador" }
        //            };
        //            DialogHost.Show(MessageDialog, "openup");
        //            break;
        //    }
        //    int result = List.Count();
        //    ShowResultInStatusBar(result);
        //}

        //private void Search(string keyword, string table)
        //{
        //    if ((bool)RadioLocal.IsChecked)
        //    {
        //        switch (table)
        //        {
        //            case "ARTICULO":
        //                if (CheckEqual.IsChecked == true)
        //                {
        //                    List = List.Where(w => w.ArticuloItem == keyword).Select(s => s);
        //                    distintos = List.Where(w => w.ArticuloItem == keyword).Select(s => s.ArticuloItem).Distinct().ToList();
        //                }
        //                else
        //                {

        //                    List = List.Where(w => w.ArticuloItem.Contains(keyword)).Select(s => s);
        //                    distintos = List.Where(w => w.EstadoCambio.Contains(keyword)).Select(s => s.EstadoCambio).Distinct().ToList();
        //                }
        //                break;

        //            case "NUMERO DE PEDIDO":
        //                if (CheckEqual.IsChecked == true)
        //                {
        //                    List = List.Where(w => w.NumeroPedido == keyword).Select(s => s);
        //                    distintos = List.Where(w => w.NumeroPedido == keyword).Select(s => s.NumeroPedido).Distinct().ToList();
        //                }
        //                else
        //                {
        //                    List = List.Where(w => w.NumeroPedido.Contains(keyword)).Select(s => s);
        //                    distintos = List.Where(w => w.NumeroPedido.Contains(keyword)).Select(s => s.NumeroPedido).Distinct().ToList();
        //                }
        //                break;

        //            case "CATEGORIA":
        //                if (CheckEqual.IsChecked == true)
        //                {
        //                    List = List.Where(w => w.CategoriaItem == keyword).Select(s => s);
        //                    distintos = List.Where(w => w.CategoriaItem == keyword).Select(s => s.CategoriaItem).Distinct().ToList();
        //                }
        //                else
        //                {

        //                    List = List.Where(w => w.CategoriaItem.Contains(keyword)).Select(s => s);
        //                    distintos = List.Where(w => w.CategoriaItem.Contains(keyword)).Select(s => s.CategoriaItem).Distinct().ToList();
        //                }
        //                break;

        //            case "MODELO":
        //                if (CheckEqual.IsChecked == true)
        //                {
        //                    List = List.Where(w => w.Modelo == keyword).Select(s => s);
        //                    distintos = List.Where(w => w.Modelo == keyword).Select(s => s.Modelo).Distinct().ToList();
        //                }
        //                else
        //                {
        //                    List = List.Where(w => w.Modelo.Contains(keyword)).Select(s => s);
        //                    distintos = List.Where(w => w.Modelo.Contains(keyword)).Select(s => s.Modelo).Distinct().ToList();
        //                }
        //                break;

        //            case "PRODUCTO":
        //                if (CheckEqual.IsChecked == true)
        //                {
        //                    List = List.Where(w => w.Producto == keyword).Select(s => s);
        //                    distintos = List.Where(w => w.Producto == keyword).Select(s => s.Producto).Distinct().ToList();
        //                }
        //                else
        //                {
        //                    List = List.Where(w => w.Producto.Contains(keyword)).Select(s => s);
        //                    distintos = List.Where(w => w.Producto.Contains(keyword)).Select(s => s.Producto).Distinct().ToList();
        //                }
        //                break;

        //            case "VERSION":
        //                if (CheckEqual.IsChecked == true)
        //                {
        //                    List = List.Where(w => w.VersionItem == keyword).Select(s => s);
        //                    distintos = List.Where(w => w.VersionItem == keyword).Select(s => s.VersionItem).Distinct().ToList();
        //                }
        //                else
        //                {
        //                    List = List.Where(w => w.VersionItem.Contains(keyword)).Select(s => s);
        //                    distintos = List.Where(w => w.VersionItem.Contains(keyword)).Select(s => s.VersionItem).Distinct().ToList();
        //                }
        //                break;

        //            case "DESCRIPCION DE ITEM":
        //                if (CheckEqual.IsChecked == true)
        //                {
        //                    List = List.Where(w => w.DescripcionItem == keyword).Select(s => s);
        //                    distintos = List.Where(w => w.DescripcionItem == keyword).Select(s => s.DescripcionItem).Distinct().ToList();
        //                }
        //                else
        //                {
        //                    List = List.Where(w => w.DescripcionItem.Contains(keyword)).Select(s => s);
        //                    distintos = List.Where(w => w.DescripcionItem.Contains(keyword)).Select(s => s.DescripcionItem).Distinct().ToList();
        //                }
        //                break;

        //            case "SECTOR CAMBIO":
        //                if (CheckEqual.IsChecked == true)
        //                {
        //                    List = List.Where(w => w.SectorCambio == keyword).Select(s => s);
        //                    distintos = List.Where(w => w.SectorCambio == keyword).Select(s => s.SectorCambio).Distinct().ToList();
        //                }
        //                else
        //                {
        //                    List = List.Where(w => w.SectorCambio.Contains(keyword)).Select(s => s);
        //                    distintos = List.Where(w => w.SectorCambio.Contains(keyword)).Select(s => s.SectorCambio).Distinct().ToList();
        //                }
        //                break;

        //            case "LEGAJO":
        //                if (CheckEqual.IsChecked == true)
        //                {
        //                    List = List.Where(w => w.Legajo == keyword).Select(s => s);
        //                    distintos = List.Where(w => w.Legajo == keyword).Select(s => s.Legajo).Distinct().ToList();
        //                }
        //                else
        //                {
        //                    List = List.Where(w => w.Legajo.Contains(keyword)).Select(s => s);
        //                    distintos = List.Where(w => w.Legajo.Contains(keyword)).Select(s => s.Legajo).Distinct().ToList();
        //                }
        //                break;

        //            case "TECNICO":
        //                if (CheckEqual.IsChecked == true)
        //                {
        //                    List = List.Where(w => w.Tecnico == keyword).Select(s => s);
        //                    distintos = List.Where(w => w.Tecnico == keyword).Select(s => s.Tecnico).Distinct().ToList();
        //                }
        //                else
        //                {
        //                    List = List.Where(w => w.Tecnico.Contains(keyword)).Select(s => s);
        //                    distintos = List.Where(w => w.Tecnico.Contains(keyword)).Select(s => s.Tecnico).Distinct().ToList();
        //                }
        //                break;

        //            case "CODIGO DE FALLA":
        //                if (CheckEqual.IsChecked == true)
        //                {
        //                    List = List.Where(w => w.CodigoFalla == keyword).Select(s => s);
        //                    distintos = List.Where(w => w.CodigoFalla == keyword).Select(s => s.CodigoFalla).Distinct().ToList();
        //                }
        //                else
        //                {
        //                    List = List.Where(w => w.CodigoFalla.Contains(keyword)).Select(s => s);
        //                    distintos = List.Where(w => w.CodigoFalla.Contains(keyword)).Select(s => s.CodigoFalla).Distinct().ToList();
        //                }
        //                break;

        //            case "DESCRIPCION DE FALLA":
        //                if (CheckEqual.IsChecked == true)
        //                {
        //                    List = List.Where(w => w.DescripcionFalla == keyword).Select(s => s);
        //                    distintos = List.Where(w => w.DescripcionFalla == keyword).Select(s => s.DescripcionFalla).Distinct().ToList();
        //                }
        //                else
        //                {
        //                    List = List.Where(w => w.DescripcionFalla.Contains(keyword)).Select(s => s);
        //                    distintos = List.Where(w => w.DescripcionFalla.Contains(keyword)).Select(s => s.DescripcionFalla).Distinct().ToList();
        //                }
        //                break;

        //            case "OBSERVACIONES":
        //                if (CheckEqual.IsChecked == true)
        //                {
        //                    List = List.Where(w => w.Observaciones == keyword).Select(s => s);
        //                    distintos = List.Where(w => w.Observaciones == keyword).Select(s => s.Observaciones).Distinct().ToList();
        //                }
        //                else
        //                {
        //                    List = List.Where(w => w.Observaciones.Contains(keyword)).Select(s => s);
        //                    distintos = List.Where(w => w.Observaciones.Contains(keyword)).Select(s => s.Observaciones).Distinct().ToList();
        //                }
        //                break;

        //            case "ESTADO DE CAMBIO":
        //                if (CheckEqual.IsChecked == true)
        //                {
        //                    List = List.Where(w => w.EstadoCambio == keyword).Select(s => s);
        //                    distintos = List.Where(w => w.EstadoCambio == keyword).Select(s => s.EstadoCambio).Distinct().ToList();
        //                }
        //                else
        //                {
        //                    List = List.Where(w => w.EstadoCambio.Contains(keyword)).Select(s => s);
        //                    distintos = List.Where(w => w.EstadoCambio.Contains(keyword)).Select(s => s.EstadoCambio).Distinct().ToList();
        //                }
        //                break;

        //            case "ID DE CAMBIO":
        //                int id;
        //                try
        //                {
        //                    id = Int32.Parse(keyword);
        //                }
        //                catch (FormatException)
        //                {
        //                    var MessageDialog1 = new MessageDialog
        //                    {
        //                        Message = { Text = "Ingrese un número como parametro de búsqueda" }
        //                    };
        //                    DialogHost.Show(MessageDialog1, "openup");
        //                    return;
        //                }
        //                List = List.Where(w => w.IdCambio == id).Select(s => s);
        //                distintos = List.Where(w => w.IdCambio == id).Select(s => s.IdCambio.ToString()).Distinct().ToList();
        //                break;

        //            default:
        //                var MessageDialog = new MessageDialog
        //                {
        //                    Message = { Text = "Error: Los parametros de la búsqueda son erróneos, avise al administrador" }
        //                };
        //                DialogHost.Show(MessageDialog, "openup");
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        Search(Keyword, Table, InitialDate, EndDate);
        //    }
        //    int result = List.Count();
        //    ShowResultInStatusBar(result);
        //}

        //private void Search(DateTime? init, DateTime? end)
        //{

        //    if ((bool)RadioLocal.IsChecked)
        //    {
        //        List = List.Where(w => w.FechaCambio >= init && w.FechaCambio <= end);
        //        return;
        //    }
        //    else
        //    {
        //        PRDB context = new PRDB();
        //        List = context.Cambio.Where(w => w.FechaCambio >= init && w.FechaCambio <= end);
        //    }
        //    int result = List.Count();
        //    ShowResultInStatusBar(result);
        //}

        //private void BuscarTodo()
        //{
        //    PRDB context = new PRDB();
        //    List = context.Cambio.Select(s => s);
        //    int result = List.Count();
        //    ShowResultInStatusBar(result);
        //}

        //private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    if (TextBoxSearchString.Text == "Buscar...")
        //    {
        //        TextBoxSearchString.Text = "";
        //    }

        //    TextBoxSearchString.Foreground = new SolidColorBrush(Colors.Black);
        //}

        //private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    if (string.IsNullOrEmpty(TextBoxSearchString.Text))
        //    {
        //        TextBoxSearchString.Text = "Buscar...";
        //        TextBoxSearchString.Foreground = new SolidColorBrush(Colors.Gray);
        //    }
        //}

        //public class WaitCursor : IDisposable
        //{

        //    private Cursor _previousCursor;

        //    public WaitCursor()
        //    {
        //        _previousCursor = Mouse.OverrideCursor;

        //        Mouse.OverrideCursor = Cursors.Wait;
        //    }

        //    #region IDisposable Members

        //    public void Dispose()
        //    {
        //        Mouse.OverrideCursor = _previousCursor;
        //    }

        //    #endregion
        //}

        //public static bool SimplePing()
        //{
        //    //string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PRDB"].ConnectionString.ToString();
        //    //string HostName = connectionString.Between("data source=", ";initial");
        //    //try
        //    //{
        //    //    IPAddress[] ip = Dns.GetHostAddresses(HostName);
        //    //    Ping pingSender = new Ping();
        //    //    PingReply reply = pingSender.Send(ip[0]);
        //    //    if (reply.Status == IPStatus.Success)
        //    //    {
        //    //        return true;
        //    //    }
        //    //    else
        //    //    {
        //    //        return false;
        //    //    }
        //    //}
        //    //catch (Exception)
        //    //{
        //    //    return false;
        //    //}
        //    return true;
        //}

        //private void ShowResultInStatusBar(int result)
        //{

        //    string field = ComboBoxTable.SelectedItem.ToString();
        //    string[] initdate = DateInit.SelectedDate.ToString().Split();
        //    string[] enddate = DateEnd.SelectedDate.ToString().Split();
        //    string keyword = TextBoxSearchString.Text;
        //    if (DateInit.SelectedDate == null)
        //    {
        //        initdate[0] = "INICIO DE REGISTROS (01/01/2018)";
        //    }

        //    if (DateEnd.SelectedDate == null)
        //    {
        //        enddate[0] = "HOY";
        //    }

        //    if (string.IsNullOrWhiteSpace(keyword) || keyword == "Buscar...")
        //    {
        //        keyword = "*";
        //    }
        //    else
        //    {
        //        keyword = TextBoxSearchString.Text.ToUpper();
        //    }
        //    if (initdate == null)
        //    {
        //        TextBlockStatusResult.Text = ("ULTIMA BÚSQUEDA EN '" + ComboBoxSector.SelectedItem.ToString() + "': Se encontró un total de " + result + " registro(s) de '" + field + "' '" + keyword + "' ingresados desde la fecha de creación de la base de datos.");
        //    }
        //    else
        //    {
        //        if (CheckToday.IsChecked == true)
        //        {
        //            TextBlockStatusResult.Text = ("ULTIMA BÚSQUEDA EN '" + ComboBoxSector.SelectedItem.ToString() + "': Se encontró un total de " + result + " registro(s) de '" + field + "' '" + keyword + "' ingresados en el dia de hoy.");
        //        }
        //        else
        //        {
        //            TextBlockStatusResult.Text = ("ULTIMA BÚSQUEDA EN '" + ComboBoxSector.SelectedItem.ToString() + "': Se encontró un total de " + result + " registro(s) de '" + field + "' '" + keyword + "' ingresados entre el " + initdate[0] + " y el " + enddate[0]);
        //        }

        //    }
        //}

        //private void ButtonResetDate_Click(object sender, RoutedEventArgs e)
        //{
        //    DateInit.SelectedDate = null;
        //    DateEnd.SelectedDate = null;
        //}

        //private void Export_Click(object sender, RoutedEventArgs e)
        //{
        //    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
        //    dlg.FileName = "Resultados de Busqueda"; // Default file name
        //    dlg.DefaultExt = ".xslx"; // Default file extension
        //    dlg.Filter = "Documentos Excel (.xlsx)|*.xlsx"; // Filter files by extension

        //    Nullable<bool> result = dlg.ShowDialog();

        //    if (result == true)
        //    {
        //        // Save document
        //        string filename = dlg.FileName;

        //        try
        //        {
        //            ExportDataSet(filename);
        //            var MessageDialog = new MessageDialog
        //            {
        //                Message = { Text = "El archivo se guardó en: " + Environment.NewLine + filename }
        //            };
        //            DialogHost.Show(MessageDialog, "openup");
        //        }
        //        catch (Exception)
        //        {
        //            var MessageDialog = new MessageDialog
        //            {
        //                Message = { Text = "Error guardando el archivo: " + Environment.NewLine + filename }
        //            };
        //            DialogHost.Show(MessageDialog, "openup");
        //        }
        //    }
        //}

        //private void ExportDataSet(string destination)
        //{
        //    var workbook = new XLWorkbook();
        //    DataTable dt;

        //    if (ComboBoxSector.SelectedIndex == 0)
        //    {
        //        dt = LINQToDataTable(List);
        //    }
        //    else
        //    {
        //        dt = LINQToDataTable(TList2);
        //    }

        //    var worksheet = workbook.Worksheets.Add("Cambios Produccion");
        //    worksheet.Cell(1, 1).InsertTable(dt);
        //    worksheet.Columns().AdjustToContents();

        //    workbook.SaveAs(destination);
        //    workbook.Dispose();
        //}

        //private DataTable LINQToDataTable<T>(IQueryable<T> varlist)
        //{
        //    DataTable dtReturn = new DataTable();

        //    // column names
        //    PropertyInfo[] oProps = null;

        //    if (varlist == null) return dtReturn;

        //    foreach (T rec in varlist)
        //    {
        //        // Use reflection to get property names, to create table, Only first time, others will follow
        //        if (oProps == null)
        //        {
        //            oProps = ((Type)rec.GetType()).GetProperties();
        //            foreach (PropertyInfo pi in oProps)
        //            {
        //                Type colType = pi.PropertyType;

        //                if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
        //                {
        //                    colType = colType.GetGenericArguments()[0];
        //                }

        //                dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
        //            }
        //        }
        //        DataRow dr = dtReturn.NewRow();

        //        foreach (PropertyInfo pi in oProps)
        //        {
        //            dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue(rec, null);
        //        }

        //        dtReturn.Rows.Add(dr);
        //    }
        //    return dtReturn;
        //}

        //private void CheckToday_Checked(object sender, RoutedEventArgs e)
        //{
        //    DateInit.IsEnabled = false;
        //    DateEnd.IsEnabled = false;
        //    CheckEver.IsChecked = false;
        //    DateInit.IsEnabled = false;
        //    DateEnd.IsEnabled = false;
        //    DateInit.SelectedDate = DateTime.Now;
        //    DateEnd.SelectedDate = DateTime.Now;

        //}

        //private void CheckToday_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    DateInit.IsEnabled = true;
        //    DateEnd.IsEnabled = true;
        //}

        //private void ComboBoxSector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (FirstRun == true)
        //    {
        //        FirstRun = false;
        //        return;
        //    }

        //    if (DataGrid.ItemsSource == null)
        //    {
        //        var MessageDialog = new MessageDialog
        //        {
        //            Message = { Text = "Realice una búsqueda en la Base de Datos primero!" }
        //        };
        //        DialogHost.Show(MessageDialog, "openup");
        //        return;
        //    }

        //    using (new WaitCursor())
        //    {
        //        if (ComboBoxSector.SelectedItem.ToString() == "TODOS LOS SECTORES")
        //        {
        //            DataGrid.ItemsSource = List.ToList();
        //            int result = List.Count();
        //            ShowResultInStatusBar(result);
        //        }
        //        else
        //        {
        //            TList2 = List.Where(w => w.SectorCambio == ComboBoxSector.SelectedItem.ToString());
        //            DataGrid.ItemsSource = TList2.ToList();
        //            int result = TList2.Count();
        //            ShowResultInStatusBar(result);
        //        }
        //    }
        //}

        //private async void Cancel_Click(object sender, RoutedEventArgs e)
        //{
        //    if (DataGrid.SelectedItem == null)
        //    {
        //        var MessageDialog = new MessageDialog
        //        {
        //            Message = { Text = "Seleccione el Item que desea cambiar!" }
        //        };
        //        await DialogHost.Show(MessageDialog, "openup");
        //        return;
        //    }
        //    //////////////////////////////////////////////////////////////////////////////////////
        //    ////////////////////////////////////////////////////////////////////////////////////////
        //    /////////////////////////////////////////////////////////////////////////////////////////
        //    ////////////////////////////////////////////////////////////////////////////////////////
        //    /////////////////////////////////////////////////////////////////////////////////////////
        //    var dialogX = await DialogHost.Show(new PasswordWindow(), "openup", dialogClosingEH);

        //}

        //private void dialogClosingEH(object sender, DialogClosingEventArgs eventArgs)
        //{
        //    Console.WriteLine("SAMPLE 1: Closing dialog with parameter: " + (eventArgs.Parameter ?? ""));

        //    //you can cancel the dialog close:
        //    //eventArgs.Cancel();

        //    //MessageBox.Show(eventArgs);
        //    if (!Equals(eventArgs.Parameter, true))
        //    {
        //        using (new WaitCursor())
        //        {
        //            if (SimplePing() == false)
        //            {
        //                //MessageBox.Show("No se encontró el servidor." + Environment.NewLine + "Revise la conexión con la Base de Datos y reintente.", "Conectando al servidor", MessageBoxButton.OK, MessageBoxImage.Warning);
        //                DialogHost.Show("No se encontró el servidor." + Environment.NewLine + "Revise la conexión con la Base de Datos y reintente.", "Conectando al servidor");
        //                return;
        //            }

        //            string obsEst = "APROBADO";
        //            string nuevoEstado;
        //            DateTime dateEst = DateTime.Now;

        //            foreach (var item in DataGrid.SelectedItems)
        //            {
        //                try
        //                {
        //                    Cambio cambioSeleccionado = (Cambio)item;
        //                    int idSeleccionada = cambioSeleccionado.IdCambio;
        //                    PRDB context = new PRDB();
        //                    Cambio c = (from x in context.Cambio
        //                                where x.IdCambio == idSeleccionada
        //                                select x).First();


        //                    if (c.EstadoCambio == "APROBADO")
        //                    {
        //                        nuevoEstado = "CANCELADO";
        //                    }
        //                    else
        //                    {
        //                        nuevoEstado = "APROBADO";
        //                    }


        //                    c.SupervisorModificacion = "admin";
        //                    c.FechaModificacion = dateEst;
        //                    c.EstadoCambio = nuevoEstado;
        //                    context.SaveChanges();
        //                }
        //                catch (Exception)
        //                {
        //                    //MessageBox.Show("Error cambiando estado!", "Cambio de Estado", MessageBoxButton.OK, MessageBoxImage.Error);
        //                    DialogHost.Show("Error cambiando estado!");
        //                }
        //            }
        //            //MessageBox.Show("Correcto!" + Environment.NewLine + "Refresque la Busqueda para visualizar el cambio de estado!", "Cambio de Estado", MessageBoxButton.OK, MessageBoxImage.Information);
        //            DialogHost.Show("Correcto!" + Environment.NewLine + "Refresque la Busqueda para visualizar el cambio de estado!");
        //        }
        //    }
        //    else
        //    {
        //        return;
        //    }
        //}

        //private void Stats_Click(object sender, RoutedEventArgs e)
        //{
        //    if (List == null)
        //    {
        //        var MessageDialog = new MessageDialog
        //        {
        //            Message = { Text = "Realice una búsqueda en la Base de Datos primero!" }
        //        };
        //        DialogHost.Show(MessageDialog, "openup");
        //        RadioGlobal.IsChecked = true;
        //        return;
        //    }
        //    else
        //    {

        //        if (distintos.Count == 1)
        //        {
        //            int countedList = List.Count();
        //            DialogHost.Show(new StatsWindow(countedList, Keyword, Table, InitialDate, EndDate), "openup");
        //        }
        //        else
        //        {
        //            string dist = null;
        //            foreach (var item in distintos)
        //            {
        //                dist = dist + "- " + item.ToString() + Environment.NewLine;
        //            }
        //            var MessageDialog = new MessageDialog
        //            {
        //                Message = { Text = "Hay mas de un resultado para: " + Table + Environment.NewLine + Environment.NewLine + "Por favor, filtre la lista haciendo una búsqueda exacta de uno de los siguientes:" + Environment.NewLine + Environment.NewLine + dist }
        //            };
        //            DialogHost.Show(MessageDialog, "openup");
        //        }
        //    }
        //}

        //private void CheckEver_Checked(object sender, RoutedEventArgs e)
        //{
        //    CheckToday.IsChecked = false;
        //    DateInit.SelectedDate = null;
        //    DateEnd.SelectedDate = null;
        //    DateInit.IsEnabled = false;
        //    DateEnd.IsEnabled = false;
        //    DateInit.SelectedDate = DateTime.Parse("01/01/2018");
        //    DateEnd.SelectedDate = DateTime.Now;
        //}

        //private void CheckEver_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    DateInit.IsEnabled = true;
        //    DateEnd.IsEnabled = true;
        //}

        //private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (DataGrid.Items.Count < 0)
        //    {
        //        return;
        //    }

        //    if (DataGrid.SelectedItem == null)
        //    {
        //        Cancel.IsEnabled = false;
        //        return;
        //    }

        //    if (DataGrid.SelectedItem.Equals(-1))
        //    {
        //        Cancel.IsEnabled = false;
        //    }
        //    else
        //    {
        //        Cancel.IsEnabled = true;
        //    }
        //}
    }
}
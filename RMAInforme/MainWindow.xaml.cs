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
using System.Collections.Generic;
using System.Reflection;

namespace RMAInforme
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IQueryable<Cambio> List;
        private bool FirstRun = true;

        public MainWindow()
        {
            InitializeComponent();

            RadioGlobal.IsChecked = true;
            TextBoxSearchString.Foreground = new SolidColorBrush(Colors.Gray);
            ComboBoxTable.Items.Add("ARTICULO");
            ComboBoxTable.Items.Add("NUMERO DE PEDIDO");
            ComboBoxTable.Items.Add("CATEGORIA");
            ComboBoxTable.Items.Add("MODELO");
            ComboBoxTable.Items.Add("PRODUCTO");
            ComboBoxTable.Items.Add("VERSION");
            ComboBoxTable.Items.Add("DESCRIPCION DE ITEM");
            ComboBoxTable.Items.Add("UUID");
            ComboBoxTable.Items.Add("LEGAJO");
            ComboBoxTable.Items.Add("TECNICO");
            ComboBoxTable.Items.Add("CODIGO DE FALLA");
            ComboBoxTable.Items.Add("DESCRIPCION DE FALLA");
            ComboBoxTable.Items.Add("OBSERVACIONES");
            ComboBoxTable.Items.Add("ESTADO DE CAMBIO");

            ComboBoxSector.Items.Add("TODOS LOS SECTORES");
            ComboBoxSector.Items.Add("PRODUCCION");
            ComboBoxSector.Items.Add("SERVICIO TECNICO");
            ComboBoxSector.SelectedIndex = 0;
            ComboBoxTable.SelectedIndex = 0;
            CheckToday.IsChecked = true;

        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            using (new WaitCursor())
            {
                ComboBoxSector.SelectedIndex = 0;

                if (SimplePing() == false)
                {
                    MessageBox.Show("No se encontró el servidor." + Environment.NewLine + "Revise la conexión con la Base de Datos y reintente.", "Conectando al servidor", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if ((bool)RadioLocal.IsChecked && List == null)
                {
                    MessageBox.Show("Realice una búsqueda en la Base de Datos primero!", "Buscar...", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    RadioGlobal.IsChecked = true;
                    return;
                }

                if (DateInit.SelectedDate == null && DateEnd.SelectedDate != null)
                {
                    MessageBox.Show("Ingrese fecha de inicio!", "Buscar...", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (DateInit.SelectedDate != null && DateEnd.SelectedDate == null)
                {
                    MessageBox.Show("Ingrese fecha final!", "Buscar...", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (DateInit.SelectedDate > DateEnd.SelectedDate)
                {
                    MessageBox.Show("La fecha de inicio no puede ser mayor a la final!", "Buscar...", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                string Keyword = TextBoxSearchString.Text;
                if (Keyword == "Buscar...")
                {
                    Keyword = null;
                }
                string Table = ComboBoxTable.SelectedValue.ToString();
                DateTime? InitialDate = DateInit.SelectedDate;
                DateTime? EndDate = DateEnd.SelectedDate;

                if (string.IsNullOrWhiteSpace(Keyword))
                {
                    if (InitialDate == null && EndDate == null)
                    {
                        if ((bool)RadioGlobal.IsChecked)
                        {
                            if (MessageBox.Show("Esta a punto de buscar TODOS los registros de la Base de Datos, esto puede llevar un tiempo prolongado." + Environment.NewLine + Environment.NewLine + "Esta seguro?", "Buscar...", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                            {
                                BuscarTodo();
                            }
                        }
                    }
                    else
                    {
                        Search(InitialDate, EndDate);
                    }
                }
                else
                {
                    if (InitialDate == null && EndDate == null)
                    {
                        Search(Keyword, Table);
                    }
                    else
                    {
                        Search(Keyword, Table, InitialDate, EndDate);
                    }
                }
                try
                {
                    if (List.FirstOrDefault() == null)
                    {
                        Export.IsEnabled = false;
                        MessageBox.Show("La búsqueda no obtuvo resultados!", "Buscar...", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else
                    {
                        DataGrid.ItemsSource = List.ToList();
                        Export.IsEnabled = true;
                        ComboBoxSector.IsEnabled = true;
                        
                    }
                }
                catch (ArgumentNullException)
                {
                    return;
                }
            }
        }

        private void Search(string keyword, string table, DateTime? init, DateTime? end)
        {
            IQueryable<Cambio> TList;

            if ((bool)RadioLocal.IsChecked)
            {
                TList = List.Where(w => w.FechaCambio >= init && w.FechaCambio <= end);
            }
            else
            {
                PRDB context = new PRDB();
                TList = context.Cambio.Where(w => w.FechaCambio >= init && w.FechaCambio <= end);
            }

            switch (table)
            {
                case "ARTICULO":
                    List = TList.Where(w => w.ArticuloItem.Contains(keyword)).Select(s => s);
                    break;
                case "NUMERO DE PEDIDO":
                    List = TList.Where(w => w.NumeroPedido.Contains(keyword)).Select(s => s);
                    break;
                case "CATEGORIA":
                    List = TList.Where(w => w.CategoriaItem.Contains(keyword)).Select(s => s);
                    break;
                case "MODELO":
                    List = TList.Where(w => w.Modelo.Contains(keyword)).Select(s => s);
                    break;
                case "PRODUCTO":
                    List = TList.Where(w => w.Producto.Contains(keyword)).Select(s => s);
                    break;
                case "VERSION":
                    List = TList.Where(w => w.VersionItem.Contains(keyword)).Select(s => s);
                    break;
                case "DESCRIPCION DE ITEM":
                    List = TList.Where(w => w.DescripcionItem.Contains(keyword)).Select(s => s);
                    break;
                case "UUID":
                    List = TList.Where(w => w.UUID.Contains(keyword)).Select(s => s);
                    break;
                case "LEGAJO":
                    List = TList.Where(w => w.Legajo.Contains(keyword)).Select(s => s);
                    break;
                case "TECNICO":
                    List = TList.Where(w => w.Tecnico.Contains(keyword)).Select(s => s);
                    break;
                case "CODIGO DE FALLA":
                    List = TList.Where(w => w.CodigoFalla.Contains(keyword)).Select(s => s);
                    break;
                case "DESCRIPCION DE FALLA":
                    List = TList.Where(w => w.DescripcionFalla.Contains(keyword)).Select(s => s);
                    break;
                case "OBSERVACIONES":
                    List = TList.Where(w => w.Observaciones.Contains(keyword)).Select(s => s);
                    break;
                case "ESTADO DE CAMBIO":
                    List = TList.Where(w => w.EstadoCambio.Contains(keyword)).Select(s => s);
                    break;
                default:
                    //ERROR
                    break;
            }
            int result = List.Count();
            ShowResultInStatusBar(result);
        }

        private void Search(string keyword, string table)
        {
            if ((bool)RadioLocal.IsChecked)
            {
                switch (table)
                {
                    case "ARTICULO":
                        List = List.Where(w => w.ArticuloItem.Contains(keyword)).Select(s => s);
                        break;
                    case "NUMERO DE PEDIDO":
                        List = List.Where(w => w.NumeroPedido.Contains(keyword)).Select(s => s);
                        break;
                    case "CATEGORIA":
                        List = List.Where(w => w.CategoriaItem.Contains(keyword)).Select(s => s);
                        break;
                    case "MODELO":
                        List = List.Where(w => w.Modelo.Contains(keyword)).Select(s => s);
                        break;
                    case "PRODUCTO":
                        List = List.Where(w => w.Producto.Contains(keyword)).Select(s => s);
                        break;
                    case "VERSION":
                        List = List.Where(w => w.VersionItem.Contains(keyword)).Select(s => s);
                        break;
                    case "DESCRIPCION DE ITEM":
                        List = List.Where(w => w.DescripcionItem.Contains(keyword)).Select(s => s);
                        break;
                    case "UUID":
                        List = List.Where(w => w.UUID.Contains(keyword)).Select(s => s);
                        break;
                    case "LEGAJO":
                        List = List.Where(w => w.Legajo.Contains(keyword)).Select(s => s);
                        break;
                    case "TECNICO":
                        List = List.Where(w => w.Tecnico.Contains(keyword)).Select(s => s);
                        break;
                    case "CODIGO DE FALLA":
                        List = List.Where(w => w.CodigoFalla.Contains(keyword)).Select(s => s);
                        break;
                    case "DESCRIPCION DE FALLA":
                        List = List.Where(w => w.DescripcionFalla.Contains(keyword)).Select(s => s);
                        break;
                    case "OBSERVACIONES":
                        List = List.Where(w => w.Observaciones.Contains(keyword)).Select(s => s);
                        break;
                    case "ESTADO DE CAMBIO":
                        List = List.Where(w => w.EstadoCambio.Contains(keyword)).Select(s => s);
                        break;
                    default:
                        //ERROR
                        break;
                }
            }
            else
            {
                PRDB context = new PRDB();
                switch (table)
                {
                    case "ARTICULO":
                        List = context.Cambio.Where(w => w.ArticuloItem.Contains(keyword)).Select(s => s);
                        break;
                    case "NUMERO DE PEDIDO":
                        List = context.Cambio.Where(w => w.NumeroPedido.Contains(keyword)).Select(s => s);
                        break;
                    case "CATEGORIA":
                        List = context.Cambio.Where(w => w.CategoriaItem.Contains(keyword)).Select(s => s);
                        break;
                    case "MODELO":
                        List = context.Cambio.Where(w => w.Modelo.Contains(keyword)).Select(s => s);
                        break;
                    case "PRODUCTO":
                        List = context.Cambio.Where(w => w.Producto.Contains(keyword)).Select(s => s);
                        break;
                    case "VERSION":
                        List = context.Cambio.Where(w => w.VersionItem.Contains(keyword)).Select(s => s);
                        break;
                    case "DESCRIPCION DE ITEM":
                        List = context.Cambio.Where(w => w.DescripcionItem.Contains(keyword)).Select(s => s);
                        break;
                    case "UUID":
                        List = context.Cambio.Where(w => w.UUID.Contains(keyword)).Select(s => s);
                        break;
                    case "LEGAJO":
                        List = context.Cambio.Where(w => w.Legajo.Contains(keyword)).Select(s => s);
                        break;
                    case "TECNICO":
                        List = context.Cambio.Where(w => w.Tecnico.Contains(keyword)).Select(s => s);
                        break;
                    case "CODIGO DE FALLA":
                        List = context.Cambio.Where(w => w.CodigoFalla.Contains(keyword)).Select(s => s);
                        break;
                    case "DESCRIPCION DE FALLA":
                        List = context.Cambio.Where(w => w.DescripcionFalla.Contains(keyword)).Select(s => s);
                        break;
                    case "OBSERVACIONES":
                        List = context.Cambio.Where(w => w.Observaciones.Contains(keyword)).Select(s => s);
                        break;
                    case "ESTADO DE CAMBIO":
                        List = context.Cambio.Where(w => w.EstadoCambio.Contains(keyword)).Select(s => s);
                        break;
                    default:
                        //ERROR
                        break;
                }
            }
            int result = List.Count();
            ShowResultInStatusBar(result);
        }

        private void Search(DateTime? init, DateTime? end)
        {
            if ((bool)RadioLocal.IsChecked)
            {
                List = List.Where(w => w.FechaCambio >= init && w.FechaCambio <= end);
                return;
            }
            else
            {
                PRDB context = new PRDB();
                List = context.Cambio.Where(w => w.FechaCambio >= init && w.FechaCambio <= end);
            }
            int result = List.Count();
            ShowResultInStatusBar(result);
        }

        private void BuscarTodo()
        {
            PRDB context = new PRDB();
            List = context.Cambio.Select(s => s);
            int result = List.Count();
            ShowResultInStatusBar(result);
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxSearchString.Text == "Buscar...")
            {
                TextBoxSearchString.Text = "";
            }

            TextBoxSearchString.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxSearchString.Text))
            {
                TextBoxSearchString.Text = "Buscar...";
                TextBoxSearchString.Foreground = new SolidColorBrush(Colors.Gray);
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

            #region IDisposable Members

            public void Dispose()
            {
                Mouse.OverrideCursor = _previousCursor;
            }

            #endregion
        }

        public static bool SimplePing()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PRDB"].ConnectionString.ToString();
            string HostName = connectionString.Between("data source=", ";initial");
            try
            {
                IPAddress[] ip = Dns.GetHostAddresses(HostName);
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(ip[0]);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ShowResultInStatusBar(int result)
        {

            string field = ComboBoxTable.SelectedItem.ToString();
            string[] initdate = DateInit.SelectedDate.ToString().Split();
            string[] enddate = DateEnd.SelectedDate.ToString().Split();
            string keyword = TextBoxSearchString.Text;
            if (string.IsNullOrWhiteSpace(keyword) || keyword == "Buscar...")
            {
                keyword = "*";
            }
            else
            {
                keyword = TextBoxSearchString.Text.ToUpper();
            }
            if (initdate == null)
            {
                TextBlockStatusResult.Text = ("ULTIMA BÚSQUEDA EN '" + ComboBoxSector.SelectedItem.ToString() + "': Se encontró un total de " + result + " registro(s) de '" + field + "' '" + keyword + "' ingresados desde la fecha de creación de la base de datos.");
            }
            else
            {
                if (CheckToday.IsChecked == true)
                {
                    TextBlockStatusResult.Text = ("ULTIMA BÚSQUEDA EN '" + ComboBoxSector.SelectedItem.ToString() + "': Se encontró un total de " + result + " registro(s) de '" + field + "' '" + keyword + "' ingresados en el dia de hoy.");
                }
                else
                {
                    TextBlockStatusResult.Text = ("ULTIMA BÚSQUEDA EN '" + ComboBoxSector.SelectedItem.ToString() + "': Se encontró un total de " + result + " registro(s) de '" + field + "' '" + keyword + "' ingresados entre el " + initdate[0] + " y el " + enddate[0]);
                }

            }
        }

        private void ButtonResetDate_Click(object sender, RoutedEventArgs e)
        {
            DateInit.SelectedDate = null;
            DateEnd.SelectedDate = null;
        }

        private void Stats_Click(object sender, RoutedEventArgs e)
        {
            if (List == null)
            {
                MessageBox.Show("Realice una búsqueda en la Base de Datos primero!", "Estadisticas", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }
            string keyword = TextBoxSearchString.Text;
            string table = ComboBoxTable.SelectedValue.ToString();
            DateTime? init = DateInit.SelectedDate;
            DateTime? end = DateEnd.SelectedDate;
            StatsWindow statsw = new StatsWindow(List, keyword, table, init, end);
            statsw.ShowDialog();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Funcion no implementada aun", ":(", MessageBoxButton.OK, MessageBoxImage.Information);

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Resultados de Busqueda"; // Default file name
            dlg.DefaultExt = ".xslx"; // Default file extension
            dlg.Filter = "Documentos Excel (.xlsx)|*.xlsx"; // Filter files by extension

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;

                try
                {
                    ExportDataSet(filename);
                    MessageBox.Show("El archivo se guardó en: " + Environment.NewLine + filename, "Exportando a Excel", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (Exception)
                {
                    MessageBox.Show("Error guardando el archivo: " + Environment.NewLine + filename, "Exportando a Excel", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void ExportDataSet(string destination)
        {
            var workbook = new XLWorkbook();

            DataTable dt = LINQToDataTable(List);

            var worksheet = workbook.Worksheets.Add("Cambios Produccion");
            worksheet.Cell(1, 1).InsertTable(dt);
            worksheet.Columns().AdjustToContents();

            workbook.SaveAs(destination);
            workbook.Dispose();
        }

        private DataTable LINQToDataTable<T>(IQueryable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow
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

        private void CheckToday_Checked(object sender, RoutedEventArgs e)
        {
            DateInit.IsEnabled = false;
            DateEnd.IsEnabled = false;
            ButtonResetDate.IsEnabled = false;
            DateInit.SelectedDate = DateTime.Now.AddDays(-1);
            DateEnd.SelectedDate = DateTime.Now.AddDays(1);

        }

        private void CheckToday_Unchecked(object sender, RoutedEventArgs e)
        {
            DateInit.IsEnabled = true;
            DateEnd.IsEnabled = true;
            ButtonResetDate.IsEnabled = true;
        }

        private void ComboBoxSector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FirstRun == true)
            {
                FirstRun = false;
                return;
            }

            if (DataGrid.ItemsSource == null)
            {
                MessageBox.Show("Realice una búsqueda en la Base de Datos primero!", "Informe RMA", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            using (new WaitCursor())
            {
                //List = List.Where(w => w.Sector == ComboBoxSector.SelectedItem.ToString());
                MessageBox.Show(ComboBoxSector.SelectedItem.ToString());
            }

            int result = List.Count();
            ShowResultInStatusBar(result);
        }
    }
}

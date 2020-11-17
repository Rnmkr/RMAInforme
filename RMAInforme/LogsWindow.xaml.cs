using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RMAInforme
{
    /// <summary>
    /// Interaction logic for LogsWindow.xaml
    /// </summary>
    public partial class LogsWindow : UserControl
    {
        private string manualTestLog;
        private string burnintestLog;
        private string supObsLog;
        private string obsLog;
        private int pixelCount = 0;
        private int reparadasCount = 0;
        private string[] numeros;
        private string serverLogsRootPath = @"\\bubba\ea2100dc89ae9fe21fa9b08ab1bf18662dca1e53a3eebd7d03afebcaf5d57515$";
        private string localTempPath = Path.Combine(Path.GetTempPath(), "RMAInforme");
        private string localPedidoRootPath;
        private string serverPedidoRootPath;
        private string pedidoEnFormaDeRuta;
        private string numeroPedido;

        public LogsWindow()
        {
            InitializeComponent();
        }

        public LogsWindow(string _numeroPedido)
        {
            InitializeComponent();

            if (!string.IsNullOrWhiteSpace(_numeroPedido))
            {
                this.numeroPedido = _numeroPedido.ToUpper();
                tbKeyword.Text = this.numeroPedido;
                SearchLogs();
            }
        }

        private void TbSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(tbKeyword.Text))
                {
                    this.numeroPedido = tbKeyword.Text.ToUpper();
                    SearchLogs();
                }
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbKeyword.Text))
            {
                this.numeroPedido = tbKeyword.Text.ToUpper();
                SearchLogs();
            }
        }

        public bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        private void SearchLogs()
        {
            using (new WaitCursor())
            {
                if (string.IsNullOrWhiteSpace(this.numeroPedido)) { return; }

                pedidoEnFormaDeRuta = Path.Combine(this.numeroPedido.Substring(0, 1), this.numeroPedido.Substring(1, 3), this.numeroPedido.Substring(4, 4));
                serverPedidoRootPath = Path.Combine(serverLogsRootPath, pedidoEnFormaDeRuta);

                if (Directory.Exists(serverPedidoRootPath) && !IsDirectoryEmpty(serverPedidoRootPath))
                {
                    localPedidoRootPath = Path.Combine(localTempPath, this.numeroPedido);
                    CopyServerFilesToLocalTempDirectory();
                    ProcessLocalFilesAndSetResults();
                }
            }
        }

        private async void CopyServerFilesToLocalTempDirectory()
        {
            await Task.Run(() =>
            {
                string[] rutasServer = Directory.GetDirectories(serverPedidoRootPath, "*", SearchOption.TopDirectoryOnly);
                this.numeros = rutasServer.Select(Path.GetFileName).ToArray();
                if (this.numeros.Length > 0)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        lbNumbers.ItemsSource = numeros;
                    });

                }

                foreach (var rutaServer in rutasServer)
                {
                    var numeroDeMaquina = Path.GetFileName(rutaServer);
                    if (numeroDeMaquina.Contains('R'))
                    {
                        reparadasCount++;
                    }
                    var archivoLog = (this.numeroPedido + numeroDeMaquina.Substring(0, 5) + ".txt");

                    var supObsFileServer = Path.Combine(serverLogsRootPath, "ObservacionesSupervisor.txt");
                    var rutaArchivoLogServer = Path.Combine(rutaServer, archivoLog);
                    var rutaArchivoBitServer = Path.Combine(rutaServer, "BIT.log");
                    var rutaArchivoObsServer = Path.Combine(rutaServer, "observacion.txt");
                    var rutaArchivoPixelServer = Path.Combine(rutaServer, "pixel.trash");

                    var rutaCarpetaEquipoLocal = Path.Combine(localPedidoRootPath, numeroDeMaquina);
                    var supObsLocal = Path.Combine(localPedidoRootPath, "ObservacionesSupervisor.txt");
                    var rutaArchivoLogLocal = Path.Combine(rutaCarpetaEquipoLocal, archivoLog);
                    var rutaArchivoBitLocal = Path.Combine(rutaCarpetaEquipoLocal, "BIT.log");
                    var rutaArchivoObsLocal = Path.Combine(rutaCarpetaEquipoLocal, "observacion.txt");
                    var rutaArchivoPixelLocal = Path.Combine(rutaCarpetaEquipoLocal, "pixeles.txt");

                    try
                    {
                        Directory.CreateDirectory(rutaCarpetaEquipoLocal);


                        if (File.Exists(supObsFileServer))
                        {
                            File.Copy(supObsFileServer, supObsLocal, true);
                            obsLog = Environment.NewLine + File.ReadAllText(supObsLocal);
                        }

                        //copio archivo log 'censurado'
                        if (File.Exists(rutaArchivoLogServer))
                        {
                            var logContent = File.ReadAllText(rutaArchivoLogServer);
                            var censoredLog = RemoveKey(logContent);
                            File.WriteAllText(rutaArchivoLogLocal, censoredLog);
                        }

                        //log burnintest
                        if (File.Exists(rutaArchivoBitServer))
                        {
                            File.Copy(rutaArchivoBitServer, rutaArchivoBitLocal, true);
                        }

                        //observaciones
                        if (File.Exists(rutaArchivoObsServer))
                        {
                            File.Copy(rutaArchivoObsServer, rutaArchivoObsLocal, true);
                        }

                        //pixeles
                        if (File.Exists(rutaArchivoPixelServer))
                        {
                            File.Copy(rutaArchivoPixelServer, rutaArchivoPixelLocal, true);
                            pixelCount++;
                        }
                    }
                    catch (IOException e)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(e.Message);
                        });
                    }
                }

            });
        }

        private string RemoveKey(string text)
        {
            Regex regexKey = new Regex(@"[A-Za-z0-9]{5}-[A-Za-z0-9]{5}-[A-Za-z0-9]{5}-[A-Za-z0-9]{5}");
            var censoredText = regexKey.Replace(text, "XXXXX-XXXXX-XXXXX-XXXXX");
            return censoredText;
        }

        private void ProcessLocalFilesAndSetResults()
        {
            tbLog.Text = "SELECCIONE UN NUMERO PARA VER EL REGISTRO DE TEST MANUAL.";

            tbBit.Text = "SELECCIONE UN NUMERO PARA VER EL REGISTRO DE BURNINTEST.";

            if (string.IsNullOrWhiteSpace(obsLog))
            {
                obsLog = Environment.NewLine + "NO SE ENCONTRARON OBSERVACIONES DEL SUPERVISOR.";
            }

            tbPx.Text = "CON PIXELES: " + pixelCount.ToString();
            tbRepa.Text = "REPARADAS: " + reparadasCount.ToString();
            tbObsView.Text = obsLog;
        }

        //en el cambio de numero
        private void SetInfo(string numero)
        {

            var archivoLog = this.numeroPedido + numero.Substring(0, 5) + ".txt";
            var rutaCarpetaEquipoLocal = Path.Combine(localPedidoRootPath, numero);
            var rutaArchivoLogLocal = Path.Combine(rutaCarpetaEquipoLocal, archivoLog);
            var rutaArchivoBitLocal = Path.Combine(rutaCarpetaEquipoLocal, "BIT.log");
            var rutaArchivoObsLocal = Path.Combine(rutaCarpetaEquipoLocal, "observacion.txt");
            var rutaArchivoPixelLocal = Path.Combine(rutaCarpetaEquipoLocal, "pixeles.txt");

            if (File.Exists(rutaArchivoPixelLocal))
            {
                obsLog = Environment.NewLine + "CANTIDAD DE PIXELES:" + File.ReadAllText(rutaArchivoPixelLocal) + Environment.NewLine;
            }
            else
            {
                obsLog = Environment.NewLine + "NO SE ENCONTRÓ EL ARCHIVO DE PIXELES.";
            }

            if (File.Exists(rutaArchivoLogLocal))
            {
                manualTestLog = File.ReadAllText(rutaArchivoLogLocal);
            }
            else
            {
                manualTestLog = "NO SE ENCONTRÓ UN LOG DE TEST MANUAL PARA ESTE NUMERO.";
            }

            if (File.Exists(rutaArchivoBitLocal))
            {
                burnintestLog = File.ReadAllText(rutaArchivoBitLocal);
            }
            else
            {
                burnintestLog = "NO SE ENCONTRÓ UN LOG DE BURNINTEST PARA ESTE NUMERO.";
            }

            if (File.Exists(rutaArchivoObsLocal))
            {
                obsLog += File.ReadAllText(rutaArchivoObsLocal);
            }
            else
            {
                obsLog += "NO SE ENCONTRARON OBSERVACIONES PARA ESTE NUMERO.";
            }

            tbLog.Text = manualTestLog;
            tbBit.Text = burnintestLog;
            tbObsView.Text = obsLog;
            tbPx.Text = "CON PIXELES DAÑADOS: " + pixelCount.ToString();
            tbRepa.Text = "REPARADAS: " + reparadasCount.ToString();

        }

        private void btnSaveBit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lbNumbers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbNumbers.SelectedValue != null)
            {
                string n = lbNumbers.SelectedValue.ToString();
                SetInfo(n);
            }
        }
    }
}

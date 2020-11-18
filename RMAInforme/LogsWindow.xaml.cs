using System;
using System.Collections.Generic;
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
        private string obsLog;
        private string supObsLog;
        private int pixelCount = 0;
        private int reparadasCount = 0;
        private List<string> numeros;
        private readonly string serverLogsRootPath = @"\\bubba\ea2100dc89ae9fe21fa9b08ab1bf18662dca1e53a3eebd7d03afebcaf5d57515$";
        private readonly string localTempPath = Path.Combine(Path.GetTempPath(), "RMAInforme");
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
                tbKeyword.SelectAll();
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

        private async void SearchLogs()
        {
            using (new WaitCursor())
            {
                if (string.IsNullOrWhiteSpace(this.numeroPedido)) { return; }
                ResetVars();
                tbKeyword.Text = tbKeyword.Text.ToUpper();
                pedidoEnFormaDeRuta = Path.Combine(this.numeroPedido.Substring(0, 1), this.numeroPedido.Substring(1, 3), this.numeroPedido.Substring(4, 4));
                serverPedidoRootPath = Path.Combine(serverLogsRootPath, pedidoEnFormaDeRuta);

                if (Directory.Exists(serverPedidoRootPath) && !IsDirectoryEmpty(serverPedidoRootPath))
                {
                    await Task.Run(() =>
                    {
                        localPedidoRootPath = Path.Combine(localTempPath, this.numeroPedido);
                        CopyServerFilesToLocalTempDirectory();
                        ProcessLocalFilesAndSetResults();
                    });
                }
            }
        }

        private void ResetVars()
        {
            numeros = new List<string>();
            lbNumbers.SelectedValue = null;
            pixelCount = 0;
            reparadasCount = 0;
        }

        private void CopyServerFilesToLocalTempDirectory()
        {

            string[] rutasServer = Directory.GetDirectories(serverPedidoRootPath, "*", SearchOption.TopDirectoryOnly);
            //this.numeros = rutasServer.Select(Path.GetFileName).ToArray();

            foreach (var rutaServer in rutasServer)
            {
                var numeroDeMaquina = Path.GetFileName(rutaServer);
                if (numeroDeMaquina.Contains('R'))
                {
                    reparadasCount++;
                }

                var archivoLog = (this.numeroPedido + numeroDeMaquina.Substring(0, 5) + ".txt");
                var supObsFileServer = Path.Combine(serverPedidoRootPath, "ObservacionesSupervisor.txt");
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
                    //pixeles
                    if (File.Exists(rutaArchivoPixelServer))
                    {
                        numeroDeMaquina += "-P";
                        rutaCarpetaEquipoLocal += "-P";
                        rutaArchivoPixelLocal = Path.Combine(rutaCarpetaEquipoLocal, "pixeles.txt");
                        rutaArchivoLogLocal = Path.Combine(rutaCarpetaEquipoLocal, archivoLog);
                        rutaArchivoBitLocal = Path.Combine(rutaCarpetaEquipoLocal, "BIT.log");
                        rutaArchivoObsLocal = Path.Combine(rutaCarpetaEquipoLocal, "observacion.txt");
                        rutaArchivoPixelLocal = Path.Combine(rutaCarpetaEquipoLocal, "pixeles.txt");
                        Directory.CreateDirectory(rutaCarpetaEquipoLocal);
                        File.Copy(rutaArchivoPixelServer, rutaArchivoPixelLocal, true);
                        pixelCount++;
                    }
                    else
                    {
                        Directory.CreateDirectory(rutaCarpetaEquipoLocal);
                    }

                    //Obs de supervisor
                    if (File.Exists(supObsFileServer))
                    {
                        File.Copy(supObsFileServer, supObsLocal, true);
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

                    this.numeros.Add(numeroDeMaquina);
                }
                catch (IOException e)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(e.Message);
                    });
                }

            }

            if (this.numeros != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    lbNumbers.ItemsSource = numeros;
                });
            }
        }

        private string RemoveKey(string text)
        {
            Regex regexKey = new Regex(@"[A-Za-z0-9]{5}-[A-Za-z0-9]{5}-[A-Za-z0-9]{5}-[A-Za-z0-9]{5}");
            var censoredText = regexKey.Replace(text, "XXXXX-XXXXX-XXXXX-XXXXX");
            return censoredText;
        }

        //luego de cargar un pedido
        private void ProcessLocalFilesAndSetResults()
        {
            var supObsLocal = Path.Combine(localPedidoRootPath, "ObservacionesSupervisor.txt");

            if (File.Exists(supObsLocal))
            {
                supObsLog = File.ReadAllText(supObsLocal);
            }
            else
            {
                supObsLog = "No se encontraron observaciones del supervisor para el pedido.";
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                tbLog.Text = "Seleccione un número para ver el registro de test manual.";
                tbBit.Text = "Seleccione un número para ver el registro de Burnintest.";
                tbObs.Text = "Seleccione un número para ver las observaciones del técnico.";
                tbSupObs.Text = supObsLog;
                tbPx.Text = "Equipos con pixeles dañados: " + pixelCount.ToString();
                tbRepa.Text = "Reparaciones: " + reparadasCount.ToString();
            });
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
                obsLog = "Cantidad de pixeles dañados: " + File.ReadAllText(rutaArchivoPixelLocal) + Environment.NewLine;
            }
            else
            {
                obsLog = "Cantidad de pixeles dañados: 0" + Environment.NewLine;
            }

            if (File.Exists(rutaArchivoLogLocal))
            {
                manualTestLog = File.ReadAllText(rutaArchivoLogLocal);
            }
            else
            {
                manualTestLog = "No se encontró un log de test manual para este número.";
            }

            if (File.Exists(rutaArchivoBitLocal))
            {
                burnintestLog = File.ReadAllText(rutaArchivoBitLocal);
            }
            else
            {
                burnintestLog = "No se encontró un log de Burnintest para este número.";
            }

            if (File.Exists(rutaArchivoObsLocal))
            {
                obsLog += File.ReadAllText(rutaArchivoObsLocal);
            }
            else
            {
                obsLog += "No se encontraron observaciones del técnico para este número.";
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                tbLog.Text = manualTestLog;
                tbBit.Text = burnintestLog;
                tbObs.Text = obsLog;
                tbPx.Text = "Equipos con pixeles dañados: " + pixelCount.ToString();
                tbRepa.Text = "Reparaciones: " + reparadasCount.ToString();
            });
        }

        private void btnSaveBit_Click(object sender, RoutedEventArgs e)
        {
            GuardarLogs();
        }

        private void GuardarLogs()
        {
            if (numeroPedido != null)
            {
                try
                {
                    if ((bool)rbSaveSelected.IsChecked)
                    {
                        if (lbNumbers.SelectedValue != null)
                        {
                            var folderName = GetSaveDir();
                            if (folderName == "?") { return; }
                            var selected = lbNumbers.SelectedValue.ToString();
                            var saveDir = Path.Combine(folderName, "RMAInforme", numeroPedido + selected);
                            var folderToCopy = Path.Combine(localPedidoRootPath, selected);
                            Directory.CreateDirectory(saveDir);

                            foreach (string file in Directory.GetFiles(folderToCopy, "*.*", SearchOption.TopDirectoryOnly))
                                File.Copy(file, file.Replace(folderToCopy, saveDir), true);
                            MessageBox.Show("Registros guardados correctamente.", "Guardar Logs", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }

                    if ((bool)rbSaveAll.IsChecked)
                    {
                        var folderName = GetSaveDir();
                        if (folderName == "?") { return; }
                        var saveDir = Path.Combine(folderName, "RMAInforme", numeroPedido);
                        foreach (string dirPath in Directory.GetDirectories(localPedidoRootPath, "*", SearchOption.AllDirectories))
                            Directory.CreateDirectory(dirPath.Replace(localPedidoRootPath, saveDir));

                        foreach (string newPath in Directory.GetFiles(localPedidoRootPath, "*.*", SearchOption.AllDirectories))
                            File.Copy(newPath, newPath.Replace(localPedidoRootPath, saveDir), true);
                        MessageBox.Show("Registros guardados correctamente.", "Guardar Logs", MessageBoxButton.OK, MessageBoxImage.Information);
                    }


                }
                catch (Exception)
                {
                    MessageBox.Show("Ocurrió un error guardando los registros en la ubicación seleccionada.", "Guardar Logs", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string GetSaveDir()
        {
            System.Windows.Forms.DialogResult result;
            string folderName = "?";
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                result = dialog.ShowDialog();
                if (result.ToString() == "OK")
                {
                    folderName = dialog.SelectedPath;
                }
            }
            return folderName;
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

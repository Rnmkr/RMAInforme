using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        private string mTestLog = null;
        private string bTestLog = null;
        private string obsLog = null;
        private string pxCount = null;
        private string reCount = null;
        private List<string> nList = new List<string>();
        public LogsWindow(string numeroPedido)
        {
            InitializeComponent();

            if (!string.IsNullOrWhiteSpace(numeroPedido))
            {
                SearchLogs(numeroPedido);
                SetNullVisibility();
            }
        }

        private void TbSearchBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                SearchLogs(tbKeyword.Text);
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            SearchLogs(tbKeyword.Text);
        }

        private void SearchLogs(string numeroPedido)
        {
            using (new WaitCursor())
            {
                var temp = Path.GetTempPath();
                //TODO:
                //Ruta = obtener ruta a partir del numero de serie (oafloor hecho)
                //copiar todos los archivos al temp censurando el del nro de serie en el log del test
                //listar carpetas para seleccionar
                //"seleccione un numero de serie" en los textboxs
                //al seleccionar leer los archivos desde el temp

                //si existe  esto de abajo en el server
                //creo una carpeta en temp
                if (File.Exists(@"\\BUBBA\1\1239\114A"))
                {
                    //comprobar si existe log de supervisor y obtenerlo
                    //getnames() ?
                    string[] dirs = Directory.GetDirectories(@"C:\Users\Rnmkr\Desktop\1\139\114A", "*", SearchOption.TopDirectoryOnly);
                    string[] names = dirs.Select(Path.GetFileName).ToArray();
                    foreach (var dir in dirs)
                    {
                        var rdir = dir;
                        //si tiene "-R" sumar 1 a reparadas

                        //cantidad de pixeles pixel.trash
                        var LogPxServer = "rutadel pedido + nombredelog";
                        var LogPxTemp = "ruta al temp";
                        if (File.Exists(LogPxServer))
                        {
                            File.Copy(LogPxServer, LogPxTemp, true);
                            //+1 a una variable de maquinas con pixel
                            //renombrar carpeta a -R-P
                        }

                        //log del test
                        var LogTestServer = "rutadel pedido + nombredelog";
                        var LogTestTemp = "ruta al temp";
                        if (File.Exists(LogTestServer)) //borrar de name "-R" o cualquier otra cosa
                        {
                            var CensoredLog = RemoveKey(LogTestServer);
                            File.WriteAllText(LogTestTemp, CensoredLog);
                        }

                        //log del burning
                        var LogBitServer = "rutadel pedido + nombredelog";
                        var LogBitTemp = "ruta al temp";
                        if (File.Exists(LogBitServer))
                        {
                            File.Copy(LogBitServer, LogBitTemp, true);
                        }

                        //observacion de numero observacion.txt
                        var LogObsNumServer = "rutadel pedido + nombredelog";
                        var LogObsNumTemp = "ruta al temp";
                        if (File.Exists(LogObsNumServer)) 
                        {
                            File.Copy(LogObsNumServer, LogObsNumTemp, true);
                        }

                        nList.Add(rdir);
                    }

                    lbNumbers.ItemsSource = nList;
                }
                SetNullVisibility();
            }
        }

        private string RemoveKey(string text)
        {
            Regex regexKey = new Regex(@" ^ ([A-Za-z0-9]{5}-){4}[A-Za-z0-9]{5}$");
            var censoredText = regexKey.Replace(text, "XXXXX-XXXXX-XXXXX-XXXXX-XXXXX");
            return censoredText;
        }

        private void btnSaveBit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SetNullVisibility()
        {
            if (string.IsNullOrWhiteSpace(tbBit.Text))
            {
                tbBit.Text = "NO SE ENCONTRÓ UN LOG DE BURNINTEST PARA ESTE NUMERO.";
            }

            if (string.IsNullOrWhiteSpace(tbLog.Text))
            {
                tbLog.Text = "NO SE ENCONTRÓ UN LOG DE TEST MANUAL PARA ESTE NUMERO.";
            }

            if (string.IsNullOrWhiteSpace(obsLog))
            {
                tbObsView.Text = Environment.NewLine + "NO SE ENCONTRARON OBSERVACIONES.";
            }

            if (string.IsNullOrWhiteSpace(pxCount))
            {
                tbPx.Text = "CON PIXEL: --";
            }

            if (string.IsNullOrWhiteSpace(reCount))
            {
                tbRepa.Text = "REPARADAS: --";
            }
        }

    }
}

using System;
using System.Windows;
using LiveCharts;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Controls;

namespace RMAInforme
{
    public partial class StatsWildcard : UserControl
    {
        private SeriesCollection Chart1WildSeries;
        private SeriesCollection Chart2WildSeries;
        private SeriesCollection Chart3WildSeries;
        private string Periodo;
        public StatsWildcard(SeriesCollection chart1WildSeries, SeriesCollection chart2WildSeries, SeriesCollection chart3WildSeries, string periodo)
        {
            InitializeComponent();

            this.Chart1WildSeries = chart1WildSeries;
            this.Chart2WildSeries = chart2WildSeries;
            this.Chart3WildSeries = chart3WildSeries;
            this.Periodo = periodo;
            lblPeriodo.Content = "MOSTRANDO ESTADISTICAS PARA EL PERIODO " + Periodo;
            CargarChart1();
            CargarChart2();
            CargarChart3();
        }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        public Func<ChartPoint, string> PointLabel { get; set; }

        private void CargarChart1()
        {
            cartHardware.Series = Chart1WildSeries;
        }
        private void CargarChart2()
        {
            cartModelos.Series = Chart2WildSeries;
        }

        private void CargarChart3()
        {
            cartProductos.Series = Chart3WildSeries;
        }



        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Estadísticas de busqueda"; // Default file name
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "Archivo de imagen (.png)|*.png"; // Filter files by extension

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;

                try
                {
                    RenderTargetBitmap(filename);
                    MessageBox.Show("El archivo se guardó en: " + Environment.NewLine + filename, "Estadisticas de búsqueda", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error guardando el archivo: " + Environment.NewLine + filename, "Estadisticas de búsqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        public void RenderTargetBitmap(string _filename)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)this.grillita.ActualWidth, (int)this.grillita.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(this.grillita);
            //using (FileStream stream = File.Create(@"C:\Users\Rnmkr\Desktop\Estadisticas.jpeg"))
            using (FileStream stream = File.Create(_filename))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder(); //se puede pasar a pngencoder...
                                                                   //encoder.QualityLevel = 100;
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);
            }
        }
    }
}
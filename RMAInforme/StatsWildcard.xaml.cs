using RMAInforme.DataAccessLayer;
using System;
using System.Linq;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Controls;
using System.Collections.Generic;

namespace RMAInforme
{
    public partial class StatsWildcard : UserControl
    {
        //public StatsWildcard(string keyword, int cantidadResultadoBusqueda, int cantidadTotalItem, int cantidadTodosFechaBusqueda, int cantidadTotalTodos, int cantidadRelevante1, int cantidadRelevante2, int cantidadRelevante3, string nombreRelevante1, string nombreRelevante2, string nombreRelevante3, string campoChart3, string FechaInicial, string FechaFinal)
        public StatsWildcard()
        {
            InitializeComponent();

            PointLabel = chartPoint => string.Format("{0}", chartPoint.Y);


            CargarChart1();
            CargarChart2();
            CargarChart3();
        }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        public Func<ChartPoint, string> PointLabel { get; set; }

        private void CargarChart1()
        {
            Labels = new[] { "MB 2448", "PA 3759", "MB 1225", "PA 2976", "CO 4588", "CA 5856", "FL 8556", "SKD 1258", "SKD TOP 3716", "MB 2178" };
            //Formatter = value => value.ToString("N");
            Formatter = value => value.ToString("n");

            SeriesCollection SeriesCollection1 = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = Labels[0],
                    Values = new ChartValues<double> { 128 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[1],
                    Values = new ChartValues<double> { 114 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[2],
                    Values = new ChartValues<double> { 92 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[3],
                    Values = new ChartValues<double> { 46 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[4],
                    Values = new ChartValues<double> { 12 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[5],
                    Values = new ChartValues<double> { 12 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[6],
                    Values = new ChartValues<double> { 11 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[7],
                    Values = new ChartValues<double> { 5 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[8],
                    Values = new ChartValues<double> { 3 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

            };


            cartHardware.Series = SeriesCollection1;
            DataContext = this;

        }
        private void CargarChart2()
        {
            Labels = new[] { "EF21", "R9", "SPANKY", "READY H2", "BITSY XQ", "XS1" };
            //Formatter = value => value.ToString("N");
            Formatter = value => value.ToString("n");

            SeriesCollection SeriesCollection2 = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = Labels[0],
                    Values = new ChartValues<double> { 212 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[1],
                    Values = new ChartValues<double> { 145 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[2],
                    Values = new ChartValues<double> { 88 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[3],
                    Values = new ChartValues<double> { 61 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[4],
                    Values = new ChartValues<double> { 36 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[5],
                    Values = new ChartValues<double> { 28 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

             };


            cartModelos.Series = SeriesCollection2;
            DataContext = this;
        }

        private void CargarChart3()
        {
            Labels = new[] { "NOTEBOOK", "ALL IN ONE", "PC", "TABLET", "MINI PC", "2 EN 1"};
            //Formatter = value => value.ToString("N");
            Formatter = value => value.ToString("n");

            SeriesCollection SeriesCollection3 = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = Labels[0],
                    Values = new ChartValues<double> { 183 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[1],
                    Values = new ChartValues<double> { 110 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[2],
                    Values = new ChartValues<double> { 92 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[3],
                    Values = new ChartValues<double> { 56 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[4],
                    Values = new ChartValues<double> { 38 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                new ColumnSeries
                {
                    Title = Labels[5],
                    Values = new ChartValues<double> { 24 },
                    LabelPoint = PointLabel,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },
            };

            cartProductos.Series = SeriesCollection3;
            DataContext = this;
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
////adding series will update and animate the chart automatically
//SeriesCollection.Add(new ColumnSeries
//{
//    Title = "2016",
//    Values = new ChartValues<double> { 11, 56, 42 }
//});

////also adding values updates and animates the chart automatically
//SeriesCollection[1].Values.Add(48d);

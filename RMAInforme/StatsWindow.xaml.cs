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
    public partial class StatsWindow : UserControl
    {
        private int cantidadResultadoBusqueda;
        private int cantidadTotalItem;
        private int cantidadTodosFechaBusqueda;
        private int cantidadTotalTodos;
        private int cantidadRelevante1;
        private int cantidadRelevante2;
        private int cantidadRelevante3;
        private string campoChart3;
        private string periodoEstadisticas;
        private string nombreRelevante1;
        private string nombreRelevante2;
        private string nombreRelevante3;
        private string keyword;

        public StatsWindow(string keyword, int cantidadResultadoBusqueda, int cantidadTotalItem, int cantidadTodosFechaBusqueda, int cantidadTotalTodos, int cantidadRelevante1, int cantidadRelevante2, int cantidadRelevante3, string nombreRelevante1, string nombreRelevante2, string nombreRelevante3, string campoChart3, string periodoEstadisticas)
        {
            InitializeComponent();

            this.cantidadResultadoBusqueda = cantidadResultadoBusqueda;
            this.cantidadTotalItem = cantidadTotalItem;
            this.cantidadTodosFechaBusqueda = cantidadTodosFechaBusqueda;
            this.cantidadTotalTodos = cantidadTotalTodos;
            this.cantidadRelevante1 = cantidadRelevante1;
            this.cantidadRelevante2 = cantidadRelevante2;
            this.cantidadRelevante3 = cantidadRelevante3;
            this.campoChart3 = campoChart3;
            this.periodoEstadisticas = periodoEstadisticas;
            this.nombreRelevante1 = nombreRelevante1;
            this.nombreRelevante2 = nombreRelevante2;
            this.nombreRelevante3 = nombreRelevante3;
            this.keyword = keyword;
            
            PointLabel = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation, Brushes.Black);
            PointLabelCart = chartPoint => string.Format("{0}", chartPoint.Y);

            CargarChart1();
            CargarChart2();
            CargarChart3();
        }

        private void CargarChart1()
        {
            var restoFecha = cantidadTodosFechaBusqueda - cantidadResultadoBusqueda;

            SeriesCollection Piechart1 = new SeriesCollection
            {
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(restoFecha) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "OTROS PRODUCTOS"
                },

                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(cantidadResultadoBusqueda) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "'" + keyword + "'"
                }
            };

            Chart1Label.Text = "EN TOTAL SE REGISTRARON " + cantidadTodosFechaBusqueda + " CAMBIOS" + Environment.NewLine + periodoEstadisticas;
            PieChart1.Series = Piechart1;
        }

        private void CargarChart2()
        {
            int restoMismo = (cantidadTotalItem - cantidadResultadoBusqueda);
            if (restoMismo < 0)
            {
                restoMismo = 0;
            }

            SeriesCollection Piechart2 = new SeriesCollection
            {
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(restoMismo) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "RESTO DEL TOTAL DE '" + keyword + "'"
                },

                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(cantidadResultadoBusqueda) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "'" + keyword + "'" + periodoEstadisticas

                }
            };

            Chart2Label.Text = cantidadTotalItem + " REGISTROS DE '" + keyword + "'" + Environment.NewLine + "DESDE INICIO DE BASE DE DATOS (01/01/2018)";
            PieChart2.Series = Piechart2;
        }

        private void CargarChart3()
        {

            string[] Labels = new[] { "MB 2448", "PA 3759", "MB 1225", "PA 2976", "CO 4588", "CA 5856", "FL 8556", "SKD 1258", "SKD TOP 3716", "MB 2178" };
            //Formatter = value => value.ToString("N");
              Func<double, string> Formatter  = value => value.ToString("n");

            SeriesCollection SeriesCollection1 = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = Labels[0],
                    Values = new ChartValues<double> { 128 },
                    LabelPoint = PointLabelCart,
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
                    LabelPoint = PointLabelCart,
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
                    LabelPoint = PointLabelCart,
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
                    LabelPoint = PointLabelCart,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 11,
                    LabelsPosition = BarLabelPosition.Parallel,
                    DataLabels = true
                },

                //new ColumnSeries
                //{
                //    Title = Labels[4],
                //    Values = new ChartValues<double> { 12 },
                //    LabelPoint = PointLabelCart,
                //    Foreground = Brushes.White,
                //    FontFamily = new FontFamily("Consolas"),
                //    FontSize = 11,
                //    LabelsPosition = BarLabelPosition.Parallel,
                //    DataLabels = true
                //},

                //new ColumnSeries
                //{
                //    Title = Labels[5],
                //    Values = new ChartValues<double> { 12 },
                //    LabelPoint = PointLabelCart,
                //    Foreground = Brushes.White,
                //    FontFamily = new FontFamily("Consolas"),
                //    FontSize = 11,
                //    LabelsPosition = BarLabelPosition.Parallel,
                //    DataLabels = true
                //},

                //new ColumnSeries
                //{
                //    Title = Labels[6],
                //    Values = new ChartValues<double> { 11 },
                //    LabelPoint = PointLabelCart,
                //    Foreground = Brushes.White,
                //    FontFamily = new FontFamily("Consolas"),
                //    FontSize = 11,
                //    LabelsPosition = BarLabelPosition.Parallel,
                //    DataLabels = true
                //},

                //new ColumnSeries
                //{
                //    Title = Labels[7],
                //    Values = new ChartValues<double> { 8 },
                //    LabelPoint = PointLabelCart,
                //    Foreground = Brushes.White,
                //    FontFamily = new FontFamily("Consolas"),
                //    FontSize = 11,
                //    LabelsPosition = BarLabelPosition.Parallel,
                //    DataLabels = true
                //},

                //new ColumnSeries
                //{
                //    Title = Labels[8],
                //    Values = new ChartValues<double> { 5 },
                //    LabelPoint = PointLabelCart,
                //    Foreground = Brushes.White,
                //    FontFamily = new FontFamily("Consolas"),
                //    FontSize = 11,
                //    LabelsPosition = BarLabelPosition.Parallel,
                //    DataLabels = true
                //},

                //new ColumnSeries
                //{
                //    Title = Labels[8],
                //    Values = new ChartValues<double> { 1 },
                //    LabelPoint = PointLabelCart,
                //    Foreground = Brushes.White,
                //    FontFamily = new FontFamily("Consolas"),
                //    FontSize = 11,
                //    LabelsPosition = BarLabelPosition.Parallel,
                //    DataLabels = true
                //}

            };

            Chart3Label.Text = campoChart3 + " DE '" + keyword + "' CON MAS REGISTROS" + Environment.NewLine + "DESDE INICIO DE BASE DE DATOS (01/01/2018)";
            cartProductos.Series = SeriesCollection1;

            //int sumaRelevantes = cantidadRelevante1 + cantidadRelevante2 + cantidadRelevante3;
            //int restoRelevantes = cantidadTotalItem - sumaRelevantes;

            //SeriesCollection Piechart3 = new SeriesCollection
            //{
            //    new PieSeries
            //    {
            //        Values = new ChartValues<ObservableValue> { new ObservableValue(cantidadRelevante1) },
            //        DataLabels = true,
            //        Foreground = Brushes.Black,
            //        FontFamily = new FontFamily("Consolas"),
            //        FontSize = 12,
            //        LabelPoint = PointLabel,
            //        LabelPosition = PieLabelPosition.InsideSlice,
            //        Title = nombreRelevante1
            //    },

            //    new PieSeries
            //    {
            //        Values = new ChartValues<ObservableValue> { new ObservableValue(cantidadRelevante2) },
            //        DataLabels = true,
            //        Foreground = Brushes.Black,
            //        FontFamily = new FontFamily("Consolas"),
            //        FontSize = 12,
            //        LabelPoint = PointLabel,
            //        LabelPosition = PieLabelPosition.InsideSlice,
            //        Title = nombreRelevante2
            //    },
            //    new PieSeries
            //    {
            //        Values = new ChartValues<ObservableValue> { new ObservableValue(cantidadRelevante3) },
            //        DataLabels = true,
            //        Foreground = Brushes.Black,
            //        FontFamily = new FontFamily("Consolas"),
            //        FontSize = 12,
            //        LabelPoint = PointLabel,
            //        LabelPosition = PieLabelPosition.InsideSlice,
            //        Title = nombreRelevante3

            //    },
            //    new PieSeries
            //    {
            //        Values = new ChartValues<ObservableValue> { new ObservableValue(restoRelevantes) },
            //        DataLabels = true,
            //        Foreground = Brushes.Black,
            //        FontFamily = new FontFamily("Consolas"),
            //        FontSize = 12,
            //        LabelPoint = PointLabel,
            //        LabelPosition = PieLabelPosition.InsideSlice,
            //        Title = "RESTO DE " + campoChart3
            //    }
            //};

            //Chart3Label.Text = campoChart3 + " DE '" + keyword + "' CON MAS REGISTROS" + Environment.NewLine + "DESDE INICIO DE BASE DE DATOS (01/01/2018)";
            //PieChart3.Series = Piechart3;

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

        public Func<ChartPoint, string> PointLabel { get; set; }
        public Func<ChartPoint, string> PointLabelCart { get; set; }
    }
}


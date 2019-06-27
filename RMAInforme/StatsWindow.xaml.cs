using RMAInforme.DataAccessLayer;
using System;
using System.Linq;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.IO;

namespace RMAInforme
{
    public partial class StatsWindow : Window
    {
        private int TotalEnResultadoBusqueda;
        private int TotalTodosEnFechaBusqueda;
        private int CantidadOtrosProductosFecha;
        private string KeywordBusqueda;
        private string[] FechaInicioBusqueda;
        private string[] FechaFinalBusqueda;

        private int Top3A;
        private int Top3B;
        private int Top3C;

        private int Top3D;
        private int Top3E;
        private int Top3F;

        private int TotalEnBaseDeDatos;
        private int TotalMismo;
        private string TablaBusqueda;
        private PRDB Context;

        public StatsWindow(int cantidadResultadoBusqueda, string keywordBusqueda, string tablaBusqueda, DateTime? fechaInicioBusqueda, DateTime? fechaFinalBusqueda)
        {
            InitializeComponent();

            TotalEnResultadoBusqueda = cantidadResultadoBusqueda;
            KeywordBusqueda = keywordBusqueda.ToUpper() ?? "TODO";
            TablaBusqueda = tablaBusqueda;
            FechaInicioBusqueda = fechaInicioBusqueda.ToString().Split();
            FechaFinalBusqueda = fechaFinalBusqueda.Value.AddDays(-1).ToString().Split();

            Context = new PRDB();
            TotalEnBaseDeDatos = Context.Cambio.Count();
            TotalTodosEnFechaBusqueda = Context.Cambio.Where(w => w.FechaCambio >= fechaInicioBusqueda && w.FechaCambio <= fechaFinalBusqueda).Count();

            CantidadOtrosProductosFecha = (TotalTodosEnFechaBusqueda - TotalEnResultadoBusqueda);

            switch (tablaBusqueda)
            {
                case "MODELO":
                    TotalMismo = Context.Cambio.Where(w => w.Modelo == keywordBusqueda).Count();
                    ////////////////////////////////////////////
                    ///Top3A = Context.Cambio.Where(w => w.Modelo == KeywordBusqueda).Select(s => s.DescripcionFalla).Distinct().Count();
                    break;

                default:
                    break;
            }

            PointLabel = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation, Brushes.Black);


            CargarChart1();
            CargarChart2();
            CargarChart3();
            //CargarChart4();
            //CargarChart5();
            //CargarChart6();
        }

        private void CargarChart1()
        {
            //Piechart1 muestra que porcentaje representa la busqueda con respecto a todos los cambios en una fecha dada

            SeriesCollection Piechart1 = new SeriesCollection {
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(TotalEnResultadoBusqueda) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "'" + KeywordBusqueda + "'"

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(CantidadOtrosProductosFecha) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "OTROS PRODUCTOS"

                }

            };

            Chart1Label.Text = "TOTAL: " + TotalTodosEnFechaBusqueda + " REGISTROS ENTRE " + FechaInicioBusqueda[0] + " Y " + FechaFinalBusqueda[0];
            PieChart1.Series = Piechart1;
        }

        private void CargarChart2()
        {
            SeriesCollection Piechart2 = new SeriesCollection {
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(TotalEnResultadoBusqueda) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "'" + KeywordBusqueda + "'" + "ENTRE " + FechaInicioBusqueda[0] + " Y " + FechaFinalBusqueda[0]

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(TotalMismo - TotalEnResultadoBusqueda) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "'" + KeywordBusqueda + "'" + " RESTANTE DESDE INICIO DE BASE DE DATOS (06/03/2018)"

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(TotalEnBaseDeDatos - TotalMismo) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "OTROS PRODUCTOS DESDE INICIO DE BASE DE DATOS (06/03/2018)"

                }
            };

            Chart2Label.Text = "TOTAL: " + TotalEnBaseDeDatos + " REGISTROS DESDE EL INICIO DE BASE DE DATOS (06/03/2018)";
            PieChart2.Series = Piechart2;
        }

        private void CargarChart3()
        {

            SeriesCollection Piechart3 = new SeriesCollection {
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(TotalEnResultadoBusqueda) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "'" + KeywordBusqueda + "'" + "ENTRE " + FechaInicioBusqueda[0] + " Y " + FechaFinalBusqueda[0]

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(TotalMismo - TotalEnResultadoBusqueda) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "'" + KeywordBusqueda + "'" + " RESTANTE DESDE INICIO DE BASE DE DATOS (06/03/2018)"

                },
            };

            Chart3Label.Text = "TOTAL: " + TotalMismo + " REGISTROS DE '" + KeywordBusqueda + "' DESDE INICIO DE BASE DE DATOS (06/03/2018)";
            PieChart3.Series = Piechart3;
        }

        private void CargarChart4()
        {

            SeriesCollection Piechart4 = new SeriesCollection {
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(76) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "NAKAMURA (422)"

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(108) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "GOMEZ (925)"

                },
            };
            //Chart4Label.Text = "TOTAL: " + TotalTodosEnFechaBusqueda + " REGISTROS ENTRE " + FechaInicioBusqueda[0] + " Y " + FechaFinalBusqueda[0];
            //PieChart4.Series = Piechart4;
        }

        private void CargarChart5()
        {

            SeriesCollection Piechart5 = new SeriesCollection {
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(127) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "CAMARA"

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(88) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "MOTHERBOARD"

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(76) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "TECLADO"

                }
            };
            //Chart5Label.Text = "TOTAL: " + TotalTodosEnFechaBusqueda + " REGISTROS ENTRE " + FechaInicioBusqueda[0] + " Y " + FechaFinalBusqueda[0];
            //PieChart5.Series = Piechart5;
        }

        private void CargarChart6()
        {

            SeriesCollection Piechart6 = new SeriesCollection {
                 new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(327) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "NO DA VIDEO"

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(148) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "PANTALLA RAYADA"

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(26) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "PIXEL MUERTO"

                }
            };
            //Chart6Label.Text = "TOTAL: " + TotalTodosEnFechaBusqueda + " REGISTROS ENTRE " + FechaInicioBusqueda[0] + " Y " + FechaFinalBusqueda[0];
            //PieChart6.Series = Piechart6;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
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
                    RenderTargetBitmapExample(filename);
                    MessageBox.Show("El archivo se guardó en: " + Environment.NewLine + filename, "Estadisticas de búsqueda", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error guardando el archivo: " + Environment.NewLine + filename, "Estadisticas de búsqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        public void RenderTargetBitmapExample(string _filename)
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


    }
}

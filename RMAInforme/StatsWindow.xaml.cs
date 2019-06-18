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
        private string KeywordBusqueda;
        private DateTime? FechaInicioBusqueda;
        private DateTime? FechaFinalBusqueda;

        private int TotalEnBaseDeDatos;
        private int TotalMismo;
        private string TablaBusqueda;
        private PRDB Context;

        public StatsWindow(int cantidadResultadoBusqueda, string keywordBusqueda, string tablaBusqueda, DateTime? fechaInicioBusqueda, DateTime? fechaFinalBusqueda)
        {
            InitializeComponent();

            TotalEnResultadoBusqueda = cantidadResultadoBusqueda;
            KeywordBusqueda = keywordBusqueda ?? "toda la base de datos";
            TablaBusqueda = tablaBusqueda;
            FechaInicioBusqueda = fechaInicioBusqueda;
            FechaFinalBusqueda = fechaFinalBusqueda;

            Context = new PRDB();

            CargarEstadisticas();
        }

        private void CargarEstadisticas()
        {
                        
            CargarSeriesPies();

        }

        private void CargarSeriesPies()
        {
            PointLabel = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
                                                                                                                                                                                                                                                                                                                                                                                         SeriesCollection Piechart1 = new SeriesCollection {
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(227) },
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = KeywordBusqueda.ToUpper()

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(203) },
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "RESTO DE CAMBIOS DEL PERIODO"

                }
            };

            SeriesCollection Piechart2 = new SeriesCollection {
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(227) },
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "XL2 01/05/2019 AL 31/05/2019"

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(637) },
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "RESTO DE CAMBIOS DE XL2"

                }
            };

            SeriesCollection Piechart3 = new SeriesCollection {
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(227) },
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "XL2 01/05/2019 AL 31/05/2019"

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(6424) },
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "RESTO DEL TOTAL DE CAMBIOS"

                }
            };

            SeriesCollection Piechart4 = new SeriesCollection {
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(864) },
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "XL2"

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(5787) },
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "RESTO TOTAL DE CAMBIOS"

                }
            };

            SeriesCollection Piechart5 = new SeriesCollection {
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(127) },
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "CAMARA"

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(88) },
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "MOTHERBOARD"

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(76) },
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "TECLADO"

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(22) },
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "PANTALLA"

                }
            };

            SeriesCollection Piechart6 = new SeriesCollection {
                 new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(327) },
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "NO DA VIDEO"

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(148) },
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "PANTALLA RAYADA"

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(26) },
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "PIXEL MUERTO"

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(12) },
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "FALLA EN BURNINTEST"

                }
            };

            PieChart1.Series = Piechart1;
            PieChart2.Series = Piechart2;
            PieChart3.Series = Piechart3;
            PieChart4.Series = Piechart4;
            PieChart5.Series = Piechart5;
            PieChart6.Series = Piechart6;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Estadísticas de búesqueda"; // Default file name
            dlg.DefaultExt = ".jpeg"; // Default file extension
            dlg.Filter = "Archivo de imagen (.jpeg)|*.jpg"; // Filter files by extension

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
                    MessageBox.Show("Error guardando el archivo: " + Environment.NewLine + filename, "Estadisticas de búsuqeda", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                JpegBitmapEncoder encoder = new JpegBitmapEncoder(); //se puede pasar a pngencoder...
                encoder.QualityLevel = 100;
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);
            }
        }

        public Func<ChartPoint, string> PointLabel { get; set; }


    }
}

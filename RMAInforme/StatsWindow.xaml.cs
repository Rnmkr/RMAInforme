using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RMAInforme
{
    public partial class StatsWindow : System.Windows.Controls.UserControl
    {
        private int cantidadResultadoBusqueda;
        private int cantidadTotalItem;
        private int cantidadTodosFechaBusqueda;
        private int cantidadTotalTodos;
        private string campoChart3;
        private string periodoEstadisticas;
        private SeriesCollection Chart3Serie;

        private string keyword;

        public StatsWindow(string keyword, int cantidadResultadoBusqueda, int cantidadTotalItem, int cantidadTodosFechaBusqueda, int cantidadTotalTodos, SeriesCollection chart3Serie, string campoChart3, string periodoEstadisticas)
        {
            InitializeComponent();

            int primaryWidth = Screen.PrimaryScreen.Bounds.Width;

            if (primaryWidth < 1367)
            {
                this.MaxWidth = 720;
                this.MaxHeight = 480;
            }

            this.cantidadResultadoBusqueda = cantidadResultadoBusqueda;
            this.cantidadTotalItem = cantidadTotalItem;
            this.cantidadTodosFechaBusqueda = cantidadTodosFechaBusqueda;
            this.cantidadTotalTodos = cantidadTotalTodos;
            this.campoChart3 = campoChart3;
            this.periodoEstadisticas = periodoEstadisticas;
            this.Chart3Serie = chart3Serie;
            this.keyword = keyword;

            PointLabel = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation, Brushes.Black);


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

            Chart1Label.Text = "SE REGISTRARON " + cantidadTodosFechaBusqueda + " CAMBIOS" + Environment.NewLine + "ENTRE EL PERIODO DEL " + periodoEstadisticas;
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
            Chart3Label.Text = campoChart3 + " DE '" + keyword + "' CON MAS REGISTROS" + Environment.NewLine + "DESDE INICIO DE BASE DE DATOS (01/01/2018)";
            cartProductos.Series = Chart3Serie;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Estadísticas de busqueda", // Default file name
                DefaultExt = ".png", // Default file extension
                Filter = "Archivo de imagen (.png)|*.png" // Filter files by extension
            };

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;

                try
                {
                    RenderTargetBitmap(filename);
                    System.Windows.MessageBox.Show("El archivo se guardó en: " + Environment.NewLine + filename, "Estadisticas de búsqueda", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show("Error guardando el archivo: " + Environment.NewLine + filename, "Estadisticas de búsqueda", MessageBoxButton.OK, MessageBoxImage.Warning);
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

    }
}


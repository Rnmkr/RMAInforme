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

namespace RMAInforme
{
    public partial class StatsWindow : UserControl
    {
        private int TotalEnResultadoBusqueda;
        private int TotalTodosEnFechaBusqueda;
        private int CantidadOtrosProductosFecha;
        private string KeywordBusqueda;
        private string[] FechaInicioBusqueda;
        private string[] FechaFinalBusqueda;
        private IQueryable<Cambio> ListaMismo;

        private int RestoChart3;
        private string TitleFourthValueChart3;
        private string[] Top3ABC = new string[3];
        private int Top3A;
        private int Top3B;
        private int Top3C;

        private int TotalEnBaseDeDatos;
        private int TotalMismo;
        private string TablaBusqueda;
        private PRDB Context;

        public StatsWindow(int cantidadResultadoBusqueda, string keywordBusqueda, string tablaBusqueda, DateTime? fechaInicioBusqueda, DateTime? fechaFinalBusqueda)
        {
            InitializeComponent();

            TotalEnResultadoBusqueda = cantidadResultadoBusqueda;
            if (string.IsNullOrWhiteSpace(keywordBusqueda))
            {
                keywordBusqueda = "TODO";
            }
            else
            {
                KeywordBusqueda = keywordBusqueda.ToUpper();
            }
            TablaBusqueda = tablaBusqueda;
            FechaInicioBusqueda = fechaInicioBusqueda.ToString().Split();
            FechaFinalBusqueda = fechaFinalBusqueda.Value.AddDays(-1).ToString().Split();

            Context = new PRDB();
            TotalEnBaseDeDatos = Context.Cambio.Count();
            TotalTodosEnFechaBusqueda = Context.Cambio.Where(w => w.FechaCambio >= fechaInicioBusqueda && w.FechaCambio <= fechaFinalBusqueda).Count();

            CantidadOtrosProductosFecha = (TotalTodosEnFechaBusqueda - TotalEnResultadoBusqueda);

            FillByTable();

            PointLabel = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation, Brushes.Black);

            CargarChart1();
            CargarChart2();
            CargarChart3();
        }

        private void FillByTable()
        {
            switch (TablaBusqueda)
            {
                case "MODELO":
                    FillModelo();
                    break;

                case "PRODUCTO":
                    FillProducto();
                    break;

                case "LEGAJO":
                    FillLegajo();
                    break;

                case "TECNICO":
                    FillLegajo();
                    break;

                case "ARTICULO":
                    FillArticulo();
                    break;

                case "CATEGORIA":
                    FillCategoria();
                    break;

                default:

                    break;
            }
        }

        private void FillCategoria()
        {
            ListaMismo = Context.Cambio.Where(w => w.CategoriaItem == KeywordBusqueda);
            TotalMismo = ListaMismo.Count();

            //MODELOS
            var mostFrequent = ListaMismo
                    .GroupBy(g => g.Modelo)
                    .OrderByDescending(o => o.Count())
                    .Take(3)
                    .Select(s => s.Key).ToList();

            Top3ABC = mostFrequent.Select(s => s).ToArray();

            string s4 = Top3ABC[0];
            string s5 = Top3ABC[1];
            string s6 = Top3ABC[2];
            Top3A = ListaMismo.Where(w => w.Modelo == s4).Count();
            Top3B = ListaMismo.Where(w => w.Modelo == s5).Count();
            Top3C = ListaMismo.Where(w => w.Modelo == s6).Count();
            RestoChart3 = TotalMismo - (Top3A + Top3B + Top3C);

            Chart3Label.Text = "MODELOS CON MAS REGISTROS DE '" + KeywordBusqueda + "' (SOBRE EL TOTAL DE REGISTROS DE '" + KeywordBusqueda + "')";
            TitleFourthValueChart3 = "RESTO DE MODELOS";
        }

        private void FillArticulo()
        {
            ListaMismo = Context.Cambio.Where(w => w.ArticuloItem == KeywordBusqueda);
            TotalMismo = ListaMismo.Count();

            //FALLAS
            var mostFrequent = ListaMismo
                    .GroupBy(g => g.DescripcionFalla)
                    .OrderByDescending(o => o.Count())
                    .Take(3)
                    .Select(s => s.Key).ToList();

            Top3ABC = mostFrequent.Select(s => s).ToArray();

            string s4 = Top3ABC[0];
            string s5 = Top3ABC[1];
            string s6 = Top3ABC[2];
            Top3A = ListaMismo.Where(w => w.DescripcionFalla == s4).Count();
            Top3B = ListaMismo.Where(w => w.DescripcionFalla == s5).Count();
            Top3C = ListaMismo.Where(w => w.DescripcionFalla == s6).Count();
            RestoChart3 = TotalMismo - (Top3A + Top3B + Top3C);

            Chart3Label.Text = "FALLAS DE '" + KeywordBusqueda + "' CON MAS REGISTROS (SOBRE EL TOTAL DE REGISTROS DE '" + KeywordBusqueda + "')";
            TitleFourthValueChart3 = "RESTO DE FALLAS";
        }

        private void FillLegajo()
        {
            ListaMismo = Context.Cambio.Where(w => w.Legajo == KeywordBusqueda);
            TotalMismo = ListaMismo.Count();

            //MODELOS
            var mostFrequent = ListaMismo
                    .GroupBy(g => g.Producto)
                    .OrderByDescending(o => o.Count())
                    .Take(3)
                    .Select(s => s.Key).ToList();

            Top3ABC = mostFrequent.Select(s => s).ToArray();

            string s4 = Top3ABC[0];
            string s5 = Top3ABC[1];
            string s6 = Top3ABC[2];
            Top3A = ListaMismo.Where(w => w.Producto == s4).Count();
            Top3B = ListaMismo.Where(w => w.Producto == s5).Count();
            Top3C = ListaMismo.Where(w => w.Producto == s6).Count();
            RestoChart3 = TotalMismo - (Top3A + Top3B + Top3C);

            Chart3Label.Text = "MODELOS DE '" + KeywordBusqueda + "' CON MAS REGISTROS (SOBRE EL TOTAL DE REGISTROS DE '" + KeywordBusqueda + "')";
            TitleFourthValueChart3 = "RESTO DE MODELOS";
        }

        private void FillProducto()
        {
            ListaMismo = Context.Cambio.Where(w => w.Producto == KeywordBusqueda);
            TotalMismo = ListaMismo.Count();

            //MODELOS
            var mostFrequent = ListaMismo
                    .GroupBy(g => g.Modelo)
                    .OrderByDescending(o => o.Count())
                    .Take(3)
                    .Select(s => s.Key).ToList();

            Top3ABC = mostFrequent.Select(s => s).ToArray();

            string s4 = Top3ABC[0];
            string s5 = Top3ABC[1];
            string s6 = Top3ABC[2];
            Top3A = ListaMismo.Where(w => w.Modelo == s4).Count();
            Top3B = ListaMismo.Where(w => w.Modelo == s5).Count();
            Top3C = ListaMismo.Where(w => w.Modelo == s6).Count();
            RestoChart3 = TotalMismo - (Top3A + Top3B + Top3C);

            Chart3Label.Text = "MODELOS DE '" + KeywordBusqueda + "' CON MAS REGISTROS (SOBRE EL TOTAL DE REGISTROS DE '" + KeywordBusqueda + "')";
            TitleFourthValueChart3 = "RESTO DE MODELOS";
        }

        private void FillModelo()
        {
            ListaMismo = Context.Cambio.Where(w => w.Modelo == KeywordBusqueda);
            TotalMismo = ListaMismo.Count();

            //CATEGORIAS
            var mostFrequent = ListaMismo
                    .GroupBy(g => g.CategoriaItem)
                    .OrderByDescending(o => o.Count())
                    .Take(3)
                    .Select(s => s.Key).ToList();

            if (mostFrequent.Count < 3)
            {
                int Length = (3 - mostFrequent.Count);
                for (int i = 0; i < Length; i++)
                {
                    mostFrequent.Add("N/A");
                }
            }

            Top3ABC = mostFrequent.Select(s => s).ToArray();

            string s4 = Top3ABC[0];
            string s5 = Top3ABC[1];
            string s6 = Top3ABC[2];

            if (s4 == "N/A")
            {
                Top3A = 0;
            }
            else
            {
                Top3A = ListaMismo.Where(w => w.CategoriaItem == s4).Count();
            }

            if (s5 == "N/A")
            {
                Top3B = 0;
            }
            else
            {
                Top3B = ListaMismo.Where(w => w.CategoriaItem == s5).Count();
            }

            if (s6 == "N/A")
            {
                Top3C = 0;
            }
            else
            {
                Top3C = ListaMismo.Where(w => w.CategoriaItem == s6).Count();
            }

            RestoChart3 = TotalMismo - (Top3A + Top3B + Top3C);

            Chart3Label.Text = "CATEGORIAS DE '" + KeywordBusqueda + "' CON MAS REGISTROS (SOBRE EL TOTAL DE REGISTROS DE '" + KeywordBusqueda + "')";
            TitleFourthValueChart3 = "RESTO DE CATEGORIAS";
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

            Chart1Label.Text = TotalTodosEnFechaBusqueda + " REGISTROS ENTRE " + FechaInicioBusqueda[0] + " Y " + FechaFinalBusqueda[0];
            PieChart1.Series = Piechart1;
        }

        private void CargarChart2()
        {
            int RestoDelMismo = (TotalMismo - TotalEnResultadoBusqueda);

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
                    Title = "'" + KeywordBusqueda + "'" + " ENTRE " + FechaInicioBusqueda[0] + " Y " + FechaFinalBusqueda[0]

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(RestoDelMismo) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = "RESTO DE '" + KeywordBusqueda + "'" + " (INICIO DE B.D.)"
                }
            };

            Chart2Label.Text = TotalMismo + " REGISTROS DE '" + KeywordBusqueda + "' DESDE INICIO DE BASE DE DATOS (06/03/2018)";
            PieChart2.Series = Piechart2;
        }

        private void CargarChart3()
        {

            SeriesCollection Piechart3 = new SeriesCollection {
                  new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Top3A) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = Top3ABC[0]

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Top3B) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = Top3ABC[1]

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Top3C) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = Top3ABC[2]

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(RestoChart3) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = TitleFourthValueChart3

        }
            };

            PieChart3.Series = Piechart3;
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
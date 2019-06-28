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
        private IQueryable<Cambio> ListaMismo;

        private int Resto6;
        private int Resto5;
        private int Resto4;

        private string[] Top3ABC;
        private int Top3A;
        private int Top3B;
        private int Top3C;
        private string[] Top3DEF;
        private int Top3D;
        private int Top3E;
        private int Top3F;
        private string[] Top3GHI;
        private int Top3G;
        private int Top3H;
        private int Top3I;


        private string TitleFourthValueChart6;
        private string TitleFourthValueChart5;
        private string TitleFourthValueChart4;

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

            FillByTable();

            PointLabel = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation, Brushes.Black);

            CargarChart1();
            CargarChart2();
            CargarChart3();
            CargarChart4();
            CargarChart5();
            CargarChart6();
        }

        private void FillByTable()
        {
            switch (TablaBusqueda)
            {
                case "MODELO":
                    //Chart4Label.Text = "TOP 3 TECNICOS PARA '" + KeywordBusqueda + "' DESDE INICIO DE BASE DE DATOS";
                    Chart5Label.Text = "TOP 3 CATEGORIAS DE '" + KeywordBusqueda + "' DESDE INICIO DE BASE DE DATOS";
                    //Chart6Label.Text = "TOP 3 FALLAS DE '" + KeywordBusqueda + "' DESDE INICIO DE BASE DE DATOS";

                    TitleFourthValueChart4 = "OTROS TECNICOS";
                    TitleFourthValueChart5 = "RESTO DE CATEGORIAS";
                    TitleFourthValueChart6 = "RESTO DE FALLAS";

                    ListaMismo = Context.Cambio.Where(w => w.Modelo == KeywordBusqueda);
                    TotalMismo = ListaMismo.Count();


                    //TECNICOS
                    var mostFrequentTechs = ListaMismo
                            .GroupBy(g => g.Tecnico)
                            .OrderByDescending(o => o.Count())
                            .Take(3)
                            .Select(s => s.Key).ToList();

                    Top3ABC = mostFrequentTechs.Select(s => s).ToArray();

                    string s1 = Top3ABC[0];
                    string s2 = Top3ABC[1];
                    string s3 = Top3ABC[2];
                    Top3A = ListaMismo.Where(w => w.Tecnico == s1).Count();
                    Top3B = ListaMismo.Where(w => w.Tecnico == s2).Count();
                    Top3C = ListaMismo.Where(w => w.Tecnico == s3).Count();
                    Resto4 = TotalMismo - (Top3A + Top3B + Top3C);



                    //CATEGORIAS
                    var mostFrequentCategories = ListaMismo
                            .GroupBy(g => g.CategoriaItem)
                            .OrderByDescending(o => o.Count())
                            .Take(3)
                            .Select(s => s.Key).ToList();

                    Top3DEF = mostFrequentCategories.Select(s => s).ToArray();

                    string s4 = Top3DEF[0];
                    string s5 = Top3DEF[1];
                    string s6 = Top3DEF[2];
                    Top3D = ListaMismo.Where(w => w.CategoriaItem == s4).Count();
                    Top3E = ListaMismo.Where(w => w.CategoriaItem == s5).Count();
                    Top3F = ListaMismo.Where(w => w.CategoriaItem == s6).Count();
                    Resto5 = TotalMismo - (Top3D + Top3E + Top3F);

                    //FALLAS
                    var mostFrequentFailures = ListaMismo
                            .GroupBy(g => g.DescripcionFalla)
                            .OrderByDescending(o => o.Count())
                            .Take(3)
                            .Select(s => s.Key).ToList();

                    Top3GHI = mostFrequentFailures.Select(s => s).ToArray();

                    string s7 = Top3GHI[0];
                    string s8 = Top3GHI[1];
                    string s9 = Top3GHI[2];
                    Top3G = ListaMismo.Where(w => w.DescripcionFalla == s7).Count();
                    Top3H = ListaMismo.Where(w => w.DescripcionFalla == s8).Count();
                    Top3I = ListaMismo.Where(w => w.DescripcionFalla == s9).Count();
                    Resto6 = TotalMismo - (Top3G + Top3H + Top3I);

                    break;


                default:
                    break;
            }
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

            Chart2Label.Text = "TOTAL: " + TotalEnBaseDeDatos + " REGISTROS DESDE INICIO DE BASE DE DATOS (06/03/2018)";
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

            //Chart3Label.Text = "TOTAL: " + TotalMismo + " REGISTROS DE '" + KeywordBusqueda + "' DESDE INICIO DE BASE DE DATOS (06/03/2018)";
            //PieChart3.Series = Piechart3;
        }

        private void CargarChart4()
        {

            SeriesCollection Piechart4 = new SeriesCollection {
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
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Resto4) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = TitleFourthValueChart4

                }
            };

            //PieChart4.Series = Piechart4;
        }

        private void CargarChart5()
        {

            SeriesCollection Piechart5 = new SeriesCollection {
                  new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Top3D) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = Top3DEF[0]

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Top3E) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = Top3DEF[1]

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Top3F) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = Top3DEF[2]

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Resto5) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = TitleFourthValueChart5

                }
            };

            PieChart5.Series = Piechart5;
        }

        private void CargarChart6()
        {

            SeriesCollection Piechart6 = new SeriesCollection {
                 new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Top3G) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = Top3GHI[0]

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Top3H) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = Top3GHI[1]

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Top3I) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = Top3GHI[2]

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Resto6) },
                    DataLabels = true,
                    Foreground = Brushes.Black,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12,
                    LabelPoint = PointLabel,
                    LabelPosition = PieLabelPosition.InsideSlice,
                    Title = TitleFourthValueChart6

                }
            };

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
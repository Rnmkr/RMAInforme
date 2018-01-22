using RMAInforme.DataAccessLayer;
using System;
using System.Linq;
using System.Windows;

namespace RMAInforme
{
    /// <summary>
    /// Interaction logic for StatsWindow.xaml
    /// </summary>
    public partial class StatsWindow : Window
    {
        IQueryable<Cambio> ListResult;
        string Keyword;
        string Table;
        DateTime? Init;
        DateTime? End;

        public StatsWindow(IQueryable<Cambio> list, string keyword, string table, DateTime? init, DateTime? end)
        {
            InitializeComponent();

            ListResult = list;
            Keyword = keyword;
            Table = table;
            Init = init;
            End = end;

            LoadStats();
        }

        private void LoadStats()
        {
            PRDB context = new PRDB();
            int total = context.Cambio.Count();
            int parcial = ListResult.Count();
            LabelTotalCambios.Content = ("Total de cambios realizados: " + total);
            LabelTotalItemsBusqueda.Content = ("Cantidad de items encontrados: " + parcial);

            int PorcentajeSobreTotalCambios = (int)Math.Round((double)(100 * parcial) / total);
            LabelPorcentajeSobreTotalCambios.Content = ("Porcentaje sobre total de cambios: %" + PorcentajeSobreTotalCambios);
            ProgressTotal.Value = PorcentajeSobreTotalCambios;


            if (Init == null)
            {
                ProgressPeriod.IsEnabled = false;
            }
            else
            {

            }

        }
    }
}

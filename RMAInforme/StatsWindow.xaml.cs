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


            //total
            LabelTotalCambios.Content = ("Total de cambios realizados: " + total);
            LabelTotalItemsBusqueda.Content = ("Items encontrados en la búsqueda: " + parcial);

            //% sobre el total
            int PorcentajeTotalCambios = (int)Math.Round((double)(100 * parcial) / total);
            LabelPorcentajeTotal.Content = ("...el %" + PorcentajeTotalCambios + " del total de cambios realizados");
            ProgressTotal.Value = PorcentajeTotalCambios;

            ////% ultimos meses...
            //DateTime Now = DateTime.Now;

            ////mes
            //DateTime MesBack = Now.AddDays(-30);
            //int totalMes = context.Cambio.Where(w => w.FechaCambio >= MesBack && w.FechaCambio <= Now).Count();
            //int PorcentajeMes = (int)Math.Round((double)(100 * parcial) / totalMes);
            //LabelPorcentajeMes.Content = "Representa el %" + PorcentajeMes + " de los cambios del último mes";
            //ProgressMes.Value = PorcentajeMes;

            ////trimestre
            //DateTime TriMesBack = Now.AddDays(-90);
            //int totalTriMes = context.Cambio.Where(w => w.FechaCambio >= TriMesBack && w.FechaCambio <= Now).Count();
            //int PorcentajeTriMes = (int)Math.Round((double)(100 * parcial) / totalTriMes);
            //LabelPorcentajeTriMes.Content = "Representa el %" + PorcentajeTriMes + " de los cambios del último trimestre";
            //ProgressTriMes.Value = PorcentajeTriMes;

            ////semestre
            //DateTime SeMesBack = Now.AddDays(-180);
            //int totalSeMes = context.Cambio.Where(w => w.FechaCambio >= SeMesBack && w.FechaCambio <= Now).Count();
            //int PorcentajeSeMes = (int)Math.Round((double)(100 * parcial) / totalSeMes);
            //LabelPorcentajeSeMes.Content = "Representa el %" + PorcentajeSeMes + " de los cambios del último semestre";
            //ProgressSeMes.Value = PorcentajeSeMes;


            //% sobre periodo seleccionado
            if (Init != null && Keyword != null)
            {
                DateTime inicio = (DateTime)Init;
                DateTime final = (DateTime)End;
                int totalPeriodo = context.Cambio.Where(w => w.FechaCambio >= Init && w.FechaCambio <= End).Count();
                int PorcentajePeriodo = (int)Math.Round((double)(100 * parcial) / totalPeriodo);
                LabelPorcentajePeriodo.Content = ("...el %" + PorcentajePeriodo + " de los cambios realizados entre " + inicio.ToShortDateString() + " y el " + final.ToShortDateString());
                ProgressPeriod.Value = PorcentajePeriodo;
            }
            else
            {
                LabelPorcentajePeriodo.Content = "No se especificó un rango de fechas";
            }

        }
    }
}

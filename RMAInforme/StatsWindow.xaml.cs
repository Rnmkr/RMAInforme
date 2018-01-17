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

        }
    }
}

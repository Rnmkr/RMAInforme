namespace RMAInforme.DataAccessLayer
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PRDB : DbContext
    {
        public PRDB()
            : base("name=PRDB")
        {
        }

        public virtual DbSet<Cambio> Cambio { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cambio>()
                .Property(e => e.Legajo)
                .IsUnicode(false);

            modelBuilder.Entity<Cambio>()
                .Property(e => e.Tecnico)
                .IsUnicode(false);

            modelBuilder.Entity<Cambio>()
                .Property(e => e.NumeroPedido)
                .IsUnicode(false);

            modelBuilder.Entity<Cambio>()
                .Property(e => e.Producto)
                .IsUnicode(false);

            modelBuilder.Entity<Cambio>()
                .Property(e => e.Modelo)
                .IsUnicode(false);

            modelBuilder.Entity<Cambio>()
                .Property(e => e.ArticuloItem)
                .IsUnicode(false);

            modelBuilder.Entity<Cambio>()
                .Property(e => e.CategoriaItem)
                .IsUnicode(false);

            modelBuilder.Entity<Cambio>()
                .Property(e => e.DescripcionItem)
                .IsUnicode(false);

            modelBuilder.Entity<Cambio>()
                .Property(e => e.VersionItem)
                .IsUnicode(false);

            modelBuilder.Entity<Cambio>()
                .Property(e => e.SectorCambio)
                .IsUnicode(false);

            modelBuilder.Entity<Cambio>()
                .Property(e => e.CodigoFalla)
                .IsUnicode(false);

            modelBuilder.Entity<Cambio>()
                .Property(e => e.DescripcionFalla)
                .IsUnicode(false);

            modelBuilder.Entity<Cambio>()
                .Property(e => e.Observaciones)
                .IsUnicode(false);

            modelBuilder.Entity<Cambio>()
                .Property(e => e.EstadoCambio)
                .IsUnicode(false);

            modelBuilder.Entity<Cambio>()
                .Property(e => e.SupervisorModificacion)
                .IsUnicode(false);
        }
    }
}

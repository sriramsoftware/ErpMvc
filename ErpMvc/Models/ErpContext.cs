using System.Data.Entity;
using AlmacenCore.Models;
using CajaCore.DbConfigurations;
using CajaCore.Models;
using ComercialCore.Models;
using CompraVentaCore.Models;
using ContabilidadCore.DbConfigurations;
using ContabilidadCore.Models;
using HumanResourcesCore.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using SeguridadCore.Models;

namespace ErpMvc.Models
{
    public class ErpContext : IdentityDbContext<Usuario>
    {
        public ErpContext()
            : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Configurations.Add(new DenominacionDeMonedasConfig());
            modelBuilder.Entity<UnidadDeMedida>().Property(u => u.FactorDeConversion).HasPrecision(15, 5);
            modelBuilder.Entity<MovimientoDeProducto>().Property(u => u.Cantidad).HasPrecision(15, 5);
            modelBuilder.Entity<ValeSalidaDeAlmacen>().HasMany(v => v.Productos).WithRequired(d => d.Vale).WillCascadeOnDelete(false);
            modelBuilder.Entity<ValeSalidaDeAlmacen>().HasRequired(v => v.Almacen).WithMany(a => a.ValesDeSalida).WillCascadeOnDelete(false);
            modelBuilder.Entity<Venta>().HasRequired(v => v.Vendedor).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<TarjetaDeAsistencia>().HasKey(v => v.VendedorId).HasRequired(v => v.Vendedor).WithOptional().WillCascadeOnDelete(false);
            modelBuilder.Entity<Trabajador>().HasOptional(t => t.Caracteristicas).WithRequired();

            //modulo contabilidad
            modelBuilder.Configurations.Add(new AsientoConfig());
            modelBuilder.Configurations.Add(new CuentaConfig());
            modelBuilder.Configurations.Add(new DisponibilidadConfig());
            modelBuilder.Configurations.Add(new MovimientoConfig());
            modelBuilder.Configurations.Add(new NivelConfig());
            modelBuilder.Configurations.Add(new ConfiguracionCuentaModuloConfig());


            base.OnModelCreating(modelBuilder);
        }

        public DbSet<LogDeAcceso> LogsDeAccesos { get; set; }

        public DbSet<Compra> Compras { get; set; } 
        public DbSet<ExistenciaAlmacen> ExistenciasEnAlmacenes { get; set; } 
        public DbSet<Almacen> Almacenes { get; set; } 
        public DbSet<ProductoConcreto> ProductosConcretos { get; set; } 
        public DbSet<Producto> Productos { get; set; } 
        public DbSet<EntradaAlmacen> EntradasAAlmacenes { get; set; } 
        public DbSet<Elaboracion> Elaboraciones { get; set; } 
        public DbSet<ProductosPorElaboracion> ProductosPorElaboraciones { get; set; }
        public DbSet<UnidadDeMedida> UnidadesDeMedidas { get; set; }
        public DbSet<TipoDeUnidadDeMedida> TiposDeUnidadesDeMedidas { get; set; }
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<PuntoDeVenta> PuntosDeVentas { get; set; }
        public DbSet<TipoDeMovimiento> TiposDeMovimientos { get; set; }
        public DbSet<MovimientoDeProducto> MovimientosDeProductos { get; set; }
        public DbSet<ExistenciaCentroDeCosto> ExistenciasEnCentroDeCostos { get; set; }
        public DbSet<Moneda> Monedas { get; set; }
        public DbSet<OtrosGastos> OtrosGastos { get; set; }
        public DbSet<ConceptoDeGasto> ConceptosDeGastos { get; set; }
        public DbSet<CentroDeCosto> CentrosDeCostos { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<TarjetaDeAsistencia> TarjetasDeAsistencia { get; set; }
        public DbSet<DenominacionDeMoneda> DenominacionDeMoneda { get; set; }
        public DbSet<Caja> Cajas { get; set; }

        //contabilidad
        public DbSet<Asiento> Asientos { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<ConfiguracionCuentaModulo> CofiguracionesCuentasModulos { get; set; }
        public DbSet<Disponibilidad> Disponibilidades { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }
        public DbSet<Nivel> Niveles { get; set; }
    }
}
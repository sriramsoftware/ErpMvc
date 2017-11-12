using System.CodeDom.Compiler;
using System.Data.Entity;
using AlmacenCore.Models;
using CajaCore.DbConfigurations;
using CajaCore.Models;
using ComercialCore.Models;
using CompraVentaCore.Models;
using ContabilidadCore.DbConfigurations;
using ContabilidadCore.Models;
using HumanResourcesCore.DbConfigurations;
using HumanResourcesCore.Models;
using IdentidadCore.DbConfigurations;
using IdentidadCore.Models;
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
            modelBuilder.Entity<ExistenciaAlmacen>().Property(u => u.ExistenciaEnAlmacen).HasPrecision(15, 5);
            modelBuilder.Entity<ExistenciaCentroDeCosto>().Property(u => u.Cantidad).HasPrecision(15, 5);
            modelBuilder.Entity<SalidaPorMerma>().Property(u => u.Cantidad).HasPrecision(18, 5);
            modelBuilder.Entity<ProductoConcreto>().Property(u => u.PrecioUnitario).HasPrecision(15, 12);
            modelBuilder.Entity<MovimientoDeProducto>().Property(u => u.Costo).HasPrecision(15, 12);
            modelBuilder.Entity<ValeSalidaDeAlmacen>().HasMany(v => v.Productos).WithRequired(d => d.Vale).WillCascadeOnDelete(false);
            modelBuilder.Entity<ValeSalidaDeAlmacen>().HasRequired(v => v.Almacen).WithMany(a => a.ValesDeSalida).WillCascadeOnDelete(false);
            modelBuilder.Entity<Venta>().HasRequired(v => v.Vendedor).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<TarjetaDeAsistencia>().HasKey(v => v.VendedorId).HasRequired(v => v.Vendedor).WithOptional().WillCascadeOnDelete(false);
            modelBuilder.Entity<PuntoDeVenta>().HasRequired(c => c.CentroDeCosto).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<PuntoDeVenta>().HasMany(c => c.Vendedores).WithRequired(v => v.PuntoDeVenta).WillCascadeOnDelete(false);
            modelBuilder.Entity<SalidaPorMerma>().HasRequired(s => s.UnidadDeMedida).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<Clasificacion>().HasMany(c => c.Elaboraciones).WithOptional(e => e.Clasificacion).WillCascadeOnDelete(false);

            //modulo contabilidad
            modelBuilder.Configurations.Add(new AsientoConfig());
            modelBuilder.Configurations.Add(new CuentaConfig());
            modelBuilder.Configurations.Add(new DisponibilidadConfig());
            modelBuilder.Configurations.Add(new MovimientoConfig());
            modelBuilder.Configurations.Add(new NivelConfig());
            modelBuilder.Configurations.Add(new ConfiguracionCuentaModuloConfig());

            //modulo identidad
            modelBuilder.Configurations.Add(new PersonaConfig());
            modelBuilder.Configurations.Add(new CaracteristicaPersonaConfig());

            //modulo recursos humanos
            modelBuilder.Configurations.Add(new TrabajadorConfig());

            modelBuilder.Entity<Agregado>()
                .ToTable("cv_agregados")
                .HasRequired(a => a.Elaboracion)
                .WithMany(e => e.Agregados).WillCascadeOnDelete(false);
            modelBuilder.Entity<Agregado>()
                            .HasRequired(a => a.Producto)
                            .WithMany().WillCascadeOnDelete(false);

            modelBuilder.Entity<PorcientoMenu>()
                .ToTable("rst_porciento_menu")
                .HasKey(p => p.ElaboracioId)
                .HasRequired(p => p.Elaboracion)
                .WithOptional();

            modelBuilder.Entity<Propina>()
                .ToTable("rst_propina")
                .HasKey(p => p.VentaId)
                .HasRequired(p => p.Venta)
                .WithOptional();

            modelBuilder.Entity<Comanda>()
                .HasMany(a => a.Comensales)
                .WithRequired(e => e.Comanda).WillCascadeOnDelete(false);

            modelBuilder.Entity<OrdenPorDetalle>()
                            .HasMany(a => a.Anotaciones)
                            .WithMany(a => a.OrdenPorDetalles).Map(a => a.ToTable("rst_anotaciones_orden_detalles"));

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<LicenciaInfo> Licencias { get; set; } 

        public DbSet<LogDeAcceso> LogsDeAccesos { get; set; }

        public DbSet<Compra> Compras { get; set; }
        public DbSet<ExistenciaAlmacen> ExistenciasEnAlmacenes { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<ProductoConcreto> ProductosConcretos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<ClasificacionDeProducto> ClasificacionesDeProductos { get; set; }
        public DbSet<GrupoDeProducto> GruposDeProductos { get; set; }
        public DbSet<EntradaAlmacen> EntradasAAlmacenes { get; set; }
        public DbSet<Elaboracion> Elaboraciones { get; set; }
        public DbSet<Clasificacion> ClasificacionesDeElaboracion { get; set; }
        public DbSet<Agregado> Agregados { get; set; }
        public DbSet<AgregadosVendidos> AgregadosVendidos { get; set; }
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
        public DbSet<CierreDeCaja> CierresDeCajas { get; set; }
        public DbSet<DenominacionesEnCierreDeCaja> DenominacionesEnCierreDeCajas { get; set; }
        public DbSet<SalidaPorMerma> SalidasPorMermas { get; set; }

        //contabilidad
        public DbSet<Asiento> Asientos { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<ConfiguracionCuentaModulo> CofiguracionesCuentasModulos { get; set; }
        public DbSet<Disponibilidad> Disponibilidades { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }
        public DbSet<Nivel> Niveles { get; set; }

        public DbSet<Entidad> Entidades { get; set; }

        public DbSet<Propina> Propinas { get; set; }
        public DbSet<PorcientoMenu> PorcientosMenus { get; set; }

        public DbSet<Persona> Personas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<PersonaCliente> PersonasClientes { get; set; }
        public DbSet<EntidadCliente> EntidadesClientes { get; set; }

        //comandas
        public DbSet<Comanda> Comandas { get; set; }
        public DbSet<DetalleDeComanda> DetallesDeComandas { get; set; }
        public DbSet<AgregadoDeComanda> AgregadosDeComandas { get; set; }
        public DbSet<Anotacion> Anotaciones { get; set; }
        public DbSet<Orden> Ordenes { get; set; }
        public DbSet<OrdenPorDetalle> OrdenesPorDetalles { get; set; }
    }
}
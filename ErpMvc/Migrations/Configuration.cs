using System.Data.Entity.Migrations;
using AlmacenCore.Models;
using ComercialCore.Models;
using CompraVentaCore.Models;
using ContabilidadCore.Models;
using ErpMvc.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using SeguridadCore.Models;

namespace ErpMvc.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ErpContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "PruebaSeguridadMvc.Models.ErpContext";
        }

        protected override void Seed(ErpContext context)
        {
            
            var rolAdmin = new IdentityRole() {Id = "1", Name = RolesMontin.Administrador};
            //var rolSuperadmin = new IdentityRole() {Id = "2", Name = "SUPERADMIN_ERP"};

            context.Roles.AddOrUpdate(
                r => r.Id,
                rolAdmin,
                new IdentityRole() { Id = "2",Name = RolesMontin.UsuarioAvanzado},
                new IdentityRole() { Id = "3",Name = RolesMontin.Vendedor}
                );

            context.Users.AddOrUpdate(
                  p => p.Id,
                  new Usuario { UserName = "admin", Activo  = true, Correo = "admin@gmail.com", PasswordHash = "ACdZ2ExozIn/nul8svk/O8WBX/xd565MEoWZqs/SqBXASVZJDkK4J1Bb9erSH2HmUQ==", Id = "8343b65e-3ad7-4eee-b09e-8ca50b9e57fa", Roles = { new IdentityUserRole() {RoleId = "1", UserId = "8343b65e-3ad7-4eee-b09e-8ca50b9e57fa" }} }
                );

            context.TiposDeUnidadesDeMedidas.AddOrUpdate(t => t.Id,
                new TipoDeUnidadDeMedida() {Id = 1,Name = "Peso"},
                new TipoDeUnidadDeMedida() {Id = 2,Name = "Capacidad"},
                new TipoDeUnidadDeMedida() {Id = 4,Name = "Cantidad"},
                new TipoDeUnidadDeMedida() {Id = 3,Name = "Distancia"});

            context.UnidadesDeMedidas.AddOrUpdate(u => u.Id,
                new UnidadDeMedida() {Id = 1, Nombre = "Mililitros", Siglas = "ml", TipoDeUnidadDeMedidaId = 2, FactorDeConversion = 1m},
                new UnidadDeMedida() { Id = 2, Nombre = "Litros", Siglas = "lt", TipoDeUnidadDeMedidaId = 2, FactorDeConversion = 0.001m},
                new UnidadDeMedida() { Id = 3, Nombre = "gramos", Siglas = "g", TipoDeUnidadDeMedidaId = 1, FactorDeConversion = 1m},
                new UnidadDeMedida() { Id = 4, Nombre = "Kilogramos", Siglas = "Kg", TipoDeUnidadDeMedidaId = 1, FactorDeConversion = 0.001m},
                new UnidadDeMedida() { Id = 6, Nombre = "Onza", Siglas = "oz", TipoDeUnidadDeMedidaId = 1, FactorDeConversion = 0.0353m},
                new UnidadDeMedida() { Id = 6, Nombre = "Unidad", Siglas = "u", TipoDeUnidadDeMedidaId = 4 , FactorDeConversion = 1m },
                new UnidadDeMedida() { Id = 5, Nombre = "Libras", Siglas = "lb", TipoDeUnidadDeMedidaId = 1, FactorDeConversion = 0.0022m}
                );

            context.TiposDeMovimientos.AddOrUpdate(t => t.Id,
                new TipoDeMovimiento() {Descripcion = TipoDeMovimientoConstantes.Entrada, Factor = 1, Id = 1},
                new TipoDeMovimiento() {Descripcion = TipoDeMovimientoConstantes.EntradaTrasladoInterno, Factor = 1, Id = 2},
                new TipoDeMovimiento() {Descripcion = TipoDeMovimientoConstantes.Merma, Factor = -1, Id = 3},
                new TipoDeMovimiento() {Descripcion = TipoDeMovimientoConstantes.SalidaAProduccion, Factor = -1, Id = 4},
                new TipoDeMovimiento() {Descripcion = TipoDeMovimientoConstantes.SalidaTrasladoInterno, Factor = -1, Id = 5},
                new TipoDeMovimiento() {Descripcion = TipoDeMovimientoConstantes.VentaIndependiente, Factor = -1, Id = 6}
                );

            context.Monedas.AddOrUpdate(m => m.Id,
                new Moneda() { Id = 1, Nombre = "Moneda nacional", Sigla = "CUP"}
                );
            context.Almacenes.AddOrUpdate(a => a.Id,
                new Almacen() {Id = 1, Codigo = "01",Descripcion = "Almacen"}
                );
            context.CentrosDeCostos.AddOrUpdate(c => c.Id,
                new CentroDeCosto() {Id = 1,Codigo = "01", Nombre = "Montin Cubano"}
                );

            context.PuntosDeVentas.AddOrUpdate(c => c.Id,
                new PuntoDeVenta() { Id = 1, Nombre = "Montin Cubano", CentroDeCostoId = 1}
                );
        }
    }
}

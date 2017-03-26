using System.Data.Entity.Migrations;
using AlmacenCore.Models;
using CajaCore.Models;
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

            var rolAdmin = new IdentityRole() { Id = "1", Name = RolesMontin.Administrador };
            //var rolSuperadmin = new IdentityRole() {Id = "2", Name = "SUPERADMIN_ERP"};

            context.Roles.AddOrUpdate(
                r => r.Id,
                rolAdmin,
                new IdentityRole() { Id = "2", Name = RolesMontin.UsuarioAvanzado },
                new IdentityRole() { Id = "3", Name = RolesMontin.Vendedor }
                );

            context.Users.AddOrUpdate(
                  p => p.Id,
                  new Usuario { UserName = "admin", Activo = true, Correo = "admin@gmail.com", PasswordHash = "ACdZ2ExozIn/nul8svk/O8WBX/xd565MEoWZqs/SqBXASVZJDkK4J1Bb9erSH2HmUQ==", Id = "8343b65e-3ad7-4eee-b09e-8ca50b9e57fa", Roles = { new IdentityUserRole() { RoleId = "1", UserId = "8343b65e-3ad7-4eee-b09e-8ca50b9e57fa" } } }
                );

            context.TiposDeUnidadesDeMedidas.AddOrUpdate(t => t.Id,
                new TipoDeUnidadDeMedida() { Id = 1, Name = "Peso" },
                new TipoDeUnidadDeMedida() { Id = 2, Name = "Capacidad" },
                new TipoDeUnidadDeMedida() { Id = 4, Name = "Cantidad" },
                new TipoDeUnidadDeMedida() { Id = 3, Name = "Distancia" });

            context.UnidadesDeMedidas.AddOrUpdate(u => u.Id,
                new UnidadDeMedida() { Id = 1, Nombre = "Mililitros", Siglas = "ml", TipoDeUnidadDeMedidaId = 2, FactorDeConversion = 1m },
                new UnidadDeMedida() { Id = 2, Nombre = "Litros", Siglas = "lt", TipoDeUnidadDeMedidaId = 2, FactorDeConversion = 0.001m },
                new UnidadDeMedida() { Id = 3, Nombre = "gramos", Siglas = "g", TipoDeUnidadDeMedidaId = 1, FactorDeConversion = 1m },
                new UnidadDeMedida() { Id = 4, Nombre = "Kilogramos", Siglas = "Kg", TipoDeUnidadDeMedidaId = 1, FactorDeConversion = 0.001m },
                new UnidadDeMedida() { Id = 6, Nombre = "Onza", Siglas = "oz", TipoDeUnidadDeMedidaId = 1, FactorDeConversion = 0.0353m },
                new UnidadDeMedida() { Id = 6, Nombre = "Unidad", Siglas = "u", TipoDeUnidadDeMedidaId = 4, FactorDeConversion = 1m },
                new UnidadDeMedida() { Id = 5, Nombre = "Libras", Siglas = "lb", TipoDeUnidadDeMedidaId = 1, FactorDeConversion = 0.0022m }
                );

            context.TiposDeMovimientos.AddOrUpdate(t => t.Id,
                new TipoDeMovimiento() { Descripcion = TipoDeMovimientoConstantes.Entrada, Factor = 1, Id = 1 },
                new TipoDeMovimiento() { Descripcion = TipoDeMovimientoConstantes.EntradaTrasladoInterno, Factor = 1, Id = 2 },
                new TipoDeMovimiento() { Descripcion = TipoDeMovimientoConstantes.Merma, Factor = -1, Id = 3 },
                new TipoDeMovimiento() { Descripcion = TipoDeMovimientoConstantes.SalidaAProduccion, Factor = -1, Id = 4 },
                new TipoDeMovimiento() { Descripcion = TipoDeMovimientoConstantes.SalidaTrasladoInterno, Factor = -1, Id = 5 },
                new TipoDeMovimiento() { Descripcion = TipoDeMovimientoConstantes.VentaIndependiente, Factor = -1, Id = 6 }
                );

            context.Monedas.AddOrUpdate(m => m.Id,
                new Moneda() { Id = 1, Nombre = "Moneda nacional", Sigla = "CUP" },
                new Moneda() { Id = 2, Nombre = "Divisa", Sigla = "CUC" }
                );
            context.Almacenes.AddOrUpdate(a => a.Id,
                new Almacen() { Id = 1, Codigo = "01", Descripcion = "Almacen" }
                );
            context.CentrosDeCostos.AddOrUpdate(c => c.Id,
                new CentroDeCosto() { Id = 1, Codigo = "01", Nombre = "Montin Cubano" }
                );

            context.PuntosDeVentas.AddOrUpdate(c => c.Id,
                new PuntoDeVenta() { Id = 1, Nombre = "Montin Cubano", CentroDeCostoId = 1 }
                );

            context.Cajas.AddOrUpdate(c => c.Id,
                new Caja() { Id = 1, Descripcion = "Caja" }
                );

            context.DenominacionDeMoneda.AddOrUpdate(
                d => d.Id,
                new DenominacionDeMoneda() { Id = 1, MonedaId = 1, Billete = true, Valor = 1000 },
                new DenominacionDeMoneda() { Id = 2, MonedaId = 1, Billete = true, Valor = 500 },
                new DenominacionDeMoneda() { Id = 3, MonedaId = 1, Billete = true, Valor = 200 },
                new DenominacionDeMoneda() { Id = 4, MonedaId = 1, Billete = true, Valor = 100 },
                new DenominacionDeMoneda() { Id = 5, MonedaId = 1, Billete = true, Valor = 50 },
                new DenominacionDeMoneda() { Id = 6, MonedaId = 1, Billete = true, Valor = 20 },
                new DenominacionDeMoneda() { Id = 7, MonedaId = 1, Billete = true, Valor = 10 },
                new DenominacionDeMoneda() { Id = 8, MonedaId = 1, Billete = true, Valor = 5 },
                new DenominacionDeMoneda() { Id = 9, MonedaId = 1, Billete = true, Valor = 3 },
                new DenominacionDeMoneda() { Id = 10, MonedaId = 1, Billete = true, Valor = 1 },
                new DenominacionDeMoneda() { Id = 11, MonedaId = 1, Billete = false, Valor = 3 },
                new DenominacionDeMoneda() { Id = 12, MonedaId = 1, Billete = false, Valor = 1 },
                new DenominacionDeMoneda() { Id = 13, MonedaId = 1, Billete = false, Valor = 0.20m },
                new DenominacionDeMoneda() { Id = 14, MonedaId = 1, Billete = false, Valor = 0.05m },
                new DenominacionDeMoneda() { Id = 15, MonedaId = 1, Billete = false, Valor = 0.02m },
                new DenominacionDeMoneda() { Id = 16, MonedaId = 1, Billete = false, Valor = 0.01m },
                new DenominacionDeMoneda() { Id = 17, MonedaId = 2, Billete = true, Valor = 100 },
                new DenominacionDeMoneda() { Id = 18, MonedaId = 2, Billete = true, Valor = 50 },
                new DenominacionDeMoneda() { Id = 19, MonedaId = 2, Billete = true, Valor = 20 },
                new DenominacionDeMoneda() { Id = 20, MonedaId = 2, Billete = true, Valor = 10 },
                new DenominacionDeMoneda() { Id = 21, MonedaId = 2, Billete = true, Valor = 5 },
                new DenominacionDeMoneda() { Id = 22, MonedaId = 2, Billete = true, Valor = 3 },
                new DenominacionDeMoneda() { Id = 23, MonedaId = 2, Billete = true, Valor = 1 },
                new DenominacionDeMoneda() { Id = 24, MonedaId = 2, Billete = false, Valor = 1 },
                new DenominacionDeMoneda() { Id = 25, MonedaId = 2, Billete = false, Valor = 0.5m },
                new DenominacionDeMoneda() { Id = 26, MonedaId = 2, Billete = false, Valor = 0.25m },
                new DenominacionDeMoneda() { Id = 27, MonedaId = 2, Billete = false, Valor = 0.10m },
                new DenominacionDeMoneda() { Id = 28, MonedaId = 2, Billete = false, Valor = 0.05m },
                new DenominacionDeMoneda() { Id = 29, MonedaId = 2, Billete = false, Valor = 0.01m }
                );

            context.Niveles.AddOrUpdate(
                n => n.Id,
                new Nivel() {Id = 1, Nombre = "Caja" , Numero = "100"},
                new Nivel() {Id = 2, Nombre = "Gastos" , Numero = "900"}
                );

            context.Cuentas.AddOrUpdate(
                c => c.Id,
                new Cuenta() {Id = 1, NivelId = 1,Naturaleza = Naturaleza.Deudora},
                new Cuenta() {Id = 2, NivelId = 2,Naturaleza = Naturaleza.Deudora}
                
                );
        }
    }
}

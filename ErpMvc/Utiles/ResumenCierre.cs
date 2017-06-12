using System.Data.Entity;
using System.Linq;
using CajaCore.Models;
using CompraVentaCore.Models;
using ContabilidadBL;
using ContabilidadCore.Models;
using ErpMvc.Models;
using ErpMvc.ViewModels;

namespace ErpMvc.Utiles
{
    public class ResumenCierre
    {
        private DbContext _db;
        private CuentasServices _cuentasServices;
        private PeriodoContableService _periodoContableService;

        public ResumenCierre(DbContext context)
        {
            _db = context;
            _cuentasServices = new CuentasServices(context);
            _periodoContableService = new PeriodoContableService(context);
        }

        public CierreViewModel VerResumen(int id)
        {
            var dia = _periodoContableService.BuscarDiaContable(id);
            var cierre = _db.Set<CierreDeCaja>().SingleOrDefault(c => c.DiaContableId == id);
            
            var porcientos = _db.Set<PorcientoMenu>().ToList();
            
            var totalVentas = 0m;
            var ventasSinPorciento = 0m;
            dynamic centrosDeCosto = 0;
            if (_db.Set<Venta>().Any(v => v.DiaContableId == dia.Id && (v.EstadoDeVenta == EstadoDeVenta.PagadaEnEfectivo || v.EstadoDeVenta == EstadoDeVenta.PagadaPorTarjeta)))
            {
                var ventas = _db.Set<Venta>()
                    .Where(
                        v =>
                            v.DiaContableId == dia.Id &&
                            (v.EstadoDeVenta == EstadoDeVenta.PagadaEnEfectivo ||
                             v.EstadoDeVenta == EstadoDeVenta.PagadaPorTarjeta)).ToList();
                totalVentas = ventas.Sum(v => v.Importe);
                ventasSinPorciento = ventas.Sum(v => v.Elaboraciones.Where(e => porcientos.Any(p => p.ElaboracioId == e.ElaboracionId && !p.SeCalcula)).Sum(s => s.ImporteTotal)) + ventas.Where(v => v.Observaciones == "Venta al costo").Sum(v => v.Importe);
                centrosDeCosto = ventas.GroupBy(v => v.PuntoDeVenta.CentroDeCosto).Select(v => new { v.Key.Nombre, Importe = v.Sum(s => s.Importe) }).ToList();
            }

            var extracciones =
                _cuentasServices.GetMovimientosDeCuenta("Caja")
                .Where(m => m.Asiento.DiaContableId == dia.Id && (m.Asiento.Detalle.StartsWith("Extracción") || m.Asiento.Detalle.StartsWith("Pago") || m.Asiento.Detalle.StartsWith("Compra"))).Sum(m => m.Importe);

            var extraccionCierre =
                _cuentasServices.GetMovimientosDeCuenta("Caja")
                .Where(m => m.Asiento.DiaContableId == dia.Id && (m.Asiento.Detalle.StartsWith("Cierre"))).Sum(m => m.Importe);

            var pagoTrabajadores =
                            _cuentasServices.GetMovimientosDeCuenta("Caja")
                            .Where(m => m.Asiento.DiaContableId == dia.Id && (m.Asiento.Detalle.StartsWith("Trabajadores"))).Sum(m => m.Importe);

            var depositos =
                _cuentasServices.GetMovimientosDeCuenta("Caja")
                .Where(m => m.Asiento.DiaContableId == dia.Id && m.TipoDeOperacion == TipoDeOperacion.Debito && m.Asiento.Detalle.StartsWith("Deposito")).Sum(m => m.Importe);

            var propinas = _db.Set<Propina>().Any(v => v.Venta.DiaContableId == dia.Id)
                ? _db.Set<Propina>().Where(v => v.Venta.DiaContableId == dia.Id).Sum(c => c.Importe)
                : 0;
            var resumen = new CierreViewModel()
            {
                Fecha = dia.Fecha,
                EfectivoAnterior = 100,
                Ventas = totalVentas,
                VentasSinPorciento = ventasSinPorciento,
                Depositos = depositos,
                Extracciones = extracciones,
                Propinas = propinas,
                ExtraccionCierre = extraccionCierre,
                PagoTrabajadores = pagoTrabajadores,
                Desgloce = cierre != null? cierre.Desglose.ToList() : null
                //CentrosDeCosto = centrosDeCosto
            };
            return resumen;
        }
    }
}
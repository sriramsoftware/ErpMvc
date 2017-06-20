using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.Data.WcfLinq.Helpers;
using DevExpress.XtraReports.UI;
using ErpMvc.Models;
using System.Data.Entity;
using System.Linq;
using AlmacenCore.Models;

namespace ErpMvc.Reportes
{
    public partial class Inventario : DevExpress.XtraReports.UI.XtraReport
    {
        public Inventario(string lugar, DateTime? fecha)
        {
            InitializeComponent();

            var db = new ErpContext();
            fecha_reporte.Text = "Fecha del reporte: " + DateTime.Now.ToShortDateString();
            if (fecha == null)
            {
                titulo_reporte.Text += " de " + lugar + " actualmente";

                if (lugar == "Almacen")
                {
                    DataSource = db.ExistenciasEnAlmacenes.Where(e => e.Producto.Producto.Activo).Select(e => new
                    {
                        Producto = e.Producto.Producto.Nombre,
                        Um = e.Producto.UnidadDeMedida.Siglas,
                        Cantidad = e.ExistenciaEnAlmacen,
                        Valor = e.Producto.PrecioUnitario * e.ExistenciaEnAlmacen
                    }).OrderBy(p => p.Producto).ToList();
                }
                else
                {
                    var cc = db.CentrosDeCostos.SingleOrDefault(c => c.Nombre == lugar);
                    DataSource = db.ExistenciasEnCentroDeCostos.Where(e => e.Producto.Producto.Activo && e.CentroDeCostoId == cc.Id).Select(e => new
                    {
                        Producto = e.Producto.Producto.Nombre,
                        Um = e.Producto.UnidadDeMedida.Siglas,
                        Cantidad = e.Cantidad,
                        Valor = Math.Round(e.Producto.PrecioUnitario * e.Cantidad, 2)
                    }).OrderBy(p => p.Producto).ToList();
                }
            }
            else
            {
                
                titulo_reporte.Text += " de " + lugar + " al cierre del dia " + fecha.Value.ToShortDateString();
                fecha = fecha.Value.AddDays(1);
                if (lugar == "Almacen")
                {
                    var entradasAlmacen =
                        db.Set<EntradaAlmacen>().Where(d => d.DiaContable.Fecha >= fecha).GroupBy(d => d.Producto).Select(d => new { Producto = d.Key, Cantidad = d.Sum(e => e.Cantidad) });
                    var salidasAlmacen = db.Set<DetalleSalidaAlmacen>().Where(d => d.Vale.DiaContable.Fecha >= fecha).GroupBy(d => d.Producto).Select(d => new { Producto = d.Key.Producto, Cantidad = d.Sum(e => e.Cantidad) });
                    var mermasAlmacen = db.Set<SalidaPorMerma>().Where(d => d.DiaContable.Fecha >= fecha).ToList().GroupBy(d => d.ExistenciaAlmacen).Select(d => new { Producto = d.Key.Producto, Cantidad = d.Sum(e => e.Cantidad * (d.Key.Producto.UnidadDeMedida.FactorDeConversion / e.UnidadDeMedida.FactorDeConversion)) }).ToList();
                    DataSource = db.ExistenciasEnAlmacenes.Where(e => e.Producto.Producto.Activo).ToList().Select(e => new
                    {
                        Producto = e.Producto.Producto.Nombre,
                        Um = e.Producto.UnidadDeMedida.Siglas,
                        Cantidad = e.ExistenciaEnAlmacen - (entradasAlmacen.Any(p => p.Producto.Id == e.ProductoId)?entradasAlmacen.SingleOrDefault(p => p.Producto.Id == e.ProductoId).Cantidad:0) +
                        (salidasAlmacen.Any(p => p.Producto.Id == e.ProductoId) ? salidasAlmacen.SingleOrDefault(p => p.Producto.Id == e.ProductoId).Cantidad : 0) +
                        (mermasAlmacen.Any(p => p.Producto.Id == e.ProductoId) ? mermasAlmacen.SingleOrDefault(p => p.Producto.Id == e.ProductoId).Cantidad : 0),

                        Valor = e.Producto.PrecioUnitario * e.ExistenciaEnAlmacen
                    }).OrderBy(p => p.Producto).ToList();
                }
                else
                {
                    var movimientos =
                        db.MovimientosDeProductos.Where(m => m.DiaContable.Fecha >= fecha).Include(m => m.Tipo).ToList().GroupBy(m => m.Producto).Select(m => new {Producto=m.Key,
                            Cantidad = m.Sum(d => d.Cantidad * d.Tipo.Factor)}).ToList();
                    var cc = db.CentrosDeCostos.SingleOrDefault(c => c.Nombre == lugar);
                    DataSource = db.ExistenciasEnCentroDeCostos.Where(e => e.Producto.Producto.Activo && e.CentroDeCostoId == cc.Id).ToList().Select(e => new
                    {
                        Producto = e.Producto.Producto.Nombre,
                        Um = e.Producto.UnidadDeMedida.Siglas,
                        Cantidad = e.Cantidad - (movimientos.Any(m => m.Producto.Id == e.Producto.Id)? movimientos.SingleOrDefault(m => m.Producto.Id == e.Producto.Id).Cantidad:0),
                        Valor = Math.Round(e.Producto.PrecioUnitario * e.Cantidad, 2)
                    }).OrderBy(p => p.Producto).ToList();
                }
            }


            this.productoCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Producto")});

            this.umCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Um")});

            this.cantidadCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Cantidad")});

            this.valorCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Valor","{0:C}")});
        }

    }
}

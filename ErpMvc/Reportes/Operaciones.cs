using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using DevExpress.Data.WcfLinq.Helpers;
using DevExpress.XtraReports.UI;
using ErpMvc.Models;
using System.Data.Entity;
using System.Linq;
using CompraVentaCore.Models;
using ErpMvc.ViewModels;

namespace ErpMvc.Reportes
{
    public partial class Operaciones : DevExpress.XtraReports.UI.XtraReport
    {
        public Operaciones(DateTime FechaInicio, DateTime FechaFin)
        {
            InitializeComponent();

            var db = new ErpContext();
            FechaFin = FechaFin.AddHours(23).AddMinutes(59);

            fecha_inicio.Text = "Desde: " + FechaInicio.ToShortDateString();
            fecha_fin.Text = "Hasta: " + FechaFin.ToShortDateString();

            //compras
            var compras = db.Compras.Where(c => c.DiaContable.Fecha >= FechaInicio && c.DiaContable.Fecha <= FechaFin).ToList();

            var comprasData = compras.SelectMany(c => c.Productos.Select(p => new
            {
                Fecha = c.Fecha,
                Tienda = c.Entidad.Nombre,
                Productos = p.Producto.Nombre,
                Cantidad = p.Cantidad + " " + p.UnidadDeMedida.Siglas,
                Importe = p.ImporteTotal
            })).ToList();

            ComprasReport.DataSource = comprasData;

            this.fechaCompraCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Fecha","{0:d}"), });

            this.compraTiendaCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Tienda")});

            this.compraProductosCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Productos")});

            this.comprasCandidadCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Cantidad")});

            this.compraImporteCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Importe","{0:C}")});

            totalComprasCell.Text = String.Format("{0:C}", comprasData.Sum(c => c.Importe));

            //ventas
            var ventas = db.Ventas.Where(c => c.DiaContable.Fecha >= FechaInicio && c.DiaContable.Fecha <= FechaFin && (c.EstadoDeVenta == EstadoDeVenta.PagadaEnEfectivo || c.EstadoDeVenta == EstadoDeVenta.PagadaPorTarjeta|| c.EstadoDeVenta == EstadoDeVenta.PagadaPorFactura)).ToList();
            var ventasData = ventas.SelectMany(v => v.Elaboraciones).GroupBy(e => e.Detalle).Select(v => new
            {
                Menus = v.Key,
                Cantidad = v.Sum(c => c.Cantidad),
                Importe = v.Sum(c => c.ImporteTotal)
            }).ToList();

            VentasReport.DataSource = ventasData.OrderBy(v => v.Menus);

            this.ventaMenusCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Menus")});

            this.ventaCantidadCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Cantidad")});

            this.ventaImporteCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Importe","{0:C}")});

            ventaTotalImporteCell.Text = String.Format("{0:C}", ventasData.Sum(c => c.Importe));

            //gastos
            var gastos = db.OtrosGastos.Where(c => c.DiaContable.Fecha >= FechaInicio && c.DiaContable.Fecha <= FechaFin).ToList();
            var gastosData = gastos.Select(g => new
            {
                Fecha = g.DiaContable.Fecha.Date,
                Concepto = g.ConceptoDeGasto.Nombre,
                Importe = g.Importe
            }).ToList();
            OtrosGastosReport.DataSource = gastosData;

            this.gastosFechaCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Fecha","{0:d}")});

            this.gastosConceptoCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Concepto")});

            this.gastosImporteCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Importe","{0:C}")});

            gastosTotalCell.Text = String.Format("{0:C}",gastosData.Sum(g => g.Importe));

            //resumen
            var totalVentas = ventasData.Sum(v => v.Importe);
            var totalCompras = comprasData.Sum(c => c.Importe);
            var totalGastos = gastosData.Sum(g => g.Importe);

            total_ingresos.Text = String.Format("{0:C}", totalVentas);
            total_gastos_directos.Text = String.Format("{0:C}", totalCompras);
            total_gastos_indirectos.Text = String.Format("{0:C}", totalGastos);
            relacion_ingresos_gastos.Text = String.Format("{0:C}", totalVentas - totalCompras - totalGastos);
            
            //grafico
            var datosGrafico = new Dictionary<string, dynamic>();

            

            var resumenGastos = new List<ResumenOperaciones>();

            resumenGastos.AddRange(gastosData.GroupBy(g => g.Fecha.Date).Select(g => new ResumenOperaciones() {Fecha = g.Key,Gastos = g.Sum(d => d.Importe)}));

            foreach (var c in comprasData)
            {
                if (resumenGastos.Any(r => r.Fecha.Year == c.Fecha.Year && r.Fecha.Month == c.Fecha.Month && r.Fecha.Day == c.Fecha.Day))
                {
                    resumenGastos.SingleOrDefault(r => r.Fecha.Year == c.Fecha.Year && r.Fecha.Month == c.Fecha.Month && r.Fecha.Day == c.Fecha.Day).Gastos += c.Importe;
                }
                else
                {
                    resumenGastos.Add(new ResumenOperaciones() { Fecha = c.Fecha.Date, Gastos = c.Importe});
                } 
            }

            var data = new List<ResumenOperaciones>();
            data.AddRange(resumenGastos);
            foreach (var v in ventas)
            {
                if (data.Any(d => d.Fecha.Date == v.Fecha.Date))
                {
                    data.SingleOrDefault(d => d.Fecha.Date == v.Fecha.Date).Ingresos += v.Importe;
                }
                else
                {
                    data.Add(new ResumenOperaciones() {Fecha = v.Fecha, Ingresos = v.Importe});
                }
            }

            datosGrafico.Add("Gastos", resumenGastos.Select(g => new { Fecha = g.Fecha, Importe = g.Gastos }));
            datosGrafico.Add("Ingresos", ventas.Select(g => new { Fecha = g.Fecha, Importe = g.Importe }));

            //var data = operaciones.GroupBy(o => o.Fecha).Select(o => new { Fecha = o.Key, Ingresos = o.Where(op => op.Importe > 0).Sum(op => op.Importe), Gastos = -o.Where(op => op.Importe < 0).Sum(op => op.Importe) }).ToList();

            grafico.DataSource = data;

            grafico.Series["Ingresos"].ArgumentDataMember = "Fecha";
            grafico.Series["Ingresos"].ValueDataMembers.AddRange(new string[] { "Ingresos" });
            grafico.Series["Gastos"].ArgumentDataMember = "Fecha";
            grafico.Series["Gastos"].ValueDataMembers.AddRange(new string[] { "Gastos" });

        }

        class ResumenOperaciones
        {
            public DateTime Fecha { get; set; }

            public decimal Gastos { get; set; }

            public decimal Ingresos { get; set; }
        }

    }
}

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
    public partial class OperacioResumenMovimientosCC : DevExpress.XtraReports.UI.XtraReport
    {
        public OperacioResumenMovimientosCC(DateTime FechaInicio, DateTime FechaFin)
        {
            InitializeComponent();

            var db = new ErpContext();

            fecha_inicio.Text = "Desde: " + FechaInicio.ToShortDateString();
            fecha_fin.Text = "Hasta: " + FechaFin.ToShortDateString();

            //compras
            var compras = db.Compras.Where(c => c.Fecha >= FechaInicio && c.Fecha <= FechaFin).ToList();

            var comprasData = compras.SelectMany(c => c.Productos.Select(p => new
            {
                Fecha = c.Fecha,
                Tienda = c.Entidad.Nombre,
                Productos = p.Producto.Nombre,
                Cantidad = p.Cantidad + " " + p.UnidadDeMedida.Siglas,
                Importe = p.ImporteTotal
            })).ToList();

            ComprasReport.DataSource = comprasData;

            this.productoCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Fecha","{0:d}"), });

            this.tipoMovCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Tienda")});

            this.fechaCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Productos")});

            this.comprasCandidadCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Cantidad")});

            this.compraImporteCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Importe","{0:C}")});

            totalComprasCell.Text = String.Format("{0:C}", comprasData.Sum(c => c.Importe));

            //ventas
            var ventas = db.Ventas.Where(c => c.Fecha >= FechaInicio && c.Fecha <= FechaFin && (c.EstadoDeVenta == EstadoDeVenta.PagadaEnEfectivo || c.EstadoDeVenta == EstadoDeVenta.PagadaPorTarjeta)).ToList();
            var ventasData = ventas.Select(v => new
            {
                Fecha = v.Fecha,
                Posicion = v.PuntoDeVenta.Nombre,
                Menus = String.Join("\n\r",v.Elaboraciones
                .Select(e => e.Elaboracion.Nombre + (e.Agregados.Any()?
                " con "+ String.Join(", ", e.Agregados.Select(a => a.Agregado.Producto.Nombre + (a.Cantidad > 1? "(" + a.Cantidad + ")": ""))):""))),
                Cantidad = String.Join("\n\r", v.Elaboraciones.Select(e => e.Cantidad)),
                Importe = v.Importe
            }).ToList();

            VentasReport.DataSource = ventasData;

            this.ventaFechaCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Fecha","{0:d}"), });

            this.ventaPosicionCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Posicion")});

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
                Fecha = g.DiaContable.Fecha,
                Concepto = g.ConceptoDeGasto.Nombre,
                Importe = g.Importe
            }).ToList();
            //OtrosGastosReport.DataSource = gastosData;

            //this.gastosFechaCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            //new DevExpress.XtraReports.UI.XRBinding("Text", null, "Fecha","{0:d}")});

            //this.gastosConceptoCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            //new DevExpress.XtraReports.UI.XRBinding("Text", null, "Concepto")});

            //this.gastosImporteCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            //new DevExpress.XtraReports.UI.XRBinding("Text", null, "Importe","{0:C}")});

            //gastosTotalCell.Text = String.Format("{0:C}",gastosData.Sum(g => g.Importe));

            //resumen
            var totalVentas = ventasData.Sum(v => v.Importe);
            var totalCompras = comprasData.Sum(c => c.Importe);
            var totalGastos = gastosData.Sum(g => g.Importe);

            //total_ingresos.Text = String.Format("{0:C}", totalVentas);
            //total_gastos_directos.Text = String.Format("{0:C}", totalCompras);
            //total_gastos_indirectos.Text = String.Format("{0:C}", totalGastos);
            //relacion_ingresos_gastos.Text = String.Format("{0:C}", totalVentas - totalCompras - totalGastos);
            
            //grafico
            var datosGrafico = new Dictionary<string, dynamic>();

            

            var resumenGastos = new List<ResumenOperaciones>();

            resumenGastos.AddRange(gastosData.Select(g => new ResumenOperaciones() {Fecha = g.Fecha,Gastos = g.Importe}));

            foreach (var c in comprasData)
            {
                if (resumenGastos.Any(r => r.Fecha.Date.Date == c.Fecha))
                {
                    resumenGastos.SingleOrDefault(r => r.Fecha.Date == c.Fecha.Date).Gastos += c.Importe;
                }
                else
                {
                    resumenGastos.Add(new ResumenOperaciones() { Fecha = c.Fecha, Gastos = c.Importe});
                } 
            }
            

        }

        class ResumenOperaciones
        {
            public DateTime Fecha { get; set; }

            public decimal Gastos { get; set; }

            public decimal Ingresos { get; set; }
        }

    }
}

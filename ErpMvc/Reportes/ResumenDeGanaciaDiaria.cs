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
using AlmacenCore.Models;
using CompraVentaCore.Models;
using ErpMvc.ViewModels;

namespace ErpMvc.Reportes
{
    public partial class ResumenDeGanaciaDiaria : DevExpress.XtraReports.UI.XtraReport
    {
        public ResumenDeGanaciaDiaria(DateTime Fecha)
        {
            InitializeComponent();

            var db = new ErpContext();

            fecha.Text = Fecha.ToShortDateString();

            var fechaInicio = Fecha.Date;
            var fechaFin = fechaInicio.AddHours(23);
            fechaFin = fechaFin.AddMinutes(59);

            //compras
            var ventas = db.Ventas.Where(c => c.DiaContable.Fecha >= fechaInicio && c.DiaContable.Fecha <= fechaFin).ToList();

            var costos =
                db.MovimientosDeProductos.Where(
                    c => c.DiaContable.Fecha >= fechaInicio && c.DiaContable.Fecha <= fechaFin 
                    && (c.Tipo.Descripcion == TipoDeMovimientoConstantes.SalidaAProduccion || c.Tipo.Descripcion == TipoDeMovimientoConstantes.Merma)).GroupBy(m => m.CentroDeCosto).Select(m => new
                    {
                        CentroDeCosto = m.Key,
                        Costos = m.Sum(c => c.Costo)
                    });

            var costosNegativos =
                db.MovimientosDeProductos.Where(
                    c => c.DiaContable.Fecha >= fechaInicio && c.DiaContable.Fecha <= fechaFin
                    && (c.Tipo.Descripcion == TipoDeMovimientoConstantes.EntradaPorErrorDeSalida)).GroupBy(m => m.CentroDeCosto).Select(m => new
                    {
                        CentroDeCosto = m.Key,
                        Costos = m.Sum(c => c.Costo)
                    });

            var data = ventas.SelectMany(v => v.Elaboraciones.GroupBy(e => e.Elaboracion.CentroDeCosto).Select(e => new
            {
                CentroDeCosto = e.Key,
                Ventas = e.Sum(m => m.ImporteTotal),
            })).GroupBy(v => v.CentroDeCosto).Select(v => new
            {
                CentroDeCosto = v.Key,
                Ventas = v.Sum(e => e.Ventas),
                Costo = (costos.Any(c => c.CentroDeCosto.Id == v.Key.Id)?costos.SingleOrDefault(c => c.CentroDeCosto.Id == v.Key.Id).Costos: 0) - (costosNegativos.Any(c => c.CentroDeCosto.Id == v.Key.Id) ? costosNegativos.SingleOrDefault(c => c.CentroDeCosto.Id == v.Key.Id).Costos : 0),
                Ganancia = v.Sum(e => e.Ventas) - ((costos.Any(c => c.CentroDeCosto.Id == v.Key.Id) ? costos.SingleOrDefault(c => c.CentroDeCosto.Id == v.Key.Id).Costos : 0) - (costosNegativos.Any(c => c.CentroDeCosto.Id == v.Key.Id) ? costosNegativos.SingleOrDefault(c => c.CentroDeCosto.Id == v.Key.Id).Costos : 0))
            });

            var otrosGastosData =
                db.OtrosGastos.Where(c => c.DiaContable.Fecha >= fechaInicio && c.DiaContable.Fecha <= fechaFin)
                    .ToList();

            DataSource = data;

            this.centroDeCosto.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[]
            {
                new DevExpress.XtraReports.UI.XRBinding("Text", null, "CentroDeCosto.Nombre"),
            });

            this.ventasCc.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[]
            {
                new DevExpress.XtraReports.UI.XRBinding("Text", null, "Ventas","{0:C}")
            });

            this.gastoConsumoCc.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[]
            {
                new DevExpress.XtraReports.UI.XRBinding("Text", null, "Costo","{0:C}")
            });

            this.ganacia.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[]
            {
                new DevExpress.XtraReports.UI.XRBinding("Text", null, "Ganancia","{0:C}")
            });

            var sumaVentas = data.Sum(d => d.Ventas);
            var sumaCostos = data.Sum(d => d.Costo);
            var sumaGastos = otrosGastosData.Sum(d => d.Importe);

            totalVentas.Text = String.Format("{0:C}",sumaVentas);
            totalGastosporConsumo.Text = String.Format("{0:C}",sumaCostos);
            otrosGastos.Text = String.Format("{0:C}",sumaGastos);

            totalGanancias.Text = String.Format("{0:C}",sumaVentas - sumaGastos - sumaCostos);
        }
    }
}

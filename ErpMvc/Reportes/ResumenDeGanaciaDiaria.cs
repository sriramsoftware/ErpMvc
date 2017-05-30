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
    public partial class ResumenDeGanaciaDiaria : DevExpress.XtraReports.UI.XtraReport
    {
        public ResumenDeGanaciaDiaria(DateTime Fecha)
        {
            InitializeComponent();

            var db = new ErpContext();

            fecha.Text = Fecha.ToShortDateString();

            var fechaInicio = Fecha.Date;
            var fechaFin = fechaInicio.AddHours(23);

            //compras
            var ventas = db.Ventas.Where(c => c.Fecha >= fechaInicio && c.Fecha <= fechaFin).ToList();

            var data = ventas.SelectMany(v => v.Elaboraciones.GroupBy(e => e.Elaboracion.CentroDeCosto).Select(e => new
            {
                CentroDeCosto = e.Key,
                Ventas = e.Sum(m => m.ImporteTotal),
                Costo = e.Sum(m => m.Costo* m.Cantidad)
            })).GroupBy(v => v.CentroDeCosto).Select(v => new
            {
                CentroDeCosto = v.Key,
                Ventas = v.Sum(e => e.Ventas),
                Costo = v.Sum(e => e.Costo),
                Ganancia = v.Sum(e => e.Ventas) - v.Sum(e => e.Costo)
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
                new DevExpress.XtraReports.UI.XRBinding("Text", null, "Ventas")
            });

            this.gastoConsumoCc.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[]
            {
                new DevExpress.XtraReports.UI.XRBinding("Text", null, "Costo")
            });

            this.ganacia.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[]
            {
                new DevExpress.XtraReports.UI.XRBinding("Text", null, "Ganancia")
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

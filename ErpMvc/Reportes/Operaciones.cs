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
using ErpMvc.ViewModels;

namespace ErpMvc.Reportes
{
    public partial class Operaciones : DevExpress.XtraReports.UI.XtraReport
    {
        public Operaciones(ICollection<ResumenDeOperaciones> operaciones, DateTime FechaInicio, DateTime FechaFin)
        {
            InitializeComponent();

            var db = new ErpContext();

            fecha_inicio.Text = "Desde: " + FechaInicio.ToShortDateString();
            fecha_fin.Text = "Hasta: " + FechaFin.ToShortDateString();

            DataSource = operaciones.ToList();

            var totalIngresos = operaciones.Where(o => o.Tipo == "Venta").Sum(o => o.Importe);
            var totalGastosDirectos = operaciones.Where(o => o.Tipo == "Compra").Sum(o => o.Importe);
            var totalGastosIndirectos = operaciones.Where(o => o.Tipo == "Gasto").Sum(o => o.Importe);

            total_ingresos.Text = String.Format("{0:C}", totalIngresos);
            total_gastos_directos.Text = String.Format("{0:C}", totalGastosDirectos);
            total_gastos_indirectos.Text = String.Format("{0:C}", totalGastosIndirectos);
            relacion_ingresos_gastos.Text = String.Format("{0:C}", totalIngresos + totalGastosDirectos + totalGastosIndirectos);

            this.fecha.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Fecha","{0:d}"), });

            this.detalleCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Detalle")});

            this.tipoOperacionCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Tipo")});

            this.importeCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Importe")});

            var gastos = operaciones.Where(o => o.Importe < 0);
            var ingresos = operaciones.Where(o => o.Importe > 0);

            var datosGrafico = new Dictionary<string, dynamic>();

            datosGrafico.Add("Gastos", gastos.Select(g => new { Fecha = g.Fecha, Importe = -g.Importe }));
            datosGrafico.Add("Ingresos", ingresos.Select(g => new { Fecha = g.Fecha, Importe = g.Importe }));

            //DataTable table = new DataTable("Table1");

            //// Add two columns to the table. 
            //table.Columns.Add("Argument", typeof(DateTime));
            //table.Columns.Add("Value", typeof(Decimal));

            //// Add data rows to the table. 
            //DataRow row = null;
            //foreach (var ing in ingresos)
            //{
            //    row = table.NewRow();
            //    row["Argument"] = ing.Fecha;
            //    row["Value"] = ing.Importe;
            //    table.Rows.Add(row);
            //}

            var data = operaciones.GroupBy(o => o.Fecha).Select(o => new { Fecha = o.Key, Ingresos = o.Where(op => op.Importe > 0).Sum(op => op.Importe), Gastos = -o.Where(op => op.Importe < 0).Sum(op => op.Importe) }).ToList();

            grafico.DataSource = data;
            //grafico.SeriesDataMember = "Operacion";
            grafico.Series["Ingresos"].ArgumentDataMember = "Fecha";
            grafico.Series["Ingresos"].ValueDataMembers.AddRange(new string[] { "Ingresos" });
            grafico.Series["Gastos"].ArgumentDataMember = "Fecha";
            grafico.Series["Gastos"].ValueDataMembers.AddRange(new string[] { "Gastos" });

            //grafico.SeriesDataMember = "Gastos";
            //grafico.SeriesDataMember = "Ingresos";

        }

    }
}

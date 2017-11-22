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
    public partial class ComprasConComprobantes : DevExpress.XtraReports.UI.XtraReport
    {
        public ComprasConComprobantes(int mes,string mesNombre)
        {
            InitializeComponent();

            var db = new ErpContext();
            
            //compras
            //todo: agregar la seleccion del año
            var compras = db.Compras.ToList().Where(c => c.DiaContable.Fecha.Month == mes && c.TieneComprobante).ToList();

            titulo_reporte.Text = "Compras con comprobantes del mes " + mesNombre;

            var comprasData = compras.GroupBy(c => c.Fecha.Date).Select(c => new
            {
                Fecha = c.Key,
                Importe = c.Sum(i => i.Productos.Sum(p => p.ImporteTotal)),
                Compras = c.Select(co => co).ToList()
            }).ToList();

            DataSource = comprasData;

            xrSubreport1.ReportSource = new Compras();

            this.fechaCompraCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Fecha","{0:d}"), });

            this.compraImporteCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Importe")});
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var lista = (List<Compra>)GetCurrentColumnValue("Compras");
            var report = ((Compras)((XRSubreport)sender).ReportSource);
            report.CargarDatos(lista);
        }
    }
}

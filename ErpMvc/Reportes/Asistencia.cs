using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.Data.WcfLinq.Helpers;
using DevExpress.XtraReports.UI;
using ErpMvc.Models;
using System.Data.Entity;
using System.Linq;

namespace ErpMvc.Reportes
{
    public partial class Asistencia : DevExpress.XtraReports.UI.XtraReport
    {
        public Asistencia()
        {
            InitializeComponent();

            var db = new ErpContext();

            fecha_reporte.Text = "Fecha: " + DateTime.Now.ToShortDateString();

            DataSource = db.ExistenciasEnCentroDeCostos.GroupBy(e => e.Producto).Select(e => new
            {
                Producto = e.Key.Producto.Nombre,
                Um = e.Key.UnidadDeMedida.Siglas,
                Cantidad = e.Sum(c => c.Cantidad),
                Valor = e.Sum(v => v.Producto.PrecioUnitario * v.Cantidad)
            }).ToList();

            this.productoCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Producto")});

            this.umCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Um")});

            this.cantidadCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Cantidad")});

            this.valorCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Valor")});
        }

    }
}

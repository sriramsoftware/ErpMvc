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
    public partial class ValeDeVenta : DevExpress.XtraReports.UI.XtraReport
    {
        public ValeDeVenta(int id)
        {
            InitializeComponent();

            var db = new ErpContext();
            var venta = db.Ventas.Find(id);

            titulo_reporte.Text = "Comanda # " + id;
            atendidoPor.Text = "Atendido por: " + venta.Vendedor.NombreCompleto;

            fecha_reporte.Text = "Fecha: " + venta.Fecha.ToShortDateString();
            var data = venta.Elaboraciones.Select(e => new
            {
                Menu = e.Elaboracion.Nombre + (e.Agregados.Count > 0? " con: " + String.Join(",",e.Agregados.Select(a => a.Agregado.Producto.Nombre + " (" + a.Cantidad + ")")):""),
                Cantidad = e.Cantidad,
                importe = Math.Round(e.ImporteTotal, 2)
            }).ToList();
            DataSource = data;

            this.menuCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Menu")});

            this.cantidadCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Cantidad")});

            this.importeCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Importe","{0:C}")});

            totalCuc.Text = String.Format("{0:C}",data.Sum(d => d.importe));
            totalCup.Text = String.Format("{0:C}", data.Sum(d => d.importe) * 25);
        }

    }
}

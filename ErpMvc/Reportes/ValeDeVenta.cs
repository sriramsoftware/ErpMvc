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

            fecha_reporte.Text = "Amelia del Mar        " + venta.Fecha.ToShortDateString();
            titulo_reporte.Text = "Comanda # " + id;
            atendidoPor.Text = "Dep: " + venta.Vendedor.NombreCompleto + " | Pos: " + venta.PuntoDeVenta.Nombre + " | Pax: " + venta.CantidadPersonas;

            var data = venta.Elaboraciones.Select(e => new
            {
                Menu = e.Elaboracion.Nombre + (e.Agregados.Count > 0? " con: " + String.Join(",",e.Agregados.Select(a => a.Agregado.Producto.Nombre + " (" + a.Cantidad + ")")):""),
                Cantidad = (int)e.Cantidad,
                Precio = e.Elaboracion.PrecioDeVenta + e.Agregados.Sum(a => a.Agregado.Precio),
                Importe = Math.Round(e.ImporteTotal, 2)
            }).ToList();
            DataSource = data;

            this.menuCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Menu")});

            this.cantidadCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Cantidad")});

            this.precioCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Precio","{0:C}")});

            this.importeCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Importe","{0:C}")});

            totalCuc.Text = String.Format("{0:C}",data.Sum(d => d.Importe));
            totalCup.Text = String.Format("{0:C}", data.Sum(d => d.Importe) * 25);
        }

    }
}

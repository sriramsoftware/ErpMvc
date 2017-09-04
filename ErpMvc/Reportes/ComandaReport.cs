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
    public partial class ComandaReport : DevExpress.XtraReports.UI.XtraReport
    {
        public ComandaReport(int id)
        {
            InitializeComponent();

            var db = new ErpContext();
            var comanda = db.Comandas.Find(id);

            fecha_reporte.Text = "Amelia del Mar (" + comanda.Fecha.ToShortDateString() + " " + comanda.Fecha.ToShortTimeString() + ")";
            titulo_reporte.Text = "Comanda # " + comanda.VentaId;
            atendidoPor.Text = "Dep: " + comanda.Vendedor.NombreCompleto + " | Pos: " + comanda.PuntoDeVenta.Nombre + " | Pax: " + comanda.CantidadPersonas;

            niñasCell.Text = "( " +
                             String.Join(",",
                                 comanda.Comensales.Where(c => c.Comensal == Comensal.Niña).Select(c => c.Numero)) + ")";
            niñosCell.Text = "( " +
                                         String.Join(",",
                                             comanda.Comensales.Where(c => c.Comensal == Comensal.Niño).Select(c => c.Numero)) + ")";
            mujeresCell.Text = "( " +
                                         String.Join(",",
                                             comanda.Comensales.Where(c => c.Comensal == Comensal.Mujer).Select(c => c.Numero)) + ")";
            hombresCell.Text = "( " +
                                         String.Join(",",
                                             comanda.Comensales.Where(c => c.Comensal == Comensal.Hombre).Select(c => c.Numero)) + ")";

            var data = comanda.Detalles.Select(e => new
            {
                Menu = e.Elaboracion.Nombre + (e.Agregados.Count > 0 ? " con: " + String.Join(",", e.Agregados.Select(a => a.Agregado.Producto.Nombre + " (" + a.Cantidad + ")")) : ""),
                Cantidad = (int)e.Cantidad,
                Detalles = String.Join("\n\r", e.Ordenes.Select(o => "[" + o.Orden.Numero + "]: " + String.Join(",", o.Anotaciones.Select(a => a.Abreviatura))))
            }).ToList();

            DataSource = data;

            this.menuCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Menu")});

            this.cantidadCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Cantidad")});

            this.detallesCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Detalles")});


        }

    }
}

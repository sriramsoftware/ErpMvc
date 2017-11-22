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
    public partial class Compras : DevExpress.XtraReports.UI.XtraReport
    {
        public Compras()
        {
            InitializeComponent();
        }

        public void CargarDatos(List<Compra> compras)
        {
            var comprasData = compras.Select(c => new
            {
                c.Fecha,
                Tienda = c.Entidad.Nombre,
                Productos = String.Join("\n\r", c.Productos.Select(p => p.Producto.Nombre)),
                Cantidad = String.Join("\n\r", c.Productos.Select(p => p.Cantidad + " " + p.UnidadDeMedida.Siglas)),
                Importe = String.Join("\n\r", c.Productos.Select(p => p.ImporteTotal)) + "\n\r" + c.Productos.Sum(p => p.ImporteTotal)
            }).ToList();

            DataSource = comprasData;


            this.fechaCompraCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Fecha","{0:d}"), });

            this.compraTiendaCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Tienda")});

            this.compraProductosCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Productos")});

            this.comprasCandidadCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Cantidad")});

            this.compraImporteCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Importe")});
        }
    }
}

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
using CompraVentaBL;
using CompraVentaCore.Models;
using ErpMvc.ViewModels;

namespace ErpMvc.Reportes
{
    public partial class FichaDeCosto : DevExpress.XtraReports.UI.XtraReport
    {
        public FichaDeCosto(int menuId)
        {
            InitializeComponent();

            var db = new ErpContext();
            var _productoService = new ProductoService(db);
            var menu = db.Set<Elaboracion>().Find(menuId);
            //compras
            nombreMenu.Text = menu.Nombre;

            var productosData = menu.Productos.Select(p => new
            {
                Producto = p.Producto.Nombre,
                p.Cantidad,
                UnidadDeMedida = p.UnidadDeMedida.Siglas,
                Costo = p.Cantidad * _productoService.GetPrecioUnitarioDeProducto(p.ProductoId, p.UnidadDeMedidaId)
        });

            ProductosReport.DataSource = productosData;

            this.produtoCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Producto"), });

            this.cantidadCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Cantidad")});

            this.uMCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "UnidadDeMedida")});

            this.costoCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Costo")});

            
            costoTotalCell.Text = String.Format("{0:C}", productosData.Sum(c => c.Costo));

            //ventas
            
            AgregosReport.DataSource = menu.Agregados;

            this.agregoCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Producto.Nombre")});

            this.agregoCantidadCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Cantidad")});

            this.agregoUmCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "UnidadDeMedida.Siglas")});

        }
    }
}

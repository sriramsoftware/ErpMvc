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
    public partial class Inventario : DevExpress.XtraReports.UI.XtraReport
    {
        public Inventario(string lugar)
        {
            InitializeComponent();

            var db = new ErpContext();

            fecha_reporte.Text = "Fecha: " + DateTime.Now.ToShortDateString();

            titulo_reporte.Text += " de " + lugar;

            if (lugar == "Almacen")
            {
                //DataSource = db.ExistenciasEnAlmacenes.Where(e => e.Producto.Producto.Activo).GroupBy(e => e.Producto).Select(e => new
                //{
                //    Producto = e.Key.Producto.Nombre,
                //    Um = e.Key.UnidadDeMedida.Siglas,
                //    Cantidad = e.Sum(c => c.ExistenciaEnAlmacen),
                //    Valor = Math.Round(e.Sum(v => v.Producto.PrecioUnitario * v.ExistenciaEnAlmacen), 2)
                //}).ToList();
                DataSource = db.ExistenciasEnAlmacenes.Where(e => e.Producto.Producto.Activo).Select(e => new
                {
                    Producto = e.Producto.Producto.Nombre,
                    Um = e.Producto.UnidadDeMedida.Siglas,
                    Cantidad = e.ExistenciaEnAlmacen,
                    Valor = e.Producto.PrecioUnitario * e.ExistenciaEnAlmacen
                }).ToList();
            }
            else
            {
                var cc = db.CentrosDeCostos.SingleOrDefault(c => c.Nombre == lugar);
                //DataSource = db.ExistenciasEnCentroDeCostos.Where(e => e.Producto.Producto.Activo && e.CentroDeCostoId == cc.Id).GroupBy(e => e.Producto).Select(e => new
                //{
                //    Producto = e.Key.Producto.Nombre,
                //    Um = e.Key.UnidadDeMedida.Siglas,
                //    Cantidad = e.Sum(c => c.Cantidad),
                //    Valor = Math.Round(e.Sum(v => v.Producto.PrecioUnitario * v.Cantidad), 2)
                //}).ToList();
                DataSource = db.ExistenciasEnCentroDeCostos.Where(e => e.Producto.Producto.Activo && e.CentroDeCostoId == cc.Id).Select(e => new
                {
                    Producto = e.Producto.Producto.Nombre,
                    Um = e.Producto.UnidadDeMedida.Siglas,
                    Cantidad = e.Cantidad,
                    Valor = Math.Round(e.Producto.PrecioUnitario * e.Cantidad, 2)
                }).ToList();
            }

            this.productoCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Producto")});

            this.umCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Um")});

            this.cantidadCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Cantidad")});

            this.valorCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Valor","{0:C}")});
        }

    }
}

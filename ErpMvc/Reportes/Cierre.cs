using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.Data.WcfLinq.Helpers;
using DevExpress.XtraReports.UI;
using ErpMvc.Models;
using System.Data.Entity;
using System.Linq;
using ErpMvc.ViewModels;

namespace ErpMvc.Reportes
{
    public partial class Cierre : DevExpress.XtraReports.UI.XtraReport
    {
        public Cierre(CierreViewModel cierre)
        {
            InitializeComponent();

            titulo_reporte.Text = "Cierre        " + cierre.Fecha.ToShortDateString();

            var efectivoEnCaja = String.Format("{0:C}", cierre.Desgloce.Sum(d => d.Cantidad * d.DenominacionDeMoneda.Valor));

            efectivoInicial.Text = String.Format("{0:C}", cierre.EfectivoAnterior);
            ventas.Text = String.Format("{0:C}", cierre.Ventas);
            depositos.Text = String.Format("{0:C}", cierre.Depositos);
            extracciones.Text = String.Format("{0:C}", cierre.Extracciones);
            extraccionCierre.Text = String.Format("{0:C}", cierre.ExtraccionCierre);
            pagoTrab.Text = String.Format("{0:C}", cierre.PagoTrabajadores);
            efectivoCaja.Text = String.Format("{0:C}", 100);

            ventasTotales.Text = String.Format("{0:C}", cierre.Ventas);
            excentasPorciento.Text = String.Format("{0:C}", cierre.VentasSinPorciento);
            calculaPorciento.Text = String.Format("{0:C}", cierre.Ventas - cierre.VentasSinPorciento);
            var porCiento = (cierre.Ventas - cierre.VentasSinPorciento) * 0.1m;
            porciento.Text = String.Format("{0:C}", porCiento);

            propina.Text = String.Format("{0:C}", cierre.Propinas);
            totalPropinaPorciento.Text = String.Format("{0:C}", porCiento + cierre.Propinas);


            DataSource = cierre.Desgloce.Select(d => new 
            {
                Valor = d.DenominacionDeMoneda.Valor,
                Cantidad = d.Cantidad,
                Importe = d.Cantidad * d.DenominacionDeMoneda.Valor
            }).OrderByDescending(d => d.Valor);

            this.denominacionCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Valor")});

            this.cantidadCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Cantidad")});

            this.importeCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Importe","{0:C}")});

            totalCuc.Text = efectivoEnCaja;
        }

    }
}

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

            efectivoInicial.Text = String.Format("{0:C}", cierre.EfectivoAnterior);
            ventas.Text = String.Format("{0:C}", cierre.Ventas);
            barVentas.Text = String.Format("{0:C}", cierre.CentrosDeCosto.SingleOrDefault(d => d.Nombre == "Bar").Importe);
            restaurantVentas.Text = String.Format("{0:C}", cierre.CentrosDeCosto.SingleOrDefault(d => d.Nombre == "Restaurant").Importe);
            depositos.Text = String.Format("{0:C}", cierre.Depositos);
            extracciones.Text = String.Format("{0:C}", cierre.Extracciones);
            extraccionCierre.Text = String.Format("{0:C}", cierre.ExtraccionCierre);
            pagoTrab.Text = String.Format("{0:C}", cierre.PagoTrabajadores);
            efectivoCaja.Text = String.Format("{0:C}", 100);

            ventasTotales.Text = String.Format("{0:C}", cierre.Ventas);
            pagoPorFactura.Text = String.Format("{0:C}", cierre.VentasPorFactura);
            excentasPorciento.Text = String.Format("{0:C}", cierre.VentasSinPorciento);
            ventasAlCosto.Text = String.Format("{0:C}", cierre.VentasAlCosto);
            calculaPorciento.Text = String.Format("{0:C}", cierre.SeLeCalculaPorciento);
            var porCiento = cierre.Porciento;
            porciento.Text = String.Format("{0:C}", porCiento);

            propina.Text = String.Format("{0:C}", cierre.Propinas);
            totalPropinaPorciento.Text = String.Format("{0:C}", porCiento + cierre.Propinas);

            var cuc =
                cierre.Desgloce.Where(d => d.DenominacionDeMoneda.Moneda.Sigla == "CUC")
                    .Sum(d => d.Cantidad*d.DenominacionDeMoneda.Valor);

            var cup =
                cierre.Desgloce.Where(d => d.DenominacionDeMoneda.Moneda.Sigla == "CUP")
                    .Sum(d => d.Cantidad * d.DenominacionDeMoneda.Valor);

            CUCReport.DataSource = cierre.Desgloce.Where(d => d.DenominacionDeMoneda.Moneda.Sigla == "CUC").Select(d => new 
            {
                Valor = d.DenominacionDeMoneda.Valor,
                Cantidad = d.Cantidad,
                Importe = d.Cantidad * d.DenominacionDeMoneda.Valor
            }).OrderByDescending(d => d.Valor);

            this.denominacionCuc.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Valor")});

            this.cantidadCuc.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Cantidad")});

            this.valorCuc.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Importe","{0:C}")});

            totalCuc.Text = String.Format("{0:C}", cuc);

            CUPReport.DataSource = cierre.Desgloce.Where(d => d.DenominacionDeMoneda.Moneda.Sigla == "CUP").Select(d => new
            {
                Valor = d.DenominacionDeMoneda.Valor,
                Cantidad = d.Cantidad,
                Importe = d.Cantidad * d.DenominacionDeMoneda.Valor
            }).OrderByDescending(d => d.Valor);

            this.denominacionCup.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Valor")});

            this.cantidadCup.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Cantidad")});

            this.valorCup.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Importe","{0:C}")});

            totalCup.Text = String.Format("{0:C}", cup);
            cupEnCucTotal.Text = String.Format("{0:C}", cup / 25);
            totalConvertidoCuc.Text = String.Format("{0:C}", cuc +(cup / 25));
        }

    }
}

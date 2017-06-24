using System;
using System.Collections.Generic;
using CajaCore.Models;

namespace ErpMvc.ViewModels
{
    public class CierreViewModel
    {
        public DateTime Fecha { get; set; }

        public decimal EfectivoAnterior { get; set; }

        public decimal Ventas { get; set; }

        public decimal VentasPorFactura { get; set; }

        public decimal VentasSinPorciento { get; set; }

        public decimal VentasAlCosto { get; set; }

        public decimal Depositos { get; set; }

        public decimal Extracciones { get; set; }

        public decimal Propinas { get; set; }

        public decimal ExtraccionCierre { get; set; }

        public decimal PagoTrabajadores { get; set; }

        public decimal SeLeCalculaPorciento
        {
            get { return Ventas + VentasPorFactura - VentasSinPorciento - VentasAlCosto; }
        }

        public decimal Porciento { get { return SeLeCalculaPorciento*0.1m; } }

        public List<DenominacionesEnCierreDeCaja> Desgloce { get; set; }

        public List<ResumenCentroCosto> CentrosDeCosto { get; set; }

        public CierreViewModel()
        {
            Desgloce = new List<DenominacionesEnCierreDeCaja>();
        }
    }

    public class ResumenCentroCosto
    {
        public string Nombre { get; set; }

        public decimal Importe { get; set; }
    }
}
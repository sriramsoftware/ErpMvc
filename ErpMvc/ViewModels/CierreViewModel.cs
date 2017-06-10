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

        public decimal VentasSinPorciento { get; set; }

        public decimal Depositos { get; set; }

        public decimal Extracciones { get; set; }

        public decimal Propinas { get; set; }

        public decimal ExtraccionCierre { get; set; }

        public decimal PagoTrabajadores { get; set; }

        public List<DenominacionesEnCierreDeCaja> Desgloce { get; set; }

        public CierreViewModel()
        {
            Desgloce = new List<DenominacionesEnCierreDeCaja>();
        }
    }
}
using System;
using System.Collections.Generic;
using CompraVentaCore.Models;
using ContabilidadCore.Models;
using ErpMvc.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeguridadCore.Models;

namespace ErpMvcTest
{
    [TestClass]
    public class VentasTest
    {
        [TestMethod]
        public void LoadTest()
        {
            var db = new ErpContext();
            var dia = new DiaContable() {Fecha = DateTime.Now,Abierto = true};
            db.Set<DiaContable>().Add(dia);

            var user = new Usuario() {UserName = "pepe"};
            db.SaveChanges();

            var venta = new Venta() {Fecha = DateTime.Now, DiaContable = dia,Elaboraciones = new List<DetalleDeVenta>() {new DetalleDeVenta() {ElaboracionId = 50,Cantidad = 2} } };
        }
    }
}

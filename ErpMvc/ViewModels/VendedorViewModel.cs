using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CompraVentaCore.Models;
using DevExpress.Office.Utils;
using SeguridadCore.ViewModels;

namespace ErpMvc.ViewModels
{
    public class VendedorViewModel
    {
        public Vendedor Vendedor { get; set; }

        public UsuarioViewModel UsuarioViewModel { get; set; }

        public List<string> Roles { get; set; }

        //public int PuntoDeVentaId { get; set; }

        //[Required]
        //public string Usuario { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //public string Contraseña { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirme la contraseña")]
        //[Compare("Contraseña", ErrorMessage = "La contrase y la confirmación no coinciden.")]
        //public string ContraseñaConfirmada { get; set; }


        public VendedorViewModel()
        {
            
        }

        public VendedorViewModel(Vendedor vendedor)
        {
            Vendedor = vendedor;
            //PuntoDeVentaId = vendedor.PuntoDeVentaId;
        }
    }
}
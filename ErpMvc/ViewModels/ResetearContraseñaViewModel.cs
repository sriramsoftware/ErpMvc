using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ErpMvc.ViewModels
{
    public class ResetearContraseñaViewModel
    {
        public string Usuarioid { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string ContraseñaNueva { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirme la nueva contraseña")]
        [Compare("ContraseñaNueva", ErrorMessage = "La contrase nueva y la confirmación no coinciden.")]
        public string ConfimaContraseña { get; set; }
    }
}
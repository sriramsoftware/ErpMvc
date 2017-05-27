using System;

namespace ErpMvc.Models
{
    public class LicenciaInfo
    {
        public int Id { get; set; }

        public string Suscriptor { get; set; }

        public string Aplicacion { get; set; }

        public DateTime FechaDeVencimiento { get; set; }

        public byte[] Hash { get; set; }
    }
}
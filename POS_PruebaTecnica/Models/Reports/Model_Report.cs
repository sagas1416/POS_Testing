using System.ComponentModel.DataAnnotations;

namespace POS_PruebaTecnica.Models.Reports
{
    public class Model_Report
    {
        [Key]
        public string id { get; set; }
        public DateTime fecha { get; set; }
        public string? tipo { get; set; }
        public decimal cantidad { get; set; }
        public string? descripcion { get; set; }
        public decimal precio { get; set; }
        public decimal montoNeto { get; set; }
        public decimal montoIVA { get; set; }
        public decimal montoGrav { get; set; }
        public string? Mensaje { get; set; }
    }
}

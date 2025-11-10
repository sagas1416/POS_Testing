using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_PruebaTecnica.Models.Productos
{ 
    public class Model_Productos
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int nkey { get; set; }
        public string? codigo { get; set; }
        public string descripcion { get; set; }
        public decimal precio { get; set; }
        public decimal inventario { get; set; }
        public bool activo { get; set; }
        public string? Mensaje { get; set; }
    } 
}

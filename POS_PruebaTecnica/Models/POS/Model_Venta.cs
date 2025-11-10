
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_PruebaTecnica.Models.POS
{
    public class Model_Venta
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int nkey { get; set; }
        public string id { get; set; }
        public int idTienda { get; set; }
        public int idTerminal { get; set; }
        public DateTime fechahora { get; set; }
        public int idUser { get; set; }
        public decimal montoNeto { get; set; }
        public decimal montoIVA { get; set; }
        public decimal montoGrav { get; set; }
        public bool invalidado { get; set; }
        public string? idInvalidacion { get; set; }
        public int tipo { get; set; }
    }
    public class Transaccion_Detalle
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int nkey { get; set; }
        public string idVenta { get; set; }
        public DateTime fecha { get; set; }
        public int idProducto { get; set; }
        public string descripcion { get; set; }
        public decimal cantidad { get; set; }
        public decimal precio { get; set; }
        public decimal precioNeto { get; set; }
        public decimal montoIVA { get; set; }
        public decimal subTotal { get; set; }

    }
    public class Sales
    {
        public Model_Venta cabecera { get; set; }
        public List<Transaccion_Detalle>? detalle { get; set; }
        public string? Mensaje { get; set; }
    }
    
}

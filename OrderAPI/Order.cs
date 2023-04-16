using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI
{
    [Serializable]
    public class Order
    {
        public long Id { get; set; }

        [Required]
        public string ProductId { get; set; }

        [DefaultValue(1)]
        public ushort Quantity { get; set; }
    }
}
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CrudASPCore.Models
{
    public class Category
    {
        [Key]
        public int Category_Id { get; set; }
        [Required]
        [DisplayName("Category Name")]
        public string Category_Name { get; set; }

        public List<Product> products { get; set; }
    }
}

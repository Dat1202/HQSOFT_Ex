using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HQSOFT_Ex1.Entities
{
    public class Book
    {
        public int BookId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public DateTime? PublishedDate { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }

}

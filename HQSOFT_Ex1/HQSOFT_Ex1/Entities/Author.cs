using System.ComponentModel.DataAnnotations;

namespace HQSOFT_Ex1.Entities
{
    public class Author
    {
        public int AuthorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Bio { get; set; }

        public ICollection<Book> Books { get; set; }
    }

}

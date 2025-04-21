namespace HQSOFT_Ex1.DTOs
{
    public class BookDto
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public DateTime? PublishedDate { get; set; }

        public int AuthorId { get; set; }
    }


}

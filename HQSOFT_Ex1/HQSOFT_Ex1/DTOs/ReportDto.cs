namespace HQSOFT_Ex1.DTOs
{
    public class ReportDto
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string AuthorName { get; set; }
    }
}

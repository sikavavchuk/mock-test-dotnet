using System.ComponentModel.DataAnnotations;

namespace mockTest.DTOs
{
    public class CreateBooksDetailsDto
    {
        [StringLength((200))]
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
    }
}


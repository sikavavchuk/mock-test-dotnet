namespace mockTest.DTOs
{
    public class CreateLoansWithBooksDto
    {
        public int LoanId { get; set; }
        public int ReaderId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public List<CreateBooksDetailsDto> Books { get; set; } = [];
    }
}


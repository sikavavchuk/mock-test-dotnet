namespace mockTest.DTOs;

public class GetLoansDetailsDto
{
    public int Id { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public List<GetBooksDetailsDto> Books { get; set; } = [];
}
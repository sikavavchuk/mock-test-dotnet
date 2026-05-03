namespace mockTest.DTOs
{
    public class GetReadersInfoWithLoansDto
    {
        public string FirstName { get; set; } =  string.Empty;
        public string LastName { get; set; } =  string.Empty;
        public string Email { get; set; } =  string.Empty;
        public string MemberCardNumber { get; set; } =  string.Empty;
        public DateTime RegisteredDate { get; set; }
        public List<GetLoansDetailsDto> Loans { get; set; } = [];
    }
}


using mockTest.DTOs;
using Microsoft.Data.SqlClient;
using mockTest.Exceptions;

namespace mockTest.Services;

public class DbService : IDbService
{
    private readonly string _connectionString;
    public DbService(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task<GetReadersInfoWithLoansDto> GetReadersLoansAsync(int readerId)
    {
        var query = """
                    SELECT read.FirstName AS FirstName,
                        read.LastName AS LastName,
                        read.Email AS Email,
                        read.MemberCardNumber AS MemberCardNumber,
                        read.RegisteredDate AS RegisteredDate,
                        l.Id AS LoanId,
                        l.LoanDate AS LoanDate,
                        r.DueDate AS DueDate,
                        r.ReturnDate AS ReturnDate,
                        b.Title AS BookTitle,
                        b.Author AS BookAuthor,
                    FROM Readers r
                    JOIN Loans l ON r.LoanId = l.LoanId
                    JOIN Books b ON r.BookId = b.BookId
                    WHERE r.ReaderID = @readerId
                    """;
        
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@readerId", readerId);
        
        await using var reader = await command.ExecuteReaderAsync();

        GetReadersInfoWithLoansDto? result = null;
        
        var ordFirstName = reader.GetOrdinal("FirstName");
        var ordLastName = reader.GetOrdinal("LastName");
        var ordEmail = reader.GetOrdinal("Email");
        var ordMemberCardNumber = reader.GetOrdinal("MemberCardNumber");
        var ordRegisteredDate = reader.GetOrdinal("RegisteredDate");
        var ordLoanId = reader.GetOrdinal("LoanId");
        var ordLoanDate = reader.GetOrdinal("LoanDate");
        var ordDueDate = reader.GetOrdinal("DueDate");
        var ordReturnDate = reader.GetOrdinal("ReturnDate");
        var ordBookTitle = reader.GetOrdinal("BookTitle");
        var ordBookAuthor = reader.GetOrdinal("BookAuthor");

        while (await reader.ReadAsync())
        {
            if (result is null)
            {
                result = new GetReadersInfoWithLoansDto()
                {
                    FirstName = reader.GetString(ordFirstName),
                    LastName = reader.GetString(ordLastName),
                    Email = reader.GetString(ordEmail),
                    MemberCardNumber = reader.GetString(ordMemberCardNumber),
                    RegisteredDate = reader.GetDateTime(ordRegisteredDate),
                    Loans = new List<GetLoansDetailsDto>()
                };
            }
            
            var loanId =  reader.GetInt32(ordLoanId);
            
            var loan = result.Loans.FirstOrDefault(l => l.Id == loanId);
            if (loan is null)
            {
                loan = new GetLoansDetailsDto()
                {
                    Id = loanId,
                    LoanDate = reader.GetDateTime(ordLoanDate),
                    ReturnDate = reader.IsDBNull(ordReturnDate)
                        ? null
                        : reader.GetDateTime(ordReturnDate),
                    Books = new List<GetBooksDetailsDto>()
                };
                result.Loans.Add(loan);
            }

            loan.Books.Add(new GetBooksDetailsDto
            {
                Title = reader.GetString(ordBookTitle),
                Author = reader.GetString(ordBookAuthor)
            });

        }
        return result ?? throw new NotFoundException("No loans found for the specified reader.");
    }

    public async Task CreateLoansWithBooksAsync(int readerId, CreateLoansWithBooksDto dto)
    {
        var createLoanQuery = """

                              """;
        var createLoanItemQuery = """

                                  """;
        var getBookIdQuery = """

                             """;
        var checkReaderQuery = """

                               """;
        
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
	    
        await using var transaction = await connection.BeginTransactionAsync();
        
        await using var command = new SqlCommand();
        command.Connection = connection;
        command.Transaction = transaction as SqlTransaction;

        try
        {
            command.Parameters.Clear();
            command.CommandText =  createLoanQuery;
            command.Parameters.AddWithValue("Id", readerId);

            var readerIdRes = await command.ExecuteScalarAsync();
            command.Parameters.Clear();
            command.CommandText = createLoanQuery;
            command.Parameters.AddWithValue("@ReaderId", readerId);
            command.Parameters.AddWithValue("@LoanId", readerId);
            command.Parameters.AddWithValue("@LoanDate", dto.LoanDate);
            command.Parameters.AddWithValue("@DueDate", dto.DueDate);
            
            var loanObject = await command.ExecuteScalarAsync();
            var loanId = Convert.ToInt32(loanObject);

            foreach (var book in dto.Books)
            {
                command.Parameters.Clear();
                command.CommandText = getBookIdQuery;
                command.Parameters.AddWithValue("@Title", book.Title);
				
                var bookObject = await command.ExecuteScalarAsync();
                if (bookObject == null)
                {
                    throw new NotFoundException($"Movie - {book.Title} - not found.");
                }
				
                var bookId = Convert.ToInt32(bookObject);
				
                command.Parameters.Clear();
                command.CommandText = createLoanItemQuery;
                command.Parameters.AddWithValue("@LoanId", loanId);
                command.Parameters.AddWithValue("@BookId", bookId);

                await command.ExecuteNonQueryAsync();
            }
		    
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
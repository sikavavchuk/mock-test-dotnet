using mockTest.DTOs;

namespace mockTest.Services;

public interface IDbService
{
    Task<GetReadersInfoWithLoansDto>  GetReadersLoansAsync(int readerId);
    Task CreateLoansWithBooksAsync(int readerId, CreateLoansWithBooksDto dto);
}
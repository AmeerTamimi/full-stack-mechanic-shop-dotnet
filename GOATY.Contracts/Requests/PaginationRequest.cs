namespace GOATY.Contracts.Requests
{
    public sealed record class PaginationRequest(int Page = 1 , int PageSize = 10);
}

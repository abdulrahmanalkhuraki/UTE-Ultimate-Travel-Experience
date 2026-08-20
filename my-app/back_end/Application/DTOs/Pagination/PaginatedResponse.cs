namespace Application.DTOs.Pagination
{
    public class PaginatedResponse<T>
    {
        public IReadOnlyCollection<T> Items { get; init; } = [];

        public PaginationMetadata Pagination { get; init; } = default!;
    }
}

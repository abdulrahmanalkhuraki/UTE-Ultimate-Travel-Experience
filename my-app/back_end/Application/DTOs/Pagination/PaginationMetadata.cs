namespace Application.DTOs.Pagination
{
    public class PaginationMetadata
    {
        public int Page { get; init; }

        public int PageSize { get; init; }

        public int TotalItems { get; init; }

        public int TotalPages =>
            (int)Math.Ceiling((double)TotalItems / PageSize);

        public bool HasPreviousPage => Page > 1;

        public bool HasNextPage => Page < TotalPages;
    }
}

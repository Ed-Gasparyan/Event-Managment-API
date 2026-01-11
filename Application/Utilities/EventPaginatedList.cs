using Application.DTOs.EventDTOs;

namespace Application.Utilities
{
    public class EventPaginatedList
    {
        public IEnumerable<EventResponseDto> Items { get; private set; } = null!;
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }
        public int PageSize { get; private set; }

        public EventPaginatedList(IEnumerable<EventResponseDto> items, int count, int pageIndex, int pageSize)
        {
            Items = items;
            TotalCount = count;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}


namespace Farm.Domain.ViewModels.Paging
{
    public class PaginationResponseDto<T> where T : class
    {
        public IReadOnlyCollection<T> Items { get; set; }

        public long Total { get; set; }
    }
}

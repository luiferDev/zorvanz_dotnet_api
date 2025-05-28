namespace zorvanz_api.Models.DTO;

public class PagedResponse<T>(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
{
    public IEnumerable<T> Data { get; set; } = data;
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
    public int TotalPages { get; set; } = (int)Math.Ceiling(totalRecords / (double)pageSize);
    public int TotalRecords { get; set; } = totalRecords;
}
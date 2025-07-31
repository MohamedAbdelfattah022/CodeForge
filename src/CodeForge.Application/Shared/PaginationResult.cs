namespace CodeForge.Application.Shared;

public class PaginationResult<T>(List<T> data, int totalItems, int pageNumber = 1, int pageSize = 5) {
	public List<T> Data { get; set; } = data;
	public int TotalItems { get; set; } = totalItems;
	public int PageNumber { get; set; } = pageNumber;
	public int PageSize { get; set; } = pageSize;
	public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
}
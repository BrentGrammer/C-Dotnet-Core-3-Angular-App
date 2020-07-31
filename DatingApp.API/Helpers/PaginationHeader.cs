namespace DatingApp.API.Helpers
{
  // This class is for information returned in response headers to the client for pagination info defined in the PagedList.cs class
  // This class is used in a created extension method in Extensions.cs to make a AddPagination method on the httpResponse which adds this info to Pagination header of the response.
  public class PaginationHeader
  {
    public int CurrentPage { get; set; }
    public int ItemsPerPage { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
    {
      this.CurrentPage = currentPage;
      this.ItemsPerPage = itemsPerPage;
      this.TotalItems = totalItems;
      this.TotalPages = totalPages;
    }
  }
}
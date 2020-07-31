namespace DatingApp.API.Helpers
{
  // This class is used to pass in pagination params received from the client request as a object into the GetUsers method for example in the users controller.
  // UserParams are passed into the GetUsers method in the Dating Repository and come from the request query params - they are mapped automatically in the controller (from pageSize and pageNumber)
  public class UserParams
  {
    // set a max page size so the user cannot request large amounts of items
    private const int _maxPageSize = 50;
    public int PageNumber { get; set; } = 1; // default to page 1 if no page number is requested from client
    private int _pageSize = 10;  // default size if user does not pass in number of items per page
                                 // use propfull shortcut to customize getter and setter in order to put the max check on the getter here
    public int PageSize
    {
      get { return _pageSize; }
      set { _pageSize = (value > _maxPageSize) ? _maxPageSize : value; } // value is builtin available in the set and get method context here
    }

  }
}
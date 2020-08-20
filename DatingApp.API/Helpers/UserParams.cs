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

    // these are used to filter results in GetUsers in the UsersController to filter out the currently logged in user and by gender
    // also these things can be passed into the query string to fetch users for additional filtering.
    public int UserId { get; set; }
    public string Gender { get; set; }
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 99;
    public string OrderBy { get; set; } // always want to specify a default order - this is done in the dating repo with an OrderByDescending call

    // these are used to return a list of liker users or likee users in the dating repo.  They are set by a query param: ex: `api/users?likees=true`
    public bool Likees { get; set; } = false;  // these bools are used in the dating repo to get a list of users liked and liked by the current logged in user.
    public bool Likers { get; set; } = false;

  }
}
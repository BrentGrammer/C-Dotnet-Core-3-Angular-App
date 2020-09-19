namespace DatingApp.API.Helpers
{
  public class MessageParams
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
    public int UserId { get; set; } // sender Id/Currently logged in user
    // allows user to select a container or read or unread messages Inbox/Outbox view
    // Unread messages are messages received filtering out the mesages sent based on the sender UserId above
    // can get messages sent also based on the userid
    public string MesssageContainer { get; set; } = "Unread";
  }
}
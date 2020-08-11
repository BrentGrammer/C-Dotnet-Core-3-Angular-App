namespace DatingApp.API.Models
{
  // Entity Framework does not currently have a system for setting up many to may relationships
  // the relationship b/w users and likes is a User has a Liker with many Likees and a User had a Likee with many Likers
  // to establish this relationship you need to override OnModelCreating from DbContext in DataContext.cs

  // This is used as a join table to link users to other users based on liker/likee relationship
  public class Like
  {
    // since there is no int Id prop, entity framework does not know what to use as primary key - in OnModelCreating override and combo of LikerId and LikeeId will be set to be
    // used as the PK.  This means a user can like another user only once.
    public int LikerId { get; set; } // id of user that is liking of another user - connection as part of User entity
    public int LikeeId { get; set; } // id of the user that is being liked by another user - connection as part of User entity
    // These props have to do with the relationship to the User table
    public User Liker { get; set; }
    public User Likee { get; set; }
  }
}

// The Likes table created will have 2 cols: LikerId and LikeeId 
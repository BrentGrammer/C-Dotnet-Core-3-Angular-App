using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
  public interface IDatingRepository
  {
    /* Create CRUD methods in here - this is your contract that the concrete implementation will use.
       - mnake methods that accept a generic type to avoid making duplicate methods for Photos and Users for example
       */
    void Add<T>(T entity) where T : class; // restricts the type passed in must be a class, entity is the parameter passed into the method call
    void Delete<T>(T entity) where T : class;
    // use a Task for saving to the database - import System.Threading.Tasks
    //return a bool to indicate whether the save was successful or not
    Task<bool> SaveAll();
    //Other read methods return the model:
    Task<PagedList<User>> GetUsers(UserParams userParams); // return a paged list with pagination info
    Task<User> GetUser(int id);
    Task<Photo> GetPhoto(int id);
    Task<Photo> GetMainPhotoForUser(int userId);

    Task<Like> GetLike(int userId, int recipientId); //used to check if a like already exists for a user - userId is the liker and recipient is the likee
    Task<Message> GetMessage(int id); // used to pass back the created at route to the message after a message is created
    Task<PagedList<Message>> GetMessagesForUser();
    Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId); // used to get conversation between users
  }
}
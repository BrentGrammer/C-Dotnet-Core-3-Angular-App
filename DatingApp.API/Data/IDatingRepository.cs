using System.Collections.Generic;
using System.Threading.Tasks;
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
    Task<IEnumerable<User>> GetUsers();
    Task<User> GetUser(int id);
    Task<Photo> GetPhoto(int id);
    Task<Photo> GetMainPhotoForUser(int userId);


  }
}
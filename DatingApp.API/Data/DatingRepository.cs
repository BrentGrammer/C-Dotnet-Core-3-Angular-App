using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

// Remember to add this as a service to Startup.cs
namespace DatingApp.API.Data
{
  public class DatingRepository : IDatingRepository //implement the repo interface created
  {
    private readonly DataContext _context;

    public DatingRepository(DataContext context)
    {
      _context = context;

    }
    public void Add<T>(T entity) where T : class
    {
      //this method (and all non save methods) only saves the changes to the context in memory. you need to actually save the changes to the database after
      _context.Add(entity);
    }

    public void Delete<T>(T entity) where T : class
    {
      _context.Remove(entity);
    }

    public async Task<Photo> GetPhoto(int id)
    {
      var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
      return photo;
    }

    public async Task<User> GetUser(int id)
    {
      // You need to include the photos with the user - because photos are a navigation property, they will not be returned with a user automatically
      //firstordefault will get the user based on id and return null if none found
      var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
      return user;
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
      var users = await _context.Users.Include(p => p.Photos).ToListAsync();
      return users;
    }

    public async Task<bool> SaveAll()
    {
      // this will return the number of changes saved to database - if 0 return false for no changes saved, else return true for a successful save
      return await _context.SaveChangesAsync() > 0;
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
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
      // Note that this is not async
      //this method (and all non save methods) only saves the changes to the context in memory. you need to actually save the changes to the database after
      _context.Add(entity);
    }

    public void Delete<T>(T entity) where T : class
    {
      _context.Remove(entity);
    }

    public async Task<Like> GetLike(int userId, int recipientId)
    {
      // check if the user is already liked by the liker to prevent duplicate likes:
      return await _context.Likes.FirstOrDefaultAsync(x => x.LikerId == userId && x.LikeeId == recipientId);
    }

    public async Task<Photo> GetMainPhotoForUser(int userId)
    {
      return await _context.Photos.Where(p => p.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
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

    public async Task<PagedList<User>> GetUsers(UserParams userParams)
    {
      // set default ordering with OrderByDescending
      var users = _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable(); // change this to a queryable so you can add the query on to it (you can't do the below Where statement otherwise)

      // get all other users n filter out current logged in user
      users = users.Where(u => u.Id != userParams.UserId); //Where returns IQueryable

      users = users.Where(u => u.Gender == userParams.Gender);

      // get list of users use has liked and a list of users that have liked the current user
      // the user has a list of liker and likee user ids so use those to get user information for all of them
      if (userParams.Likers)
      {
        var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
        users = users.Where(u => userLikers.Contains(u.Id)); // likers of the current user
      }
      if (userParams.Likees)
      {
        var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
        users = users.Where(u => userLikees.Contains(u.Id)); // likers of the current user
      }


      // filter by age - check if defaults have been changed which means user specified an age search
      if (userParams.MinAge != 18 || userParams.MaxAge != 99)
      {
        // calculate min and max date of birth to send back (because that's what you're storing in the data)
        var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
        var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

        users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
      }

      // set ordering if set by user
      if (!string.IsNullOrEmpty(userParams.OrderBy))
      {
        switch (userParams.OrderBy)
        {
          case "created":
            users = users.OrderByDescending(u => u.Created);
            break;
          default:
            users = users.OrderByDescending(u => u.LastActive);
            break;
        }
      }

      // return an instance of Paged List with pagination info - create a paged list with the static create method on the class
      return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
    }

    private async Task<IEnumerable<int>> GetUserLikes(int currentUserId, bool likers)
    {
      // get include likers and likees user ids for making lists of liker and likee users to send to client
      var user = await _context.Users
        .Include(x => x.Likers)
        .Include(x => x.Likees)
        .FirstOrDefaultAsync(u => u.Id == currentUserId);

      if (likers)
      {
        return user.Likers.Where(u => u.LikeeId == currentUserId).Select(i => i.LikerId); // List of user id ints that like the current user
      }
      else
      {
        return user.Likees.Where(u => u.LikerId == currentUserId).Select(i => i.LikeeId);
      }
    }

    public async Task<bool> SaveAll()
    {
      // this will return the number of changes saved to database - if 0 return false for no changes saved, else return true for a successful save
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Message> GetMessage(int id)
    {
      return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
    {
      // you need to include the Sender information since that is a User linked navigation prop
      // also need to include the User photo to use
      var messages = _context.Messages
        .Include(u => u.Sender).ThenInclude(p => p.Photos) // chains onto User in the first Indlude
        .Include(u => u.Recipient).ThenInclude(p => p.Photos)
        .AsQueryable(); // enables use of Where clause later in the switch below instead of having to chain immediately onto this chain to use Where

      // filter out messages you don't want to return (with inbox outbox container system)
      switch (messageParams.MessageContainer)
      {
        case "Inbox":
          messages = messages.Where(u => u.RecipientId == messageParams.UserId); // messageparams contains logged in user id
          break;
        case "Outbox":
          messages = messages.Where(u => u.SenderId == messageParams.UserId);
          break;
        default:
          messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.IsRead == false);
          break;
      }

      // order messages most recent first
      messages = messages.OrderByDescending(m => m.MessageSent);
      return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
    {
      var messages = await _context.Messages
       .Include(u => u.Sender).ThenInclude(p => p.Photos) // chains onto User in the first Indlude
       .Include(u => u.Recipient).ThenInclude(p => p.Photos)
       .Where(m =>
        m.RecipientId == userId && m.SenderId == recipientId
        || m.RecipientId == recipientId && m.SenderId == userId)
        .OrderByDescending(m => m.MessageSent)
        .ToListAsync();

      return messages;
    }
  }
}
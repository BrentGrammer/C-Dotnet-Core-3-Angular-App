using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
/**
Auth Repository is responsible for querying the database - need to inject the data context to use EF
*/

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public Task<User> Login(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public async Task<User> Register(User user, string password)
        {
            // declare placeholder vars here
            byte[] passwordHash, passwordSalt;
            // use 'out' to pass a reference to the hash and salt - when they are updated in the method, they will be updated in the outer scope as well
            // Normally you pass by value for the args into a method
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // this tracks the entity in EF which is required before saving to the db (may not need to be async??)
            await _context.Users.AddAsync(user);
            // this saves the new user to the database
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // using implements IDisposable which is like a context mgr - anything in the block is disposed of and cleaned up after use
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                // the hmac hash built in method provides a random key that can be used as a salt:
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                // note: these are setting values to the references passed in for passwordJash and salt
            }
        }

        public Task<User> UserExists(string username)
        {
            throw new System.NotImplementedException();
        }
    }
}
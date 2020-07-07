using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        //NOTE: MICROSOFT now recommends that seed methods are called in the Program.cs class (this is called in the main method there)
        public static void SeedUsers(DataContext context)
        {
            // First, check if the db is empty so you don't keep adding seed data everytime this method is run
            // note(use System.Linq import to get Any())
            if (!context.Users.Any())
            {
                // the path is relative to the proj folder
                //this json file was generated using an online json generator
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                //convert the json text data into user objects you can loop through
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                // populate users objs with needed data and add to the db context
                foreach (var user in users)
                {
                    // add a password for each user(the passwords are stored as byte arrays)
                    byte[] passwordHash, passwordSalt;
                    // just hardcode password since they are not on the user object
                    CreatePasswordHash("password", out passwordHash, out passwordSalt);

                    // add the password data to each user
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    //make username lowercase like you're doing before storing it in the database in the app
                    user.Username = user.Username.ToLower();
                    //add the users to the db context
                    context.Users.Add(user);
                }
                // NOTE: Because this runs on app startup before other users make calls, there is no need to make this async since there will not be other concurrent calls
                context.SaveChanges();
            }
        }

        // This is copied and pasted from the AuthRepository since it is private and used to make the password (needed to be made static since seedusers method here where it is being used is static as well)
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
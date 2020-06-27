using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        // also need to call the options from the class you're deriving from (DbContext) 
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // tell data context class about entities to put in database:
        // pass in the model class to DbSet and specify the name of the table as the property:
        public DbSet<Value> Values { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Photo> Photos { get; set; }
    }
}
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
    // Likes has a relationship with Users (sort of Many to Many):
    public DbSet<Like> Likes { get; set; } // to set up the relationship you need to override the OnModelCreating method on the inherited DbContext

    /**
    * Override OnModelCreating from DbContext class to change what Entity Framework does on creating the tables - needed to establish relationship b/w User and Likes
    */
    protected override void OnModelCreating(ModelBuilder builder)
    {
      // specify entity to manage creation of
      // Since Like entity does not only have an int Id prop, EF does not know what to use as the primary key - you need to tell it in this case with .HasKey
      builder.Entity<Like>()
        .HasKey(k => new { k.LikerId, k.LikeeId });  // use combo of these for the PK of Likes table - prevents user from liking same user more than once

      // use Fluent API to configure the relationship b/w Users and Likers/Likees
      builder.Entity<Like>()
          .HasOne(u => u.Likee)  // one likee can have many likers (reverse is also true)
          .WithMany(u => u.Likers)
          .HasForeignKey(u => u.LikeeId) // user table will have a Likee/LikerId 
          .OnDelete(DeleteBehavior.Restrict); // prevent cascading delete of a user if a like is deleted
      builder.Entity<Like>()
          .HasOne(u => u.Liker)  // a liker can have many likees
          .WithMany(u => u.Likees)
          .HasForeignKey(u => u.LikerId)
          .OnDelete(DeleteBehavior.Restrict); // prevent cascading delete of a user if a like is deleted
    }
  }
}
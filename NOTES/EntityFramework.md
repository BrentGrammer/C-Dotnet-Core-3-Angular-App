# Entity Framework

### Add a new Entity (table)

1. Create the model class

```c#
using System;
using System.Collections.Generic;

namespace DatingApp.API.Models
{
  public class User
  {
    public int Id { get; set; }
    public string Username { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
  }
}
```

1. Create the DbSet in the Data Context file
1. Optionally Configure relationships with the builder in the Data Context file

```c#
namespace DatingApp.API.Data
{
  public class DataContext : DbContext
  {
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      // configure relationships if you need to
      builder.Entity<Like>()
        .HasKey(k => new { k.LikerId, k.LikeeId });
      builder.Entity<Message>()
        .HasOne(u => u.Sender)
        .WithMany(m => m.MessagesSent)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}
```

1. Run migration script and apply changes to the database

- `dotnet ef migrations add MyMigrationName`
- `dotnet ef database update`

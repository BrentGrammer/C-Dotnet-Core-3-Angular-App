namespace DatingApp.API.Models
{
    // This is the name of the table to be created
    public class Value
    {
        // These properties are the names of the columns in the table that will be created by EF

        // Id will automatically be made a primary key if named that way
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
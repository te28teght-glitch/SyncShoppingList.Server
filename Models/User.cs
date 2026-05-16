using SQLite;

namespace SyncShoppingList.Server.Models
{
    [Table("Users")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id {get; set;}

        [Indexed]
        public string Nickname {get; set;} = "";

        [Indexed]
        public int GroupId {get; set;}
    }
}
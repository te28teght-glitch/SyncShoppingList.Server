using SQLite;

namespace SyncShoppingList.Server.Models
{
    [Table("Groups")]
    public class Group
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string Name { get; set; } = "";
        
        [Indexed]
        public string InviteCode { get; set; } = "";
    }
}
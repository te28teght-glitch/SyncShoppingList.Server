using SQLite;

namespace SyncShoppingList.Server.Models
{
    [Table("Products")]
    public class Product
    {
        [PrimaryKey, AutoIncrement]
        public int Id {get; set;}

        [Indexed]
        public int GroupId {get; set;}
        public string Name { get; set;} = "";
        public string AddedBy { get; set;} = "";
        public DateTime AddedAt {get; set;}
        public bool IsPurchased {get; set;}
        public string? PurchasedBy {get; set;} 
        public DateTime? PurchasedAt { get; set; }
    }
}
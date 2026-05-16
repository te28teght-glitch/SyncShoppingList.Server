using SQLite;
using SyncShoppingList.Server.Models;

namespace SyncShoppingList.Server.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;
        private string _dbPath;

        public DatabaseService()
        {
            _dbPath = Path.Combine(AppContext.BaseDirectory, "shopping.db");
        }

        public async Task Init()
        {
            _database = new SQLiteAsyncConnection(_dbPath);
            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<Group>();
            await _database.CreateTableAsync<Product>();
        }

        public async Task<Group?> CreateGroup(string name, string inviteCode)
        {
            var group = new Group { Name = name, InviteCode = inviteCode };
            await _database.InsertAsync(group);
            return group;
        }

        public async Task<int?> JoinGroup(string nickname, string inviteCode)
        {
            var group = await _database.Table<Group>().FirstOrDefaultAsync(g => g.InviteCode == inviteCode);
            if (group == null) return null;
            
            var user = new User { Nickname = nickname, GroupId = group.Id };
            await _database.InsertAsync(user);
            return group.Id;
        }

        public async Task AddProduct(int groupId, string name, string addedBy)
        {
            var product = new Product
            {
                GroupId = groupId,
                Name = name,
                AddedBy = addedBy,
                AddedAt = DateTime.Now,
                IsPurchased = false
            };
            await _database.InsertAsync(product);
        }

        public async Task<List<Product>> GetProducts(int groupId)
        {
            return await _database.Table<Product>().Where(p => p.GroupId == groupId).ToListAsync();
        }

        public async Task<bool> CheckProduct(int productId, string purchasedBy)
        {
            var product = await _database.Table<Product>().FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) return false;
            
            product.IsPurchased = true;
            product.PurchasedBy = purchasedBy;
            product.PurchasedAt = DateTime.Now;
            await _database.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            var product = await _database.Table<Product>().FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) return false;
            
            await _database.DeleteAsync(product);
            return true;
        }
    }
}
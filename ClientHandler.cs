using System.Net.Sockets;
using System.Text;
using SyncShoppingList.Server.Services;

namespace SyncShoppingList.Server
{
    public class ClientHandler
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private DatabaseService _db;
        private string? _currentNickname;
        private int? _currentGroupId;

        public ClientHandler(TcpClient client, DatabaseService db)
        {
            _client = client;
            _stream = client.GetStream();
            _db = db;
        }

        public async Task Run()
        {
            byte[] buffer = new byte[4096];

            try
            {
                while (true)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Получено: {message}");
                    
                    string response = await ProcessCommand(message);
                    
                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    await _stream.WriteAsync(responseData, 0, responseData.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            finally
            {
                _client.Close();
            }
        }

        private async Task<string> ProcessCommand(string command)
        {
            var parts = command.Split('|');
            var cmd = parts[0];

            switch (cmd)
            {
                case "CREATE_GROUP":
                    string nickname = parts[1];
                    string groupName = parts[2];
                    string inviteCode = Guid.NewGuid().ToString().Substring(0, 8);
                    var group = await _db.CreateGroup(groupName, inviteCode);
                    var userId = await _db.JoinGroup(nickname, inviteCode);
                    return $"GROUP_CREATED|{inviteCode}";

                case "JOIN_GROUP":
                    string nick = parts[1];
                    string code = parts[2];
                    var groupId = await _db.JoinGroup(nick, code);
                    if (groupId == null) return "ERROR|Invalid invite code";
                    return $"JOIN_OK|{groupId}";

                case "ADD_PRODUCT":
                    int groupIdAdd = int.Parse(parts[1]);
                    string productName = parts[2];
                    string addedBy = parts[3];
                    await _db.AddProduct(groupIdAdd, productName, addedBy);
                    return $"PRODUCT_ADDED|{productName}";

                case "GET_PRODUCTS":
                    int groupIdGet = int.Parse(parts[1]);
                    var products = await _db.GetProducts(groupIdGet);
                    var productStrings = products.Select(p => $"{p.Name}|{p.IsPurchased}|{p.AddedBy}");
                    return $"PRODUCTS|{string.Join(';', productStrings)}";

                case "CHECK_PRODUCT":
                    int productId = int.Parse(parts[1]);
                    string purchaser = parts[2];
                    bool success = await _db.CheckProduct(productId, purchaser);
                    return success ? $"PRODUCT_CHECKED|{productId}|{purchaser}" : "ERROR";

                case "DELETE_PRODUCT":
                    int productIdDel = int.Parse(parts[1]);
                    bool deleted = await _db.DeleteProduct(productIdDel);
                    return deleted ? $"PRODUCT_DELETED|{productIdDel}" : "ERROR";

                default:
                    return "UNKNOWN_COMMAND";
            }
        }
    }
}
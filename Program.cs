using System.Net;
using System.Net.Sockets;
using SyncShoppingList.Server.Services;

namespace SyncShoppingList.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Инициализация базы данных...");
            var db = new DatabaseService();
            await db.Init();
            Console.WriteLine("База данных готова");

            TcpListener listener = new TcpListener(IPAddress.Any, 8889);
            listener.Start();
            Console.WriteLine("Сервер запущен на порту 8889");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Клиент подключился");
                
                var handler = new ClientHandler(client, db);
                _ = Task.Run(handler.Run);
            }
        }
    }
}
using System;
using System.Threading.Tasks;

namespace MetaChat
{
    class Program
    {
        static App app;
        static async Task Main(string[] args)
        {
            try
            {
                string serverPort = args.Length >= 1 ? args[0] : "11010";
                string gamePort = args.Length >= 2 ? args[1] : "11020";
                string tcpPort = args.Length >= 3 ? args[2] : "11030";

                app = new App
                {
                    serverPort = serverPort,
                    gamePort = gamePort,
                    tcpPort = tcpPort
                };
                app.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            Console.ReadKey();
        }
    }
}

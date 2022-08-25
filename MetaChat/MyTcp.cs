using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;
using SuperSimpleTcp;

namespace MetaChat
{
    class MyTcp
    {
        public App app;
        public string port;
        public SimpleTcpServer server;
        //public event EventHandler<ConnectionEventArgs> Received;
        public delegate void ReceivedHandler(object? sender, DataReceivedEventArgs e, SyncJN inter, string raw);
        public event ReceivedHandler Received;
        public void Start()
        {
            server = new SimpleTcpServer($"localhost:{port}");
            server.Events.ClientConnected += ClientConnected;
            server.Events.ClientDisconnected += ClientDisconnected;
            server.Events.DataReceived += DataReceived;
            server.Start();
            // server.Send("[ClientIp:Port]", "Hello, world!");
        }

        void ClientConnected(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine($"[{e.IpPort}] client connected");
        }

        void ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine($"[{e.IpPort}] client disconnected: {e.Reason}");
        }

        void DataReceived(object sender, DataReceivedEventArgs e)
        {
            string payload = Encoding.UTF8.GetString(e.Data);
            var jn = JsonSerializer.Deserialize<SyncJN>(payload);
            var back = app.rtc?.GetBack() ?? new RtcJN();
            var s = JsonSerializer.Serialize(back);
            server.Send(e.IpPort, s);
            app.rtc?.SendSync(jn);
        }
    }
}
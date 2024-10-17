using System.Net.Sockets;
using System.Net;
using System.Text;

var clients = new List<Socket>();

var listener = new Socket(AddressFamily.InterNetwork,
                         SocketType.Stream,
                         ProtocolType.Tcp);


var ip = IPAddress.Parse("192.168.56.1");
var port = 45678;
var ep = new IPEndPoint(ip, port);


listener.Bind(ep);

var backlog = 1;
listener.Listen(backlog);



while (true)
{
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine($"Listening on {listener.LocalEndPoint}");
    var client = listener.Accept();
    clients.Add(client);
    Console.WriteLine($"{client.RemoteEndPoint} connected...");

    Task.Run(() =>
    {
        var colorRandomNumber = ((client.RemoteEndPoint as IPEndPoint)?.Port % 15) + 1;
        var consoleColor = (ConsoleColor)(colorRandomNumber ?? 15);


        var buffer = new byte[1024];
        var len = 0;
        var msg = string.Empty;

        while (true)
        {
            len = client.Receive(buffer);
            msg = Encoding.Default.GetString(buffer, 0, len);

            if (msg == "exit")
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                break;
            }


            msg = $"{client.RemoteEndPoint} : {msg}";

            Console.ForegroundColor = consoleColor;
            Console.WriteLine(msg);

            foreach (var c in clients)
            {
                if (client != c)
                {
                    c.Send(Encoding.Default.GetBytes(msg));
                }
            }
        }
    });

}
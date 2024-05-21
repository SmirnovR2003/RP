using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chain;

class Program
{
    public static void StartInitChain(int listeningPort, string nextHost, int nextPort)
    {
        try
        {
            if (nextHost == "localhost") nextHost = "127.0.0.1";
            IPAddress nextIpAddress = IPAddress.Parse(nextHost);
            IPAddress currIpAddress = IPAddress.Parse("127.0.0.1");

            IPEndPoint senderEP = new IPEndPoint(nextIpAddress, nextPort);
            IPEndPoint listenerEP = new IPEndPoint(currIpAddress, listeningPort);

            // CREATE
            Socket sender = new Socket(
                nextIpAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            Socket listener = new Socket(
                currIpAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            try
            {
                listener.Bind(listenerEP);
                listener.Listen(10);

                int x = int.Parse(Console.ReadLine());

                byte[] msg = Encoding.UTF8.GetBytes(x.ToString());
                sender.Connect(senderEP);
                int bytesSent = sender.Send(msg);

                Socket listenerHandler = listener.Accept();
                byte[] buf = new byte[1024];
                int bytesRec = listenerHandler.Receive(buf);
                x = int.Parse(Encoding.UTF8.GetString(buf, 0, bytesRec));
                Console.WriteLine(x);

                msg = Encoding.UTF8.GetBytes(x.ToString());
                bytesSent = sender.Send(msg);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                listenerHandler.Shutdown(SocketShutdown.Both);
                listenerHandler.Close();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
    
    public static void StartChain(int listeningPort, string nextHost, int nextPort)
    {
        try
        {
            if (nextHost == "localhost") nextHost = "127.0.0.1";
            IPAddress nextIpAddress = IPAddress.Parse(nextHost);
            IPAddress currIpAddress = IPAddress.Parse("127.0.0.1");

            IPEndPoint senderEP = new IPEndPoint(nextIpAddress, nextPort);
            IPEndPoint listenerEP = new IPEndPoint(currIpAddress, listeningPort);

            Socket sender = new Socket(
                nextIpAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            Socket listener = new Socket(
                currIpAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            try
            {
                listener.Bind(listenerEP);
                listener.Listen(10);

                int x = int.Parse(Console.ReadLine());

                Socket listenerHandler = listener.Accept();
                byte[] buf = new byte[1024];
                int bytesRec = listenerHandler.Receive(buf);
                int y = int.Parse(Encoding.UTF8.GetString(buf, 0, bytesRec));

                x = int.Max(x, y);

                byte[] msg = Encoding.UTF8.GetBytes(x.ToString());
                sender.Connect(senderEP);
                int bytesSent = sender.Send(msg);

                bytesRec = listenerHandler.Receive(buf);
                x = int.Parse(Encoding.UTF8.GetString(buf, 0, bytesRec));
                msg = Encoding.UTF8.GetBytes(x.ToString());
                bytesSent = sender.Send(msg);
                Console.WriteLine(x);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                listenerHandler.Shutdown(SocketShutdown.Both);
                listenerHandler.Close();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    static void Main(string[] args)
    {
        try
        {
            if(args.Length > 3 && bool.Parse(args[3]))
            {
                StartInitChain(int.Parse(args[0]), args[1], int.Parse(args[2]));
                return;
            }
            StartChain(int.Parse(args[0]), args[1], int.Parse(args[2]));

        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
        }
    }
}
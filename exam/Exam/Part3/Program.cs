using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Data.Sqlite;

namespace Part3
{
    class Program
    {
        static void Main(string[] args)
        {
           IPAddress ipAddress = IPAddress.Loopback; 
   
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);  

            int port = 3000;
 
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);  
  
            try 
            {
                socket.Connect(remoteEP); 
  
                Console.WriteLine("Socket connected to {0}", socket.RemoteEndPoint.ToString()); 
                while (true)
                {    try
                    {
                        Console.Write("Please, input command: ");
                        string command = Console.ReadLine();
                        ProcessWhileConnected(socket, command);
                    }
                    catch 
                    {
                        break; 
                    }
                }

                socket.Close();  
  
            } 
            catch 
            {  
                Console.WriteLine("Can not connect to server at port:" + port);  
            }  
        }

        static void ProcessWhileConnected(Socket socket, string command)
        {
            string text = FormatRequest(command); 
            int bytesCount = text.Length;
            string delimeter = "{ignore}";
            Console.WriteLine("Text length: " + bytesCount);

            byte[] msg = Encoding.ASCII.GetBytes($"{"GET"}{text}{delimeter}");  

            int bytesSent = socket.Send(msg);  
 
            byte[] buffer = new byte[1024];  
     
            int receivedBytes = socket.Receive(buffer);  
            string message = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
            Console.WriteLine("Response: " + message);  
        }

        static string FormatRequest(string command)
        {
            int year = 2000;
            string nomination = "Actor in a Supporting Role";

            nomination.Replace(" ", "%20");
            string request = "";

            request = $"/ceremonies/{year}/nomination/{nomination}/winner";
            //request = $"ceremonies/{year}/nominations";

            return request; 
        }
    }
}

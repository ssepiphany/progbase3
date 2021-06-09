using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string databaseFileName = "../exam.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={databaseFileName}");

            AwardRepository repo = new AwardRepository(connection);
           IPAddress ipAddress = IPAddress.Loopback; 
            int port = 3000;

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            try 
            {  
                listener.Bind(localEndPoint);  
                listener.Listen();  

                Console.WriteLine($"Server is listening on port: {port}");  
                while (true) 
                {  
                    Console.WriteLine($"Waiting for a new client...");  
                    
                    Socket newClientSocket = listener.Accept();  
                    Console.WriteLine($"New client connected: " + newClientSocket.RemoteEndPoint);  

                    Thread thread = new Thread(StartNewThread);
                    thread.Start(newClientSocket);
                }  
            }
            catch (Exception e) 
            {  
                Console.WriteLine(e.ToString());  
            }  
        }

        static void StartNewThread(object obj)
        {
            Socket socket = (Socket)obj;
            ProcessClient(socket);
            socket.Close();
            Console.WriteLine("Connection closed");
        }

        static void ProcessClient(Socket newClientSocket)
        {
            byte[] buffer = new byte[1024]; 

            while (true)
            {
                try
                {
                    Console.WriteLine($"Waiting for a message...");  
                    string text = "";
                    int nBytes = newClientSocket.Receive(buffer);  
                    string inputText = Encoding.ASCII.GetString(buffer,0,nBytes); 
                    string[] parts = inputText.Split(' ');

                    Console.WriteLine($"Received: ({part[1].Length})");

                    while (true)
                    {
                        nBytes = newClientSocket.Receive(buffer);  
                        inputText = Encoding.ASCII.GetString(buffer,0,nBytes);  
                        parts[1] += inputText;

                        Console.WriteLine( "Received: {0}",nBytes);  

                        if (parts[1].IndexOf("{ignore}") > -1)
                        {
                            break; 
                        }
                    }
                    Console.WriteLine("Full message received ");
                    string response = ProcessCommand(parts);


                    byte[] outputText = Encoding.ASCII.GetBytes(response);
                    newClientSocket.Send(outputText);
                    Console.WriteLine($"Response sent.\r\n{text}");

                    if (response == "bye")
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        static string ProcessCommand(string[] request)
        {
            string response = "";
            if (request[0] != "GET")
            {
                response = "BAD REQUEST";
                return; 
            }
            string[] command = request[1].Split('/');
            int year = int.Parse(command[1]);

            if(command.Length == 5)
            {

            }
            else if (command.Length == 3)
            {

            }

        }
    }
}

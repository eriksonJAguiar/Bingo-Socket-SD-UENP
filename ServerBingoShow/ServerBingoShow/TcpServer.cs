﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ServerBingoShow
{
    class TcpServer
    {

        // Incoming data from the client.  
        public static string data = null;
        public static List<Socket> clientesList = new List<Socket>();
        public static byte[] bytes = new Byte[1024];
        private static string cliente;
        private static List<int> numerosSorterio = new List<int>();

        public static void StartListening(int port)
        {     
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                Console.WriteLine("Server Listen on Port {0}", port);

                bytes = new Byte[1024];

                Socket handler;

                while (true)
                {
                   handler = listener.Accept();
                    clientesList.Add(handler);
                    //nome do cliente
                    int bytesRec = handler.Receive(bytes);
                    cliente = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    var cartela = sendCartela();

                    ParameterizedThreadStart param = new ParameterizedThreadStart(handleClient);
                    Thread th = new Thread(param);
                    th.Start(handler);

                    //broadcastNumber();

                }

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static void handleClient(object obj)
        {

            Socket handler = (Socket)obj;

            Console.WriteLine("{0} Entrou...", cliente);

            var cartela = sendCartela();

            bytes = new Byte[2048];

            bytes = ObjectToByteArray(cartela);

            handler.Send(bytes);

            while (true)
            {
                /*bytes = new byte[1024];
               int bytesRec = handler.Receive(bytes);
                if (bytes == null) break;
                var data = Encoding.ASCII.GetString(bytes, 0, bytesRec);*/
                
            }

           

            // Echo the data back to the client.  
            //byte[] msg = Encoding.ASCII.GetBytes(data);

            //handler.Send(msg);
        }
        public static void broadcastNumber()
        {
            int n = sendNumber();

            byte[] dataByte = Encoding.ASCII.GetBytes(n.ToString());

            foreach (Socket s in clientesList)
            {
                s.Send(dataByte);
            }
        }
        public static int sendNumber()
        {
            Random r = new Random();

            var achou = true;

            int n = 0;

            do
            {
                n = r.Next(1, 91);
                if (!numerosSorterio.Contains(n))
                {
                    numerosSorterio.Add(n);
                    achou = false;
                }
                    

            } while (achou);

            return n;
        }
        public static int sorteio()
        {
            Random r = new Random();

            return r.Next(1, 91);
        }
        public static List<int> sendCartela()
        {
            List<int> cartela = new List<int>();

            for (int i = 0; i < 16; i++)
            {
                var n = sorteio();

                if (!cartela.Contains(n))
                    cartela.Add(n);
                else
                    i--;
            }

            return cartela;

        }
       public static byte[] ObjectToByteArray(Object obj)
        {
                if (obj == null)
                    return null;

                BinaryFormatter bf = new BinaryFormatter();

                MemoryStream ms = new MemoryStream();

                bf.Serialize(ms, obj);

                return ms.ToArray();
        }
    
}

}

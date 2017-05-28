﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bingo.Classes
{
    class TcpClient
    {

        private static TcpClient instance = null;

        private TcpClient()
        {

        }

        public static TcpClient getInstance()
        {
            if (instance == null)
            {
                instance = new TcpClient();

                return instance;
            }

            return instance;
        }

        public List<int> cartela { get; set; }
        public bool connected { get; set; }
        public int sorteado { get; set; }
        public bool ganhou { get; set; }
        private static AutoResetEvent autoEvent;

        public void StartClient(Conf_Player conf)
        {
            byte[] bytes = new byte[1024];
            ganhou = false;

            try
            {
                IPHostEntry ipHostInfo = Dns.Resolve(conf.ip);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, conf.porta);

                Socket socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    var _isFirst = true;

                    while (!ganhou)
                    {
                        socket.Connect(remoteEP);
                        connected = socket.Connected;

                        if (_isFirst)
                        {
                            byte[] msg = Encoding.ASCII.GetBytes(conf.nome);

                            int bytesSent = socket.Send(msg);

                            bytes = new Byte[2048];

                            int b = socket.Receive(bytes);

                            cartela = deserialize(bytes);

                            _isFirst = false;

                           continue;
                        }

                        bytes = new Byte[2048];

                        int bytesRec = socket.Receive(bytes);
                        string data = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                        //Console.WriteLine("Sorteou {0}", data);

                        sorteado = int.Parse(data);

                        Thread.Sleep(30000);
                    }

                    //usuario grita Bingo
                    byte[] dataByte = Encoding.ASCII.GetBytes("GANHOU");
                    socket.Send(dataByte);

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

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
        public List<int> deserialize(byte[] fluxo)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();

            memStream.Write(fluxo, 0, fluxo.Length);

            memStream.Seek(0, SeekOrigin.Begin);

            List<int> c = (List<int>)binForm.Deserialize(memStream);

            return c;
        }

    }
}

using GamePadObsMachine.GamePadGameMachine;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GamePadObsMachine
{
    public static class SynchronousSocketListener
    {
        private static byte[] bytes = new byte[1024];
        private static IPAddress ipAddress = IPAddress.Parse(Program.config.ServerIP);
        private static IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Program.config.ServerPort);
        private static Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static Socket handler;

        public static Stopwatch ReceivedWatch = new Stopwatch();
        public static object lockObj = new object();
        public static JObject GamepadStateObj = default(JObject);

        public static void StartListening()
        {
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                handler = listener.Accept();

                // Start listening for connections.  
                while(true)
                {
                    Debug.Print("Waiting for a connection...");
                    ReceivedWatch.Start();
                    ReceiveData();

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveData()
        {
            while(true)
            {
                try
                {
                    string data = string.Empty;
                    while(true)
                    {
                        bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                        if(data.IndexOf("¤") > -1)
                        {
                            ReceivedWatch.Restart();
                            break;
                        }
                    }
                    try
                    {
                        lock(lockObj)
                        {
                            GamepadStateObj = JObject.Parse(data.Substring(0, data.Length - 1));
                        }
                        Debug.Print("Text received : {0}", data);
                    }
                    catch(Exception ex)
                    {
                        Debug.Print(ex.ToString());
                    }
                    Debug.Print("Text received : {0}", data);
                    // Echo the data back to the client.  
                    byte[] msg = Encoding.UTF8.GetBytes("ack");
                    handler.Send(msg);
                }
                catch(Exception ex)
                {
                    Debug.Print(ex.ToString());
                }
            }
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GamePadGameMachine
{
    public static class SynchronousSocketClient
    {
        private static byte[] bytes = new byte[1024];
        private static IPAddress ipAddress;
        private static IPEndPoint remoteEP;
        private static Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.AutoReset);
        public static ConcurrentQueue<string> SendQueue = new ConcurrentQueue<string>();
        public static Stopwatch delayState = new Stopwatch();
        public static void StartClient()
        {
            try
            {
                ipAddress = IPAddress.Parse(Program.config.ServerIP);
                remoteEP = new IPEndPoint(ipAddress, Program.config.ServerPort);
                try
                {
                    delayState.Start();
                    Thread.Sleep(1000);
                    sender.Connect(remoteEP);
                    sender.SendTimeout = 1000;
                    sender.ReceiveTimeout = 1000;
                    Debug.Print("Socket connected to {0}", sender.RemoteEndPoint.ToString());

                    SendLoop();
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                catch(ArgumentNullException ane)
                {
                    Debug.Print("ArgumentNullException : {0}", ane.ToString());
                }
                catch(SocketException se)
                {
                    Debug.Print("SocketException : {0}", se.ToString());
                }
                catch(Exception e)
                {
                    Debug.Print("Unexpected exception : {0}", e.ToString());
                }

            }
            catch(Exception e)
            {
                Debug.Print(e.ToString());
            }
        }

        private static void SendLoop()
        {
            while(true)
            {
                try
                {
                    wait.WaitOne();

                    while(SendQueue.Count > 0)
                    {
                        var message = string.Empty;
                        SendQueue.TryDequeue(out message);
                        if(!string.IsNullOrWhiteSpace(message))
                        {
                            byte[] msg = Encoding.UTF8.GetBytes($@"{message}¤");
                            int bytesSent = sender.Send(msg);
                            int bytesRec = sender.Receive(bytes);
                            Debug.Print("Echoed test = {0}", Encoding.UTF8.GetString(bytes, 0, bytesRec));
                            bytes = new byte[1024];
                        }
                    }
                }
                catch(Exception ex)
                {
                    Debug.Print(ex.ToString());
                }
            }
        }
    }
}

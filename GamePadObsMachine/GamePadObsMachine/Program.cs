﻿using GamePadObsMachine.GamePadGameMachine;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GamePadObsMachine
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        public static Config config;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var configDirectory = Environment.CurrentDirectory;
            config = JsonConvert.DeserializeObject<Config>(File.ReadAllText($@"{configDirectory}\config.ini"));

            Task.Factory.StartNew(() =>
            {
                SynchronousSocketListener.StartListening();
            });
            using (var game = new Game1())
                game.Run();
        }
    }
#endif
}

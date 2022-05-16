using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueBot
{
    class Methods : ModuleBase<SocketCommandContext>
    {

        public static void addTokenToQueue(string token, int amount)
        {
            string queuePath = $"{Environment.CurrentDirectory}\\velocity\\queue.txt";
            string velocityPath = $"{Environment.CurrentDirectory}\\velocity\\velocitysniper.exe";

            Console.WriteLine("Trying to close existing velocity");

            //Try to find and close process with the name 'velocitysniper'
            try
            {
                foreach (var process in Process.GetProcessesByName("velocitysniper"))
                {
                    process.Kill();
                    Console.WriteLine("Closed velocitysniper.exe");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Could not find an open velocity program");
            }


            //Add new tokens to queue

            Console.WriteLine("Trying to add token to queue");
            try
            {
                int b = 1;
                for (int i = 0; i < amount; i++)
                {
                    
                    File.AppendAllText(queuePath, token + Environment.NewLine);
                    Console.WriteLine($"Added token {b} times");
                    b++;

                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to add token");
            }



            //Open velocitysniper again
            Console.WriteLine("Trying to open velocitysniper");
            try
            {
                Process.Start(velocityPath);
                Console.WriteLine("Velocity should be open now");
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to open velocity");
            }
        }

        public static void forceRestart()
        {

            string velocityPath = $"{Environment.CurrentDirectory}\\velocity\\velocitysniper.exe";

            //Try to find and close process with the name 'velocitysniper'
            try
            {
                foreach (var process in Process.GetProcessesByName("velocitysniper"))
                {
                    process.Kill();
                    Console.WriteLine("Closed velocitysniper.exe");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Could not find an open velocity program");
            }

            //Open velocitysniper again
            Console.WriteLine("Trying to open velocitysniper");
            try
            {
                Process.Start(velocityPath);
                Console.WriteLine("Velocity should be open now");

            }

            catch (Exception)
            {
                Console.WriteLine("Failed to open velocity");
            }
            
        }

    }
}

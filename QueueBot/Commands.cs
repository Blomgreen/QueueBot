using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QueueBot
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Help()
        {
            var User = Context.User as SocketUser;
            string Message = Context.Message.Content;
            

        }
        
        [Command("queueadd")]
        public async Task addToken(string token, int amount)
        {
            string Message = Context.Message.Content;
            Console.WriteLine(Message);
            Console.WriteLine($"I should add token: {token}, {amount} times");
            Methods.addTokenToQueue(token, amount);
        }
        
        [Command("restart")]
        [Alias("ForceRestart", "ForceStart", "StartSniper")]
        public async Task restart()
        {
            await ReplyAsync("Restarting velocity, please wait.");
            Methods.forceRestart();
            foreach (var process in Process.GetProcessesByName("velocitysniper"))
            {
            await ReplyAsync("Velocity found - Sniper should now be running");

            }

        }

        [Command("queueremove")]
        [Alias("removetoken")]
        public async Task removeToken(string token)
        {
            string queuePath = $"{Environment.CurrentDirectory}\\velocity\\queue.txt";
            if (File.ReadAllLines(queuePath).Contains(token))
            {
                List<string> tokenList = new List<string>();

                int i = 0;
                foreach (string line in File.ReadAllLines(queuePath))
                {

                    if (!line.Contains(token))
                    {
                        tokenList.Add(line);
                    }
                    else if (line.Contains(token))
                    {
                        Console.WriteLine("Token to remove detected, not adding to list");
                    }
                }

                File.WriteAllLines(queuePath, tokenList.ToArray());
                await ReplyAsync($"{token} has been removed from the queue - Don't forget to restart the sniper");

            }
            else
            {
                await ReplyAsync($"Couldn't find {token} in queue.txt");
            }
        }

        [Command("updatequeue")]
        [Alias("queue", "printqueue")]
        public async Task updateQueue()
        {
            List<string> queue = Methods.printQueue();
            string[] queueContent = queue.ToArray();
            string content = null;
            Console.WriteLine($"This is current content {content}");
            foreach (var line in queueContent)
            {
                content += line + "\n";
            }


            var embed = new EmbedBuilder
            {
                // Embed property can be set within object initializer
                Title = "Hello world!",
                Description = "I am a description set by initializer."
            };
            // Or with methods
            embed.AddField("Users in queue", content)
                .WithAuthor(Context.Client.CurrentUser)
                .WithFooter(footer => footer.Text = "Blomgreen#6969")
                .WithColor(Color.Blue)
                .WithTitle("Queue list")
                .WithDescription("Here you'll find the current queue, including the amount of snipes remaining.")
                .WithCurrentTimestamp();

            //Your embed needs to be built before it is able to be sent
            await ReplyAsync(embed: embed.Build());
        }

        [Command("stats")]
        [Alias("status")]
        public async Task status()
        {
            string[] api = File.ReadAllLines("APIkey.txt");

            string APIKEY = api[0];

            WebClient wc = new WebClient();
            string rawData = wc.DownloadString($"https://genefit.cc/velocity/api/stats?key={APIKEY}");
            Console.WriteLine(rawData);

            string servercountStep1 = rawData.Split(',')[0];
            string serverCount = servercountStep1.Split(':')[1];
            Console.WriteLine($"servercount: {serverCount}");

            string altcountStep1 = rawData.Split(',')[1];
            string altcount = altcountStep1.Split(':')[1];
            Console.WriteLine($"Altcount: {altcount}");

            string claimednitroStep1 = rawData.Split(',')[2];
            string claimedNitro = claimednitroStep1.Split(':')[1];
            Console.WriteLine($"Claimed nitro: {claimedNitro}");

            string content = $"Server count: {serverCount}\nAlt count: {altcount}\nSniped nitro: {claimedNitro}";


            var embed = new EmbedBuilder
            {
                // Embed property can be set within object initializer
                Title = "Hello world!",
                Description = "I am a description set by initializer."
            };
            // Or with methods
            embed.AddField("Stats",
                content)
                .WithAuthor(Context.Client.CurrentUser)
                .WithFooter(footer => footer.Text = "Blomgreen#6969")
                .WithColor(Color.Blue)
                .WithTitle("Status ")
                .WithDescription("Here you'll find the sniper stats")
                .WithCurrentTimestamp();

            //Your embed needs to be built before it is able to be sent
            await ReplyAsync(embed: embed.Build());
        }
    }
}

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
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public async Task Help()
        {
            var User = Context.User as SocketUser;
            string Message = Context.Message.Content;
            var embed = new EmbedBuilder
            {
                // Embed property can be set within object initializer
                Title = "Hello world!",
                Description = "I am a description set by initializer."
            };
            // Or with methods
            embed.AddField("Commands",
                "/queueadd {token} {snipe amount} - Add tokens to the queue\n/restart - restarts / forcestarts the sniper\n/queueremove {token} - Removes the given token from the queue completely")
                .WithAuthor(Context.Client.CurrentUser)
                .WithFooter(footer => footer.Text = "Blomgreen#6969")
                .WithColor(Color.Blue)
                .WithTitle("Command list")
                .WithDescription("Here you'll find the list of commands")
                .WithCurrentTimestamp();

            //Your embed needs to be built before it is able to be sent
            await ReplyAsync(embed: embed.Build());
            

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
    }
}

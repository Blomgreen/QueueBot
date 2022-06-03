using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
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
        public string permissionsPath = $"{Environment.CurrentDirectory}\\config\\botPermissions.txt";
        public string apiPath = $"{Environment.CurrentDirectory}\\config\\APIkey.txt";
        public string emojiesPath = $"{Environment.CurrentDirectory}\\config\\emojies.txt";



        [Command("help")]
        public async Task Help()
        {
            var User = Context.User as SocketUser;
            string Message = Context.Message.Content;

            if (File.ReadAllText(permissionsPath).Contains(User.Id.ToString())) // check perms
            {
                var embed = new EmbedBuilder
                {
                    // Embed property can be set within object initializer
                    Title = "Hello world!",
                    Description = "I am a description set by initializer."
                };
                string content = "/help - Display all available commands\n/queueadd {token} {amount} - add a token to the sniping queue\n/restart - restart / forcestart the sniper\n/removetoken {token} - completely removes a token from the queue\n/queue - Print the current queue, AND check if their tokens are valid\n/status - Use your API key, to get stats regarding servers, alts and sniped nitro\n/check {token} - Check a token, before adding it, to make sure it's fully verified";
                // Or with methods
                embed.AddField("Command list",
                    content)
                    .WithAuthor(Context.Client.CurrentUser)
                    .WithFooter(footer => footer.Text = "Blomgreen#6969")
                    .WithColor(Color.Blue)
                    .WithTitle("Commands")
                    .WithDescription("Here you'll find the list of commands")
                    .WithCurrentTimestamp();

                //Your embed needs to be built before it is able to be sent
                await ReplyAsync(embed: embed.Build());
            }
            else
            {
                await ReplyAsync("you do not have permission to use this command");
            }



        }

        [Command("queueadd")]
        public async Task addToken(string token, int amount)
        {
            var User = Context.User as SocketUser;

            if (File.ReadAllText(permissionsPath).Contains(User.Id.ToString()))
            {
                string Message = Context.Message.Content;
                Console.WriteLine(Message);
                Console.WriteLine($"I should add token: {token}, {amount} times");
                Methods.addTokenToQueue(token, amount);

            }
            else
            {
                await ReplyAsync("You do not have permission to use this command");
            }
        }

        [Command("restart")]
        [Alias("ForceRestart", "ForceStart", "StartSniper")]
        public async Task restart()
        {
            var User = Context.User as SocketUser;
            if (File.ReadAllText(permissionsPath).Contains(User.Id.ToString()))
            {
                await ReplyAsync("Restarting velocity, please wait.");
                Methods.forceRestart();
                foreach (var process in Process.GetProcessesByName("velocitysniper"))
                {
                    await ReplyAsync("Velocity found - Sniper should now be running");

                }
            }
            else
            {
                await ReplyAsync("You do not have permission to use this command");
            }

        }

        [Command("queueremove")]
        [Alias("removetoken")]
        public async Task removeToken(string token)
        {
            var User = Context.User as SocketUser;
            if (File.ReadAllText(permissionsPath).Contains(User.Id.ToString()))
            {
                string queuePath = $"{Environment.CurrentDirectory}\\queue.txt";
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
            else
            {
                await ReplyAsync("You do not have permission to use this command");
            }

            
        }

        [Command("updatequeue")]
        [Alias("queue", "printqueue")]
        public async Task updateQueue()
        {
            string Active_Emoji = File.ReadAllLines(emojiesPath)[0];
            string Awaiting_Emoji = File.ReadAllLines(emojiesPath)[1];

            var User = Context.User as SocketUser;
            if (File.ReadAllText(permissionsPath).Contains(User.Id.ToString()))
            {
                List<string> queue = Methods.printQueue();
                string[] queueContent = queue.ToArray();
                string content = null;
                Console.WriteLine($"This is current content {content}");
                int firstToken = 0;
                foreach (var line in queueContent)
                {
                    if (firstToken == 0)
                    {
                        content += line + Active_Emoji + "\n";

                    }
                    content += line + Awaiting_Emoji + "\n";
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
            else
            {
                await ReplyAsync("You do not have permission to use this command");
            }

           
        }

        [Command("stats")]
        [Alias("status")]
        public async Task status()
        {

            var User = Context.User as SocketUser;
            if (File.ReadAllText(permissionsPath).Contains(User.Id.ToString()))
            {
                string[] api = File.ReadAllLines(apiPath);

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
            else
            {
                await ReplyAsync("You do not have permission to use this command");
            }

            
        }
        [Command("check")]
        [Alias("Check")]
        public async Task tokenCheck(string token)
        {
            var User = Context.User as SocketUser;
            string Message = Context.Message.Content;

            if (File.ReadAllText(permissionsPath).Contains(User.Id.ToString())) // check perms
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v9/users/@me");
                    request.ContentType = "application/json";
                    request.Method = WebRequestMethods.Http.Get;
                    request.Timeout = 20000;
                    request.Headers = new WebHeaderCollection()
                {
                       {
                        "Authorization", token
                        }
                };

                    WebResponse response = request.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(responseStream);
                    string result = sr.ReadToEnd();

                    dynamic json = JsonConvert.DeserializeObject(result);

                    Console.WriteLine(result);
                    string username = json.username;
                    string discriminator = json.discriminator;

                    string discordID = json.id;
                    string email = json.email;

                    string verified = json.verified;
                    string phoneNum = json.phone;

                    var embed = new EmbedBuilder
                    {
                        // Embed property can be set within object initializer
                        Title = "Hello world!",
                        Description = "I am a description set by initializer."
                    };
                    string content = $"**Token:** {token}\n**Username:** {username}#{discriminator}\n**ID:** {discordID}\n**Verified?:** {verified}\n**PhoneNumber:** {phoneNum}\n**Mail:** {email}";
                    // Or with methods
                    embed.AddField("Checked Token",
                        content)
                        .WithAuthor(Context.Client.CurrentUser)
                        .WithFooter(footer => footer.Text = "Blomgreen#6969")
                        .WithColor(Color.Blue)
                        .WithTitle("")
                        .WithDescription("")
                        .WithCurrentTimestamp();

                    //Your embed needs to be built before it is able to be sent
                    await ReplyAsync(embed: embed.Build());

                }
                catch (Exception)
                {

                }
            }
            else
            {
                await ReplyAsync("you do not have permission to use this command");

            }
        }
    }
}

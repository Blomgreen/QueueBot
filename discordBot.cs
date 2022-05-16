using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SpotiGrade.Logs;

namespace DiscordNETBotTemplate
{
    class Program
    {
        public static string prefix = "S?"; // default prefix

        static void Main_(string[] args)
        {
            Thread tr = new Thread(() => new Program().RunBotAsync().GetAwaiter().GetResult());
            tr.Start();
            tr.Join();
        }

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            //_services = new ServiceCollection().AddSingleton(_client).AddSingleton(_commands).BuildServiceProvider(); FUCK YOU STACKOVERFLOW EXCEPTION!

            _client.Log += _client_Log;
            _client.Ready += _client_Ready;

            await RegisterCommandsAsync();
            try
            {
                await _client.LoginAsync(TokenType.Bot, "TOKEN HERE");
            }
            catch (Exception ex)
            {
                Logger.LogCrash(ex.ToString());
                Console.WriteLine();
                Console.WriteLine("Token not valid, or no internet connection!");
                Console.ReadLine();
                Environment.Exit(1);
            }

            await _client.StartAsync();

            await Task.Delay(-1); // infinite wait
        }

        private async Task _client_Ready()
        {
            Console.WriteLine("Bot is online and working perfectly fine!");
            new Thread(RPC).Start(); // start the RPC messages
        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg); // log discord.net specific messages (NOT CHAT)
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            Thread thread = new Thread(() => CommandHandler(arg));
            thread.Start();
            thread.Join();
        }

        private async void CommandHandler(SocketMessage arg)
        {
            try
            {
                var message = arg as SocketUserMessage;
                var context = new SocketCommandContext(_client, message);

                if (message.Author.IsBot) return;

                if (context.IsPrivate) // DM Message
                {
                    var colors = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write($"{DateTime.Now.Hour}-{DateTime.Now.Minute}|[{message.Author}|{message.Author.Id}]:\n");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(message.Content);
                    Console.ForegroundColor = colors;
                }
                else // Everywhere else
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write($"{DateTime.Now.Hour}-{DateTime.Now.Minute}|[{context.Guild.Id}|{context.Guild}|{message.Channel.Id}|{message.Channel}|{message.Author}|{message.Author.Id}|]:\n");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(message.Content);
                    Console.ForegroundColor = color;
                }

                int argPos = 0;
                if (message.HasStringPrefix(prefix.ToLower(), ref argPos) || message.HasStringPrefix(prefix, ref argPos)) // check if message starts with the set prefix
                {
                    try { await arg.DeleteAsync(); } catch { }

                    var result = await _commands.ExecuteAsync(context, argPos, _services); // execute the command

                    if (!result.IsSuccess)
                    {
                        var colors = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(result.ErrorReason);
                        Console.ForegroundColor = colors;
                        await arg.Channel.SendMessageAsync("[ERROR]: " + result.ErrorReason);
                    }
                    if (result.Error.Equals(CommandError.UnmetPrecondition)) await message.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
            catch
            {
                Console.WriteLine("Discord Embed Message Error!"); // not important, just keep it
            }
        }

        private async void RPC() // cycle between messages
        {
            await _client.SetGameAsync("Starting up bot...");
            while (true)
            {
                Thread.Sleep(5000);
                await _client.SetGameAsync("Message 1");
                Thread.Sleep(5000);
                await _client.SetGameAsync("Message 2");
                Thread.Sleep(5000);
                await _client.SetGameAsync("Message 3");
                Thread.Sleep(5000);
                await _client.SetGameAsync("Message 4");
                Thread.Sleep(5000);
                await _client.SetGameAsync("Message 5");
            }
        }
    }
}
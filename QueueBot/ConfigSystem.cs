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


namespace Queuebot
{
    public class Config
    {
        public static string BotToken { get; set; } = "";
        public static string Velocity_API_Key { get; set; } = "";
        public static ulong[] PermissonID { get; set; }
    }

    public class ConfigSystem
    {
        public static Config config = new Config();
        public static string configpath = "settings.cfg";
        public static void LoadConfig()
        {
            if (!File.Exists(configpath))
                SaveConfig();
            string content = File.ReadAllText(configpath);
            config = JsonConvert.DeserializeObject<Config>(content);
        }
        public static void SaveConfig()
        {
            try
            {
                string json = JsonConvert.SerializeObject(config);
                File.WriteAllText(configpath, json);
            }
            catch { }
        }
    }
}
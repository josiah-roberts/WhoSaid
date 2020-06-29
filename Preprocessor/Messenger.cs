using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using static System.Text.Json.JsonSerializer;
using static System.Console;

#nullable enable
namespace Preprocessor
{
    class Messenger
    {
        private static string MessengerDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Documents/ChatProject/Messenger/inbox";
        private static readonly string[] Chats = { "tehlobby_jfc1g8xmwq", "theoldguard_yvo9bwauea", "imaginaryinternetassociatesanonymous_mr34flrspw" };
        public static IEnumerable<Record> ReadMessenger()
        {
            return Chats.SelectMany(chatName => Directory.GetFiles(MessengerDirectory + $"/{chatName}"))
                .SelectMany(ParseMessengerFile);
        }

        private static IEnumerable<Record> ParseMessengerFile(string path)
        {
            var chat = Path.GetFileName(Path.GetDirectoryName(path))?.Split('_').First();

            var json = File.ReadAllText(path);
            foreach (var messengerRecord in Deserialize<MessengerFile>(json).Messages)
                if (messengerRecord is (string message, string name))
                    yield return (message, name.Split(' ').First().ToLower(), DateTime.UnixEpoch.AddMilliseconds(messengerRecord.TimestampMs), $"messenger#{chat}");

        }

        class MessengerFile
        {
            [JsonPropertyName("messages")]
            public MessengerRecord[] Messages { get; set; }
        }

        class MessengerRecord
        {
            [JsonPropertyName("content")]
            public string? Text { get; set; }

            [JsonPropertyName("sender_name")]
            public string SenderName { get; set; }

            [JsonPropertyName("timestamp_ms")]
            public double TimestampMs { get; set; }

            public void Deconstruct(out string? message, out string? person)
            {
                message = Text;
                person = SenderName;
            }
        }
    }
}
#nullable restore

using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using static System.Text.Json.JsonSerializer;
#nullable enable
namespace Preprocessor
{
    static class Slack
    {
        public static IEnumerable<Record> ReadSlack(string? path = null)
        {
            var specifiedPath = path ?? $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Documents/ChatProject/Slack";
            return Directory.GetDirectories(specifiedPath)
                .SelectMany(Directory.GetFiles)
                .SelectMany(ParseSlackFile);
        }

        private static readonly Regex SlackTokenRegex = new Regex(@"<(.+)\|.+>");
        private static readonly Regex SlackUriRegex = new Regex(@"<([^|>]+)>");

        static string CleanSlackMessage(string slackMessage)
        {
            var replacedToken = SlackTokenRegex.Replace(slackMessage, "$1");
            var replacedUri = SlackUriRegex.Replace(replacedToken, "$1");
            return replacedUri;
        }

        static IEnumerable<Record> ParseSlackFile(string path)
        {
            var channel = Path.GetFileName(Path.GetDirectoryName(path));
            var json = File.ReadAllText(path);
            foreach (var slackRecord in Deserialize<SlackRecord[]>(json))
                if (slackRecord is (string message, string name))
                    yield return (CleanSlackMessage(message), name.Split(' ').First().ToLower(), DateTime.UnixEpoch.AddSeconds(slackRecord.Timestamp), $"slack#{channel}");
        }

        class SlackRecord
        {

            [JsonPropertyName("text")]
            public string Text { get; set; }

            [JsonPropertyName("user_profile")]
            public SlackProfile? Profile { get; set; }

            [JsonIgnore]
            public double Timestamp { get; private set; }

            [JsonPropertyName("ts")]
            public string ts { get => Timestamp.ToString(); set => Timestamp = Double.Parse(value); }
            public void Deconstruct(out string message, out string? person)
            {
                message = Text;
                person = Profile?.RealName;
            }
        }
        class SlackProfile
        {
            [JsonPropertyName("real_name")]
            public string? RealName { get; set; }
        }
    }
}
#nullable restore

using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

#nullable enable
namespace Preprocessor
{
    static class Extensions
    {
        public static void Deconstruct<TKey, TValue>(this IGrouping<TKey, TValue> grouping, out TKey key, out IEnumerable<TValue> enumerable)
        {
            key = grouping.Key;
            enumerable = grouping;
        }
    }

    class Program
    {
        static readonly string NormalizedOut = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Documents/ChatProject/NormalizedData/records.json";
        static readonly string CsvOut = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Documents/ChatProject/NormalizedData/records.tsv";
        static bool NotAUri(Record record) => !Uri.IsWellFormedUriString(record.Message, UriKind.Absolute);
        static Func<Record, bool> LongEnough(int length) => record => record.Message.Length >= length;
        static bool NoHttp(Record record) => !record.Message.Contains("http");
        static bool NotAshley(Record record) => record.Person != "ashley";
        static void Main(string[] args)
        {
            var messenger = Messenger.ReadMessenger().ToArray();
            var slack = Slack.ReadSlack().ToArray();
            var records = messenger.Concat(slack).OrderBy(x => x.Timestamp).ToArray();
            Directory.CreateDirectory(Path.GetDirectoryName(NormalizedOut));
            File.WriteAllBytes(NormalizedOut, JsonSerializer.SerializeToUtf8Bytes(records));
            File.WriteAllLines(CsvOut, new[] { "Person\tMessage" }.Concat(
                records.Where(NotAUri)
                .Where(LongEnough(30))
                .Where(NotAshley)
                .Select(x => $"{x.Person}\t{x.Message.Replace('\t', ' ').Replace('\n', ' ').ToLower()}")
            ));
        }
    }
}
#nullable restore

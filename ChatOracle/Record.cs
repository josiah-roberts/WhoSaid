#nullable enable
using System;
using Microsoft.ML.Data;

namespace ChatOracle
{
    class Record
    {

        public string? Context { get; set; }
        public string Message { get; set; }
        public string Person { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString() => $"{Person}: {Message}";
        public string ToLongString() => $"{Person}|{Context ?? "unknown"}|{Timestamp.ToString("yyyy-MM-dd H:mm")}| {Message}";
    }

    class MessageGuess
    {
        [ColumnName("PredictedLabel")]
        public string Person { get; set; }
    }
}
#nullable restore
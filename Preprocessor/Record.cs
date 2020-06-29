#nullable enable
using System;

namespace Preprocessor
{
    readonly struct Record
    {
        private Record(string message, string person, DateTime timestamp, string? context = null)
        {
            Message = message;
            Person = person;
            Timestamp = timestamp;
            Context = context;
        }

        public string? Context { get; }
        public string Message { get; }
        public string Person { get; }
        public DateTime Timestamp { get; }

        public static implicit operator Record((string Message, string Person, DateTime Timestamp) t) => new Record(t.Message, t.Person, t.Timestamp);
        public static implicit operator Record((string Message, string Person, DateTime Timestamp, string? Context) t) => new Record(t.Message, t.Person, t.Timestamp, t.Context);

        public void Deconstruct(out string message, out string person, out DateTime timestamp, out string? context)
        {
            message = Message;
            person = Person;
            timestamp = Timestamp;
            context = Context;
        }

        public override string ToString() => $"{Person}: {Message}";
        public string ToLongString() => $"{Person}|{Context ?? "unknown"}|{Timestamp.ToString("yyyy-MM-dd H:mm")}| {Message}";
    }
}
#nullable restore
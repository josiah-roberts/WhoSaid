using System.Linq;
using System.Collections.Generic;

namespace WhoSaid
{
    public class Prediction
    {
        public string Label { get; }
        public Score[] Scores { get; }
        public Prediction(string label, Score[] scores)
        {
            Label = label;
            Scores = scores;
        }

        public static implicit operator Prediction((string Label, IEnumerable<(string, float)> Scores) t)
            => new Prediction(t.Label, t.Scores.Select(x => (Score)x).OrderByDescending(x => x.Confidence).ToArray());
    }

    public class Score
    {
        public Score(string label, float confidence)
        {
            Label = label;
            Confidence = confidence;
        }

        public string Label { get; }
        public float Confidence { get; }

        public static implicit operator Score((string label, float confidence) t)
            => new Score(t.label, t.confidence);
    }
}
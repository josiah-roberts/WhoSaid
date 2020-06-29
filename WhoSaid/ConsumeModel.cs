using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Linq;
using static System.Environment;

namespace WhoSaid
{
    public class ConsumeModel
    {
        static readonly string ModelPath = GetEnvironmentVariable("WHOSAID_MODEL")
            ?? $"{GetFolderPath(SpecialFolder.UserProfile)}/Documents/ChatProject/WhoSaid/MLModel.zip";
        static Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictionEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(CreatePredictionEngine);

        public static Prediction Predict(string text)
        {
            var result = PredictionEngine.Value.Predict(new ModelInput() { Message = text });
            var labelNames = new List<string>();

            var column = PredictionEngine.Value.OutputSchema.GetColumnOrNull("Person");
            if (column.HasValue)
            {
                VBuffer<ReadOnlyMemory<char>> vbuffer = new VBuffer<ReadOnlyMemory<char>>();
                column.Value.GetKeyValues(ref vbuffer);

                foreach (ReadOnlyMemory<char> denseValue in vbuffer.DenseValues())
                    labelNames.Add(denseValue.ToString());
            }

            return (result.Prediction, result.Score.Select((score, index) => (labelNames[index], score)));
        }

        public static PredictionEngine<ModelInput, ModelOutput> CreatePredictionEngine()
        {
            var mlContext = new MLContext();

            var mlModel = mlContext.Model.Load(ModelPath, out var modelInputSchema);
            return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }
    }
}

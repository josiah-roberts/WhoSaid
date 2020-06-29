using System;
using Microsoft.ML;
using System.Text.Json;
using System.IO;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;

namespace ChatOracle
{
    class Program
    {
        private static readonly string DataPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/Documents/ChatProject/NormalizedData/records.json";
        static bool NotAUri(Record record) => !Uri.IsWellFormedUriString(record.Message, UriKind.Absolute);

        static IEstimator<ITransformer> GetEstimator(MLContext context)
        {
            return context.Transforms.Conversion.MapValueToKey(inputColumnName: "Person", outputColumnName: "Label")
                .Append(context.Transforms.Text.FeaturizeText(inputColumnName: "Message", outputColumnName: "MessageFeaturized"))
                .Append(context.Transforms.Concatenate("Features", "MessageFeaturized"))
                .AppendCacheCheckpoint(context);
        }

        static TransformerChain<KeyToValueMappingTransformer> BuildAndTrainModel(MLContext context, IDataView trainingDataView, IEstimator<ITransformer> pipeline)
        {
            var trainingPipeline = pipeline.Append(context.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
            return trainingPipeline.Fit(trainingDataView);
        }

        static void Evaluate(DataViewSchema trainingDataView)
        {

        }

        static void Main(string[] args)
        {
            var records = JsonSerializer.Deserialize<Record[]>(File.ReadAllText(DataPath));
            var context = new MLContext();
            IDataView chatData = context.Data.LoadFromEnumerable(records);
            var filteredData = context.Data.FilterByCustomPredicate<Record>(chatData, NotAUri);
            var pipeline = GetEstimator(context);
            var trainedModel = BuildAndTrainModel(context, filteredData, pipeline);
            var predictionEngine = context.Model.CreatePredictionEngine<Record, MessageGuess>(trainedModel);

        }
    }
}

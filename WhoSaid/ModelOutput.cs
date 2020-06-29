using System;
using Microsoft.ML.Data;

namespace WhoSaid
{
    public class ModelOutput
    {
        [ColumnName("PredictedLabel")]
        public String Prediction { get; set; }
        public float[] Score { get; set; }
    }
}

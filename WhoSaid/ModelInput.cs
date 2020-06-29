//*****************************************************************************************
//*                                                                                       *
//* This is an auto-generated file by Microsoft ML.NET CLI (Command-Line Interface) tool. *
//*                                                                                       *
//*****************************************************************************************

using Microsoft.ML.Data;

namespace WhoSaid
{
    public class ModelInput
    {
        [ColumnName("Person"), LoadColumn(0)]
        public string Person { get; set; }


        [ColumnName("Message"), LoadColumn(1)]
        public string Message { get; set; }
    }
}

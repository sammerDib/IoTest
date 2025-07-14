using System;
using System.Collections.Generic;

namespace UnitySC.DataAccess.ResultScanner.Implementation
{
    public class ResultPrio : Dto.ResultItem, IComparable
    {
        public int PrioCounter { get; set; } = 0;

        public ResultPrio()
            : base()
        {
        }

        public ResultPrio(SQL.ResultItem sqlResultItem)
           : base()
        {
            PrioCounter = 0;

            // MapSQLtoDTO(SQL.ResultItem sqlResultItem) like -- should be consistent with SpecificMapper.MapSQLtoWaferData
            var resultvalues = new List<Dto.ResultItemValue>();
            if (sqlResultItem.ResultItemValues != null)
            {
                foreach (var stats in sqlResultItem.ResultItemValues)
                    resultvalues.Add(new Dto.ResultItemValue
                    {
                        Id = stats.Id,
                        ResultItemId = stats.ResultItemId,
                        Type = stats.Type,
                        Name = stats.Name,
                        Value = stats.Value,
                        UnitType = stats.UnitType                        
                    }); ;
            }

            Id = sqlResultItem.Id;
            ResultId = sqlResultItem.ResultId;
            ResultType = sqlResultItem.ResultType;
            Date = sqlResultItem.Date;
            FileName = sqlResultItem.FileName;
            Name = sqlResultItem.Name;
            Idx = sqlResultItem.Idx;
            State = sqlResultItem.State;
            InternalState = sqlResultItem.InternalState;

            Result = new Dto.Result
            {
                Id = sqlResultItem.ResultId,
                WaferResultId = sqlResultItem.Result.WaferResultId,
                ChamberId = sqlResultItem.Result.ChamberId,
                RecipeId = sqlResultItem.Result.RecipeId,

                ActorType = sqlResultItem.Result.ActorType,
                Date = sqlResultItem.Result.Date,
                State = sqlResultItem.Result.State,

                Chamber = new Dto.Chamber
                {
                    Id = sqlResultItem.Result.ChamberId,
                    Name = sqlResultItem.Result.Chamber.Name,
                    Tool = new Dto.Tool
                    {
                        Id = sqlResultItem.Result.Chamber.Tool.Id,
                        Name = sqlResultItem.Result.Chamber.Tool.Name
                    },
                    ActorType = sqlResultItem.Result.Chamber.ActorType
                },

                Recipe = new Dto.Recipe
                {
                    Id = sqlResultItem.Result.Recipe.Id,
                    Name = sqlResultItem.Result.Recipe.Name,
                    Created = sqlResultItem.Result.Recipe.Created,
                    Step = new Dto.Step
                    {
                        Id = sqlResultItem.Result.Recipe.Step.Id,
                        Name = sqlResultItem.Result.Recipe.Step.Name
                    }
                },

                WaferResult = new Dto.WaferResult
                {
                    Id = sqlResultItem.Result.WaferResult.Id,
                    JobId = sqlResultItem.Result.WaferResult.JobId,
                    SlotId = sqlResultItem.Result.WaferResult.SlotId,
                    WaferName = sqlResultItem.Result.WaferResult.WaferName,
                    Date = sqlResultItem.Result.WaferResult.Date,
                    ProductId = sqlResultItem.Result.WaferResult.ProductId,
                    Job = new Dto.Job
                    {
                        Id = sqlResultItem.Result.WaferResult.Job.Id,
                        JobName = sqlResultItem.Result.WaferResult.Job.JobName,
                        LotName = sqlResultItem.Result.WaferResult.Job.LotName,
                        RecipeName = sqlResultItem.Result.WaferResult.Job.RecipeName,
                        Date = sqlResultItem.Result.WaferResult.Job.Date,
                        RunIter = sqlResultItem.Result.WaferResult.Job.RunIter,
                        ToolId = sqlResultItem.Result.WaferResult.Job.ToolId
                    },
                    Product = new Dto.Product
                    {
                        Id = sqlResultItem.Result.WaferResult.ProductId,
                        Name = sqlResultItem.Result.WaferResult.Product.Name
                    }
                }
            };
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            var otherItem = obj as ResultPrio;
            if (otherItem != null)
            {
                int nCmp1 = PrioCounter.CompareTo(otherItem.PrioCounter);
                if (nCmp1 == 0)
                    return 0 - Id.CompareTo(otherItem.Id);
                else
                    return nCmp1;
            }
            else
                throw new ArgumentException("Bad Item ResultItem type");
        }
    }
}

using System;
using System.Collections.Generic;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.DataAccess.Service.Implementation
{
    public sealed class SpecificMapper
    {
        static public Dto.ResultItem MapSQLtoDTO(SQL.ResultItem x)
        {
            // Warning SQL.ResultItem x should be extrated from a full depth query
            // need to have ResultItem, chamber, tool and result values

            var resultitemvalues = new List<Dto.ResultItemValue>();
            if (x.ResultItemValues != null)
            {
                foreach (var stats in x.ResultItemValues)
                    resultitemvalues.Add(new Dto.ResultItemValue
                    {
                        Id = stats.Id,
                        ResultItemId = stats.ResultItemId,
                        Type = stats.Type,
                        Name = stats.Name,
                        Value = Math.Round(stats.Value, 2),
                        UnitType = stats.UnitType
                    });
            }

            return new Dto.ResultItem
            {
                Id = x.Id,
                ResultId = x.ResultId,
                ResultType = x.ResultType,
                FileName = x.FileName,
                Date = x.Date,
                Name = x.Name,
                Idx = x.Idx,
                State = x.State,
                InternalState = x.InternalState,

                Result = new Dto.Result
                { 
                    Id = x.ResultId,
                    WaferResultId = x.Result.WaferResultId,
                    ChamberId = x.Result.ChamberId,
                    RecipeId = x.Result.RecipeId,

                    ActorType = x.Result.ActorType,
                    Date = x.Result.Date,
                    State = x.Result.State,

                    Chamber = new Dto.Chamber
                    {
                        Id = x.Result.ChamberId,
                        Name = x.Result.Chamber.Name,
                        ChamberKey = x.Result.Chamber.ChamberKey,
                        Tool = new Dto.Tool
                        {
                            Id = x.Result.Chamber.Tool.Id,
                            Name = x.Result.Chamber.Tool.Name,
                            ToolKey = x.Result.Chamber.Tool.ToolKey
                        },
                        ActorType = x.Result.Chamber.ActorType
                    },

                    Recipe = new Dto.Recipe
                    { 
                        Id = x.Result.Recipe.Id,
                        Name = x.Result.Recipe.Name,
                        Created = x.Result.Recipe.Created, 
                        Step = new Dto.Step
                        { 
                            Id = x.Result.Recipe.Step.Id,
                            Name = x.Result.Recipe.Step.Name
                        }
                    },

                    WaferResult = new Dto.WaferResult
                    {
                        Id = x.Result.WaferResult.Id,
                        JobId = x.Result.WaferResult.JobId,
                        SlotId = x.Result.WaferResult.SlotId,
                        WaferName = x.Result.WaferResult.WaferName,
                        Date = x.Result.WaferResult.Date,
                        ProductId = x.Result.WaferResult.ProductId,
                        Job = new Dto.Job
                        {
                            Id = x.Result.WaferResult.Job.Id,
                            JobName = x.Result.WaferResult.Job.JobName,
                            LotName = x.Result.WaferResult.Job.LotName,
                            RecipeName = x.Result.WaferResult.Job.RecipeName,
                            Date = x.Result.WaferResult.Job.Date,
                            RunIter = x.Result.WaferResult.Job.RunIter,
                            ToolId = x.Result.WaferResult.Job.ToolId
                        },
                        Product = new Dto.Product 
                        {
                            Id = x.Result.WaferResult.ProductId,
                            Name = x.Result.WaferResult.Product.Name
                        }
                    }
                },

                ResultItemValues = resultitemvalues
            };
        }

        static public Dto.ModelDto.LocalDto.WaferResultData MapSQLtoWaferData(SQL.ResultItem x)
        {
            // Warning SQL.ResultItem x should be extrated from a full depth query
            // need to have parent result, chamber, tool and resultitem values
            // + product
            // + step

            var wafdt = new Dto.ModelDto.LocalDto.WaferResultData();

            wafdt.SlotId = x.Result.WaferResult.SlotId;
            wafdt.WaferName = x.Result.WaferResult.SlotId + " - " + x.Result.WaferResult.WaferName;
            wafdt.ResultItem = MapSQLtoDTO(x);
            wafdt.ActorType = (ActorType)x.Result.ActorType;

            return wafdt;
        }
    }
}

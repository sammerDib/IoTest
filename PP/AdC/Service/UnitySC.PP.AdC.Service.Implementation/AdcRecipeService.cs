using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Xml;

using AcquisitionAdcExchange;

using UnitySC.PP.ADC.Service.Implementation.Proxy;
using UnitySC.PP.ADC.Service.Interface;
using UnitySC.PP.ADC.Service.Interface.Recipe;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PP.ADC.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AdcRecipeService : DuplexServiceBase<IADCRecipeServiceCallback>, IADCRecipeService
    {
        private DbRecipeServiceProxy _dbRecipeService;
        private Mapper _mapper = ClassLocator.Default.GetInstance<Mapper>();
        private PPConfigurationADC _ppConfiguration;

        public AdcRecipeService(ILogger logger) : base(logger, ExceptionType.RecipeException)
        {
            _dbRecipeService = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();
            _ppConfiguration = ClassLocator.Default.GetInstance<PPConfigurationADC>();
        }

        public Response<ADCRecipe> CreateRecipe(string name = null, int stepId = -1, int userId = 0)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                _logger.Debug("CreateRecipe");
                var recipe = new ADCRecipe();
                recipe.Name = name;
                recipe.ActorType = ActorType.ADC;
                recipe.Key = Guid.NewGuid();
                recipe.StepId = stepId;
                recipe.UserId = userId;
                recipe.Created = DateTime.Now;
                recipe.CreatorChamberId = _ppConfiguration.ChamberId;

                ADCEngine.Recipe adcEngineRecipe = new ADCEngine.Recipe();
                recipe.ADCEngineRecipe = adcEngineRecipe;
                XmlDocument xmldoc = new XmlDocument();
                //Recipe.SetInputDir(currentFileName);
                XmlNode node = adcEngineRecipe.Save(xmldoc, false);
                xmldoc.AppendChild(node);

                var sb = new StringBuilder();
                var sw = new StringWriter(sb);
                xmldoc.Save(sw);

                recipe.ADCEngineRecipeXml = sb.ToString();


                var dbrecipe = _mapper.AutoMap.Map<DataAccess.Dto.Recipe>(recipe);
                dbrecipe.AddOutput(ResultType.NotDefined);
                dbrecipe.AddInput(ResultType.NotDefined);
                _dbRecipeService.SetRecipe(dbrecipe, true);

                return recipe;
            });
        }

        public Response<ADCRecipe> GetRecipeFromKey(Guid recipeKey, bool takeArchivedRecipes = false)
        {
            return InvokeDataResponse(() =>
            {
                var dbrecipe = _dbRecipeService.GetLastRecipe(recipeKey, /*includeRecipeFileInfos*/ false, takeArchivedRecipes);
                var recipe = PrepareRecipe(dbrecipe);
                return recipe;
            });
        }

        /*
        public List<TCPMRecipe> GetTCRecipeList()
        {
            var chamberId = _pmConfiguration.ChamberId;
            var toolId = ClassLocator.Default.GetInstance<DbToolServiceProxy>().GetChamber(chamberId).ToolId;
            return _dbRecipeService.GetTCRecipeList(toolId);
        }
        */
        [Obsolete]
        public Response<VoidResult> SaveRecipe(ADCRecipe recipe)
        {
            return InvokeVoidResponse(messagesContainer =>
            {

                var dbrecipe = _mapper.AutoMap.Map<DataAccess.Dto.Recipe>(recipe);

                recipe.ADCEngineRecipe = new ADCEngine.Recipe();

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(recipe.ADCEngineRecipeXml);


                recipe.ADCEngineRecipe.Load(xmldoc);

                // YOU SHOULD NOT USE channel ID any more
                var inputs = recipe.ADCEngineRecipe.ModuleList.Values.OfType<ADCEngine.IDataLoader>().SelectMany(x => x.CompatibleResultTypes.Cast<eChannelID>()).Distinct();


                foreach (var input in inputs)
                {
                    ResultType rt = ResultType.NotDefined;

                    switch (input)
                    {

                        case eChannelID.DemeterDeflect_Front: rt = ResultType.DMT_CurvatureX_Front; break;
                        case eChannelID.DemeterDeflect_Back: rt = ResultType.DMT_CurvatureX_Back; break;

                        case eChannelID.DemeterBrightfield_Front: rt = ResultType.DMT_Brightfield_Front; break;
                        case eChannelID.DemeterBrightfield_Back: rt = ResultType.DMT_Brightfield_Back; break;

                        //case eChannelID.DemeterDeflect_Die_Front: rt = ResultType.NotDefined; break;
                        //case eChannelID.DemeterDeflect_Die_Back: rt = ResultType.NotDefined; break;
                        //case eChannelID.DemeterReflect_Die_Front: rt = ResultType.NotDefined; break;
                        //case eChannelID.DemeterReflect_Die_Back: rt = ResultType.NotDefined; break;

                        case eChannelID.DemeterDarkImage_Front: rt = ResultType.DMT_Dark_Front; break;
                        case eChannelID.DemeterDarkImage_Back: rt = ResultType.DMT_Dark_Back; break;

                        //case eChannelID.DemeterGlobalTopo_Front: rt = ResultType.DMT_PhaseMask_Front; break;
                        //case eChannelID.DemeterGlobalTopo_Back: rt = ResultType.DMT_PhaseMask_Back; break;

                        case eChannelID.DemeterObliqueLight_Front: rt = ResultType.DMT_ObliqueLight_Front; break;
                        case eChannelID.DemeterObliqueLight_Back: rt = ResultType.DMT_ObliqueLight_Back; break;

                        //case eChannelID.DemeterPhaseX_Front: rt = ResultType.DMT_TopoPhaseX_Front; break;
                        //case eChannelID.DemeterPhaseY_Front: rt = ResultType.DMT_TopoPhaseY_Front; break;
                        //case eChannelID.DemeterPhaseX_Back: rt = ResultType.DMT_TopoPhaseX_Back; break;
                        //case eChannelID.DemeterPhaseY_Back: rt = ResultType.DMT_TopoPhaseY_Back; break;

                    }
                    if (rt != ResultType.NotDefined)
                        dbrecipe.AddInput(rt);
                }

                dbrecipe.AddOutput(ResultType.NotDefined);

                _dbRecipeService.SetRecipe(dbrecipe, true);
            });
        }

        public Response<VoidResult> Test()
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                _logger.Information("Test");
            });
        }

        public ADCRecipe GetRecipeWithTC(string recipeName)
        {
            var dbrecipe = _dbRecipeService.GetRecipeWithTC(recipeName);
            return PrepareRecipe(dbrecipe);
        }

        private ADCRecipe PrepareRecipe(DataAccess.Dto.Recipe dbrecipe)
        {
            if (dbrecipe == null)
                return null;

            var recipe = _mapper.AutoMap.Map<ADCRecipe>(dbrecipe);
            return recipe;
        }
    }
}

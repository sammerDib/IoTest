using System;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml;

using AcquisitionAdcExchange;

using ADCEngine;

using UnitySC.Dataflow.Service.Interface;
using UnitySC.PP.ADC.Service.Implementation.Proxy;
using UnitySC.PP.ADC.Service.Interface;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PP.ADC.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ADCService : DuplexServiceBase<IADCServiceCallback>, IADCService, IAdcExecutor
    {
        private DbRecipeServiceProxy _dbRecipeService;
        public ServiceInvoker<IDAP> _DAPService = ClassLocator.Default.GetInstance<ServiceInvoker<IDAP>>();
       
        private Mapper _mapper = ClassLocator.Default.GetInstance<Mapper>();
        private PPConfigurationADC _ppConfiguration;

        private IAdcExecutor _adcExecutor = null;

        public ADCService(ILogger logger, IAdcExecutor adcExecutor) : base(logger, ExceptionType.RecipeException)
        {
            _dbRecipeService = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();
            _ppConfiguration = ClassLocator.Default.GetInstance<PPConfigurationADC>();

            _adcExecutor = adcExecutor;

            //MergeContext.Context.Initializer.Init();
            ADCEngine.ADC.Instance.Init();

        }


        private Recipe LoadRecipe(string recipeFileName)
        {
            Recipe recipe = null;


            XmlDocument xmldocLoad = new XmlDocument();

            xmldocLoad.Load(recipeFileName);

            recipe = new Recipe();
            recipe.Load(xmldocLoad);

            recipe.SetInputDir(recipeFileName);

            return recipe;
        }

        private Recipe LoadRecipeBdD(string recipeGuid)
        {
            Recipe recipe = null;

            Guid rg = new Guid(recipeGuid);
            UnitySC.DataAccess.Dto.Recipe recipeDto = _dbRecipeService.GetLastRecipe(rg);



            XmlDocument xmldocLoad = new XmlDocument();
            xmldocLoad.LoadXml(recipeDto.XmlContent);

            //xmldocLoad.Load(recipeFileName);

            recipe = new Recipe();
            recipe.Load(xmldocLoad);

            recipe.SetInputDir(recipeGuid);
            recipe.OutputDir = recipeGuid;

            return recipe;
        }
        /// <summary>
        /// Load Ada in recipe
        /// </summary>
        /// <param name="adaFile"></param>
        /// <param name="recipe"></param>
        private void LoadAda(string adaFile, Recipe recipe)
        {
            MergeContext.AdaLoader adaloader = new MergeContext.AdaLoader(adaFile);
            adaloader.LoadAda();
            MergeContext.MergeContext mergectx = new MergeContext.MergeContext(recipe, adaloader.RecipeData);
            mergectx.Merge();
            mergectx.SetAcquitionImages(adaloader.AcqImageList);

        }

        private int ExecuteREcipe(Recipe recipe)
        {
            int recipeId = 0;
            XmlDocument xmldocSave = new XmlDocument();
            //recipe.SetInputDir(recipeFileName);
            XmlNode node = recipe.Save(xmldocSave, recipe.IsMerged);
            xmldocSave.AppendChild(node);
            using (MemoryStream stream = new MemoryStream())
            {
                xmldocSave.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                byte[] data = stream.ToArray();

                // [TODO] : rendre la fnct async
                RecipeId recipeIdADC = _adcExecutor.ReprocessRecipe(data);    // execution de la recette via AdcExecutor
                recipeId = recipeIdADC.Id;

            }
            return recipeId;
        }


        Response<VoidResult> IADCService.Test()
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                _logger.Information("Test");
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recipeFileNam"></param>
        /// <param name="adaFile"></param>
        /// <returns>id de le recette</returns>
        Response<int> IADCService.ExecuteRecipe(string recipeFileName, string adaFile)
        {


            return InvokeDataResponse(messagesContainer =>
            {
                /*
                //string recipeFileName = "";
                Recipe recipe = null;

                int recipeId = 0;

                // chargement de la recette

                XmlDocument xmldocLoad = new XmlDocument();
                xmldocLoad.Load(recipeFileName);

                recipe = new Recipe();
                recipe.Load(xmldocLoad);
                */



                // chargement de la recette
                Recipe recipe = LoadRecipe(recipeFileName);


                // if (Recipe.HasUnknwonModules())
                //     AttentionMessageBox.Show("Some modules failed to load.");

                //if (!ServiceRecipe.Instance().ValidateInputData())
                //{
                //    if (Services.Services.Instance.PopUpService.ShowConfirmeYesNo("Invalid input data", "This merged recipe contains invalid paths." + Environment.NewLine + "Do you want to update them ?"))
                //        Services.Services.Instance.PopUpService.ShowDialogWindow("Select picture paths", new PicturesSelectionViewModel(), 500, 200, true);
                //}

                LoadAda(adaFile, recipe);

                /*
                MergeContext.AdaLoader adaloader = new MergeContext.AdaLoader(adaFile);
                adaloader.LoadAda();
                MergeContext.MergeContext mergectx = new MergeContext.MergeContext(recipe, adaloader.RecipeData);
                mergectx.Merge();
                mergectx.SetAcquitionImages(adaloader.AcqImageList);
                */

                // Execute Recipe
                //...............

                int recipeId = ExecuteREcipe(recipe);

                /*
                XmlDocument xmldocSave = new XmlDocument();
                recipe.SetInputDir(recipeFileName);
                XmlNode node = recipe.Save(xmldocSave, recipe.IsMerged);
                xmldocSave.AppendChild(node);
                using (MemoryStream stream = new MemoryStream())
                {
                    xmldocSave.Save(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    byte[] data = stream.ToArray();

                    // [TODO] : rendre la fnct async
                    RecipeId recipeIdADC = _adcExecutor.ReprocessRecipe(data);    // execution de la recette via AdcExecutor
                    recipeId = recipeIdADC.Id;

                }
                */

                return recipeId;
            });
            // trace   RefreshNodeStatistics();
        }

        RecipeId IAdcExecutor.ReprocessRecipe(byte[] RecipeData)
        {
            return _adcExecutor.ReprocessRecipe(RecipeData);
        }

        void IAdcExecutor.AbortRecipe(RecipeId recipeId)
        {
            _adcExecutor.AbortRecipe(recipeId);
        }

        RecipeId IAdcExecutor.ExecuteRecipe(Recipe recipe)
        {
            return _adcExecutor.ExecuteRecipe(recipe);
        }

        RecipeId IAdcExecutor.GetCurrentRecipeId()
        {
            return _adcExecutor.GetCurrentRecipeId();
        }

        WaferInfo IAdcExecutor.GetCurrentWaferInfo()
        {
            return _adcExecutor.GetCurrentWaferInfo();
        }

        byte[] IAdcExecutor.GetRecipe(RecipeId recipeId)
        {
            return _adcExecutor.GetRecipe(recipeId);
        }

        RecipeStat IAdcExecutor.GetRecipeStat(RecipeId recipeId)
        {
            return _adcExecutor.GetRecipeStat(recipeId);
        }

        void IAdcExecutor.SetLocalRecipePath(string localRecipePath)
        {
            _adcExecutor.SetLocalRecipePath(localRecipePath);
        }



        /*************************** IDataflowManagerPP ********************************/

        Recipe currentRecipe = null;
        string currentdataFlowId = null;
        string currentactorId = null;

        /// <summary>
        /// Execute the Recipe in a Task
        /// </summary>
        /// <param name="recipe"></param>
        private void ExecuteRecipe(Recipe recipe)
        {
            // [TODO] Check if all Data is available

            Task.Run(() =>
            {
                int recipeId = ExecuteREcipe(recipe);             
            });


        }


        Response<VoidResult> IADCService.StartPPRecipe(string actorRecipeName, string actorId, string dataFlowId)
        {
            _logger.Debug($"IDataflowManagerPP.StartPPRecipe - actorRecipeName: {actorRecipeName}, actorId: {actorId}, dataFlowId: {dataFlowId}");


            currentactorId = actorId;

            currentdataFlowId = dataFlowId;
            currentRecipe = LoadRecipeBdD(actorRecipeName);


            // _PPDataflowManagerService.Invoke(i => i.RecipeCanBeExecuted(currentdataFlowId, currentactorId));



            return new Response<VoidResult>();

        }

        Response<bool> IADCService.DataAvailable(string actorID, string dataflowID, Guid dapToken)
        {
            _logger.Debug($"IDataflowManagerPP.DataAvailable - actorId: {actorID}, dataFlowId: {dataflowID}, dapToken: {dapToken.ToString()}");

            if (currentdataFlowId == dataflowID)
            {

                DAPData dAPdata = _DAPService.Invoke(i => i.GetData(dapToken));

                string adafile = dAPdata.Data;

                LoadAda(adafile, currentRecipe);

                ExecuteRecipe(currentRecipe);


                return new Response<bool>() { Result = true };

            }
            else
            {
                return new Response<bool>() { Result = false };

            }
        }

        Response<bool> IADCService.RecipeEnded(string actorID, string dataflowID)
        {
            _logger.Debug($"IDataflowManagerPP.RecipeEnded - actorId: {actorID}, dataFlowId: {dataflowID}");

            return new Response<bool>() { Result = true };

        }



    }
}

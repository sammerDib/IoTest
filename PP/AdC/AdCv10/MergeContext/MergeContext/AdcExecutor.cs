using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Xml;

using AcquisitionAdcExchange;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.Tools;


namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false)]
    public class AdcExecutor : IAdcExecutor, IAdcAcquisition
    {
        private class RecipeExec
        {
            public int Id;
            public XmlDocument Xmldoc = new XmlDocument();
            public Recipe Recipe = new Recipe();
            public MergeContext.MergeContext mergectx;
            public bool IsRunning;
        }


        private RecipeExec currentExec;

        private int lastRecipeId;
        private string _localRecipePath;
        private AdcToRobot.AdcToRobot adcToRobot = new AdcToRobot.AdcToRobot();
        private ServiceHost host;   // Service WCF

        //=================================================================
        //
        //=================================================================
        void IAdcAcquisition.InitADC(MIL_ID applicationId, MIL_ID systemId)
        {
            //-------------------------------------------------------------
            // Init ADC
            //-------------------------------------------------------------
            ADC.Instance.Init(applicationId, systemId);

            //-------------------------------------------------------------
            // Init AdcToRobot
            //-------------------------------------------------------------
            bool enableTransferToRobot = bool.Parse(ConfigurationManager.AppSettings["AdaToAdc.TransferToRobot.Enable"]);
            bool embeddedTransferToRobot = bool.Parse(ConfigurationManager.AppSettings["AdaToAdc.TransferToRobot.Embedded"]);

            if (enableTransferToRobot)
            {
                if (embeddedTransferToRobot)
                {
                    adcToRobot = new AdcToRobot.AdcToRobot();
                    adcToRobot.Start();
                }
                else
                {
                    ADC.Instance.TransferToRobotStub.Connect();
                }
            }

            //-------------------------------------------------------------
            // Init DataBase
            //-------------------------------------------------------------
            bool useExportedDataBase = Convert.ToBoolean(ConfigurationManager.AppSettings["DatabaseConfig.UseExportedDatabase"]);
          //  Database.Service.BootStrapper.DefaultRegister(useExportedDataBase);
          // to do bootstrapp some datbase stuff here or not

            //-------------------------------------------------------------
            // Création des services WCF
            //-------------------------------------------------------------
            host = new ServiceHost(this);
            foreach (var endpoint in host.Description.Endpoints)
                ADC.log("Creating service on \"" + endpoint.Address + "\"");
            host.Open();
        }

        //=================================================================
        //
        //=================================================================
        void IAdcAcquisition.StopADC()
        {
            if (host != null)
            {
                ADC.log("Stop service");
                host.Close();
            }
        }

        //=================================================================
        //
        //=================================================================
        RecipeId IAdcExecutor.ReprocessRecipe(byte[] recipeData)
        {
            using (Stream stream = new MemoryStream(recipeData, writable: false))
            {
                RecipeExec exec = new RecipeExec();

                exec.Xmldoc.Load(stream);
                exec.Recipe.Load(exec.Xmldoc);
                exec.Id = Interlocked.Increment(ref lastRecipeId);

                //Update Recipe info
                var recipeGuid = exec.Recipe.Wafer.waferInfo[eWaferInfo.ADCRecipeGUID];
                exec.Recipe.Name = exec.Recipe.Wafer.waferInfo[eWaferInfo.ADCRecipeName];
                exec.Recipe.Key = new Guid (exec.Recipe.Wafer.waferInfo[eWaferInfo.ADCRecipeGUID]);
               
                currentExec = exec;
                exec.Recipe.recipeExecutedEvent += RecipeExecutedEventHandler; // fuite memoire ??? oui !!
                exec.IsRunning = true;
                exec.Recipe.Start(reprocess: true);
                return new RecipeId() { Id = exec.Id };
            }
        }

        //=================================================================
        //
        //=================================================================
        public void AbortRecipe(RecipeId recipeId)
        {
            RecipeExec exec = currentExec;
            if (exec == null || exec.Id != recipeId.Id)
                return;

            exec.Recipe.Abort();
        }

        //=================================================================
        //
        //=================================================================
        RecipeId IAdcExecutor.ExecuteRecipe(Recipe recipe)
        {
            RecipeExec exec = new RecipeExec();

            exec.Recipe = recipe;
            exec.Id = Interlocked.Increment(ref lastRecipeId);

            currentExec = exec;
            exec.Recipe.recipeExecutedEvent += (s, e) => exec.IsRunning = false;
            exec.IsRunning = true;
            exec.Recipe.Start(reprocess: true);

            return new RecipeId() { Id = exec.Id };
        }


        //=================================================================
        //

        //=================================================================
        RecipeId IAdcExecutor.GetCurrentRecipeId()
        {
            if (currentExec != null)
                return new RecipeId() { Id = currentExec.Id };
            else
                return null;
        }

        WaferInfo IAdcExecutor.GetCurrentWaferInfo()
        {
            return currentExec?.mergectx?.WaferInfo;
        }

        //=================================================================
        //
        //=================================================================
        byte[] IAdcExecutor.GetRecipe(RecipeId recipeId)
        {
            RecipeExec exec = currentExec;
            if (exec == null || exec.Id != recipeId.Id)
                return null;

            MemoryStream stream = new MemoryStream();
            exec.Xmldoc.Save(stream);
            return stream.ToArray();
        }

        //=================================================================
        //
        //=================================================================
        RecipeStat IAdcExecutor.GetRecipeStat(RecipeId recipeId)
        {
            RecipeExec exec = currentExec;
            if (exec == null || exec.Id != recipeId.Id)
                return null;

            RecipeStat recipeStat = new RecipeStat();
            foreach (ModuleBase module in exec.Recipe.ModuleList.Values)
            {
                ModuleStat mstat = new ModuleStat();
                mstat.ModuleId = module.Id;
                module.GetStats(out mstat.nbObjectsIn, out mstat.nbObjectsOut);
                mstat.State = module.State;
                mstat.HasError = module.HasError;

                mstat.ErrorMessage = module.ErrorMessage;
                recipeStat.ModuleStat.Add(module.Id, mstat);
            }

            if (exec.Recipe.FaultyModule != null)
                recipeStat.FaultyModuleId = exec.Recipe.FaultyModule.Id;
            recipeStat.HasError = exec.Recipe.HasError;
            recipeStat.IsRunning = exec.IsRunning;

            return recipeStat;
        }

        //=================================================================
        //
        //=================================================================
        void IAdcExecutor.AbortRecipe(RecipeId recipeId)
        {
            RecipeExec exec = currentExec;
            if (exec == null || exec.Id != recipeId.Id)
                throw new ApplicationException("recipe is not running, id:" + recipeId.Id);

            exec.Recipe.Abort();
        }

        //=================================================================
        //
        //=================================================================
        void IAdcExecutor.SetLocalRecipePath(string localRecipePath)
        {
            _localRecipePath = localRecipePath;
        }

        //=================================================================
        //
        //=================================================================
        RecipeId IAdcAcquisition.StartRecipe(RecipeData recipeData)
        {
            //-------------------------------------------------------------
            // Merge Context
            //-------------------------------------------------------------
            MergeContext.MergeContext mergectx = new MergeContext.MergeContext(recipeData);
            mergectx.LocalRecipePath = _localRecipePath;
            mergectx.Merge();

            //-------------------------------------------------------------
            // Crée le RecipeExec
            //-------------------------------------------------------------
            RecipeExec exec = new RecipeExec();
            exec.Xmldoc = mergectx.Xmldoc;
            exec.Recipe = mergectx.Recipe;
            exec.mergectx = mergectx;
            exec.Id = Interlocked.Increment(ref lastRecipeId);

            //-------------------------------------------------------------
            // Start
            //-------------------------------------------------------------
            currentExec = exec;
            exec.Recipe.recipeExecutedEvent += RecipeExecutedEventHandler;
            exec.IsRunning = true;

            exec.Recipe.Start(reprocess: false);

            return new RecipeId() { Id = exec.Id };
        }

        //=================================================================
        //
        //=================================================================
        void IAdcAcquisition.SetAcquitionImages(RecipeId recipeId, List<AcquisitionData> AcquisitionImageList)
        {
            RecipeExec exec = currentExec;
            if (exec == null || exec.Id != recipeId.Id)
                throw new ApplicationException("recipe is not running, id:" + recipeId.Id);

            exec.mergectx.SetAcquitionImages(AcquisitionImageList);

            PathString logfolder = ConfigurationManager.AppSettings["LogFolder"];
            PathString filename = logfolder / "Wafers" / ("wafer-" + exec.Recipe.Wafer.Basename + "--" + DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + ".adcmge");
            exec.Recipe.Save(filename);
        }

        //=================================================================
        //
        //=================================================================
        private void RecipeExecutedEventHandler(object sender, EventArgs e)
        {
            currentExec.Recipe.recipeExecutedEvent -= RecipeExecutedEventHandler;
            currentExec.IsRunning = false;
        }

        //=================================================================
        //
        //=================================================================
        void IAdcAcquisition.StopRecipe(RecipeId recipeId)
        {
            RecipeExec exec = currentExec;
            if (exec == null || exec.Id != recipeId.Id)
                throw new ApplicationException("recipe is not running, id:" + recipeId.Id);

            exec.Recipe.Stop();
        }

        //=================================================================
        //
        //=================================================================
        bool IAdcAcquisition.FeedImage(RecipeId recipeId, AcquisitionData acqdata)
        {
            RecipeExec exec = currentExec;
            if (exec == null || exec.Id != recipeId.Id)
                throw new ApplicationException("recipe is not running, id:" + recipeId.Id);

            return exec.mergectx.FeedImage((AcquisitionMilImage)acqdata);
        }

        //=================================================================
        //
        //=================================================================
        RecipeStatus IAdcAcquisition.GetRecipeStatus(RecipeId recipeId)
        {
            RecipeStatus recipeStatus = new RecipeStatus();

            RecipeExec exec = currentExec;
            if (exec == null || exec.Id != recipeId.Id)
            {
                recipeStatus.Status = eRecipeStatus.Unknown;
            }
            else if (exec.IsRunning)
            {
                recipeStatus.Status = eRecipeStatus.Processing;
            }
            else if (exec.Recipe.HasError)
            {
                recipeStatus.Status = eRecipeStatus.Error;
                recipeStatus.Message = exec.Recipe.ErrorMessage;
            }
            else
            {
                recipeStatus.Status = eRecipeStatus.Completed;
            }

            return recipeStatus;
        }
    }
}

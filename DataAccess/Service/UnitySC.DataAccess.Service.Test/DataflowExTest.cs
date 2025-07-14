using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.DataAccess.Service.Test
{
    [TestClass]
    public class DataflowExTest : BaseTest
    {
        private IDbRecipeService _recipeService => ClassLocator.Default.GetInstance<IDbRecipeService>();

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {

        }

        [TestInitialize]
        public void TestInit()
        {
            base.Init();
        }

        [TestMethod]
        public void T001_AddAndStartDataflow()
        {
            /* string psdActorID = "psd";
             string edgeActorID = "edge";
             string adc1ActorID = "adc1";
             string darkfieldActorID = "darkfield";
             string adc2ActorID = "adc2";
             string dataflowRecipeName = "TestDataflow2";

             // Add dataflow to database
             using (UnitOfWorkUnity unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
             {
                 var dataflowRecipe = new DataflowRecipeComponent();
                 dataflowRecipe.Name = dataflowRecipeName;
                 dataflowRecipe.ActorType = ActorType.DataflowManager;
                 var psd = new DataflowRecipeComponent() { Name = "PSD", RecipeId = 62, ActorType = ActorType.PSD };
                 var edge = new DataflowRecipeComponent() { Name = "Edge", RecipeId = 65, ActorType = ActorType.Edge };
                 var adc = new DataflowRecipeComponent() { Name = "ADC", RecipeId = 69, ActorType = ActorType.ADC };
                 var athos = new DataflowRecipeComponent() { Name = "Darkfield", RecipeId = 73, ActorType = ActorType.Darkfield };
                 var adc2 = new DataflowRecipeComponent() { Name = "ADC2", RecipeId = 75, ActorType = ActorType.ADC };
                 athos.ChildRecipes.Add(new DataflowRecipeAssociation(AssociationRecipeType.All, adc2));
                 adc.ChildRecipes.Add(new DataflowRecipeAssociation(AssociationRecipeType.All, athos));
                 psd.ChildRecipes.Add(new DataflowRecipeAssociation(AssociationRecipeType.All, adc));
                 edge.ChildRecipes.Add(new DataflowRecipeAssociation(AssociationRecipeType.All, adc));
                 dataflowRecipe.ChildRecipes.Add(new DataflowRecipeAssociation(AssociationRecipeType.All, psd));
                 dataflowRecipe.ChildRecipes.Add(new DataflowRecipeAssociation(AssociationRecipeType.All, edge));
                 RecipeService.AddDataflowRecipe(unitOfWork, dataflowRecipe, 4, GetLayers().First().Id, "This is a test comment from DataflowTest.AddAndStartDataflow2");
             }

             // create dataflow;
             var dataflowManager = ClassLocator.Default.GetInstance<DataflowManager>();

             // Test error before subscribe
             Assert.IsNotNull(dataflowManager.StateChanged(PPState.Available, adc1ActorID).Exception, "Exception missing for state change before subscirbe");

             // Subscribe actor
             Assert.IsNull(dataflowManager.SubscribeActor(psdActorID, ActorType.PSD).Exception);
             Assert.IsNull(dataflowManager.SubscribeActor(edgeActorID, ActorType.Edge).Exception);
             Assert.IsNull(dataflowManager.SubscribeActor(adc1ActorID, ActorType.ADC).Exception);
             Assert.IsNull(dataflowManager.SubscribeActor(darkfieldActorID, ActorType.Darkfield).Exception);
             Assert.IsNull(dataflowManager.SubscribeActor(adc2ActorID, ActorType.ADC).Exception);

             // Test bad actor ID
             Assert.IsNotNull(dataflowManager.StateChanged(PPState.Available, "dsfdsfdsfdsfdsfds"), "Exception missing for bad actor ID");

             // State change for actor
             Assert.IsNull(dataflowManager.StateChanged(PPState.Available, adc1ActorID).Exception);
             Assert.IsNull(dataflowManager.StateChanged(PPState.Available, adc2ActorID).Exception);
             Assert.IsNull(dataflowManager.StateChanged(PMState.Available, psdActorID).Exception);
             Assert.IsNull(dataflowManager.StateChanged(PMState.Available, edgeActorID).Exception);
             Assert.IsNull(dataflowManager.StateChanged(PMState.Available, darkfieldActorID).Exception);

             string dataflowID = "yoyo";
             Assert.IsNull(dataflowManager.RecipeCanBeExecuted(dataflowRecipeName, psdActorID, dataflowID).Exception);
             Debug.WriteLine(dataflowManager.ToString());*/
        }

        [TestMethod]
        public void T002_GetProductAndSteps()
        {
            /*  GetSteps();*/
        }

        [TestMethod]
        public void T003_GetDataflows()
        {
            /*var dataflows = _recipeService.GetDatalows(GetSteps().First().Id);
            Assert.IsNull(dataflows.Exception);
            Assert.IsTrue(dataflows.ResultItem.Any());*/
        }
    }
}

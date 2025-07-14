using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.EME.Client.Recipe.ViewModel;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client.Recipe.Test
{
    [TestClass]
    public class EmeRecipeMapperTests
    {
        private Mapper _mapper;
        private EMERecipe _sourceRecipe;
        private DataAccess.Dto.Recipe _destinationRecipe;

        [TestInitialize]
        public void SetupTest()
        {
            _mapper = new Mapper();
            _sourceRecipe = new EMERecipe();
            _sourceRecipe.Name = "SourceRecipe";
            _sourceRecipe.ActorType = ActorType.EMERA;
            _sourceRecipe.Key = Guid.NewGuid();
            _sourceRecipe.Acquisitions = new List<Acquisition>() {
                new Acquisition { Name = "Acquisition1", ExposureTime = 1.0, Filter = EMEFilter.NoFilter },
                new Acquisition { Name = "Acquisition2", ExposureTime = 2.0, Filter = EMEFilter.BandPass450nm50 }
            };
            _destinationRecipe = new DataAccess.Dto.Recipe { /* initialisez ici les propriétés nécessaires */ };
        }
        [TestMethod]
        public void XmlContentResolver_Resolve_ReturnsCorrectXmlContent()
        {
            // Arrange
            var resolver = new XmlContentResolver();

            // Act
            var result = resolver.Resolve(_sourceRecipe, _destinationRecipe, null, null);

            // Assert
            Assert.AreEqual(XML.SerializeToString(_sourceRecipe), result);
        }
        [TestMethod]
        public void RecipeTypeResolver_Resolve_ReturnsCorrectActorType()
        {
            // Arrange
            var resolver = new RecipeTypeResolver();

            // Act
            var result = resolver.Resolve(_sourceRecipe, _destinationRecipe, ActorType.EMERA, null);

            // Assert
            Assert.AreEqual(ActorType.EMERA, result);
        }
        [TestMethod]
        public void EmeRecipe_Mapper_ok()
        {
            // Act                                        
            var emeRecipeVM = _mapper.AutoMap.Map<EMERecipeVM>(_sourceRecipe);

            // Assert
            Assert.IsNotNull(emeRecipeVM);
            Assert.AreEqual(emeRecipeVM.Name, _sourceRecipe.Name);
            Assert.AreEqual(expected: emeRecipeVM.ActorType, _sourceRecipe.ActorType);
            Assert.AreEqual(emeRecipeVM.Key, _sourceRecipe.Key);
            Assert.AreEqual(emeRecipeVM.FileVersion, _sourceRecipe.FileVersion);
            CollectionAssert.AreEqual(emeRecipeVM.Acquisitions, _sourceRecipe.Acquisitions);
        }
    }
}

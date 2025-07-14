using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.DataAccess.SQL;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class ToolService : DataAccesServiceBase, IToolService
    {
        public ToolService(ILogger logger) : base(logger)
        {
        }

        #region Tool

        public Response<int> SetTool(Dto.Tool tool, int? userId)
        {
            return InvokeDataResponse(() =>
            {
                if (tool == null || tool.Name == string.Empty)
                    throw new InvalidOperationException("Bad tool content");

                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var sqlTool = Mapper.Map<SQL.Tool>(tool);
                    if (sqlTool.Id != 0)
                    {
                        _logger.Debug("Update tool " + tool.Name);
                        if (userId.HasValue)
                            DataAccessHelper.LogInDatabase(unitOfWork, userId.Value, Dto.Log.ActionTypeEnum.Edit, Dto.Log.TableTypeEnum.Tool, $"Update tool {sqlTool.Name}  category {sqlTool.ToolCategory} is archived {sqlTool.IsArchived}", _logger);
                        unitOfWork.ToolRepository.Update(sqlTool);
                    }
                    else
                    {
                        _logger.Debug("Add tool " + tool.Name);
                        if (userId.HasValue)
                            DataAccessHelper.LogInDatabase(unitOfWork, userId.Value, Dto.Log.ActionTypeEnum.Add, Dto.Log.TableTypeEnum.Tool, $"Add tool {sqlTool.Name}  category {sqlTool.ToolCategory} is archived {sqlTool.IsArchived}", _logger);
                        unitOfWork.ToolRepository.Add(sqlTool);
                    }

                    unitOfWork.Save();
                    return sqlTool.Id;
                }
            });
        }

        public Response<List<Dto.Tool>> GetAllTools(bool includeChambers = false)
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    if (includeChambers)
                    {
                        return Mapper.Map<List<Dto.Tool>>(unitOfWork.ToolRepository.CreateQuery(false, x => x.Chambers).ToList());
                    }
                    return Mapper.Map<List<Dto.Tool>>(unitOfWork.ToolRepository.CreateQuery().ToList());
                }
            });
        }

        public Response<Dto.Tool> GetTool(int toolKey, bool includeChambers = false)
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    if (includeChambers)
                    {
                        return Mapper.Map<Dto.Tool>(unitOfWork.ToolRepository.CreateQuery(false, x => x.Chambers)
                            .Where(x => x.ToolKey == toolKey)
                            .FirstOrDefault());
                    }
                    return Mapper.Map<Dto.Tool>(unitOfWork.ToolRepository.CreateQuery()
                            .Where(x => x.ToolKey == toolKey)
                            .FirstOrDefault());
                }
            });
        }

        #endregion Tool

        #region Chamber

        public Response<List<Dto.Chamber>> GetAllChambers()
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    return Mapper.Map<List<Dto.Chamber>>(unitOfWork.ChamberRepository.CreateQuery().ToList());
                }
            });
        }

        public Response<int> SetChamber(Dto.Chamber chamber, int? userId)
        {
            return InvokeDataResponse(() =>
            {
                if (chamber == null || chamber.Name == string.Empty)
                    throw new InvalidOperationException("Bad chamber content");

                if (chamber.ToolId == 0)
                    throw new InvalidOperationException("The toolId is missing");

                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var sqlChamber = Mapper.Map<SQL.Chamber>(chamber);
                    if (sqlChamber.Id != 0)
                    {
                        _logger.Debug("Update chamber " + chamber.Name);
                        if (userId.HasValue)
                            DataAccessHelper.LogInDatabase(unitOfWork, userId.Value, Dto.Log.ActionTypeEnum.Edit, Dto.Log.TableTypeEnum.Chamber, $"Update chamber {sqlChamber.Name}  is archived {sqlChamber.IsArchived}",_logger);
                        unitOfWork.ChamberRepository.Update(sqlChamber);
                    }
                    else
                    {
                        _logger.Debug("Add chamber " + chamber.Name);
                        if (userId.HasValue)
                            DataAccessHelper.LogInDatabase(unitOfWork, userId.Value, Dto.Log.ActionTypeEnum.Add, Dto.Log.TableTypeEnum.Chamber, $"Add chamber {sqlChamber.Name}  is archived {sqlChamber.IsArchived}", _logger);
                        unitOfWork.ChamberRepository.Add(sqlChamber);
                    }

                    unitOfWork.Save();
                    return sqlChamber.Id;
                }
            });
        }

        public Response<Dto.Chamber> GetChamber(int chamberId)
        {
            _logger.Debug("Get chamber");
            return InvokeDataResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var chamber = unitOfWork.ChamberRepository.CreateQuery(false, x => x.Tool).SingleOrDefault(x => x.Id == chamberId);
                    return Mapper.Map<Dto.Chamber>(chamber);
                }
            });
        }

        public Response<Dto.Chamber> GetChamberFromKeys(int toolKey, int chamberKey)
        {
            _logger.Debug("Get chamberfromKeys");
            return InvokeDataResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var chamber = unitOfWork.ChamberRepository.CreateQuery(false, x => x.Tool).SingleOrDefault(x => (x.ChamberKey == chamberKey) && (x.Tool.ToolKey == toolKey));
                    return Mapper.Map<Dto.Chamber>(chamber);
                }
            });
        }



        #endregion Chamber

        #region Product and Steps

        public Response<List<Dto.Product>> GetProductAndSteps(bool takeArchived = false)
        {
            _logger.Debug("Get product and steps");
            return InvokeDataResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var products = Mapper.Map<List<Dto.Product>>(unitOfWork.ProductRepository.CreateQuery(false, x => x.Steps, y => y.WaferCategory, z => z.Steps.Select(a => a.Layers)).Where(x => takeArchived || !x.IsArchived).ToList());
                    if (!takeArchived)
                    {
                        foreach (var product in products)
                        {
                            product.Steps = product.Steps.Where(x => !x.IsArchived).ToList();
                        }
                    }
                    return products;
                }
            });
        }

        public Response<int> SetProduct(Dto.Product product, int userId)
        {
            return InvokeDataResponse(() =>
            {
                if (product == null || string.IsNullOrEmpty(product.Name))
                    throw new InvalidOperationException("Bad product content");

                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    if (unitOfWork.ProductRepository.CreateQuery().Any(x => !x.IsArchived && x.Name == product.Name && x.Id != product.Id))
                        throw new InvalidOperationException("Product with the same name already exist");

                    var sqlProduct = Mapper.Map<SQL.Product>(product);
                    sqlProduct.WaferCategory = null;
                    if (sqlProduct.Id != 0)
                    {
                        _logger.Debug("Update product " + product.Name);
                        DataAccessHelper.LogInDatabase(unitOfWork, userId, Dto.Log.ActionTypeEnum.Edit, Dto.Log.TableTypeEnum.Product, $"Update product {sqlProduct.Name}  is archived {sqlProduct.IsArchived}", _logger);
                        unitOfWork.ProductRepository.Update(sqlProduct);
                    }
                    else
                    {
                        _logger.Debug("Add product " + product.Name);
                        DataAccessHelper.LogInDatabase(unitOfWork, userId, Dto.Log.ActionTypeEnum.Add, Dto.Log.TableTypeEnum.Product, $"Add product {sqlProduct.Name}  is archived {sqlProduct.IsArchived}", _logger);
                        unitOfWork.ProductRepository.Add(sqlProduct);
                    }

                    unitOfWork.Save();
                    return sqlProduct.Id;
                }
            });
        }

        public Response<int> SetStep(Dto.Step step, int userId)
        {
            return InvokeDataResponse(() =>
            {
                if (step == null || step.Name == string.Empty)
                    throw new InvalidOperationException("Bad step content");

                if (step.ProductId == 0)
                    throw new InvalidOperationException("The Product Id is missing");

                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    if (unitOfWork.StepRepository.CreateQuery().Any(x => !x.IsArchived && x.Name == step.Name && x.Id != step.Id && x.ProductId == step.ProductId))
                        throw new InvalidOperationException("Step with the same name already exist");

                    var sqlStep = Mapper.Map<SQL.Step>(step);
                    if (sqlStep.Id != 0)
                    {
                        _logger.Debug("Update step " + step.Name);
                        DataAccessHelper.LogInDatabase(unitOfWork, userId, Dto.Log.ActionTypeEnum.Edit, Dto.Log.TableTypeEnum.Step, $"Update step {sqlStep.Name}  is archived {sqlStep.IsArchived}", _logger);
                        UpdateLayers(sqlStep, unitOfWork);
                        sqlStep.Layers.Clear();
                        unitOfWork.StepRepository.Update(sqlStep);
                    }
                    else
                    {
                        _logger.Debug("Add step " + step.Name);
                        DataAccessHelper.LogInDatabase(unitOfWork, userId, Dto.Log.ActionTypeEnum.Add, Dto.Log.TableTypeEnum.Step, $"Add step {sqlStep.Name}  is archived {sqlStep.IsArchived}", _logger);
                        unitOfWork.StepRepository.Add(sqlStep);
                    }

                    unitOfWork.Save();
                    return sqlStep.Id;
                }
            });
        }

        private void UpdateLayers(SQL.Step sqlStep, UnitOfWorkUnity unitOfWork)
        {
            // Add/Update layers
            foreach (var layer in sqlStep.Layers)
            {
                var sqlLayer = Mapper.Map<SQL.Layer>(layer);
                sqlLayer.Step = null;
                if (layer.Id != 0)
                {
                    _logger.Debug($"Update layer {layer.Name}");
                    unitOfWork.LayerRepository.Update(sqlLayer);
                }
                else
                {
                    _logger.Debug($"Add layer {layer.Name}");
                    unitOfWork.LayerRepository.Add(sqlLayer);
                }
            }
            unitOfWork.Save();

            var stepInDb = unitOfWork.StepRepository.CreateQuery(false, x => x.Layers).Where(x => x.Id == sqlStep.Id).First();
            // Remove layers
            var layerToRemove = new List<SQL.Layer>();
            foreach (var layerInDb in stepInDb.Layers)
            {
                if (!sqlStep.Layers.Any(x => x.Id == layerInDb.Id))
                {
                    layerToRemove.Add(layerInDb);
                    _logger.Debug($"Remove layer {layerInDb.Name}");
                }
            }
            unitOfWork.LayerRepository.RemoveRange(layerToRemove);
            unitOfWork.Save();
        }

        public Response<Dto.Step> GetStep(int stepId)
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    _logger.Debug("Get step " + stepId);
                    var step = unitOfWork.StepRepository.CreateQuery(false, x => x.Layers).Single(x => x.Id == stepId);
                    return Mapper.Map<Dto.Step>(step);
                }
            });
        }

        public Response<VoidResult> ArchiveAStep(int id, int userId)
        {
            _logger.Debug($"Archive step {id} by userId {userId}");
            return InvokeVoidResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    ArchiveAStep(id, userId, unitOfWork);
                    unitOfWork.Save();
                }
            });
        }

        private void ArchiveAStep(int id, int userId, UnitOfWorkUnity unitOfWork)
        {
            var step = unitOfWork.StepRepository.CreateQuery(true, x => x.Recipes, x => x.Dataflows).Single(x => x.Id == id);
            step.IsArchived = true;

            DataAccessHelper.LogInDatabase(unitOfWork,
               userId,
               Dto.Log.ActionTypeEnum.Remove,
               Dto.Log.TableTypeEnum.Step,
               $"Archive step {id}- {step.Name}", _logger);

            foreach (var dataflow in step.Dataflows)
            {
                DataAccessHelper.ArchiveAllVerionsOfDataflow(dataflow.KeyForAllVersion, userId, unitOfWork, _logger);
            }

            foreach (var recipe in step.Recipes)
            {
                DataAccessHelper.ArchiveAllVerionsOfRecipe(recipe.KeyForAllVersion, userId, unitOfWork, _logger, false);
            }
        }

        public Response<VoidResult> RestoreAStep(int id, int userId)
        {
            _logger.Debug($"Restore acrhived step {id} by userId {userId}");
            return InvokeVoidResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var step = unitOfWork.StepRepository.CreateQuery(true).Single(x => x.Id == id && x.IsArchived == true);
                    step.IsArchived = false;

                    DataAccessHelper.LogInDatabase(unitOfWork,
                       userId,
                       Dto.Log.ActionTypeEnum.Restore,
                       Dto.Log.TableTypeEnum.Step,
                       $"Restore archived step {id}- {step.Name}", _logger);

                    unitOfWork.Save();
                }
            });
        }

        public Response<VoidResult> ArchiveAProduct(int id, int userId)
        {
            _logger.Debug($"Archive product {id} by userId {userId}");
            return InvokeVoidResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var product = unitOfWork.ProductRepository.CreateQuery(true, x=>x.Steps).Single(x => x.Id == id);
                    product.IsArchived = true;

                    DataAccessHelper.LogInDatabase(unitOfWork,
                       userId,
                       Dto.Log.ActionTypeEnum.Remove,
                       Dto.Log.TableTypeEnum.Product,
                       $"Archive product {id} {product.Name}", _logger);

                    foreach (var step in product.Steps)
                    {
                        ArchiveAStep(step.Id, userId, unitOfWork);
                    }

                    unitOfWork.Save();
                }
            });
        }

        public Response<VoidResult> RestoreAProduct(int id, int userId)
        {
            _logger.Debug($"Restore archived product {id} by userId {userId}");
            return InvokeVoidResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var product = unitOfWork.ProductRepository.CreateQuery(true).Single(x => x.Id == id && x.IsArchived == true);
                    product.IsArchived = false;

                    DataAccessHelper.LogInDatabase(unitOfWork,
                       userId,
                       Dto.Log.ActionTypeEnum.Restore,
                       Dto.Log.TableTypeEnum.Product,
                       $"restore archived product {id} {product.Name}", _logger);

                    unitOfWork.Save();
                }
            });
        }

        public Response<List<Dto.WaferCategory>> GetWaferCategories()
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var waferCategories = Mapper.Map<List<Dto.WaferCategory>>(unitOfWork.WaferCategoryRepository.CreateQuery().ToList());
                    foreach (var waferCategory in waferCategories)
                    {
                        waferCategory.DimentionalCharacteristic = (WaferDimensionalCharacteristic)Buisness.Deserialize<WaferDimensionalCharacteristic>(waferCategory.XmlContent);
                    }
                    return waferCategories;
                }
            });
        }

        public Response<int> SetWaferCategory(Dto.WaferCategory waferCategory, int userId)
        {
            return InvokeDataResponse(() =>
            {
                if (waferCategory.DimentionalCharacteristic == null)
                    throw new InvalidOperationException("DimentionalCharacteristic is missing");

                waferCategory.XmlContent = Buisness.Serialize(waferCategory.DimentionalCharacteristic);

                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var sqlWaferCategory = Mapper.Map<SQL.WaferCategory>(waferCategory);
                    if (sqlWaferCategory.Id != 0)
                    {
                        _logger.Debug("Update wafer category " + waferCategory.Name);
                        DataAccessHelper.LogInDatabase(unitOfWork, userId, Dto.Log.ActionTypeEnum.Edit, Dto.Log.TableTypeEnum.WaferCategory, $"Update wafer category {sqlWaferCategory.Name}", _logger);
                        unitOfWork.WaferCategoryRepository.Update(sqlWaferCategory);
                    }
                    else
                    {
                        _logger.Debug("Add wafer category " + waferCategory.Name);
                        DataAccessHelper.LogInDatabase(unitOfWork, userId, Dto.Log.ActionTypeEnum.Add, Dto.Log.TableTypeEnum.WaferCategory, $"Add wafer category {sqlWaferCategory.Name}", _logger);
                        unitOfWork.WaferCategoryRepository.Add(sqlWaferCategory);
                    }

                    unitOfWork.Save();
                    return sqlWaferCategory.Id;
                }
            });
        }

        public Response<List<Dto.Material>> GetMaterials(bool takeArchived = false)
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var materials = Mapper.Map<List<Dto.Material>>(unitOfWork.MaterialRepository.CreateQuery().Where(x => takeArchived || !x.IsArchived).ToList());
                    foreach (var material in materials)
                    {
                        material.Characteristic = (MaterialCharacteristic)Buisness.Deserialize<MaterialCharacteristic>(material.XmlContent);
                    }
                    return materials;
                }
            });
        }

        public Response<int> SetMaterial(Dto.Material material, int userId)
        {
            return InvokeDataResponse(() =>
            {
                if (material.Characteristic == null)
                    throw new InvalidOperationException("Material characteristic is missing");

                material.XmlContent = Buisness.Serialize(material.Characteristic);

                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var sqlMaterial = Mapper.Map<SQL.Material>(material);
                    if (sqlMaterial.Id != 0)
                    {
                        _logger.Debug("Update material " + material.Name);
                        DataAccessHelper.LogInDatabase(unitOfWork, userId, Dto.Log.ActionTypeEnum.Edit, Dto.Log.TableTypeEnum.Material, $"Update material {sqlMaterial.Name}", _logger);
                        unitOfWork.MaterialRepository.Update(sqlMaterial);
                    }
                    else
                    {
                        _logger.Debug("Add material " + material.Name);
                        DataAccessHelper.LogInDatabase(unitOfWork, userId, Dto.Log.ActionTypeEnum.Add, Dto.Log.TableTypeEnum.Material, $"Add material {sqlMaterial.Name}", _logger);
                        unitOfWork.MaterialRepository.Add(sqlMaterial);
                    }

                    unitOfWork.Save();
                    return sqlMaterial.Id;
                }
            });
        }

        public Response<VoidResult> ArchiveMaterial(int materialId, int userId)
        {
            _logger.Debug($"Archive material {materialId} by userId {userId}");
            return InvokeVoidResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var material = unitOfWork.MaterialRepository.CreateQuery(true).Single(x => x.Id == materialId);
                    material.IsArchived = true;

                    DataAccessHelper.LogInDatabase(unitOfWork,
                       userId,
                       Dto.Log.ActionTypeEnum.Remove,
                       Dto.Log.TableTypeEnum.Material,
                       $"Archive material {materialId} {material.Name}", _logger);

                    unitOfWork.Save();
                }
            });
        }

        public Response<VoidResult> RestoreMaterial(int materialId, int userId)
        {
            _logger.Debug($"Restore archived material {materialId} by userId {userId}");
            return InvokeVoidResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var material = unitOfWork.MaterialRepository.CreateQuery(true).Single(x => x.Id == materialId && x.IsArchived == true);
                    material.IsArchived = false;

                    DataAccessHelper.LogInDatabase(unitOfWork,
                       userId,
                       Dto.Log.ActionTypeEnum.Restore,
                       Dto.Log.TableTypeEnum.Material,
                       $"Restore archived material {materialId} {material.Name}", _logger);

                    unitOfWork.Save();
                }
            });
        }

        #endregion Product and Steps

        #region Vid

        public Response<int> FirstLowerFreeVid()
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    int freeId = 0;
                    var sqlVid = unitOfWork.VidRepository.CreateQuery().OrderBy(x => x.Id).FirstOrDefault();
                    if(sqlVid != null)
                        freeId = sqlVid.Id - 1;

                    if (freeId <= 0)
                    {
                        // No room left between max id and 0 --> search for upper limit 
                        sqlVid = unitOfWork.VidRepository.CreateQuery().OrderByDescending(x => x.Id).FirstOrDefault();
                        if (sqlVid != null)
                            freeId = sqlVid.Id + 1;
                        else
                            freeId = 1; // DB should be empty of vid
                    }
                    return freeId;
                }
            });
        }

        public Response<int> FirstUpperFreeVid()
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    int freeId = 0;
                    var sqlVid = unitOfWork.VidRepository.CreateQuery().OrderByDescending(x => x.Id).FirstOrDefault();
                    if (sqlVid != null)
                        freeId = sqlVid.Id + 1;
                    else
                        freeId = 1; // DB should be empty of vid

                    return freeId;
                }
            });
        }

        public Response<int> SetVid(Dto.Vid vid, int userId)
        {
            return InvokeDataResponse(() =>
            {
                if (vid == null || string.IsNullOrEmpty(vid.Label))
                    throw new InvalidOperationException("Bad Vid content, null or empty vid label");

                if (vid.Id == 0)
                    throw new InvalidOperationException("The VID Id is missing");

                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    bool updateExistingVid = false;
                    if (unitOfWork.VidRepository.CreateQuery().Any(x => x.Id == vid.Id))
                        updateExistingVid = true;

                    if (unitOfWork.VidRepository.CreateQuery().Any(x => x.Label == vid.Label && x.Id != vid.Id))
                        throw new InvalidOperationException($"Vid with the same Label already exist : <{vid.Label}>");

                    var sqlVid = Mapper.Map<SQL.Vid>(vid);
                    if (updateExistingVid)
                    {
                        _logger.Debug($"Update existing Vid {sqlVid.Id}:{vid.Label}");
                        DataAccessHelper.LogInDatabase(unitOfWork, userId, Dto.Log.ActionTypeEnum.Edit, Dto.Log.TableTypeEnum.Vid, $"Update Vid <{sqlVid.Id}:{sqlVid.Label}> => {vid.Label}", _logger);
                        unitOfWork.VidRepository.Update(sqlVid);
                    }
                    else
                    {
                        _logger.Debug($"Add New Vid <{sqlVid.Id}:{sqlVid.Label}>");
                        DataAccessHelper.LogInDatabase(unitOfWork, userId, Dto.Log.ActionTypeEnum.Add, Dto.Log.TableTypeEnum.Vid, $"Add New Vid <{sqlVid.Id}:{sqlVid.Label}>", _logger);
                        unitOfWork.VidRepository.Add(sqlVid);
                    }
                    unitOfWork.Save();
                    return sqlVid.Id;
                }
            });
        }

        public Response<List<Dto.Vid>> GetAllVid()
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    return Mapper.Map<List<Dto.Vid>>(unitOfWork.VidRepository.CreateQuery().ToList());            
                }
            });
        }

        public Response<VoidResult> SetAllVid(List<Dto.Vid> vid, int userId)
        {
            _logger.Debug($"Set All Vids by userId {userId}");
            return InvokeVoidResponse(messagesContainer =>
            {

            });
        }

        public Response<VoidResult> RemoveVid(Dto.Vid vid, int userId)
        {
            _logger.Debug($"Remove Vid <{vid?.Id}:{vid?.Label}> by userId {userId}");
            return InvokeVoidResponse(messagesContainer =>
            {
                if (vid == null || string.IsNullOrEmpty(vid.Label))
                    throw new InvalidOperationException("remove failure: Bad Vid content, null or empty vid label");

                if (vid.Id == 0)
                    throw new InvalidOperationException("remove failure: the VID Id is missing");

                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    unitOfWork.VidRepository.RemoveById(vid.Id);
                    DataAccessHelper.LogInDatabase(unitOfWork, userId, Dto.Log.ActionTypeEnum.Remove, Dto.Log.TableTypeEnum.Vid, $"Remove Vid <{vid.Id}:{vid.Label}>", _logger);
                    unitOfWork.Save();
                }
            });
        }

      

        #endregion
    }
}

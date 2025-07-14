using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.DataAccess.Base;
using UnitySC.DataAccess.Base.Implementation;
using UnitySC.DataAccess.SQL.ModelSQL.LocalSQL;

namespace UnitySC.DataAccess.SQL
{
    /// <summary>
    /// Partial class : Other part behind UnitOfWork.tt
    /// </summary>
    public partial class UnitOfWorkUnity : UnitOfWorkBase, IDisposable
    {
        public UnitOfWorkUnity(string connectionString) : this(new InspectionEntities(connectionString)) //"metadata=res://*/UnityControl.csdl|res://*/UnityControl.ssdl|res://*/UnityControl.msl;provider=System.Data.SqlClient;provider connection string='data source=10.100.20.19\\SQLUNITYSC;initial catalog=UnityControlv8FDS2;persist security info=True;user id=admin;password=inspection;MultipleActiveResultSets=True;App=EntityFramework'"))
        {
        }

        public UnitOfWorkUnity(InspectionEntities context) : base(context)
        {
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        /// <summary>
        /// Cette méthode appelle la procedure stockée dbo.sp_GetJobResults
        /// </summary>
        /// <param name="pTooolId"></param>
        /// <param name="pStartDate"></param>
        /// <param name="pEndDate"></param>
        /// <param name="ProductId"></param>
        /// <param name="pJobId"></param>
        /// <param name="pRecipeId"></param>
        /// <param name="pChamberId"></param>
        /// <param name="pWaferId"></param>
        /// <param name="pWaferState"></param>
        /// <param name="pTop"></param>
        /// <returns></returns> Liste des jobs repondant aux critères de recherche 
        public List<SQL.Job> GetJobsList(SearchParam pParam)
        {
            var toolId = new SqlParameter("@pToolId", SqlDbType.Int)
            {
                Value = pParam.ToolId ?? (object)DBNull.Value
            };
            var dateStart = new SqlParameter("@pStartDate", SqlDbType.DateTime2)
            {
                Value = pParam.StartDate ?? (object)DBNull.Value
            };
            var dateEnd = new SqlParameter("@pEndDate", SqlDbType.DateTime2)
            {
                Value = pParam.EndDate ?? (object)DBNull.Value
            };
            var prodId = new SqlParameter("@pProductId", SqlDbType.Int)
            {
                Value = pParam.ProductId ?? (object)DBNull.Value
            };
            var lotName = new SqlParameter("@pLotName", SqlDbType.NVarChar)
            {
                Value = pParam.LotName ?? (object)DBNull.Value
            };
            var recipeName = new SqlParameter("@pRecipeName", SqlDbType.NVarChar)
            {
                Value = pParam.RecipeName ?? (object)DBNull.Value
            };
            ;
            var actorType = new SqlParameter("@pActorType", SqlDbType.Int)
            {
                Value = pParam.ActorType ?? (object)DBNull.Value
            };
            var ResultState = new SqlParameter("@pResultState", SqlDbType.Int)
            {
                Value = pParam.ResultState ?? (object)DBNull.Value
            };
            var waferName = new SqlParameter("@pWaferName", SqlDbType.NVarChar)
            {
                Value = pParam.WaferName ?? (object)DBNull.Value
            };
            var tag = new SqlParameter("@pTag", SqlDbType.Int)
            {
                Value = pParam.ResultFilterTag ?? (object)DBNull.Value
            };

            var sqlParams = new SqlParameter[] { toolId, dateStart, dateEnd, prodId, lotName, recipeName, actorType, ResultState, waferName, tag };
            var jobsList = _context.Database.SqlQuery<SQL.Job>("EXEC dbo.sp_GetJobResults @pToolId,@pStartDate,@pEndDate,@pProductId,@pLotName,@pRecipeName,@pActorType,@pResultState,@pWaferName,@pTag", sqlParams).ToList();

            return jobsList;
        }

        public void DeleteAllData()
        {
            _context.Database.ExecuteSqlCommand("EXEC dbo.DeleteAllData");
        }

        
    }
}

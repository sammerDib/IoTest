using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace UnitySC.PM.Shared.ADCConfiguration
{
    public class DatabaseDirectAccess
    { 
        // Derniere version de chaque recette
        private const string SqlCommandRecipes = "WITH summary AS ( "
                                              + "SELECT r.Id, r.Name, r.Comment, r.Created, r.CreatorUserId, r.Version, r.DataLoaderTypes, ROW_NUMBER()"
                                              + "OVER(PARTITION BY r.Name "
                                              + "ORDER BY r.Version DESC) AS rk "
                                              + "FROM dbo.Recipe r "
                                              + "WHERE r.IsArchived = 0) "
                                              + "SELECT s.Id, s.Name, s.Comment, s.Created, s.CreatorUserId, s.Version, s.DataLoaderTypes "
                                              + "FROM summary s "
                                              + "INNER JOIN [dbo].[User] ON s.CreatorUserId = [dbo].[User].Id "
                                              + "WHERE s.rk = 1";

        // Liste des wafers et des surfaces associées
        private const string SqlCommandWaferTypes = "SELECT wafer.Id, Name, Diameter, SizeX, SizeY, Comment, Surface.Label as SurfaceName, FlatVerticalX, FlatHorizontalY, Shape "
                                                 + "FROM dbo.WaferType wafer "
                                                 + "INNER JOIN dbo.Surface ON wafer.SurfaceId = dbo.Surface.Id "
                                                 + "WHERE wafer.IsArchived = 0";

        // Obtient le contenu d'une recette
        private const string SqlCommandRecipeContent = "SELECT XmlContent "
                                                    + "FROM dbo.Recipe "
                                                    + "WHERE dbo.Recipe.Id = {0}";
                                                    
        private string _connectionString;

        public DatabaseDirectAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Obtient la derniére version de chaque recette
        /// </summary>
        /// <returns></returns>
        public List<ADCRecipe> GetRecipes()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(SqlCommandRecipes, connection);
                DataTable dataTable = GetDataTable(command);
                return ToList<ADCRecipe>(dataTable);
            }
        }

        /// <summary>
        /// Obtient la liste des types de wafer
        /// </summary>
        /// <returns></returns>
        public List<WaferType> GetWaferTypes()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(SqlCommandWaferTypes, connection);
                DataTable dataTable = GetDataTable(command);
                return ToList<WaferType>(dataTable);
            }
        }

        /// <summary>
        /// Obtient la liste des classes de défauts pour une recette
        /// </summary>
        /// <param name="recipeId"></param>
        /// <returns></returns>
        public List<string> GetDefectClasses(int recipeId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(string.Format(SqlCommandRecipeContent, recipeId), connection);
                DataTable dataTable = GetDataTable(command);
                string xmlContent = (string)dataTable.Rows[0][0];
                return XmlRecipeToDefectClasses(xmlContent);
            }
        }

        /// <summary>
        /// DataTable to listl
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        private static List<T> ToList<T>(DataTable table) where T : class, new()
        {
            List<T> list = new List<T>();

            foreach (DataRow row in table.AsEnumerable())
            {
                T obj = new T();

                foreach (var prop in obj.GetType().GetProperties())
                {
                    try
                    {
                        PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);

                        if (row.Table.Columns.Contains(prop.Name) && row[prop.Name] != System.DBNull.Value)
                        {
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                list.Add(obj);
            }

            return list;
        }

        /// <summary>
        /// Sql command to Datatable
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static DataTable GetDataTable(SqlCommand cmd)
        {
            SqlDataAdapter dataAdapt = new SqlDataAdapter();
            dataAdapt.SelectCommand = cmd;
            DataTable dataTable = new DataTable();
            dataAdapt.Fill(dataTable);
            return dataTable;
        }

        /// <summary>
        /// Récupére les classes de défaut à partir du xml d'une recette
        /// </summary>
        /// <param name="xmlContent">Contenu xml d'une recette</param>
        /// <returns> List des défauts</returns>
        private static List<string> XmlRecipeToDefectClasses(string xmlContent)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            XmlNode root = doc.DocumentElement;
            XmlNodeList xPathRes = root.SelectNodes("/Recipe/Graph//Module[@Name='Classification']/Parameters//Parameter[@Label='DefectClass']/@Value");
            List<string> res = xPathRes.Cast<XmlNode>().Select(x => x.Value).Distinct().ToList();
            return res;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ADCSorting;
using Common;
using System.IO;
using System.Windows.Forms;

namespace Common.Sorting
{
    public class CSearchSortingObject
    {
        Dictionary<String, String> m_DicSortingRecipeFilePathName = new Dictionary<String, String>();
        ADCSorting_Treatment m_ADCSorting;
        Dictionary<String, List<SortingResultsInfo>> m_DicProcessRecipeByLotID = new Dictionary<String, List<SortingResultsInfo>>();

        public CSearchSortingObject()
        {

        }

        public List<String> SortingRecipeList
        {
            get { return m_DicSortingRecipeFilePathName.Keys.ToList(); }
        }

        public String GetSortingRecipeFilePathNameList(String pRecipeName)
        {
            String lSortingRecipeFilePathName = null;
            if (m_DicSortingRecipeFilePathName.TryGetValue(pRecipeName, out lSortingRecipeFilePathName))
            {
                return lSortingRecipeFilePathName;
            }
            else
                return "";
        }

        /// <summary>
        /// Recherche la liste des recipes de sorting + les PM associés
        /// </summary>
        public List<String> SearchRecipeAvailable()
        {
            // Utiliser Sorting.DLL pour obtenir les recipes
            List<String> lRecipeList = GetSortingRecipeList();
            if ((lRecipeList != null) && (lRecipeList.Count > 0))
            {
                m_DicSortingRecipeFilePathName.Clear();
                // Utiliser Sorting.DLL pour obtenir la liste des PM par recipe
                for (int i = 0; i < lRecipeList.Count; i++)
                {
                    String lCurrentRecipeName = Path.GetFileNameWithoutExtension(lRecipeList[i]);
                    m_DicSortingRecipeFilePathName.Add(lCurrentRecipeName, lRecipeList[i]);
                }
                return SortingRecipeList;
            }
            else
                return null;

        }
        /// <summary>
        /// Recherche la liste des LotID de sorting 
        /// </summary>
        public String[] SearchLotIDAvailable(String pRecipeName, String pLotIDRegExp)
        {
            List<String> lList = GetLotIDList(pRecipeName, pLotIDRegExp, CConfiguration.SortingType);
            String[] lResultList = new string[lList.Count];
            lList.CopyTo(lResultList);
            return lResultList;

        }


        public String[] SearchLotIDAvailableGrading(String pLotIDRegExp)
        {
            return SearchLotIDAvailable("", pLotIDRegExp);

        }

        #region Methodes pour interroger la DB de sorting via Sorting.DLL
        /// <summary>
        /// Permet d'interroger la DB de sorting pour obtenir la liste des PM par recette
        /// </summary>
        /// <param name="lCurrentRecipeName"></param>
        /// <returns>Liste des PM pour la recipe en parametre</returns>
        private List<string> GetPMListByRecipe(string pRecipeName)
        {
            try
            {
                // Call Sorting.DLL
                List<String> lPMList = new List<String>();
                if (m_ADCSorting == null) m_ADCSorting = new ADCSorting_Treatment();

                List<SortingResultsInfo> pSortingLotListAvailable = m_ADCSorting.GetAvailableSortingLot_List(true);
                foreach (SortingResultsInfo CurrItem in pSortingLotListAvailable)
                {
                    if ((lPMList.IndexOf(CurrItem.sModuleName) < 0) && (CurrItem.sRecipe == pRecipeName))
                        lPMList.Add(CurrItem.sModuleName);
                }
                return lPMList;

            }
            catch (Exception Ex)
            {
                MessageBox.Show("GetPMListByRecipe in ADC sorting DLL failed in exception : " + Ex.Message + " - " + Ex.StackTrace);
                return new List<String>();
            }
        }
        /// <summary>
        /// Permet d'interroger la DB de sorting pour obtenir la liste des recipes de sorting
        /// </summary>
        /// <returns>Liste des recipes de sorting</returns>
        private List<String> GetSortingRecipeList()
        {
            // Call sorting.DLL
            List<String> lSortingRecipeIDList = new List<String>();

            String[] lSortingRecipeTab = Directory.GetFiles(CConfiguration.SortingRecipesPath);
            if ((lSortingRecipeTab != null) && (lSortingRecipeTab.Length > 0))
                lSortingRecipeIDList.AddRange(lSortingRecipeTab);
            return lSortingRecipeIDList;

        }
        /// <summary>
        /// Permet d'interroger la DB de sorting pour obtenir la liste des LotID de sorting selon une recipe et un module
        /// </summary>
        /// <param name="ModeUsed">Get mode used ("GRADING") </param>  
        /// <returns>Liste des LotID de sorting</returns>
        private List<String> GetLotIDList(String pRecipeName, String pLotIDRegExp, String ModeUsed)
        {
            try
            {
                List<String> lLotIDList = new List<String>();
                Regex CheckLotID = null;
                //if (pLotIDRegExp != "")
                //    CheckLotID = new Regex(pLotIDRegExp);
                if (m_ADCSorting == null) m_ADCSorting = new ADCSorting_Treatment();

                List<SortingResultsInfo> pSortingLotListAvailable = null;
                m_DicProcessRecipeByLotID.Clear();
                if (ModeUsed == "GRADING")
                {
                    pSortingLotListAvailable = new List<SortingResultsInfo>();
                    List<String> lList = new List<String>();
                    m_ADCSorting.GetLotIDListGradingFile(ref lList);
                    for (int i = 0; i < lList.Count; i++)
                    {
                        List<String> lListRecipeLotID = new List<string>();
                        String lLotIDTested = lList[i];
                        m_ADCSorting.GetRecipeIDListGradingFile(lLotIDTested, ref lListRecipeLotID);
                        String lRecipeFound = String.Empty;
                        foreach (var recipeName in lListRecipeLotID)
                        {
                            SortingResultsInfo lnewSortInfo = new SortingResultsInfo();
                            lnewSortInfo.sLotID = lLotIDTested;
                            lnewSortInfo.nDBModID = i;
                            lnewSortInfo.sFileName = String.Empty;
                            lnewSortInfo.sFullFilePath = String.Empty;
                            lnewSortInfo.sModuleName = "Unknown";
                            lnewSortInfo.sRecipe = recipeName;
                            lnewSortInfo.dtDate = DateTime.Now;
                            pSortingLotListAvailable.Add(lnewSortInfo);
                        }
                        lLotIDList.Add(lLotIDTested);
                        m_DicProcessRecipeByLotID.Add(lLotIDTested, pSortingLotListAvailable);
                        pSortingLotListAvailable = new List<SortingResultsInfo>();
                    }
                }
                else
                {
                    pSortingLotListAvailable = m_ADCSorting.GetAvailableSortingLot_List(false);

                    foreach (SortingResultsInfo CurrItem in pSortingLotListAvailable)
                    {
                        if ((lLotIDList.IndexOf(CurrItem.sLotID) < 0) && ((CheckLotID == null) || (CheckLotID.IsMatch(CurrItem.sLotID))))
                        {
                            lLotIDList.Add(CurrItem.sLotID);
                            m_DicProcessRecipeByLotID.Add(CurrItem.sLotID, new List<SortingResultsInfo>() { CurrItem });
                        }
                    }
                }
                return lLotIDList;
            }
            catch (Exception Ex)
            {
                MessageBox.Show("GetLotIDList in ADC sorting DLL failed in exception : " + Ex.Message + " - " + Ex.StackTrace);
                return new List<String>();
            }        
        }

        public List<String> GetProcessRecipeList(String pLotID)
        {
            try
            {
                if (m_ADCSorting == null)
                    m_ADCSorting = new ADCSorting_Treatment();
                List<String> lProcessRecipeList = new List<String>();

                foreach (KeyValuePair<string, List<SortingResultsInfo>> CurrItem in m_DicProcessRecipeByLotID)
                {
                    if (pLotID == CurrItem.Key)
                    {
                        foreach (SortingResultsInfo CurrValue in CurrItem.Value)
                        {
                            lProcessRecipeList.Add(CurrValue.sRecipe);
                        }
                    }
                }
                return lProcessRecipeList;
            }
            catch (Exception Ex)
            {
                MessageBox.Show("GetProcessRecipeList in ADC sorting DLL failed in exception : " + Ex.Message + " - " + Ex.StackTrace);
                return new List<String>();
            }

        }
        public void GetSortingGradingWaferList(String pLotID, String pProcessRecipe, ref List<ADCSorting.ADCSorting_Treatment.CWaferStatus> pWaferStatusList)
        {
            try
            {
                m_ADCSorting.SortingUsedGrading(pLotID, pProcessRecipe, ref pWaferStatusList);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("GetSortingGradingWaferList in ADC sorting DLL failed in exception : " + Ex.Message + " - " + Ex.StackTrace);
            }
        }
        public void GetSortingWaferList(String pLotID, String pSortingRecipe, String pProcessRecipe, ref List<ADCSorting.ADCSorting_Treatment.CWaferStatus> pWaferStatusList)
        {
            try
            {
                m_ADCSorting.SortingUsedModule(pLotID, pProcessRecipe, GetSortingRecipeFilePathNameList(pSortingRecipe), ref pWaferStatusList);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("GetSortingWaferList in ADC sorting DLL failed in exception : " + Ex.Message + " - " + Ex.StackTrace);
            }
        } 

        public void DeleteSortingLot(String pLotID, String pProcessRecipe)
        {
            try 
            {
                if (m_ADCSorting == null) m_ADCSorting = new ADCSorting_Treatment();

                if (CConfiguration.SortingType == "GRADING")
                    m_ADCSorting.DeleteGradingResult(pLotID, pProcessRecipe);
                else
                    m_ADCSorting.DeleteSortingResult(pLotID, pProcessRecipe);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("DeleteSortingLot in ADC sorting DLL failed in exception : " + Ex.Message + " - " + Ex.StackTrace);
            }
        }

        #endregion        


    }
}

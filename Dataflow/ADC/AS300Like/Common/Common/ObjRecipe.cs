using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using Common;
using System.Globalization;
using System.IO;
using Database.Robot;
using IniFileTools;
using Common.Communication;
using Common.SocketMessage;
using Common.EException;
using static Common.Win32Tools;

namespace E95GUIFrameWork.InfoPanel
{
    public enum FieldUsed
    {
        UNKNOWN_FIELD,
        BOOLEAN,
        TEXT_FIELD,
        NUMERIC_FIELD,
        NUMERIC_DECIMAL_FIELD,
        NUMERIC_2DECIMAL_FIELD,
        NUMERIC_3DECIMAL_FIELD,
        NUMERIC_4DECIMAL_FIELD,
        NUMERIC_5DECIMAL_FIELD,
        COMBOBOX_FIELD,
        MULTISELECT_LBOX_FIELD,
        NAMEFILE_COMBOBOX_FIELD,
        SEQUENCE_FIELD,
        XML_FIELD,
        LINKWITH_DATA_FIELD,
        LINKWITH_BOOLEAN_FIELD,
        NUMERIC_COMPAREINF_FIELD,
        NUMERIC_DECIMAL_COMPAREINF_FIELD,
        NUMERIC_2DECIMAL_COMPAREINF_FIELD,
        NUMERIC_3DECIMAL_COMPAREINF_FIELD,
        NUMERIC_4DECIMAL_COMPAREINF_FIELD,
        NUMERIC_5DECIMAL_COMPAREINF_FIELD,
        NUMERIC_COMPARESUP_FIELD,
        NUMERIC_DECIMAL_COMPARESUP_FIELD,
        NUMERIC_2DECIMAL_COMPARESUP_FIELD,
        NUMERIC_3DECIMAL_COMPARESUP_FIELD,
        NUMERIC_4DECIMAL_COMPARESUP_FIELD,
        NUMERIC_5DECIMAL_COMPARESUP_FIELD,
        DATABASE_LIST_FIELD,
        // Starting here are "details" fields. Each of them is associated to a data class (here: LsAcquisitionParams), initialized from the following [DETAILS] section.
        DETAILS_LsAcquisitionParams,
        DETAILS_LsTargetValues
    }

    public static class FieldUsedExt
    {
        /// <summary>
        /// Checks if this is a "details" field.
        /// </summary>
        public static bool IsDetailsField(this FieldUsed this_)
        {
            return this_ >= FieldUsed.DETAILS_LsAcquisitionParams;
        }

        ///// <summary>
        ///// Creates a new "details" data object.
        ///// </summary>
        public static RobotDetailsData InstantiateDetails(this FieldUsed this_)
        {
            switch (this_)
            {
                case FieldUsed.DETAILS_LsAcquisitionParams:
                    return new LsAcquisitionParams();

                case FieldUsed.DETAILS_LsTargetValues:
                    return CRecipeLScalibrationData.NewAllNegative();

                default:
                    throw new IOException(this_.ToString() + ": not a details fieldd!");
            }
        }
    }

    public enum XMLFieldTypes
    {
        ftNone = 0,
        ftFringesList,
        ftImagesList,
        ftFilterWheel
    }

    public enum enumDATABASERequest
    {
        rstNone = 0,
        GetWaferTypes,
        GetRecipes // ADC Recipes from Database
    }

    public class RecipeItems
    {
        public RecipeItems(ObjRecipe recipe, string sectionName)
        {
            Recipe = recipe;
            SectionName = sectionName;
        }
        public readonly ObjRecipe Recipe;
        public string SectionName;

        public String KeyName;
        public Object Value;
        public Decimal LimitSup, LimitInf;
        public String Info, Unit;
        public FieldUsed FieldUsed;
        public string[] ComboItems;
        public String ImageFileName;

        /// <summary>
        /// Checks if this is a "details" item.
        /// In that case, Value has the associated data type.
        /// </summary>
        public bool IsDetails => FieldUsed.IsDetailsField();
    }

    public class RecipeSection
    {
        public String SectionName;
        public List<RecipeItems> RecipeItems;
        public List<string> RecipeItemsName;

        /// <summary>
        /// As list of name / value pairs.
        /// </summary>
        public IList<KeyValuePair<string, string>> StringsList
        {
            get
            {
                KeyValuePair<string, string>[] ret = new KeyValuePair<string, string>[RecipeItems.Count];
                for (Int32 i = -1; (++i) < RecipeItems.Count;)
                {
                    RecipeItems item = RecipeItems[i];
                    ret[i] = new KeyValuePair<string, string>(item.KeyName, item.Value.ToString()); // [sde] TODO what happens in case of culture change?
                }

                return ret;
            }
        }

        /// <summary>
        /// Checks if this is a "details" section (containing a signe "details" item).
        /// </summary>
        public bool IsDetails => (RecipeItems.Count == 1) && (RecipeItems[0].IsDetails);
    }

    public enum ItemType
    {
        UNKNOWN_TYPE,
        VALUE_ITEM,
        INFO_ITEM,
        LIMITESUP_ITEM,
        LIMITEINF_ITEM,
        UNIT_ITEM,
        FIELDUSED_ITEM,
        IMAGE_ITEM
    };
    public struct CompareKeySectionAndKeyField
    {
        public String CompareSetion;
        public String CompareKeyField;
        public CompareKeySectionAndKeyField(String pSection, String pKeyField)
        {
            CompareSetion = pSection;
            CompareKeyField = pKeyField;
        }
    }
    public class ObjRecipe
    {
        const string INI_FILES_DIRECTORY = "C:\\CIMConnectProjects\\Equipment1\\Projects\\";

        public List<RecipeSection> m_ListSection = new List<RecipeSection>();
        public List<String> m_ListSectionName = new List<string>();
        private String m_IniFileName;

        List<String> m_LinkWithKeyNameList = new List<string>();
        String m_LinkWithBooleanTypeField1; // Define which type of logic used - COMPLEMENT, OR, XOR...
        String m_LinkWithBooleanTypeField2; // Define which type of logic used - COMPLEMENT, OR, XOR...
        List<String> m_LinkWithDataField1 = new List<string>();
        List<String> m_LinkWithDataField2 = new List<string>();
        public Dictionary<CompareKeySectionAndKeyField, String> m_CompareInfSectionDataField = new Dictionary<CompareKeySectionAndKeyField, String>();
        public Dictionary<CompareKeySectionAndKeyField, String> m_CompareSupSectionDataField = new Dictionary<CompareKeySectionAndKeyField, String>();
        public object[] m_DataSetADCWaferTypes;
        public object[] m_DataSetADCRecipes;
        static String m_DatabaseConnexionString = String.Empty;  // data source=172.20.25.2\SQLEXPRESS;initial catalog=Inspection;user id=InspectionUser;password=UnitySql;MultipleActiveResultSets=True;App=EntityFramework&quot;
        String m_ADCFrontMachineName;
        bool m_bADCFrontMachineEnabled;
        String m_ADCBackMachineName;
        bool m_bADCBackMachineEnabled;
        String m_ADCDarkfieldMachineName;
        bool m_bADCDarkfieldMachineEnabled;
        String m_ADCBrightFiel1dMachineName;
        String m_ADCBrightFiel2dMachineName;
        bool m_bADCBrightField1MachineEnabled;
        bool m_bADCBrightField2MachineEnabled;
        bool m_bADCLightSpeedMachineEnabled;
        String m_ADCLightSpeedMachineName;

        String m_ADCEdgeMachineName;
        bool m_bADCEdgeMachineEnabled;
        String m_ModuleDarkfieldMachineName;
        bool m_bModuleDarkfieldEnabled;
        Dictionary<String, List<String>> m_ModulePMMachineName = new Dictionary<String, List<String>>();
        bool[] m_bModuleBrightFieldEnabled = new bool[4];
        public String[] m_OCRRecipesFrontList;
        public String[] m_OCRRecipesBackList;

        public ObjRecipe(String pModuleDarkfieldMachineName)
        {
            INIfile MyIni = new INIfile(CValues.PMALTASIGHT_INI);

            m_ADCFrontMachineName = MyIni["ADC FRONTSIDE", "ServerName"];
            m_ADCBackMachineName = MyIni["ADC BACKSIDE", "ServerName"];
            m_ADCDarkfieldMachineName = MyIni["ADC DARKVIEW", "ServerName"];
            m_ADCBrightFiel1dMachineName = MyIni["ADC BRIGHTFIELD 1", "ServerName"];
            m_ADCBrightFiel2dMachineName = MyIni["ADC BRIGHTFIELD 2", "ServerName"];
            m_ADCEdgeMachineName = MyIni["ADC EDGE", "ServerName"];
            m_ADCLightSpeedMachineName = MyIni["ADC LIGHTSPEED", "ServerName"];

            m_ModuleDarkfieldMachineName = pModuleDarkfieldMachineName;

            if (m_ADCFrontMachineName == null)
                m_ADCFrontMachineName = "";
            if (m_ADCBackMachineName == null)
                m_ADCBackMachineName = "";
            if (m_ADCDarkfieldMachineName == null)
                m_ADCDarkfieldMachineName = "";
            if (m_ADCBrightFiel1dMachineName == null)
                m_ADCBrightFiel1dMachineName = "";
            if (m_ADCBrightFiel2dMachineName == null)
                m_ADCBrightFiel2dMachineName = "";
            if (m_ModuleDarkfieldMachineName == null)
                m_ModuleDarkfieldMachineName = "";
            if (m_ADCEdgeMachineName == null)
                m_ADCEdgeMachineName = "";
            if (m_ADCLightSpeedMachineName == null)
                m_ADCLightSpeedMachineName = "";
            int iADCFrontMachineEnabled = Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC FRONTSIDE", "Enabled", 0);
            m_bADCFrontMachineEnabled = (iADCFrontMachineEnabled == 1);
            int iADCBackMachineEnabled = Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC BACKSIDE", "Enabled", 0);
            m_bADCBackMachineEnabled = (iADCBackMachineEnabled == 1);
            int iADCDarkfieldMachineEnabled = Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC DARKVIEW", "Enabled", 0);
            m_bADCDarkfieldMachineEnabled = (iADCDarkfieldMachineEnabled == 1);
            int iModuleDarkfieldEnabled = Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC DARKVIEW", "Enabled", 0);
            m_bModuleDarkfieldEnabled = (iModuleDarkfieldEnabled == 1);
            int iADCEdgeMachineEnabled = Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC EDGE", "Enabled", 0);
            m_bADCEdgeMachineEnabled = (iADCEdgeMachineEnabled == 1);
            int iADCBrighfieldMachineEnabled = Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 1", "Enabled", 0);
            m_bADCBrightField1MachineEnabled = (iADCBrighfieldMachineEnabled == 1);
            iADCBrighfieldMachineEnabled = Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC BRIGHTFIELD 2", "Enabled", 0);
            m_bADCBrightField2MachineEnabled = (iADCBrighfieldMachineEnabled == 1);
            int iADCLightSpeedMachineEnabled = Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "ADC LIGHTSPEED", "Enabled", 0);
            m_bADCLightSpeedMachineEnabled = (iADCLightSpeedMachineEnabled == 1);

            for (int i = 0; i < 4; i++)
            {
                try
                {
                    String lPMType = Win32Tools.GetIniFileString(INI_FILES_DIRECTORY + "CxExpertPM_" + (i + 1).ToString() + ".ini", "Expert PM", "Type", "Unknown");
                    List<String> lMachineNameList;
                    String lMachineName = Win32Tools.GetIniFileString(INI_FILES_DIRECTORY + "CxExpertPM_" + (i + 1).ToString() + ".ini", "Module Parameters", "ServerName", "Unknown");
                    String lFunction = Win32Tools.GetIniFileString(INI_FILES_DIRECTORY + "CxExpertPM_" + (i + 1).ToString() + ".ini", "Module Parameters", "Function", "NotFound");
                    if (lFunction == "NotFound")
                        lFunction = "";
                    if (lMachineName != "Unknown")
                    {
                        if (m_ModulePMMachineName.TryGetValue(lPMType + lFunction, out lMachineNameList))
                        {
                            lMachineNameList.Add(lMachineName);
                        }
                        else
                        {
                            lMachineNameList = new List<String>();
                            lMachineNameList.Add(lMachineName);
                            m_ModulePMMachineName.Add(lPMType + lFunction, lMachineNameList);
                        }
                    }
                }
                catch
                {
                }
            }

            if ((m_DataSetADCWaferTypes == null) || (m_DatabaseConnexionString == String.Empty))
            {
                //Get m_DatabaseConnexionString
                String RegistryKey = @"SOFTWARE\WOW6432Node\Cimetrix\ADC DB";
                String RegistryKeyChain = "ConnectionString";
                m_DatabaseConnexionString = Win32Tools.RegistryGetValueSZ(RegistryKey, RegistryKeyChain);
            }

        }

        public int LoadFromIniFile(string FilePath, string DataFilePath)
        {
            String sLine;
            String sCurrentSectionName = "";
            TreeNode ChildNode = new TreeNode();
            TreeNode SectionNode = new TreeNode();
            IniFileName = FilePath;
            StreamReader sr = new StreamReader(FilePath);
            try
            {
                while (!sr.EndOfStream)
                {
                    sLine = sr.ReadLine(); // Recupere la ligne courante
                    if ((sLine.Length != 0) && (sLine[0] != '#'))                 // SI pas ligne de commentaire ou d'information sur une cléf ALORS
                    {
                        sLine = sLine.Trim();
                        sLine = sLine.Replace("\t", "");
                        if (sLine[0] == '[') // SI ligne de section ALORS
                        {
                            sLine = sLine.Remove(0, 1);              // Enleve '['
                            sLine = sLine.Remove(sLine.Length - 1, 1); // Enleve ']'
                            sCurrentSectionName = sLine;
                            AddSection(sCurrentSectionName);  // Ajout de la section du fichier ini                        
                        }
                        else if (sLine.Contains("=")) // SI ligne de clef ALORS
                        {
                            String[] sTab = sLine.Split('='); // sTab[0] = Clé; sTab[1] = Value 

                            // Recupere la clef
                            Int32 IndexSectionFound, IndexKeyFound;
                            if (!IsKeyExist(sCurrentSectionName, sTab[0], out IndexSectionFound, out IndexKeyFound))
                            {
                                if (sTab[1].ToUpper().Contains("TRUE") || sTab[1].ToUpper().Contains("FALSE")) // SI type boolean ALORS
                                {                                   
                                    bool bValue = Convert.ToBoolean(sTab[1]);
                                    AddItem(sCurrentSectionName, sTab[0], bValue); // Ajout item de type booleen dans objet recipe                                    
                                }
                                else
                                    AddItem(sCurrentSectionName, sTab[0], sTab[1]); // Ajout item de type Text dans objet recipe                         

                                ItemType eItemType = ItemType.UNKNOWN_TYPE;
                                // Recupere les infos liées à la clef
                                // key@Info
                                String sValue = FileTools.UserFileTools.GetIniFileString(DataFilePath, sCurrentSectionName, sTab[0] + "@INFO", "KeyUnknown");
                                if (sValue != "KeyUnknown") // SI c'est une cléf existante ALORS
                                {
                                    eItemType = ItemType.INFO_ITEM; // Set item type
                                    AddNewItem(sCurrentSectionName, sTab[0], eItemType, sValue); // Add or set item values with info value in ObjRecipe object
                                }
                                // key@LS
                                sValue = FileTools.UserFileTools.GetIniFileString(DataFilePath, sCurrentSectionName, sTab[0] + "@LS", "KeyUnknown");
                                if (sValue != "KeyUnknown") // SI c'est une cléf existante ALORS
                                {
                                    eItemType = ItemType.LIMITESUP_ITEM; // Set item type
                                    AddNewItem(sCurrentSectionName, sTab[0], eItemType, sValue); // Add or set item values with info value in ObjRecipe object
                                }
                                // key@LI
                                sValue = FileTools.UserFileTools.GetIniFileString(DataFilePath, sCurrentSectionName, sTab[0] + "@LI", "KeyUnknown");
                                if (sValue != "KeyUnknown") // SI c'est une cléf existante ALORS
                                {
                                    eItemType = ItemType.LIMITEINF_ITEM; // Set item type
                                    AddNewItem(sCurrentSectionName, sTab[0], eItemType, sValue); // Add or set item values with info value in ObjRecipe object
                                }
                                // key@UNIT
                                sValue = FileTools.UserFileTools.GetIniFileString(DataFilePath, sCurrentSectionName, sTab[0] + "@UNIT", "KeyUnknown");
                                if (sValue != "KeyUnknown") // SI c'est une cléf existante ALORS
                                {
                                    eItemType = ItemType.UNIT_ITEM; // Set item type
                                    AddNewItem(sCurrentSectionName, sTab[0], eItemType, sValue); // Add or set item values with info value in ObjRecipe object
                                }
                                // key@TYPE
                                sValue = FileTools.UserFileTools.GetIniFileString(DataFilePath, sCurrentSectionName, sTab[0] + "@TYPE", "KeyUnknown");
                                if (sValue != "KeyUnknown") // SI c'est une cléf existante ALORS
                                {
                                    eItemType = ItemType.FIELDUSED_ITEM; // Set item type
                                    AddNewItem(sCurrentSectionName, sTab[0], eItemType, sValue); // Add or set item values with info value in ObjRecipe object
                                }
                                // key@IMAGE
                                sValue = FileTools.UserFileTools.GetIniFileString(DataFilePath, sCurrentSectionName, sTab[0] + "@IMAGE", "KeyUnknown");
                                if (sValue != "KeyUnknown") // SI c'est une cléf existante ALORS
                                {
                                    eItemType = ItemType.IMAGE_ITEM; // Set item type
                                    AddNewItem(sCurrentSectionName, sTab[0], eItemType, sValue); // Add or set item values with info value in ObjRecipe object
                                }
                            }
                            else
                            {
                                //
                            }
                        }
                    }
                }

                // Instanciate custom data types for "details" fields.
                InstantiateDetailsInfos();
                return 0;
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Cannot read file" + FilePath + " \nwith error:" + Ex.Message);
                sr.Close();
                return 1;
            }
            finally
            {
                sr.Close();
            }
        }

        /// <summary>
        /// Scans for "details" fields, and replace their value with the corresponding class, initialized with the data from the associated [DETAILS] section (which gets removed from this object).
        /// IOException in case of file error.
        /// </summary>
        public void InstantiateDetailsInfos()
        {
            for (int i = -1; (++i) < m_ListSection.Count;)
            {
                RecipeSection section = m_ListSection[i];
                if (section.IsDetails)
                {
                    // The next section should list the "details" values.
                    string detailsSection = section.SectionName.Replace("CHILD1", "DETAILS");
                    Int32 detailsSectionIdx = i + 1;

                    if (m_ListSection.Count <= detailsSectionIdx)
                    {
                        throw new IOException(detailsSection + ": section missing!");
                    }

                    RecipeSection nextSection = m_ListSection[detailsSectionIdx];
                    if (nextSection.SectionName != detailsSection)
                    {
                        throw new IOException(detailsSection + ": section missing!");
                    }

                    // Load "details" data.
                    RecipeItems item = section.RecipeItems[0];
                    item.SectionName = item.SectionName.Replace("CHILD1", "DETAILS");

                    //>IOException
                    RobotDetailsData data = item.FieldUsed.InstantiateDetails();

                    //>IOException
                    data.StringPairsSet(nextSection.StringsList);

                    item.Value = data;

                    // Skip feeding section.
                    m_ListSectionName.Remove(detailsSection);
                    m_ListSection.RemoveAt(detailsSectionIdx);
                }
            }
        }

        public String IniFileName
        {
            get { return m_IniFileName; }
            set { m_IniFileName = value; }
        }

        public void AddSection(String SectionName)
        {
            // Cree la section
            RecipeSection mSection = new RecipeSection();
            mSection.RecipeItems = new List<RecipeItems>();
            mSection.RecipeItemsName = new List<string>();
            mSection.SectionName = SectionName.ToUpper();
            // Ajoute la section dans une liste et le nom de section dans une autre liste pour la recherche d'index par le nom
            m_ListSection.Add(mSection);
            m_ListSectionName.Add(mSection.SectionName);
        }

        public void AddItem(String SectionName, String KeyName, Object Value)
        {
            int IndexSectionFound, IndexKeyFound;
            String InfoValue = "";
            if (IsKeyExist(SectionName, KeyName, out IndexSectionFound, out IndexKeyFound)) // SI Clef existe déjà ALORS
            {
                InfoValue = m_ListSection[IndexSectionFound].RecipeItems[IndexKeyFound].Info; // Memoriser sa valeur
                m_ListSection[IndexSectionFound].RecipeItems.RemoveAt(IndexKeyFound);         // Supprimer la Clef
                m_ListSection[IndexSectionFound].RecipeItemsName.RemoveAt(IndexKeyFound);     // Supprimer aussi dans liste de recherche 
            }
            // Crée l'item
            RecipeItems mRecipeItems = new RecipeItems(this, SectionName);
            mRecipeItems.KeyName = KeyName;
            mRecipeItems.Value = Value;
            mRecipeItems.Info = InfoValue;
            // Ajoute l'item            
            m_ListSection[IndexSectionFound].RecipeItems.Add(mRecipeItems);
            m_ListSection[IndexSectionFound].RecipeItemsName.Add(mRecipeItems.KeyName);
        }

        public void AddNewItem(String Section, String Key, ItemType eItemType, Object objValue)
        {
            int IndexSection, IndexKey;
            Object KeyValue = null;
            String InfoValue = "";
            String strImageFileNameValue = null;
            Decimal dLimitSup = 0;
            Decimal dLimitInf = 0;
            String sUnit = "No unit";
            FieldUsed eFieldUsed = FieldUsed.UNKNOWN_FIELD;
            List<String> ListItems = new List<String>();
            bool bSkipSearch;
            // Set the new value in the target field
            switch (eItemType)
            {
                case ItemType.VALUE_ITEM: KeyValue = objValue; break;
                case ItemType.INFO_ITEM: InfoValue = (String)objValue; break;
                case ItemType.IMAGE_ITEM: strImageFileNameValue = (String)objValue; break;
                case ItemType.LIMITESUP_ITEM: dLimitSup = Convert.ToDecimal((String)objValue, CultureInfo.InvariantCulture); break;
                case ItemType.LIMITEINF_ITEM: dLimitInf = Convert.ToDecimal((String)objValue, CultureInfo.InvariantCulture); break;
                case ItemType.UNIT_ITEM: sUnit = (String)objValue; break;
                case ItemType.FIELDUSED_ITEM:
                    if (objValue.ToString().Contains("UNKNOWN")) eFieldUsed = FieldUsed.UNKNOWN_FIELD;
                    if (objValue.ToString().Contains("BOOLEAN")) eFieldUsed = FieldUsed.BOOLEAN;
                    if (objValue.ToString().Contains("TEXT")) eFieldUsed = FieldUsed.TEXT_FIELD;
                    if (objValue.ToString().Contains("NUMERIC")) eFieldUsed = FieldUsed.NUMERIC_FIELD;
                    if (objValue.ToString().Contains("NUMERICDECIMAL")) eFieldUsed = FieldUsed.NUMERIC_DECIMAL_FIELD;
                    if (objValue.ToString().Contains("NUMERIC2DECIMAL")) eFieldUsed = FieldUsed.NUMERIC_2DECIMAL_FIELD;
                    if (objValue.ToString().Contains("NUMERIC3DECIMAL")) eFieldUsed = FieldUsed.NUMERIC_3DECIMAL_FIELD;
                    if (objValue.ToString().Contains("NUMERIC4DECIMAL")) eFieldUsed = FieldUsed.NUMERIC_4DECIMAL_FIELD;
                    if (objValue.ToString().Contains("NUMERIC5DECIMAL")) eFieldUsed = FieldUsed.NUMERIC_5DECIMAL_FIELD;
                    if (objValue.ToString().Contains("COMBOBOX"))eFieldUsed = FieldUsed.COMBOBOX_FIELD;
                    if (objValue.ToString().Contains("NAMELIST")) eFieldUsed = FieldUsed.NAMEFILE_COMBOBOX_FIELD;
                    if (objValue.ToString().Contains("SEQUENCE")) eFieldUsed = FieldUsed.SEQUENCE_FIELD;
                    if (objValue.ToString().Contains("XMLFILE")) eFieldUsed = FieldUsed.XML_FIELD;
                    if (objValue.ToString().Contains("LINKWITH_DATA")) eFieldUsed = FieldUsed.LINKWITH_DATA_FIELD;
                    if (objValue.ToString().Contains("LINKWITH_BOOLEAN")) eFieldUsed = FieldUsed.LINKWITH_BOOLEAN_FIELD;
                    if (objValue.ToString().Contains("MULTISELECT_LBOX")) eFieldUsed = FieldUsed.MULTISELECT_LBOX_FIELD;
                    if (objValue.ToString().Contains("NUMERIC_COMPAREINF")) eFieldUsed = FieldUsed.NUMERIC_COMPAREINF_FIELD;
                    if (objValue.ToString().Contains("NUMERICDECIMAL_COMPAREINF")) eFieldUsed = FieldUsed.NUMERIC_DECIMAL_COMPAREINF_FIELD;
                    if (objValue.ToString().Contains("NUMERIC2DECIMAL_COMPAREINF")) eFieldUsed = FieldUsed.NUMERIC_2DECIMAL_COMPAREINF_FIELD;
                    if (objValue.ToString().Contains("NUMERIC3DECIMAL_COMPAREINF")) eFieldUsed = FieldUsed.NUMERIC_3DECIMAL_COMPAREINF_FIELD;
                    if (objValue.ToString().Contains("NUMERIC4DECIMAL_COMPAREINF")) eFieldUsed = FieldUsed.NUMERIC_4DECIMAL_COMPAREINF_FIELD;
                    if (objValue.ToString().Contains("NUMERIC5DECIMAL_COMPAREINF")) eFieldUsed = FieldUsed.NUMERIC_5DECIMAL_COMPAREINF_FIELD;
                    if (objValue.ToString().Contains("NUMERIC_COMPARESUP")) eFieldUsed = FieldUsed.NUMERIC_COMPARESUP_FIELD;
                    if (objValue.ToString().Contains("NUMERICDECIMAL_COMPARESUP")) eFieldUsed = FieldUsed.NUMERIC_DECIMAL_COMPARESUP_FIELD;
                    if (objValue.ToString().Contains("NUMERIC2DECIMAL_COMPARESUP")) eFieldUsed = FieldUsed.NUMERIC_2DECIMAL_COMPARESUP_FIELD;
                    if (objValue.ToString().Contains("NUMERIC3DECIMAL_COMPARESUP")) eFieldUsed = FieldUsed.NUMERIC_3DECIMAL_COMPARESUP_FIELD;
                    if (objValue.ToString().Contains("NUMERIC4DECIMAL_COMPARESUP")) eFieldUsed = FieldUsed.NUMERIC_4DECIMAL_COMPARESUP_FIELD;
                    if (objValue.ToString().Contains("NUMERIC5DECIMAL_COMPARESUP")) eFieldUsed = FieldUsed.NUMERIC_5DECIMAL_COMPARESUP_FIELD;
                    if (objValue.ToString().Contains("DATABASE")) eFieldUsed = FieldUsed.DATABASE_LIST_FIELD;
                    if (objValue.ToString().Contains("DETAILS_LsAcquisitionParams")) eFieldUsed = FieldUsed.DETAILS_LsAcquisitionParams;
                    if (objValue.ToString().Contains("DETAILS_LsTargetValues")) eFieldUsed = FieldUsed.DETAILS_LsTargetValues;

                    break;
                case ItemType.UNKNOWN_TYPE: break;
            }
            // Check if the key is already recorded, if yes, we have to delete it and add again with its new value, if no it's not normal we ignore this item           
            if (IsKeyExist(Section, Key, out IndexSection, out IndexKey)) // SI Clef existe déjà ALORS
            {
                // We memorize each value of key except the target field. 
                // --- Key value
                if (eItemType != ItemType.VALUE_ITEM)
                    KeyValue = m_ListSection[IndexSection].RecipeItems[IndexKey].Value; // Memoriser sa valeur

                // --- Info value                              
                if (eItemType != ItemType.INFO_ITEM)
                    InfoValue = m_ListSection[IndexSection].RecipeItems[IndexKey].Info; // Memoriser sa valeur

                // --- Image value                              
                if (eItemType != ItemType.IMAGE_ITEM)
                    strImageFileNameValue = m_ListSection[IndexSection].RecipeItems[IndexKey].ImageFileName; // Memoriser sa valeur

                // --- Upper limite value
                if (eItemType != ItemType.LIMITESUP_ITEM)
                    dLimitSup = (Decimal)m_ListSection[IndexSection].RecipeItems[IndexKey].LimitSup;

                // --- Lower limit value                
                if (eItemType != ItemType.LIMITEINF_ITEM)
                    dLimitInf = (Decimal)m_ListSection[IndexSection].RecipeItems[IndexKey].LimitInf;

                // --- Unit value
                if (eItemType != ItemType.UNIT_ITEM)
                    sUnit = m_ListSection[IndexSection].RecipeItems[IndexKey].Unit;

                // --- Field used value
                if (eItemType != ItemType.FIELDUSED_ITEM)
                {
                    eFieldUsed = m_ListSection[IndexSection].RecipeItems[IndexKey].FieldUsed;
                    if (m_ListSection[IndexSection].RecipeItems[IndexKey].ComboItems != null)
                        ListItems = new List<String>(m_ListSection[IndexSection].RecipeItems[IndexKey].ComboItems);
                }
                else
                {
                    String[] Temp;
                    switch (eFieldUsed)
                    {
                        case FieldUsed.COMBOBOX_FIELD:
                            Temp = objValue.ToString().Split('*');
                            if(Temp.Length < 2)
                                Temp = objValue.ToString().Split('-'); // The 1st item = "COMBOBOX", do not take it in the combobox control.                        
                            ListItems = new List<String>(Temp);
                            if (ListItems.Count > 1)
                                ListItems.RemoveAt(0);
                            if ((Section.ToUpper().Contains("OCR")) && (Key.ToUpper().Contains("#DOWNLOADED")))
                            {
                                //Get OCR Recipe

                            }
                            break;
                        case FieldUsed.NAMEFILE_COMBOBOX_FIELD:
                            Temp = objValue.ToString().Split('*');
                            if (Temp.Length < 2)
                                Temp = objValue.ToString().Split('-'); // The 1st item = "COMBOBOX", do not take it in the combobox control.                        
                            String lMachineName = "Unknown";
                            List<String> lMachineNameList;
                            if (Section.ToUpper().Contains("ADC RECIPE"))
                            {
                                if ((Key.ToUpper().Contains("EDGERECIPEFILENAME") || Key.ToUpper().Contains("RECIPE_EDGE") || Key.ToUpper().Contains("RECIPE_EYEEDGE")) && m_bADCEdgeMachineEnabled)
                                    lMachineName = m_ADCEdgeMachineName;
                                if ((Key.ToUpper().Contains("FRONTSIDERECIPEFILENAME") || Key.ToUpper().Contains("RECIPE_PSDFRONTSIDE") || Key.ToUpper().Contains("RECIPE_TOPOGRAPHY") || Key.ToUpper().Contains("RECIPE_PSD_FRONTSIDE")) && m_bADCFrontMachineEnabled)
                                    lMachineName = m_ADCFrontMachineName;
                                if ((Key.ToUpper().Contains("BACKSIDERECIPEFILENAME") || Key.ToUpper().Contains("RECIPE_PSDBACKSIDE") || Key.ToUpper().Contains("RECIPE_TOPOGRAPHY_BACKSIDE") || Key.ToUpper().Contains("RECIPE_PSD_BACKSIDE")) && m_bADCBackMachineEnabled)
                                    lMachineName = m_ADCBackMachineName;
                                if ((Key.ToUpper() == "RECIPE_BRIGHTFIELD") && m_bADCBrightField1MachineEnabled)
                                    lMachineName = m_ADCBrightFiel1dMachineName;
                                if (Key.ToUpper().Contains("RECIPE_BRIGHTFIELD2D3D") && m_bADCBrightField1MachineEnabled)
                                    lMachineName = m_ADCBrightFiel1dMachineName;
                                if (Key.ToUpper().Contains("RECIPE_BRIGHTFIELD2D") && m_bADCBrightField1MachineEnabled)
                                    lMachineName = m_ADCBrightFiel1dMachineName;
                                if (Key.ToUpper().Contains("RECIPE_BRIGHTFIELD3D") && m_bADCBrightField2MachineEnabled)
                                    lMachineName = m_ADCBrightFiel2dMachineName;
                                if (Key.ToUpper().Contains("RECIPE_DARKVIEW") && m_bADCDarkfieldMachineEnabled)
                                    lMachineName = m_ADCDarkfieldMachineName;
                                if (Key.ToUpper().Contains("RECIPE_LIGHTSPEED") && m_bADCLightSpeedMachineEnabled)
                                    lMachineName = m_ADCLightSpeedMachineName;
                            }
                            else
                                if (Section.ToUpper().Contains("DARKFIELD") && m_bADCDarkfieldMachineEnabled)
                                lMachineName = m_ADCDarkfieldMachineName;
                            if (Section.ToUpper().Contains("EDGE") && m_bADCEdgeMachineEnabled)
                                lMachineName = m_ADCEdgeMachineName;
                            if (Section.ToUpper().Contains("FRONTSIDE PROCESS") && m_bADCFrontMachineEnabled)
                                lMachineName = m_ADCFrontMachineName;
                            if (Section.ToUpper().Contains("BACKSIDE") && m_bADCBackMachineEnabled)
                                lMachineName = m_ADCBackMachineName;
                            if (Section.ToUpper().Contains("DARKVIEW"))
                                lMachineName = m_ModuleDarkfieldMachineName;
                            if (Section.ToUpper().Contains("BRIGHTFIELD 2D INSPECTION 1"))
                            {
                                if (((Key == "ADCConfigurationFile") || (Key == "PatternMaskAlignment")) && (m_ModulePMMachineName != null) && (m_ModulePMMachineName.Count > 0))
                                {
                                    if (m_ModulePMMachineName.TryGetValue("BRIGHTFIELD2DONLY", out lMachineNameList) && lMachineNameList.Count > 0)
                                    {
                                        lMachineName = lMachineNameList[0];
                                    }
                                    else
                                        if (m_ModulePMMachineName.TryGetValue("BRIGHTFIELD2D3D", out lMachineNameList) && lMachineNameList.Count > 0)
                                    {
                                        lMachineName = lMachineNameList[0];
                                    }
                                }
                            }
                            if (Section.ToUpper().Contains("BRIGHTFIELD 2D INSPECTION 2"))
                            {
                                if (((Key == "ADCConfigurationFile") || (Key == "PatternMaskAlignment")) && (m_ModulePMMachineName != null) && (m_ModulePMMachineName.Count > 0))
                                {
                                    if (m_ModulePMMachineName.TryGetValue("BRIGHTFIELD2DONLY", out lMachineNameList))
                                    {
                                        if (lMachineNameList.Count == 1)
                                            lMachineName = lMachineNameList[0];
                                        else
                                            if (lMachineNameList.Count > 1)
                                            lMachineName = lMachineNameList[1];
                                    }
                                    else
                                        if (m_ModulePMMachineName.TryGetValue("BRIGHTFIELD2D3D", out lMachineNameList))
                                    {
                                        if (lMachineNameList.Count == 1)
                                            lMachineName = lMachineNameList[0];
                                        else
                                            if (lMachineNameList.Count > 1)
                                            lMachineName = lMachineNameList[1];
                                    }
                                }
                            }
                            if (Section.ToUpper().Contains("BRIGHTFIELD 3D INSPECTION 1"))
                            {
                                if (((Key == "ADCConfigurationFile") || (Key == "PatternMaskAlignment")) && (m_ModulePMMachineName != null) && (m_ModulePMMachineName.Count > 0))
                                {
                                    if (m_ModulePMMachineName.TryGetValue("BRIGHTFIELD3DONLY", out lMachineNameList) && lMachineNameList.Count > 0)
                                    {
                                        lMachineName = lMachineNameList[0];
                                    }
                                    else
                                        if (m_ModulePMMachineName.TryGetValue("BRIGHTFIELD2D3D", out lMachineNameList) && lMachineNameList.Count > 0)
                                    {
                                        lMachineName = lMachineNameList[0];
                                    }
                                }
                            }
                            if (Section.ToUpper().Contains("BRIGHTFIELD 3D INSPECTION 2"))
                            {
                                if (((Key == "ADCConfigurationFile") || (Key == "PatternMaskAlignment")) && (m_ModulePMMachineName != null) && (m_ModulePMMachineName.Count > 0))
                                {
                                    if (m_ModulePMMachineName.TryGetValue("BRIGHTFIELD3DONLY", out lMachineNameList) && lMachineNameList.Count > 0)
                                    {
                                        lMachineName = lMachineNameList[0];
                                    }
                                    else
                                        if (m_ModulePMMachineName.TryGetValue("BRIGHTFIELD2D3D", out lMachineNameList) && lMachineNameList.Count > 0)
                                    {
                                        lMachineName = lMachineNameList[0];
                                    }
                                }
                            }
                            if (Section.ToUpper().Contains("PM REVIEW"))
                            {
                                if (((Key == "PatternMaskAlignment")) && (m_ModulePMMachineName != null) && (m_ModulePMMachineName.Count > 0))
                                {
                                    if (m_ModulePMMachineName.TryGetValue("REVIEW", out lMachineNameList) && lMachineNameList.Count > 0)
                                    {
                                        lMachineName = lMachineNameList[0];
                                    }
                                }
                            }
                            if (Section.ToUpper().Contains("PM WAFERID READING"))
                            {
                                if ((Key == "RecipeName") && (m_ModulePMMachineName != null) && (m_ModulePMMachineName.Count > 0))
                                {
                                    if (m_ModulePMMachineName.TryGetValue("BRIGHTFIELD2DONLY", out lMachineNameList) && lMachineNameList.Count > 0)
                                    {
                                        lMachineName = lMachineNameList[0];
                                    }
                                    else
                                        if (m_ModulePMMachineName.TryGetValue("BRIGHTFIELD3DONLY", out lMachineNameList) && lMachineNameList.Count > 0)
                                    {
                                        lMachineName = lMachineNameList[0];
                                    }
                                    else
                                            if (m_ModulePMMachineName.TryGetValue("BRIGHTFIELD2D3D", out lMachineNameList) && lMachineNameList.Count > 0)
                                    {
                                        lMachineName = lMachineNameList[0];
                                    }
                                }
                            }


                            if (Section.ToUpper().Contains("OCR"))
                            {
                                if ((Key == "RecipeName"))
                                {
                                    lMachineName = "127.0.0.1";
                                }
                            }
                            if ((Section.ToUpper().Contains("PSD FRONTSIDE INSPECTION")) || (Section.ToUpper().Contains("PSD BACKSIDE INSPECTION")))
                            {
                                if (Key.ToUpper().Contains("CUTUPRECIPE") && (m_ModulePMMachineName != null) && (m_ModulePMMachineName.Count > 0))
                                    if (m_ModulePMMachineName.TryGetValue("PSD", out lMachineNameList) && lMachineNameList.Count > 0)
                                        lMachineName = lMachineNameList[0];
                            }
                            String[] sTab = lMachineName.Split('?'); // Créer une liste vide avec le separateur ? caractere introuvable dans MachineName
                            if (lMachineName.Length == 0)
                                lMachineName = "Unknown";
                            if (lMachineName != "Unknown")
                            {
                                try
                                {
                                    bSkipSearch = (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "GENERAL", "SkipSearchOnRemotePCForRecipeData", 0) == 1);
                                    if (!bSkipSearch)
                                    {                                      
                                        String IPAddress = String.Empty;
                                        try
                                        {
                                            IPAddress = Win32Tools.GetAdresseIP(lMachineName);
                                        }
                                        catch
                                        {
                                            IPAddress = String.Empty;
                                        }
                                        if (IPAddress.Length > 0)
                                        {
                                            if (Section.ToUpper().Contains("DARKVIEW")) // SI pour recette du Darkview
                                                sTab = System.IO.Directory.GetFiles("\\\\" + lMachineName + "\\" + Temp[1], "*.rcp");
                                            else
                                            if (Section.ToUpper().Contains("BRIGHTFIELD 2D INSPECTION") && (Key == "PatternMaskAlignment")) // SI pour recette du BrightField
                                                sTab = System.IO.Directory.GetFiles("\\\\" + lMachineName + "\\" + Temp[1], "*.*im");
                                            else
                                            if (Section.ToUpper().Contains("BRIGHTFIELD 3D INSPECTION") && (Key == "PatternMaskAlignment")) // SI pour recette du BrightField
                                                sTab = System.IO.Directory.GetFiles("\\\\" + lMachineName + "\\" + Temp[1], "*.*im");
                                            else
                                            if (Section.ToUpper().Contains("PM WAFERID READING") && (Key == "RecipeName")) // SI pour recette du BrightField
                                                sTab = System.IO.Directory.GetFiles("\\\\" + lMachineName + "\\" + Temp[1], "*.*4dm");
                                            else
                                            if (Section.ToUpper().Contains("PM REVIEW") && (Key == "PatternMaskAlignment")) // SI pour recette du BrightField
                                                sTab = System.IO.Directory.GetFiles("\\\\" + lMachineName + "\\" + Temp[1], "*.*im");
                                            else
                                            if ((Section.ToUpper().Contains("PSD FRONTSIDE INSPECTION") || (Section.ToUpper().Contains("PSD BACKSIDE INSPECTION"))) && Key.ToUpper().Contains("CUTUPRECIPE")) // SI pour recette DiesCutUp du PSD
                                            {
                                                String[] sTab2 = System.IO.Directory.GetFiles("\\\\" + lMachineName + "\\" + Temp[1], "*.xml");
                                                sTab = new string[sTab2.Length + 1];
                                                sTab[0] = "None";
                                                sTab2.CopyTo(sTab, 1);
                                            }
                                            else
                                            if (Section.ToUpper().Contains("OCR")) // SI pour recette OCR
                                            {
                                                String lExt = "*.*";
                                                switch (CConfiguration.GetOCR_ReaderType)
                                                {
                                                    case enumOCReaderType.rtRORZE_IOSS:
                                                        lExt = "*." + CConfiguration.GetOCR_IOSS_RecipesExtension;
                                                        break;
                                                    case enumOCReaderType.rtRORZE_DEFAULT:
                                                    case enumOCReaderType.rtMECHATRONIC_IOSS:
                                                        break;
                                                    case enumOCReaderType.rtRORZE_TAI:// TODO LISE
                                                    default:
                                                        break;
                                                }
                                                String[] sTab2 = System.IO.Directory.GetFiles("\\\\" + lMachineName + "\\" + Temp[1], lExt);
                                                sTab = new string[sTab2.Length + 1];
                                                sTab[0] = "None";
                                                sTab2.CopyTo(sTab, 1);
                                            }
                                            // SINON c'est les fichiers configuration ADC
                                            else
                                                sTab = System.IO.Directory.GetFiles("\\\\" + lMachineName + "\\" + Temp[1], "*.clf");
                                            for (int i = 0; i < sTab.Length; i++)
                                            {
                                                int PosStart = sTab[i].LastIndexOf('\\');
                                                sTab[i] = sTab[i].Remove(0, PosStart + 1);
                                                sTab[i] = sTab[i].Replace(".rcp", "");
                                            }
                                        }
                                    }
                                    else
                                        sTab = new string[1] { "Nothing" };
                                }
                                catch
                                {
                                    MessageBox.Show("Configuration files list on machine " + lMachineName + " has been not found (" + "\\\\" + lMachineName + "\\" + Temp[1] + "). Check connection or directory access on this machine.");
                                }
                            }
                            ListItems = new List<String>(sTab);
                            break;
                        case FieldUsed.SEQUENCE_FIELD:
                            break;
                        case FieldUsed.NUMERIC_COMPAREINF_FIELD:
                        case FieldUsed.NUMERIC_DECIMAL_COMPAREINF_FIELD:
                        case FieldUsed.NUMERIC_2DECIMAL_COMPAREINF_FIELD:
                        case FieldUsed.NUMERIC_3DECIMAL_COMPAREINF_FIELD:
                        case FieldUsed.NUMERIC_4DECIMAL_COMPAREINF_FIELD:
                        case FieldUsed.NUMERIC_5DECIMAL_COMPAREINF_FIELD:
                            Temp = objValue.ToString().Split('-'); // First, do not used, NUMERIC3DECIMAL_COMPAREINF-ScanOuterRadius                        
                            if (Temp.Length > 1)
                            {
                                String CompareField = Temp[1];
                                CompareKeySectionAndKeyField SectionAndKey = new CompareKeySectionAndKeyField(Section, Key);
                                if (!m_CompareInfSectionDataField.ContainsKey(SectionAndKey))
                                {
                                    m_CompareInfSectionDataField.Add(SectionAndKey, CompareField);
                                }
                            }
                            break;
                        case FieldUsed.NUMERIC_COMPARESUP_FIELD:
                        case FieldUsed.NUMERIC_DECIMAL_COMPARESUP_FIELD:
                        case FieldUsed.NUMERIC_2DECIMAL_COMPARESUP_FIELD:
                        case FieldUsed.NUMERIC_3DECIMAL_COMPARESUP_FIELD:
                        case FieldUsed.NUMERIC_4DECIMAL_COMPARESUP_FIELD:
                        case FieldUsed.NUMERIC_5DECIMAL_COMPARESUP_FIELD:
                            Temp = objValue.ToString().Split('-'); // First, do not used, NUMERIC3DECIMAL_COMPAREINF-ScanOuterRadius                        
                            if (Temp.Length > 1)
                            {
                                String CompareField = Temp[1];
                                CompareKeySectionAndKeyField SectionAndKey = new CompareKeySectionAndKeyField(Section, Key);
                                if (!m_CompareSupSectionDataField.ContainsKey(SectionAndKey))
                                {
                                    m_CompareSupSectionDataField.Add(SectionAndKey, CompareField);
                                }
                            }
                            break;
                        case FieldUsed.LINKWITH_BOOLEAN_FIELD:
                            Temp = objValue.ToString().Split('-'); // The 1st item = "LINKWITH", do not used.                        
                            if (Temp.Length > 1)
                            {
                                String lKeyName = Temp[1];
                                if (m_LinkWithKeyNameList.IndexOf(lKeyName) < 0)
                                    m_LinkWithKeyNameList.Add(lKeyName);
                                int lPos = m_LinkWithKeyNameList.IndexOf(lKeyName);
                                if (lPos == 0)
                                    m_LinkWithBooleanTypeField1=Temp[2];
                                else
                                    m_LinkWithBooleanTypeField2 = Temp[2];
                            }
                            break;
                        case FieldUsed.LINKWITH_DATA_FIELD:                     
                            Temp = objValue.ToString().Split('-'); // The 1st item = "LINKWITH", do not used.                        
                            if (Temp.Length > 1)
                            {
                                String lKeyName = Temp[1];
                                if (m_LinkWithKeyNameList.IndexOf(lKeyName) < 0)
                                    m_LinkWithKeyNameList.Add(lKeyName);
                                int lPos = lKeyName.IndexOf(Key);
                                if (lPos == 0)
                                    m_LinkWithDataField1.Add(Temp[2]);
                                else
                                    m_LinkWithDataField2.Add(Temp[2]);
                            }
                            break;
                        case FieldUsed.XML_FIELD:

                            //RBL : Comme ce champ peut être de plusieurs types, ajout dudit type entre la nature du champ (XMLTYPE) et la valeur (Chemin)                           
                            String[] TempStr;
                            TempStr = objValue.ToString().Split('-'); // The 1st item = "XMLFILE", do not take it.
                            XMLFieldTypes FieldType = XMLFieldTypes.ftNone;
                            String sFilePathName;
                            List<String> TempList = new List<string>(TempStr);
                            if (TempStr.Length == 2)
                            {
                                TempList.Insert(1, "ftFringesList");
                            }
                            FieldType = (XMLFieldTypes)Enum.Parse(typeof(XMLFieldTypes), TempList[1]);
                            switch (FieldType)
                            {
                                case XMLFieldTypes.ftFringesList:
                                    bSkipSearch = (Win32Tools.GetIniFileInt(CValues.PMALTASIGHT_INI, "GENERAL", "SkipSearchOnRemotePCForRecipeData", 0) == 1);
                                    if (!bSkipSearch)
                                    {
                                        sFilePathName = TempList[2];
                                        try
                                        {
                                            SerializableFringeList lFringeObject = SerializableFringeList.Deserialize(sFilePathName);
                                            String[] sTab2 = new string[lFringeObject.FringeList.Count];
                                            lFringeObject.FringeList.CopyTo(sTab2);
                                            if (sTab2 != null)
                                                ListItems.AddRange(sTab2);
                                        }
                                        catch(Exception ex)
                                        {
                                            MessageBox.Show("Fringe list file can no be read. Name=" + sFilePathName);
                                        }
                                    }
                                    break;
                                case XMLFieldTypes.ftImagesList:
                                    sFilePathName = TempList[2];
                                    try
                                    {
                                        SerializableFringeList lFringeObject = SerializableFringeList.Deserialize(sFilePathName);
                                        String[] sTab2 = new string[lFringeObject.FringeList.Count];
                                        lFringeObject.FringeList.CopyTo(sTab2);
                                        if (sTab2 != null)
                                            ListItems.AddRange(sTab2);
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Image list file can no be read. Name=" + sFilePathName);
                                    }
                                    break;
                                case XMLFieldTypes.ftFilterWheel:
                                    sFilePathName = TempList[2];
                                    try
                                    {
                                        SerializableFilterWheelManagerConfig lFilterWheelObject = SerializableFilterWheelManagerConfig.ReadFromFile(sFilePathName);
                                        String[] sTab2 = new string[lFilterWheelObject.FilterWheels[0].Labels.Count]; //
                                                                                                                      // recuperer tous les label, mettre dans tableau sTab2
                                        for (int i = 0; i < sTab2.Length; i++)
                                        {
                                            sTab2[i] = lFilterWheelObject.FilterWheels[0].Labels[i].Label;
                                        }
                                        if (sTab2 != null)
                                            ListItems.AddRange(sTab2);
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Laser filter wheel configuration list file can no be read. Name=" + sFilePathName);
                                    }
                                    break;
                            }

                            break;
                        case FieldUsed.MULTISELECT_LBOX_FIELD:
                            Temp = objValue.ToString().Split('-'); // The 1st item = "LISTBOX", do not take it in the combobox control.                        
                            ListItems = new List<String>(Temp);
                            if (ListItems.Count > 1)
                                ListItems.RemoveAt(0);
                            break;
                        case FieldUsed.DATABASE_LIST_FIELD:
                            enumDATABASERequest enumRequest = enumDATABASERequest.rstNone;
                            Temp = objValue.ToString().Split('-'); // ADC_WaferType_NAME @TYPE=DATABASE-GetWaferType
                            if (Temp.Length == 2)
                            {

                                enumRequest = (enumDATABASERequest)Enum.Parse(typeof(enumDATABASERequest), Temp[1]);
                                try
                                {
                                    switch (enumRequest)
                                    {

                                        case enumDATABASERequest.GetWaferTypes:

                                            if ((m_DataSetADCWaferTypes == null) && (m_DatabaseConnexionString != null) && (m_DatabaseConnexionString != String.Empty))
                                            {
                                                DatabaseDirectAccess databaseAcces = new DatabaseDirectAccess(m_DatabaseConnexionString);
                                                List<Database.Robot.WaferType> listWaferTypes = databaseAcces.GetWaferTypes();
                                                m_DataSetADCWaferTypes = listWaferTypes.ToArray();
                                            }
                                            break;
                                        case enumDATABASERequest.GetRecipes:
                                            if ((m_DataSetADCRecipes == null) && (m_DatabaseConnexionString != null) && (m_DatabaseConnexionString != String.Empty))
                                            {
                                                DatabaseDirectAccess databaseAcces = new DatabaseDirectAccess(m_DatabaseConnexionString);
                                                List<Database.Robot.Recipe> listRecipes = databaseAcces.GetRecipes();
                                                m_DataSetADCRecipes = listRecipes.ToArray();

                                            }
                                            break;
                                    }
                                }
                                catch (Exception Ex)
                                {
                                    MessageBox.Show("Can not read recipe data from database with error : " + Ex.Message);
                                    m_DatabaseConnexionString = String.Empty;

                                }

                            }
                            break;
                    }
                }
                // Suppress the key in object lists
                m_ListSection[IndexSection].RecipeItems.RemoveAt(IndexKey);         // Supprimer la Clef
                m_ListSection[IndexSection].RecipeItemsName.RemoveAt(IndexKey);     // Supprimer aussi dans liste de recherche 

                // Create new item
                RecipeItems RecipeItems = new RecipeItems(this, Section); // Nouvel enregistrement 
                RecipeItems.KeyName = Key;
                RecipeItems.Value = KeyValue;
                RecipeItems.Info = InfoValue; // Mise à jour des valeurs 
                RecipeItems.ImageFileName = strImageFileNameValue;
                RecipeItems.LimitSup = dLimitSup;
                RecipeItems.LimitInf = dLimitInf;
                RecipeItems.Unit = sUnit;
                RecipeItems.FieldUsed = eFieldUsed;
                RecipeItems.ComboItems = ListItems.ToArray();
                // Add our new item
                m_ListSection[IndexSection].RecipeItems.Add(RecipeItems); // Ajout à nouveau dans la liste
                m_ListSection[IndexSection].RecipeItemsName.Add(Key);
            }
        }

        public String GetInfoItem(String Section, String Key)
        {
            int IndexSection, IndexKey;
            if (IsKeyExist(Section, Key, out IndexSection, out IndexKey))
                return m_ListSection[IndexSection].RecipeItems[IndexKey].Info;
            else
                return "";

        }

        public String GetImageItem(String Section, String Key)
        {
            int IndexSection, IndexKey;
            if (IsKeyExist(Section, Key, out IndexSection, out IndexKey))
                return m_ListSection[IndexSection].RecipeItems[IndexKey].ImageFileName;
            else
                return "";

        }

        public String GetUnitItem(String Section, String Key)
        {
            int IndexSection, IndexKey;
            if (IsKeyExist(Section, Key, out IndexSection, out IndexKey))
                return m_ListSection[IndexSection].RecipeItems[IndexKey].Unit;
            else
                return "";

        }
        public Decimal GetLimitSupItem(String Section, String Key)
        {
            int IndexSection, IndexKey;

            if (IsKeyExist(Section, Key, out IndexSection, out IndexKey))
                return m_ListSection[IndexSection].RecipeItems[IndexKey].LimitSup;
            else
                return 0;

        }
        public Decimal GetLimitInfItem(String Section, String Key)
        {
            int IndexSection, IndexKey;
            if (IsKeyExist(Section, Key, out IndexSection, out IndexKey))
                return (Decimal)m_ListSection[IndexSection].RecipeItems[IndexKey].LimitInf;
            else
                return 0;

        }

        public FieldUsed GetFieldTypeUsed(String Section, String Key)
        {
            int IndexSection, IndexKey;
            if (IsKeyExist(Section, Key, out IndexSection, out IndexKey))
                return m_ListSection[IndexSection].RecipeItems[IndexKey].FieldUsed;
            else
                return FieldUsed.UNKNOWN_FIELD;
        }

        public String[] GetComboBoxItems(String Section, String Key)
        {
            int IndexSection, IndexKey;
            if (IsKeyExist(Section, Key, out IndexSection, out IndexKey))
                return m_ListSection[IndexSection].RecipeItems[IndexKey].ComboItems;
            else
                return null;
        }
		
        /// <summary>
        /// Save multiple items in the same section.
        /// </summary>
        public void Save(string section, SortedDictionary<string, string> items)
        {
            INIfile mIniFile = new INIfile(m_IniFileName);

            foreach (KeyValuePair<string, string> item in items)
            {
                mIniFile[section, item.Key] = item.Value;
            }

            mIniFile.writeFile();
        }

        /// <summary>
        /// Refreshes multiple items from the same section.
        /// </summary>
        public void Reset(string section, SortedDictionary<string, string> items)
        {
            INIfile mIniFile = new INIfile(m_IniFileName);

            List<string> itemNames = new List<string>(items.Keys);
            foreach (string itemName in itemNames)
            {
                items[itemName] = mIniFile[section, itemName];
            }
        }

        public RecipeItems AccessItem(string Section, string Item)
        {
            Int32 IndexFoundSection = m_ListSectionName.IndexOf(Section.ToUpper());
            Int32 IndexFoundItem = m_ListSection[IndexFoundSection].RecipeItemsName.IndexOf(Item);
            if ((IndexFoundSection != -1) && (IndexFoundItem != -1))
            {
                return m_ListSection[IndexFoundSection].RecipeItems[IndexFoundItem];
            }
            else
                return null;
        }

        public String this[string Section, string Item]
        {
            get
            {
                Int32 IndexFoundSection = m_ListSectionName.IndexOf(Section.ToUpper());
                Int32 IndexFoundItem = m_ListSection[IndexFoundSection].RecipeItemsName.IndexOf(Item);
                if ((IndexFoundSection != -1) && (IndexFoundItem != -1))
                {
                    //    return (String) m_ListSection[IndexFoundSection].RecipeItems[IndexFoundItem].Value; 
                    INIfile mIniFile = new INIfile(m_IniFileName);
                    return mIniFile[m_ListSection[IndexFoundSection].SectionName,
                             m_ListSection[IndexFoundSection].RecipeItems[IndexFoundItem].KeyName];
                }
                else
                    return "";
            }
            set
            {
                Int32 IndexFoundSection = m_ListSectionName.IndexOf(Section.ToUpper());
                Int32 IndexFoundItem = m_ListSection[IndexFoundSection].RecipeItemsName.IndexOf(Item);
                if ((IndexFoundSection != -1) && (IndexFoundItem != -1))
                {
                    INIfile mIniFile = new INIfile(m_IniFileName);
                    mIniFile[m_ListSection[IndexFoundSection].SectionName,
                             m_ListSection[IndexFoundSection].RecipeItems[IndexFoundItem].KeyName] = value;
                    mIniFile.writeFile();
                }


            }
        }

        public bool IsKeyExist(string Section, string Key, out int IndexSectionFound, out int IndexKeyFound)
        {
            IndexSectionFound = m_ListSectionName.IndexOf(Section.ToUpper());
            IndexKeyFound = m_ListSection[IndexSectionFound].RecipeItemsName.IndexOf(Key);
            return ((IndexSectionFound != -1) && (IndexKeyFound != -1));
        }

        public String[] GetComboBoxItemsLinkField(String pSection, String pKey)
        {
            int IndexSection, IndexKey;
            if (IsKeyExist(pSection, pKey, out IndexSection, out IndexKey))
            {
                for (int i = 0; i < m_LinkWithKeyNameList.Count; i++)
                {
                    int lPos = m_LinkWithKeyNameList[i].IndexOf(pKey);
                    if (lPos >= 0)
                    {
                        String[] lData;
                        if (lPos == 0)
                        {
                            lData = m_LinkWithDataField1[i].Split('#');
                        }
                        else
                        {
                            lData = m_LinkWithDataField2[i].Split('#');
                        }
                        String lKey2 = m_LinkWithKeyNameList[i].Replace(pKey, "");
                        int lIndexSection2, lIndexKey2;
                        if (IsKeyExist(pSection, lKey2, out lIndexSection2, out lIndexKey2))
                        {
                            String lKey2Value = this[pSection, lKey2];
                            for (int j = 0; j < lData.Length; j = j + 2)
                            {
                                if (lData[j] == lKey2Value)
                                {
                                    return lData[j + 1].Split('.');
                                }
                            }
                        }
                    }
                }
                return null;
            }
            else
                return null;
        }

        public int SetBooleanItemsLinkField(String pSection, String pKey, String pValueKey)
        {
            this[pSection, pKey] = pValueKey.ToUpper();
            bool lbValueKey = Convert.ToBoolean(pValueKey);
            int IndexSection, IndexKey;
            if (IsKeyExist(pSection, pKey, out IndexSection, out IndexKey))
            {
                for (int i = 0; i < m_LinkWithKeyNameList.Count; i++)
                {
                    int lPos = m_LinkWithKeyNameList[i].IndexOf(pKey);
                    if (lPos >= 0)
                    {
                        String lLogicType;
                        if (lPos == 0)
                        {
                            lLogicType = m_LinkWithBooleanTypeField1;
                        }
                        else
                        {
                            lLogicType = m_LinkWithBooleanTypeField2;
                        }
                        String lKey2 = m_LinkWithKeyNameList[i].Replace(pKey, "");
                        int lIndexSection2, lIndexKey2;
                        if (IsKeyExist(pSection, lKey2, out lIndexSection2, out lIndexKey2))
                        {
                            bool lbValueKey2 = Convert.ToBoolean(this[pSection, lKey2]);
                            switch (lLogicType)
                            {
                                case "COMPLEMENT": this[pSection, lKey2] = (!lbValueKey).ToString().ToUpper(); return 1; // Mettre l'inverse de l'autre champ
                                case "ONLY_ME": this[pSection, lKey2] = (!lbValueKey && lbValueKey2).ToString().ToUpper(); return 1; // Uniquement si l'autre champ est à 0 alors je peux mettre à 1
                                default:
                                    break;
                            }
                        }else
                            return -1;
                    }
                }
                return 0;
            }
            return -1;
        }
    }
}

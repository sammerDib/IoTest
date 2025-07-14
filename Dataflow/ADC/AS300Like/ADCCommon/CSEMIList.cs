using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace UnitySC.ADCAS300Like.Common
{
    public enum enumSemiListType {stText=0, stBracketList};
    public class CListStartEnd
    {
        public int iStart = 0;
        public int iEnd = 0;
    }


    public class CListBracket
    {
        List<Object> m_DataInList = new List<object>();
        List<enumSemiListType> m_DataTypeInList = new List<enumSemiListType>();

        public Object this[int index]
        {
            get { return m_DataInList[index]; }
        }

        public enumSemiListType GetListType(int index)
        {
            return m_DataTypeInList[index]; 
        }

        public CListBracket GetList(int index)
        {
            CListBracket lEmptyBracketList = new CListBracket();
            if ((m_DataInList.Count > index) && (m_DataTypeInList[index] == enumSemiListType.stBracketList))
            {
                return (CListBracket)m_DataInList[index];
            }
            else
                return lEmptyBracketList;
        }
        public String GetText(int index)
        {
            if ((m_DataInList.Count > index) && (m_DataTypeInList[index] == enumSemiListType.stText))
            {
                return (String)m_DataInList[index];
            }
            else
                return "";
        }

        public int ListCount
        {
            get { return m_DataInList.Count; }
        }

        // Text = L{......}
        public void SetList(String Text)
        {
            List<String> lListItemText = ExtractContentList(Text);
            for (int i = 0; i < lListItemText.Count; i++)
            {
                if (lListItemText[i][0] == '{')
                {
                    if (lListItemText[i][1] == 'L')
                    {
                        CListBracket NewListBraket = new CListBracket();
                        String strList = lListItemText[i].Substring(1, lListItemText[i].Length - 2);
                        NewListBraket.SetList(strList);
                        m_DataInList.Add(NewListBraket);
                        m_DataTypeInList.Add(enumSemiListType.stBracketList);
                        continue;
                    }
                    else
                    {
                        CListStartEnd lNewList = FindEndBracket(lListItemText[i]);
                        String strData = lListItemText[i].Substring(lNewList.iStart + 1, lNewList.iEnd - 1);
                        m_DataInList.Add(strData);
                        m_DataTypeInList.Add(enumSemiListType.stText);
                        continue;
                    }
                }
            }
        }


        // Text = {.......}, cette fonction separe les different item de la liste
        public List<String> SetContentInList(String Text)
        {
            List<String> lResultList = new List<string>();
            CListStartEnd lNewList;
            String SearchText = Text;
            String strData = "";

            while (SearchText.Length > 0)
            {
                SearchText = SearchText.TrimStart();
                SearchText = SearchText.TrimEnd();
                if (SearchText[0] == 'L')
                {
                    lNewList = new CListStartEnd();
                    String lSubText = SearchText.Substring(1, SearchText.Length - 1);
                    lNewList = FindEndBracket(lSubText);
                    strData = lSubText.Substring(lNewList.iStart + 1, lNewList.iEnd - 1);
                    lResultList.Add(strData);
                    SearchText = SearchText.Remove(0, strData.Length + 3); // extrait L{....} du text dans lequel on recherche
                    continue;
                }
                if (SearchText[0] == '{')
                {
                    lNewList = FindEndBracket(SearchText);
                    strData = SearchText.Substring(lNewList.iStart + 1, lNewList.iEnd - 1);
                    lResultList.Add(strData);
                    SearchText = SearchText.Remove(lNewList.iStart, lNewList.iEnd);
                }
            }
            return lResultList;
        }

        public List<String> ExtractContentList(String Text)
        {
            List<String> lResultList = new List<string>();
            CListStartEnd lNewList;
            String SearchText = Text;
            String strData = "";
            int lCpt = 0;
            while (SearchText.Length > 0)
            {
                SearchText = SearchText.TrimStart();
                SearchText = SearchText.TrimEnd();
                if (SearchText[0] == 'L')
                {
                    lNewList = new CListStartEnd();
                    String lSubText = SearchText.Substring(1, SearchText.Length - 1);
                    lNewList = FindEndBracket(lSubText);
                    strData = lSubText.Substring(lNewList.iStart, lNewList.iEnd + 1);
                    lResultList.Add(strData);
                    SearchText = SearchText.Remove(0, strData.Length + 1); // extrait L{....} du text dans lequel on recherche
                    continue;
                }
                if (SearchText[0] == '{')
                {
                    lNewList = FindEndBracket(SearchText);
                    strData = SearchText.Substring(lNewList.iStart, lNewList.iEnd + 1);
                    lResultList.Add(strData);
                    SearchText = SearchText.Remove(lNewList.iStart, lNewList.iEnd + 1);
                }
                lCpt++;
                if (lCpt > (10 * SearchText.Length))
                    break;
            }
            return lResultList;
        }

        public CListStartEnd FindEndBracket(String Text)
        {
            bool bStartListFound = false;
            int iCptCurrentOpen = 0;
            CListStartEnd NewListSemi = new CListStartEnd();
            if (Text[0] != '{')
                return NewListSemi;
            for (int i = 0; i < Text.Length; i++)
            {
                if (Text[i] == '{')
                {
                    if (!bStartListFound)
                    {
                        bStartListFound = true;
                        NewListSemi.iStart = i;
                        iCptCurrentOpen = 1;
                        continue;
                    }
                }
                if (bStartListFound && Text[i] == '{')
                {
                    iCptCurrentOpen++;
                    continue;
                }
                if (bStartListFound && (iCptCurrentOpen > 1) && Text[i] == '}')
                {
                    iCptCurrentOpen--;
                    continue;
                }
                if (bStartListFound && (iCptCurrentOpen == 1) && Text[i] == '}')
                {
                    NewListSemi.iEnd = i;
                    break;
                }
            }
            return NewListSemi;
        }

    }


    public class CSEMIList
    {
        public String m_strListData;
        public CListBracket m_ListContent;

        public CSEMIList(string strText, bool bFile)
        {
            try
            {
                m_strListData = strText;
                m_ListContent = new CListBracket();
                m_ListContent.SetList(m_strListData);
            }
            catch
            {
                MessageBox.Show("Invalid Text data to create CJob");
                throw;
            }
        }

        public CSEMIList(string pFilePath)
        {
            try
            {
                StreamReader sr = new StreamReader(pFilePath);
                string strText = sr.ReadToEnd();
                strText = strText.Trim();
                m_strListData = strText;
                sr.Close();
            }
            catch
            {
                MessageBox.Show("Cannot open file " + pFilePath);
            }

            try
            {
                m_ListContent = new CListBracket();
                m_ListContent.SetList(m_strListData);
            }
            catch
            {
                MessageBox.Show("Invalid job file: Name = " + pFilePath);
                throw;
            }
        }        

        public CListBracket GetList(int index)
        {
            if ((m_ListContent.ListCount > index) && (m_ListContent.GetListType(index) == enumSemiListType.stBracketList))
            {
                return m_ListContent.GetList(index);
            }
            else
                return null;
        }
        
        // Give list of all process list
        public CListBracket SequenceList
        {
            get { return m_ListContent; } // Item 0 = Sequence List
        }                

        public CListBracket GetProcessListAt(int index)
        {
            if ((SequenceList.ListCount > index - 1) && (SequenceList.GetListType(index - 1) == enumSemiListType.stBracketList))
            {
                return (CListBracket)SequenceList[index-1];
            }
            else
                return null;
        }

        public CListBracket Process1List
        {
            get
            {
                return GetProcessListAt(0);
            }
        }
        public CListBracket Process2List
        {
            get
            {
                return GetProcessListAt(1);
            }
        }
        public CListBracket Process3List
        {
            get
            {
                return GetProcessListAt(2);
            }
        }
    }
}

using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem
{
    [ComVisible(true)]
    [Guid("53D6D51A-F416-446D-90FC-A59B04494DD2")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ISecsDataFactory
    {
        #region Public Methods
        ISecsError CreateEmptySecsError();

        ISecsErrorList CreateEmptySecsErrorList();

        ISecsItem CreateEmptySecsItem();

        ISecsItemList CreateEmptySecsItemList();

        ISecsVariable CreateEmptySecsVariable();

        ISecsVariableList CreateEmptySecsVariableList();

        ITableElement CreateEmptySecsTableElement();

        ITableElementList CreateEmptySecsTableElementList();

        ITableElementListList CreateEmptySecsTableElementListList();

        ITableData_S13F13 CreateEmptyTableData_S13F13();

        ITableData_S13F16 CreateEmptyTableData_S13F16();

        ITableData_S13F13 CreateTableData_S13F13(string serializedTable);

        #endregion Public Methods
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(ISecsDataFactory))]
    [Guid("C12B917D-3919-45AF-993D-D18D18910B0D")]
    public class SecsDataFactory : ISecsDataFactory
    {
        #region Public Constructors

        public SecsDataFactory()
        {

        }

        #endregion Public Constructors

        #region Public Methods

        public ISecsError CreateEmptySecsError() => new SecsError(-1, "");

        public ISecsErrorList CreateEmptySecsErrorList() => new SecsErrorList();

        public ISecsItem CreateEmptySecsItem() => new SecsItem(SecsFormat.Undefined);

        public ISecsItemList CreateEmptySecsItemList() => new SecsItemList();

        public ITableElement CreateEmptySecsTableElement() => new TableElement("", new SecsItem(SecsFormat.Undefined));

        public ITableElementList CreateEmptySecsTableElementList() => new TableElementList();

        public ITableElementListList CreateEmptySecsTableElementListList() => new TableElementListList();

        public ISecsVariable CreateEmptySecsVariable() => new SecsVariable("");

        public ISecsVariableList CreateEmptySecsVariableList() => new SecsVariableList();

        public ITableData_S13F13 CreateEmptyTableData_S13F13()
        {
            TableData_S13F13 Result = new TableData_S13F13();
            Result.Attributes = new SecsAttributeList();
            Result.TableElements = new TableElementListList();
            return Result;
        }

        public ITableData_S13F16 CreateEmptyTableData_S13F16()
        {
            TableData_S13F16 Result = new TableData_S13F16();
            Result.Attributes = new SecsAttributeList();
            Result.Errors = new SecsErrorList();
            Result.TableElements = new TableElementListList();
            return Result;
        }

        TableElementListList CreateSecsTableElementListListFromString(string listString)
        {
            bool ParseBooleanEntry(string unparsedString)
            {
                bool returnValue = false;

                returnValue = unparsedString.Contains('T');

                // todo: exceptions on invalid strings

                return returnValue;
            }

            int ParseInt4Entry(string unparsedString)
            {
                int returnValue = int.Parse(Regex.Match(unparsedString, @"[-+]?\d+").Value, NumberFormatInfo.InvariantInfo);

                return returnValue;
            }

            double ParseFloat8Entry(string unparsedString)
            {
                double returnValue = double.Parse(Regex.Match(unparsedString, @"[-+]?\d+\.?\d*").Value, NumberFormatInfo.InvariantInfo);

                return returnValue;
            }

            string ParseStringEntry(string unparsedString)
            {
                string[] stringParts = unparsedString.Split('\'');
                string returnString = String.Empty;
                for (int i = 1; i < stringParts.Length; i++)
                {
                    returnString += stringParts[i];
                    // todo: add escaped chars
                }
                return returnString;
            }

            TableElementListList Result = new TableElementListList();

            // split into lists
            string[] lists = listString.Split(new string[] { "<L," }, StringSplitOptions.RemoveEmptyEntries);
            // the first list contains the column headers
            string[] columnHeaders = lists[0].Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            int columns = int.Parse(Regex.Match(columnHeaders[0], @"\d+").Value, NumberFormatInfo.InvariantInfo);

            for (int k = 1; k < columnHeaders.Length; k++)
            {
                columnHeaders[k] = ParseStringEntry(columnHeaders[k]);
            }

            // skip the list of lists
            // todo: check list count
            // parse the other lists
            for (int i = 2; i < lists.Length; i++)
            {
                // new table row
                TableElementList elementList = new TableElementList();

                string[] entrystrings = lists[i].Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                // todo: assert columns matches entrystrings.Length - 1
                // todo: assert number in entrystrings[0] matches entrystrings.Length - 1, too

                for (int j = 1; j < entrystrings.Length; j++)
                {
                    // new cell data
                    SecsItem secsItem;

                    string trimmedEntry = entrystrings[j].Remove(0, entrystrings[j].IndexOf('>'));

                    if (Regex.Match(entrystrings[j], @"<I4,").Success)
                    {
                        int int4Value = ParseInt4Entry(trimmedEntry);
                        secsItem = new SecsItem(SecsFormat.Int4, int4Value);
                    }
                    else if (Regex.Match(entrystrings[j], @"<BO,").Success)
                    {
                        bool boolValue = ParseBooleanEntry(trimmedEntry);
                        secsItem = new SecsItem(SecsFormat.Boolean, boolValue);
                    }
                    else if (Regex.Match(entrystrings[j], @"<F8,").Success)
                    {
                        double float8Value = ParseFloat8Entry(trimmedEntry);
                        secsItem = new SecsItem(SecsFormat.Float8, float8Value);
                    }
                    else if (Regex.Match(entrystrings[j], @"<A,").Success)
                    {
                        string stringValue = ParseStringEntry(trimmedEntry);
                        secsItem = new SecsItem(SecsFormat.Character, stringValue);
                    }
                    else
                    {
                        secsItem = new SecsItem(SecsFormat.Undefined);
                    }

                    TableElement element = new TableElement(columnHeaders[j], secsItem);
                    elementList.Add(element);
                }

                Result.Add(elementList);
            }

            return Result;
        }

        public ITableData_S13F13 CreateTableData_S13F13(string serializedTable)
        {
            TableData_S13F13 Result = new TableData_S13F13();
            Result.Attributes = new SecsAttributeList();
            Result.TableElements = CreateSecsTableElementListListFromString(serializedTable);
            return Result;
        }

        #endregion Public Methods
    }
}

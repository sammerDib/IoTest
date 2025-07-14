using System.Globalization;

using Agileo.Semi.Communication.Abstractions.E5;
using Agileo.Semi.Gem.Abstractions.E30;

using UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager;
using UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces;
using UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem;
using UnitySC.UTO.Controller.Remote.E5.DataItems;
using UnitySC.UTO.Controller.Remote.E5.MessageDescriptions;

namespace UnitySC.UTO.Controller.Remote.E5.MessageHandlers
{
    public class S13F15MessageHandler : E30MessageHandler
    {
        private readonly ToolControlManager _toolControlManager;

        public S13F15MessageHandler(ToolControlManager toolControlManager)
            : base(13, 15)
        {
            _toolControlManager = toolControlManager;
        }

        public override MessageHandlerResult Handle(Message message)
        {
            var s13F15 = message.As<S13F15>();

            var columnElementDescriptions = new ComStringList();
            foreach (var columnDefinition in s13F15.ColumnDefinitions)
            {
                columnElementDescriptions.Add(columnDefinition);
            }

            var tableElements = new SecsItemList();
            foreach (var tableElement in s13F15.TableElements)
            {
                switch (tableElement.Format)
                {
                    case DataItemFormat.ASC:
                        tableElements.Add(
                            new SecsItem(SecsFormat.Ascii) { StringValue = tableElement });
                        break;
                    case DataItemFormat.BIN:
                        tableElements.Add(
                            new SecsItem(SecsFormat.Binary) { BinaryValue = tableElement });
                        break;
                    case DataItemFormat.BOO:
                        tableElements.Add(
                            new SecsItem(SecsFormat.Boolean) { BooleanValue = tableElement });
                        break;
                    case DataItemFormat.SI1:
                        tableElements.Add(
                            new SecsItem(SecsFormat.Int1) { Int1Value = tableElement });
                        break;
                    case DataItemFormat.SI2:
                        tableElements.Add(
                            new SecsItem(SecsFormat.Int2) { Int2Value = tableElement });
                        break;
                    case DataItemFormat.SI4:
                        tableElements.Add(
                            new SecsItem(SecsFormat.Int4) { Int4Value = tableElement });
                        break;
                    case DataItemFormat.SI8:
                        tableElements.Add(
                            new SecsItem(SecsFormat.Int8) { Int8Value = tableElement });
                        break;
                    case DataItemFormat.UI1:
                        tableElements.Add(
                            new SecsItem(SecsFormat.UInt1) { Uint1Value = tableElement });
                        break;
                    case DataItemFormat.UI2:
                        tableElements.Add(
                            new SecsItem(SecsFormat.UInt2) { Uint2Value = tableElement });
                        break;
                    case DataItemFormat.UI4:
                        tableElements.Add(
                            new SecsItem(SecsFormat.UInt4) { Uint4Value = tableElement });
                        break;
                    case DataItemFormat.UI8:
                        tableElements.Add(
                            new SecsItem(SecsFormat.UInt8) { Uint8Value = tableElement });
                        break;
                    case DataItemFormat.FP4:
                        tableElements.Add(
                            new SecsItem(SecsFormat.Float4) { Float4Value = tableElement });
                        break;
                    case DataItemFormat.FP8:
                        tableElements.Add(
                            new SecsItem(SecsFormat.Float8) { Float8Value = tableElement });
                        break;
                    case DataItemFormat.LST:
                        var dataList = tableElement.AdaptTo<DataList>();
                        var listSecsItem = new SecsItem(SecsFormat.List);
                        BuildSecsItemFromDataList(listSecsItem.ItemList, dataList);
                        tableElements.Add(listSecsItem);
                        break;
                }
            }

            _toolControlManager?.DataSetRequest(
                new TableDataRequest()
                {
                    ColumnElementDescriptions = columnElementDescriptions,
                    DataId = s13F15.DataID,
                    DecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator,
                    ObjSpec = s13F15.ObjSpec,
                    TableCommand = s13F15.TableCmd,
                    TableElements = tableElements,
                    TableId = s13F15.TableID,
                    TableType = s13F15.TableType
                });

            return MessageHandlerResult.WithResponse(
                S13F16.New(s13F15.TableType, s13F15.TableID, TBLACK.Success));
        }

        private void BuildSecsItemFromDataList(SecsItemList secsItemList, DataList mainDataList)
        {
            secsItemList ??= new SecsItemList();

            foreach (var item in mainDataList.ToList())
            {
                switch (item.Format)
                {
                    case DataItemFormat.ASC:
                        secsItemList.Add(
                            new SecsItem(SecsFormat.Ascii)
                            {
                                StringValue = item.ValueTo<string>()
                            });
                        break;
                    case DataItemFormat.BIN:
                        secsItemList.Add(
                            new SecsItem(SecsFormat.Binary) { BinaryValue = item.ValueTo<byte>() });
                        break;
                    case DataItemFormat.BOO:
                        secsItemList.Add(
                            new SecsItem(SecsFormat.Boolean)
                            {
                                BooleanValue = item.ValueTo<bool>()
                            });
                        break;
                    case DataItemFormat.SI1:
                        secsItemList.Add(
                            new SecsItem(SecsFormat.Int1) { Int1Value = item.ValueTo<sbyte>() });
                        break;
                    case DataItemFormat.SI2:
                        secsItemList.Add(
                            new SecsItem(SecsFormat.Int2) { Int2Value = item.ValueTo<short>() });
                        break;
                    case DataItemFormat.SI4:
                        secsItemList.Add(
                            new SecsItem(SecsFormat.Int4) { Int4Value = item.ValueTo<int>() });
                        break;
                    case DataItemFormat.SI8:
                        secsItemList.Add(
                            new SecsItem(SecsFormat.Int8) { Int8Value = item.ValueTo<long>() });
                        break;
                    case DataItemFormat.UI1:
                        secsItemList.Add(
                            new SecsItem(SecsFormat.UInt1) { Uint1Value = item.ValueTo<byte>() });
                        break;
                    case DataItemFormat.UI2:
                        secsItemList.Add(
                            new SecsItem(SecsFormat.UInt2) { Uint2Value = item.ValueTo<ushort>() });
                        break;
                    case DataItemFormat.UI4:
                        secsItemList.Add(
                            new SecsItem(SecsFormat.UInt4) { Uint4Value = item.ValueTo<uint>() });
                        break;
                    case DataItemFormat.UI8:
                        secsItemList.Add(
                            new SecsItem(SecsFormat.UInt8) { Uint8Value = item.ValueTo<ulong>() });
                        break;
                    case DataItemFormat.FP4:
                        secsItemList.Add(
                            new SecsItem(SecsFormat.Float4)
                            {
                                Float4Value = item.ValueTo<float>()
                            });
                        break;
                    case DataItemFormat.FP8:
                        secsItemList.Add(
                            new SecsItem(SecsFormat.Float8)
                            {
                                Float8Value = item.ValueTo<float>()
                            });
                        break;
                    case DataItemFormat.LST:
                        var dataList = item.AdaptTo<DataList>();
                        var listSecsItem = new SecsItem(SecsFormat.List);
                        BuildSecsItemFromDataList(listSecsItem.ItemList, dataList);
                        secsItemList.Add(listSecsItem);
                        break;
                }
            }
        }
    }
}

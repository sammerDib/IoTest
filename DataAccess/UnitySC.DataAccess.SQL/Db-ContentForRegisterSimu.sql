/* USE [UnityControlv8] 
GO */
SET IDENTITY_INSERT [dbo].[Tool] ON 

INSERT [dbo].[Tool] ([Id], [Name], [IsArchived], [ToolCategory], [Toolkey]) VALUES (3, N'PsdEdge 1', 0, N'PSD Edge', 1)
INSERT [dbo].[Tool] ([Id], [Name], [IsArchived], [ToolCategory], [Toolkey]) VALUES (4, N'LsAna 2', 0, N'HeLios Analyse', 2)
INSERT [dbo].[Tool] ([Id], [Name], [IsArchived], [ToolCategory], [Toolkey]) VALUES (5, N'Ana 3', 0, N'Analyse', 3)
SET IDENTITY_INSERT [dbo].[Tool] OFF
SET IDENTITY_INSERT [dbo].[Chamber] ON 

INSERT [dbo].[Chamber] ([Id], [Name], [ToolId], [PhysicalConfigurationId], [IsArchived], [ActorType], [ChamberKey]) VALUES (1, N'PSD-1', 3, NULL, 0, 17, 1)
INSERT [dbo].[Chamber] ([Id], [Name], [ToolId], [PhysicalConfigurationId], [IsArchived], [ActorType], [ChamberKey]) VALUES (2, N'Edge-1', 3, NULL, 0, 145, 2)
INSERT [dbo].[Chamber] ([Id], [Name], [ToolId], [PhysicalConfigurationId], [IsArchived], [ActorType], [ChamberKey]) VALUES (4, N'HeLios-2', 4, NULL, 0, 193, 1)
INSERT [dbo].[Chamber] ([Id], [Name], [ToolId], [PhysicalConfigurationId], [IsArchived], [ActorType], [ChamberKey]) VALUES (6, N'Ana-2', 4, NULL, 0, 161, 2)
INSERT [dbo].[Chamber] ([Id], [Name], [ToolId], [PhysicalConfigurationId], [IsArchived], [ActorType], [ChamberKey]) VALUES (7, N'Ana-3', 5, NULL, 0, 161, 1)
SET IDENTITY_INSERT [dbo].[Chamber] OFF
SET IDENTITY_INSERT [dbo].[WaferCategory] ON 

INSERT [dbo].[WaferCategory] ([Id], [Name], [XmlContent]) VALUES (1, N'Notch 200mm', N'<WaferDimensionalCharacteristic xmlns:i="http://www.w3.org/2001/XMLSchema-instance" z:Id="1" xmlns:z="http://schemas.microsoft.com/2003/10/Serialization/" xmlns="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Data">
  <Category z:Id="2">1.9</Category>
  <Diameter xmlns:d2p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" z:Id="3">
    <d2p1:Unit>Millimeter</d2p1:Unit>
    <d2p1:Value>200</d2p1:Value>
  </Diameter>
  <DiameterTolerance xmlns:d2p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" i:nil="true" />
  <Flats i:nil="true" />
  <Notch z:Id="4">
    <Angle xmlns:d3p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" z:Id="5">
      <d3p1:Unit>Degree</d3p1:Unit>
      <d3p1:Value>0</d3p1:Value>
    </Angle>
    <AngleNegativeTolerance xmlns:d3p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" z:Id="6">
      <d3p1:Unit>Degree</d3p1:Unit>
      <d3p1:Value>1</d3p1:Value>
    </AngleNegativeTolerance>
    <AnglePositiveTolerance xmlns:d3p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" z:Id="7">
      <d3p1:Unit>Degree</d3p1:Unit>
      <d3p1:Value>5</d3p1:Value>
    </AnglePositiveTolerance>
    <Depth xmlns:d3p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" z:Id="8">
      <d3p1:Unit>Millimeter</d3p1:Unit>
      <d3p1:Value>1</d3p1:Value>
    </Depth>
    <DepthPositiveTolerance xmlns:d3p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" z:Id="9">
      <d3p1:Unit>Millimeter</d3p1:Unit>
      <d3p1:Value>0.25</d3p1:Value>
    </DepthPositiveTolerance>
  </Notch>
  <SampleHeight xmlns:d2p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" i:nil="true" />
  <SampleWidth xmlns:d2p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" i:nil="true" />
  <WaferShape>Notch</WaferShape>
</WaferDimensionalCharacteristic>')
INSERT [dbo].[WaferCategory] ([Id], [Name], [XmlContent]) VALUES (2, N'Flat 100mm', N'<WaferDimensionalCharacteristic xmlns:i="http://www.w3.org/2001/XMLSchema-instance" z:Id="1" xmlns:z="http://schemas.microsoft.com/2003/10/Serialization/" xmlns="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Data">
  <Category z:Id="2">1.5/1.6</Category>
  <Diameter xmlns:d2p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" z:Id="3">
    <d2p1:Unit>Millimeter</d2p1:Unit>
    <d2p1:Value>100</d2p1:Value>
  </Diameter>
  <DiameterTolerance xmlns:d2p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" i:nil="true" />
  <Flats z:Id="4" z:Size="1">
    <FlatDimentionalCharacteristic z:Id="5">
      <Angle xmlns:d4p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" z:Id="6">
        <d4p1:Unit>Degree</d4p1:Unit>
        <d4p1:Value>0</d4p1:Value>
      </Angle>
      <AngleTolerance xmlns:d4p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" i:nil="true" />
      <ChordLength xmlns:d4p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" z:Id="7">
        <d4p1:Unit>Millimeter</d4p1:Unit>
        <d4p1:Value>32.5</d4p1:Value>
      </ChordLength>
      <ChordLengthTolerance xmlns:d4p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" i:nil="true" />
    </FlatDimentionalCharacteristic>
  </Flats>
  <Notch i:nil="true" />
  <SampleHeight xmlns:d2p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" i:nil="true" />
  <SampleWidth xmlns:d2p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" i:nil="true" />
  <WaferShape>Flat</WaferShape>
</WaferDimensionalCharacteristic>')
SET IDENTITY_INSERT [dbo].[WaferCategory] OFF
SET IDENTITY_INSERT [dbo].[Product] ON 

INSERT [dbo].[Product] ([Id], [Name], [WaferCategoryId], [Comment], [IsArchived]) VALUES (3, N'Notch 200', 1, N'default', 0)
INSERT [dbo].[Product] ([Id], [Name], [WaferCategoryId], [Comment], [IsArchived]) VALUES (4, N'Flat 100', 2, N'no comment', 0)
SET IDENTITY_INSERT [dbo].[Product] OFF
SET IDENTITY_INSERT [dbo].[Step] ON 

INSERT [dbo].[Step] ([Id], [Name], [ProductId], [Comment], [XmlContent], [IsArchived]) VALUES (1, N'StepSimu', 3, N'test simu', NULL, 0)
SET IDENTITY_INSERT [dbo].[Step] OFF
SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([Id], [Name], [ToolId], [IsArchived]) VALUES (1, N'User1', 3, 0)
SET IDENTITY_INSERT [dbo].[User] OFF
SET IDENTITY_INSERT [dbo].[Recipe] ON 

INSERT [dbo].[Recipe] ([Id], [KeyForAllVersion], [Name], [Comment], [Created], [IsArchived], [CreatorUserId], [XmlContent], [Version], [ActorType], [CreatorChamberId], [IsShared], [IsTemplate], [StepId], [IsValidated]) VALUES (2, N'fbdfbc2f-d18b-4042-8ff8-0243ad42a1bc', N'PSDSimu', N'PSD simulated recipe', CAST(N'2021-10-05 14:00:00.0000000' AS DateTime2), 0, 1, NULL, 1, 17, 1, 0, 0, 1, 1)
SET IDENTITY_INSERT [dbo].[Recipe] OFF
SET IDENTITY_INSERT [dbo].[Layer] ON 

INSERT [dbo].[Layer] ([Id], [Name], [MaterialId], [Thickness], [RefractiveIndex], [StepId]) VALUES (2, N'LayerSimu', NULL, 0, NULL, 1)
SET IDENTITY_INSERT [dbo].[Layer] OFF
SET IDENTITY_INSERT [dbo].[DatabaseVersion] ON 

INSERT [dbo].[DatabaseVersion] ([Id], [Version], [Date]) VALUES (1, N'8.0.0', CAST(N'2023-06-19 14:00:00.0000000' AS DateTime2))
SET IDENTITY_INSERT [dbo].[DatabaseVersion] OFF
INSERT [dbo].[GlobalResultSettings] ([Id], [ResultFormat], [Date], [DataSetting], [XmlSetting]) VALUES (0, 655360, CAST(N'2021-10-12 18:15:41.8380001' AS DateTime2), N'1-gtr', NULL)
SET IDENTITY_INSERT [dbo].[KlarfBinSettings] ON 

INSERT [dbo].[KlarfBinSettings] ([Id], [AreaIntervalMax], [SquareWidth]) VALUES (5, 10000, 1000)
INSERT [dbo].[KlarfBinSettings] ([Id], [AreaIntervalMax], [SquareWidth]) VALUES (19, 56250, 1225)
INSERT [dbo].[KlarfBinSettings] ([Id], [AreaIntervalMax], [SquareWidth]) VALUES (20, 100000, 1400)
INSERT [dbo].[KlarfBinSettings] ([Id], [AreaIntervalMax], [SquareWidth]) VALUES (21, 625000, 2125)
INSERT [dbo].[KlarfBinSettings] ([Id], [AreaIntervalMax], [SquareWidth]) VALUES (22, 2500000, 4000)
INSERT [dbo].[KlarfBinSettings] ([Id], [AreaIntervalMax], [SquareWidth]) VALUES (23, 5625000, 6500)
INSERT [dbo].[KlarfBinSettings] ([Id], [AreaIntervalMax], [SquareWidth]) VALUES (24, 10000000, 8500)
INSERT [dbo].[KlarfBinSettings] ([Id], [AreaIntervalMax], [SquareWidth]) VALUES (25, 250000000, 15000)
INSERT [dbo].[KlarfBinSettings] ([Id], [AreaIntervalMax], [SquareWidth]) VALUES (26, 562500000, 27500)
SET IDENTITY_INSERT [dbo].[KlarfBinSettings] OFF
SET IDENTITY_INSERT [dbo].[KlarfRoughSettings] ON 

INSERT [dbo].[KlarfRoughSettings] ([Id], [RoughBin], [Label], [Color]) VALUES (1, 666663, N'Def_666663', -3461055)
INSERT [dbo].[KlarfRoughSettings] ([Id], [RoughBin], [Label], [Color]) VALUES (2, 666662, N'Def_666662', -65281)
INSERT [dbo].[KlarfRoughSettings] ([Id], [RoughBin], [Label], [Color]) VALUES (3, 666661, N'Def_666661', -10636687)
INSERT [dbo].[KlarfRoughSettings] ([Id], [RoughBin], [Label], [Color]) VALUES (4, 666664, N'Def_666664', -2885998)
INSERT [dbo].[KlarfRoughSettings] ([Id], [RoughBin], [Label], [Color]) VALUES (5, 666665, N'Def_666665', -10062947)
INSERT [dbo].[KlarfRoughSettings] ([Id], [RoughBin], [Label], [Color]) VALUES (6, 666666, N'Def_666666', -14821207)
INSERT [dbo].[KlarfRoughSettings] ([Id], [RoughBin], [Label], [Color]) VALUES (7, 666660, N'Def_666660', -9435775)
INSERT [dbo].[KlarfRoughSettings] ([Id], [RoughBin], [Label], [Color]) VALUES (8, 201, N'Def_201', -256)
INSERT [dbo].[KlarfRoughSettings] ([Id], [RoughBin], [Label], [Color]) VALUES (9, 202, N'Def_202', -2469817)
INSERT [dbo].[KlarfRoughSettings] ([Id], [RoughBin], [Label], [Color]) VALUES (10, 203, N'Def_203', -12944665)
INSERT [dbo].[KlarfRoughSettings] ([Id], [RoughBin], [Label], [Color]) VALUES (11, 205, N'Def_205', -6357309)
INSERT [dbo].[KlarfRoughSettings] ([Id], [RoughBin], [Label], [Color]) VALUES (12, 204, N'Def_204', -6280003)
INSERT [dbo].[KlarfRoughSettings] ([Id], [RoughBin], [Label], [Color]) VALUES (13, 206, N'Def_206', -9126387)
INSERT [dbo].[KlarfRoughSettings] ([Id], [RoughBin], [Label], [Color]) VALUES (14, 207, N'Def_207', -15079999)
INSERT [dbo].[KlarfRoughSettings] ([Id], [RoughBin], [Label], [Color]) VALUES (15, 102, N'Def_102', -557909)
INSERT [dbo].[KlarfRoughSettings] ([Id], [RoughBin], [Label], [Color]) VALUES (16, 103, N'Def_103', -5834255)
INSERT [dbo].[KlarfRoughSettings] ([Id], [RoughBin], [Label], [Color]) VALUES (17, 0, N'Def_0', -15497692)
SET IDENTITY_INSERT [dbo].[KlarfRoughSettings] OFF

/* USE [UnityControlv8] 
GO */
SET IDENTITY_INSERT [dbo].[WaferCategory] ON 

INSERT [dbo].[WaferCategory] ([Id], [Name], [XmlContent]) VALUES (1, N'Notch 300mm', N'<WaferDimensionalCharacteristic xmlns:i="http://www.w3.org/2001/XMLSchema-instance" z:Id="1" xmlns:z="http://schemas.microsoft.com/2003/10/Serialization/" xmlns="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Data">
  <Category z:Id="2">1.15</Category>
  <Diameter xmlns:d2p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" z:Id="3">
    <d2p1:Unit>Millimeter</d2p1:Unit>
    <d2p1:Value>300</d2p1:Value>
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

INSERT [dbo].[WaferCategory] ([Id], [Name], [XmlContent]) VALUES (2, N'Notch 200mm', N'<WaferDimensionalCharacteristic xmlns:i="http://www.w3.org/2001/XMLSchema-instance" z:Id="1" xmlns:z="http://schemas.microsoft.com/2003/10/Serialization/" xmlns="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Data">
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

INSERT [dbo].[WaferCategory] ([Id], [Name], [XmlContent]) VALUES (3, N'Notch 150mm', N'<WaferDimensionalCharacteristic xmlns:i="http://www.w3.org/2001/XMLSchema-instance" z:Id="1" xmlns:z="http://schemas.microsoft.com/2003/10/Serialization/" xmlns="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Data">
  <Category z:Id="2">2.28</Category>
  <Diameter xmlns:d2p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" z:Id="3">
    <d2p1:Unit>Millimeter</d2p1:Unit>
    <d2p1:Value>150</d2p1:Value>
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

INSERT [dbo].[WaferCategory] ([Id], [Name], [XmlContent]) VALUES (4, N'Notch 100mm', N'<WaferDimensionalCharacteristic xmlns:i="http://www.w3.org/2001/XMLSchema-instance" z:Id="1" xmlns:z="http://schemas.microsoft.com/2003/10/Serialization/" xmlns="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Data">
  <Category z:Id="2">2.65</Category>
  <Diameter xmlns:d2p1="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Tools.Units" z:Id="3">
    <d2p1:Unit>Millimeter</d2p1:Unit>
    <d2p1:Value>100</d2p1:Value>
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

INSERT [dbo].[WaferCategory] ([Id], [Name], [XmlContent]) VALUES (5, N'Flat 100mm', N'<WaferDimensionalCharacteristic xmlns:i="http://www.w3.org/2001/XMLSchema-instance" z:Id="1" xmlns:z="http://schemas.microsoft.com/2003/10/Serialization/" xmlns="http://schemas.datacontract.org/2004/07/UnitySC.Shared.Data">
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


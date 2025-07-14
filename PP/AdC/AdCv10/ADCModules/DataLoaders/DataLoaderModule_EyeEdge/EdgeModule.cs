using System;
using System.Collections.Generic;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using BasicModules.DataLoader;

using UnitySC.Shared.Data.Enum;

namespace DataLoaderModule_EyeEdge
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class EdgeModule : MosaicDataLoaderBase
    {
        [System.Reflection.Obfuscation(Exclude = true)]
        public enum eImageType { TopSensor, TopBevelSensor, ApexSensor, BottomBevelSensor, BottomSensor };

        public override string DisplayName { get { return Factory.ModuleName + "-" + Id + "\n" + paramImageType.Value.ToString(); } }
        public override string DisplayNameInParamView { get { return Factory.Label + "\n" + paramImageType.Value.ToString(); } }
        public override string LayerName { get { return paramImageType.Value.ToString(); } }

        public override ActorType DataLoaderActorType => ActorType.Edge;
        public override IEnumerable<ResultType> CompatibleResultTypes => GetExpectedResultTypes();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly EnumParameter<eImageType> paramImageType;

        //=================================================================
        // Constructeur
        //=================================================================
        public EdgeModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramImageType = new EnumParameter<eImageType>(this, "Image Source");
        }

        //=================================================================
        // 
        //=================================================================
        public override MatrixBase CreateMatrix(MatrixInfo matrixinfo)
        {
            // Matrice Eye Edge
            //.................
            EyeEdgeMatrix matrix = new EyeEdgeMatrix();

            double π = Math.PI;
            switch (paramImageType.Value)
            {
                case eImageType.TopSensor:
                    matrix.SensorRadiusPosition = matrixinfo.SensorRadiusPosition;
                    matrix.SensorVerticalAngle = -π / 2;
                    break;
                case eImageType.TopBevelSensor:
                    matrix.SensorRadiusPosition = matrixinfo.SensorRadiusPosition;
                    matrix.SensorVerticalAngle = -π / 4;
                    break;
                case eImageType.ApexSensor:
                    // Le Sensor voit toujours le bord du Wafer
                    matrix.SensorRadiusPosition = ((NotchWafer)Recipe.Wafer).Diameter / 2;
                    matrix.SensorVerticalAngle = 0;
                    break;
                case eImageType.BottomBevelSensor:
                    matrix.SensorRadiusPosition = matrixinfo.SensorRadiusPosition;
                    matrix.SensorVerticalAngle = π / 4;
                    break;
                case eImageType.BottomSensor:
                    matrix.SensorRadiusPosition = matrixinfo.SensorRadiusPosition;
                    matrix.SensorVerticalAngle = π / 2;
                    break;
                default:
                    throw new ApplicationException("unkown image type: " + paramImageType.Value);
            }

            if (!(matrix.SensorRadiusPosition > 0))   // le test prend en compte les NaNs
                matrix.SensorRadiusPosition = ((NotchWafer)Recipe.Wafer).Diameter / 2;

            matrix.WaferPositionOnChuck.X = (float)matrixinfo.WaferPositionOnChuckX;
            matrix.WaferPositionOnChuck.Y = (float)matrixinfo.WaferPositionOnChuckY;
            matrix.WaferAngle = matrixinfo.AlignerAngleRadian;
            matrix.PixelSize.Width = (float)matrixinfo.PixelWidth;
            matrix.PixelSize.Height = (float)matrixinfo.PixelHeight;
            matrix.NotchY = matrixinfo.NotchY;
            matrix.WaferPositionCorrected = matrixinfo.WaferPositionCorrected;
            matrix.StartAngle = matrixinfo.AcquisitionStartAngle;
            matrix.ChuckOriginY = matrixinfo.ChuckOriginY;
            matrix.WaferDiameter = ((NotchWafer)Recipe.Wafer).Diameter;

            bool valid = matrix.Validate();
            if (!valid)
                throw new ApplicationException("Invalid matrix");

            return matrix;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();
            Layer.name = paramImageType.Value.ToString();
        }
        public override bool FilterImage(ResultType resultType, int NoCumn = -1)
        {
            if (resultType.GetActorType() != ActorType.Edge)
                return false;

            return GetExpectedResultTypes().Contains(resultType);      
        }

        private List<ResultType> GetExpectedResultTypes()
        {
            List<ResultType> expectedrestyp;
            switch (paramImageType.Value)
            {
                case eImageType.BottomSensor:
                    expectedrestyp = new List<ResultType>()
                    {
                        ResultType.NotDefined, // (int)eChannelID.EyeEdge_Bottom;
                    };
                    break;
                case eImageType.BottomBevelSensor:
                    expectedrestyp = new List<ResultType>()
                    {
                         ResultType.NotDefined, // (int)eChannelID.EyeEdge_BottomBevel;
                    };
                    break;
                case eImageType.ApexSensor:
                    expectedrestyp = new List<ResultType>()
                    {
                         ResultType.NotDefined, // (int)eChannelID.EyeEdge_Apex;
                    };
                    break;
                case eImageType.TopBevelSensor:
                    expectedrestyp = new List<ResultType>()
                    {
                        ResultType.NotDefined, // (int)eChannelID.EyeEdge_UpBevel;
                    };
                    break;
                case eImageType.TopSensor:
                    expectedrestyp = new List<ResultType>()
                    {
                        ResultType.NotDefined, // (int)eChannelID.EyeEdge_Up;
                    };
                    break;

                default:
                    throw new ApplicationException("unkown image type: " + paramImageType);
            }
            return expectedrestyp;
        }
    }
}

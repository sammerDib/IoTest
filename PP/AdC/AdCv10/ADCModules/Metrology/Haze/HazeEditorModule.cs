using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using ADCEngine;

using BasicModules.Edition.DataBase;

using OStorageTools.Ole;

namespace HazeModule
{
    // DARkVIEW HAZE VERSION

    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    [Obsolete("Only For DarkviewModule - use Haze LS otherwise", false)]
    public class HazeEditorModule : DatabaseEditionModule
    {
        private int wafersize_mm = 0;
        // Requested for Edition and registration matters
        protected override List<int> RegisteredResultTypes()
        {
            List<int> Rtypes = new List<int>(1);
            Rtypes.Add((int)ResultTypeFile.Haze_AZE);
            return Rtypes;
        }

        protected string _Filename = String.Empty;

        //=================================================================
        // Constructeur
        //=================================================================
        public HazeEditorModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            _Filename = GetResultFullPathName(ResultTypeFile.Haze_AZE);

            WaferBase wafer = Recipe.Wafer;
            if (Wafer is NotchWafer)
            {
                wafersize_mm = (int)((Wafer as NotchWafer).Diameter / 1000.0);
            }
            else if (Wafer is FlatWafer)
            {
                wafersize_mm = (int)((Wafer as FlatWafer).Diameter / 1000.0);

            }
            else if (Wafer is RectangularWafer)
            {
                float width_mm = (Wafer as RectangularWafer).Width / 1000.0f;
                float Height_mm = (Wafer as RectangularWafer).Height / 1000.0f;
                wafersize_mm = (int)(Math.Sqrt(width_mm * width_mm + Height_mm * Height_mm));
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("Process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            try
            {
                log("Creating Haze map: " + _Filename);
                WriteAze((HazeMeasure)obj);

                ResultState resstate = ResultState.Ok; // TO DO -- check grading reject , rework if exist, or partial result
                if (State == eModuleState.Aborting)
                    resstate = ResultState.Error;
                RegisterResultInDatabase(ResultTypeFile.Haze_AZE, resstate);
            }
            catch (Exception ex)
            {

                RegisterResultInDatabase(ResultTypeFile.Haze_AZE, ResultState.Error);
                string msg = "Haze map edition failed!" + _Filename;
                throw new ApplicationException(msg, ex);
            }
        }

        protected virtual void WriteAze(HazeMeasure p_Measure)
        {
            const int _FILE_AZE_CURRENT_VERSION = 2;

            // [ROOT]
            // ->FileVersion    (NEW since 20/06/2014) 
            // ->Size
            // ->WaferInfo      (NEW Since 30/03/2015)
            // ->Img
            // ->Msk      
            // ->Thumb_8
            // ->Thumb_256
            // ->SelHazeRange
            // ->ContainAreaData
            // ->[8bits]
            //   --> ColorMap
            //   --> Levels     (Modified since 07/08/2014)
            //   --> HazeArray
            //   --> HazePPM    (Modified since 20/06/2014) 
            //   --> HazeArea   (Modified since 20/06/2014) 
            // ->[256bits]
            //   --> ColorMap
            //   --> Levels     (Modified since 07/08/2014)
            //   --> HazeArray
            //   --> HazePPM    (Modified since 20/06/2014) 
            //   --> HazeArea   (Modified since 20/06/2014) 
            //->HazeRangeMax    (NEW since 20/06/2014) 

            if (File.Exists(_Filename))
            {
                logWarning("Haze Edition overwrite older result : " + _Filename);
                File.Delete(_Filename);
            }

            using (OleStorage storage = OleStorage.CreateWritableInstance(_Filename))
            {
                OleStream oStream = null;

                // FileVersion
                oStream = storage.CreateStream("FileVersion");
                oStream.WriteInt(_FILE_AZE_CURRENT_VERSION);
                oStream.Close();

                //Size
                oStream = storage.CreateStream("Size");
                oStream.WriteInt(p_Measure.SizeWidth);
                oStream.WriteInt(p_Measure.SizeHeight);
                oStream.Close();

                //WaferInfo
                oStream = storage.CreateStream("WaferInfo");
                oStream.WriteDouble(p_Measure.PixelSizeX);
                oStream.WriteDouble(p_Measure.PixelSizeY);
                oStream.WriteInt(wafersize_mm);
                oStream.WriteInt(p_Measure.EdgeExlusion_um / 1000);
                oStream.Close();

                //Img
                oStream = storage.CreateStream("Img");
                oStream.Write(p_Measure.InputImageGL);
                oStream.Close();

                //Msk
                oStream = storage.CreateStream("Msk");
                oStream.Write(p_Measure.InputWaferMask);
                oStream.Close();

                //thumbnail - 8 bits
                oStream = storage.CreateStream("Thumb_8");
                oStream.WriteInt(p_Measure.D8.ThumbSizeX);
                oStream.WriteInt(p_Measure.D8.ThumbSizeY);
                oStream.Write(p_Measure.D8.Thumbbail);

                //thumbnail - 256 bits
                oStream = storage.CreateStream("Thumb_256");
                oStream.WriteInt(p_Measure.D256.ThumbSizeX);
                oStream.WriteInt(p_Measure.D256.ThumbSizeY);
                oStream.Write(p_Measure.D256.Thumbbail);

                //SelHazeRange
                oStream = storage.CreateStream("SelHazeRange"); // OBSOLETE
                oStream.WriteInt(0); // for altaviewer retro-compatibility purpose only
                oStream.Close();

                //ContainAreaData
                oStream = storage.CreateStream("ContainAreaData"); // OBSOLETE
                oStream.WriteInt(0); // for altaviewer retro-compatibility purpose only
                oStream.Close();

                //---------------
                // Storage 8 bits
                //---------------
                using (OleStorage stg8bit = storage.CreateStorage("8bits"))
                {
                    //ColorMap
                    byte[] lutR = p_Measure.GetLut(HazeMeasure._R_, false);
                    byte[] lutG = p_Measure.GetLut(HazeMeasure._G_, false);
                    byte[] lutB = p_Measure.GetLut(HazeMeasure._B_, false);
                    byte[] colormap = new byte[256 * 3]; // pack RGB 24
                    for (int i = 0; i < 256; i++)
                    {
                        colormap[i * 3] = lutR[i];
                        colormap[i * 3 + 1] = lutG[i];
                        colormap[i * 3 + 2] = lutB[i];
                    }
                    oStream = stg8bit.CreateStream("ColorMap");
                    oStream.Write(colormap);
                    oStream.Close();
                    lutR = lutG = lutB = colormap = null;

                    //levels
                    oStream = stg8bit.CreateStream("Levels");
                    oStream.WriteInt(p_Measure.D8.Levels.Length);
                    foreach (int nlvl in p_Measure.D8.Levels)
                    {
                        oStream.WriteInt(nlvl);
                    }
                    oStream.Close();

                    //HazeArray
                    oStream = stg8bit.CreateStream("HazeArray");
                    for (int i = 0; i < 9; i++)
                    {
                        oStream.WriteInt((int)Math.Round(p_Measure.D8.HazeNbPixels[i]));
                        oStream.WriteFloat(p_Measure.D8.HazeAeraPct[i]);
                    }
                    oStream.Close();

                    // HazePPM
                    oStream = stg8bit.CreateStream("HazePPM");
                    oStream.WriteDouble((double)p_Measure.D8.Globalppm);
                    oStream.Close();
                }

                //-----------------
                // Storage 256 bits
                //-----------------
                using (OleStorage stg256bit = storage.CreateStorage("256bits"))
                {
                    //ColorMap
                    byte[] lutR = p_Measure.GetLut(HazeMeasure._R_, true);
                    byte[] lutG = p_Measure.GetLut(HazeMeasure._G_, true);
                    byte[] lutB = p_Measure.GetLut(HazeMeasure._B_, true);
                    byte[] colormap = new byte[256 * 3]; // pack RGB 24
                    for (int i = 0; i < 256; i++)
                    {
                        colormap[i * 3] = lutR[i];
                        colormap[i * 3 + 1] = lutG[i];
                        colormap[i * 3 + 2] = lutB[i];
                    }
                    oStream = stg256bit.CreateStream("ColorMap");
                    oStream.Write(colormap);
                    oStream.Close();
                    lutR = lutG = lutB = colormap = null;

                    //levels
                    oStream = stg256bit.CreateStream("Levels");
                    oStream.WriteInt(p_Measure.D256.Levels.Length);
                    foreach (int nlvl in p_Measure.D256.Levels)
                    {
                        oStream.WriteInt(nlvl);
                    }
                    oStream.Close();

                    //HazeArray
                    oStream = stg256bit.CreateStream("HazeArray");
                    for (int i = 0; i < 9; i++)
                    {
                        oStream.WriteInt((int)Math.Round(p_Measure.D256.HazeNbPixels[i]));
                        oStream.WriteFloat(p_Measure.D256.HazeAeraPct[i]);
                    }
                    oStream.Close();

                    // HazePPM
                    oStream = stg256bit.CreateStream("HazePPM");
                    oStream.WriteDouble((double)p_Measure.D256.Globalppm);
                    oStream.Close();
                }

                //HazeRangeMax
                oStream = storage.CreateStream("HazeRangeMax");
                oStream.WriteString(p_Measure.RangeName);  // this write An int contains string length * sizeof(char) +  array of char
                oStream.WriteInt(p_Measure.RangeScaleMax.Length);
                foreach (float fval in p_Measure.RangeScaleMax)
                {
                    oStream.WriteFloat(fval);
                }
                oStream.Close();
            }
        }
    }
}

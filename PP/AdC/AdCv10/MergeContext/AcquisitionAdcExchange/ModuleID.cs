using System;

namespace AcquisitionAdcExchange
{
    // Ces enum sont partagés entre l'acquisition et l'ADC, donc il ne faut
    // pas les changer, même si certains noms ne sont pas pertinents.

    //=================================================================
    /// <summary>
    /// Les des Modules d'acquisition.
    /// </summary>
	//=================================================================
    public enum eModuleID
    {
        /// <summary> Demeter </summary>
		Topography = 0,
        BrightField2D = 1,
        Darkfield = 2,
        BrightFieldPattern = 3,
        EyeEdge = 4,
        NanoTopography = 5,
        Lightspeed = 6,           // nouveau nom pour le velociraptor
        BrightField3D = 7,
        /// <summary> anciens Edge (3 sensors) </summary>
		EdgeInspect = 8,
        PL = 9
    }

    //=================================================================
    /// <summary>
    /// Nom des canaux des modules d'acquisition
    /// </summary>
    //=================================================================
    public enum eChannelID
    {
        //-------------------------------------------------------------
        // Channels for Topography (Demeter)
        //-------------------------------------------------------------
        DemeterDeflect_Front = 0,        // Deflectivity module image (amplitude + curvature on each axis (sinuosidal fringes) ) on FRONTSIDE module
        DemeterDeflect_Back = 1,         // Deflectivity module image (amplitude + curvature on each axis (sinuosidal fringes) ) on Backside module
        DemeterBrightfield_Front = 2,        // Reflection module image (a simple photo of reflection of wafer with a blank screen light) on FRONTSIDE module
        DemeterBrightfield_Back = 3,         // Reflection module image (a simple photo of reflection of wafer with a blank screen light) on Backside module

        DemeterDeflect_Die_Front = 4,    // Die to die
        DemeterDeflect_Die_Back = 5,
        DemeterReflect_Die_Front = 6,
        DemeterReflect_Die_Back = 7,

        DemeterDarkImage_Front = 8,        // Image Demeter en mode DarkField - FrontSide
        DemeterDarkImage_Back = 9,         // Image Demeter en mode DarkField - BackSide

        DemeterGlobalTopo_Front = 10,    // Global topography 3D heightmap Row/Warp measures - FrontSide
        DemeterGlobalTopo_Back = 11,     // Global topography 3D heightmap Row/Warp measures - BackSide

        DemeterObliqueLight_Front = 12, // Image Demeter en Reflectivité Custom (projection image sur ecran) - FrontSide
        DemeterObliqueLight_Back = 13,  // Image Demeter en Reflectivité Custom (projection image sur ecran) - BackSide

        DemeterPhaseX_Front = 14,
        DemeterPhaseY_Front = 15,
        DemeterPhaseX_Back = 16,
        DemeterPhaseY_Back = 17,

        //-------------------------------------------------------------
        // Channels for BrightField2D
        //-------------------------------------------------------------
        /// <summary> Mosaïque </summary>
        BrightField2D = 0,
        BrightField2D_FullImage = 1,
        BrightField2D_LowRes = 2,

        //-------------------------------------------------------------
        // Channels for Darkfield
        //-------------------------------------------------------------
        Darkfield = 0,

        //-------------------------------------------------------------
        // Channels for BrightFieldPattern
        //-------------------------------------------------------------
        BrightFieldPattern = 0,

        //-------------------------------------------------------------
        // Channels for BrightField3D
        //-------------------------------------------------------------
        BrightField3D_HighRes = 0,  // Mosaïque
        BrightField3D_DieToDie = 1,
        BrightField3D_LowRes = 2,

        //-------------------------------------------------------------
        // Channels for EyeEdge / EdgeInspect
        //-------------------------------------------------------------
        EyeEdge_Up = 0,
        EyeEdge_UpBevel = 1,
        EyeEdge_Apex = 2,
        EyeEdge_BottomBevel = 3,
        EyeEdge_Bottom = 4,

        //-------------------------------------------------------------
        // Channels for NanoTopography
        //-------------------------------------------------------------
        NanoTopography = 0,

        //-------------------------------------------------------------
        // Channels for Lightspeed
        //-------------------------------------------------------------
        Vraissemblance_Forward = 0,
        Vraissemblance_Backward = 1,
        Visibility_Forward = 2,
        Visibility_Backward = 3,
        Amplitude_Forward = 4,
        Amplitude_Backward = 5,
        Haze_Forward = 6,
        Haze_Backward = 7,
        Saturation_Forward = 8,
        Saturation_Backward = 9,
        Haze_Tot = 10,
        //-------------------------------------------------------------
        // Channels for SiC Photoluminescence (6" wafer = 12 captures with the setup we have 06/04/2023)
        // Only one channel I guess 
        //-------------------------------------------------------------
        PhotoluminescenceChannel_0 = 0,
        PhotoluminescenceChannel_1 = 1,
        PhotoluminescenceChannel_2 = 2,
        PhotoluminescenceChannel_3 = 3,


    }

    //=================================================================
    /// <summary>
    /// Nom des images d'un canal d'un module d'acquisition
    /// </summary>
    //=================================================================
    public enum eImageID
    {
        //-------------------------------------------------------------
        // Pour le PSD
        //-------------------------------------------------------------
        CurvatureX = 0,
        CurvatureY = 1,
        AmplitudeX = 2,
        AmplitudeY = 3,

        //-------------------------------------------------------------
        // Pour le DarkField
        //-------------------------------------------------------------
        A0picture = 0,
        A1picture = 1,
        R0picture = 2,
        R1picture = 3,
        //-------------------------------------------------------------
        // Images for SiC Photoluminescence (6" wafer = 12 captures with the setup we have 06/04/2023)
        //-------------------------------------------------------------
        Image0_0 = 0,
        Image0_1 = 1,
        Image0_2 = 2,
        Image1_0 = 3,
        Image1_1 = 4,
        Image1_2 = 5,
        Image2_0 = 6,
        Image2_1 = 7,
        Image2_2 = 8,
        Image3_0 = 9,
        Image3_1 = 10,
        Image3_2 = 11,
    }


    public static class ChannelHelper
    {
        public static string GetChannelName(eModuleID moduleId, eChannelID channelId)
        {
            string channelName = Enum.GetName(typeof(eChannelID), channelId);
            switch (moduleId)
            {
                //  Channels for Topography (Demeter)
                case eModuleID.Topography:
                    if (IsNotTopographyChannelId(channelId))
                    {
                        throw new ApplicationException($"invalid channel ID: {channelId:D} for module ID: {moduleId}");
                    }
                    break;
                //  Channels for BrightField2D
                case eModuleID.BrightField2D:
                    if (IsNotBrightfield2DChannelId(channelId))
                    {
                        throw new ApplicationException($"invalid channel ID: {channelId:D} for module ID: {moduleId}");
                    }
                    break;

                //  Channels for Darkfield
                case eModuleID.Darkfield:
                //  Channels for BrightFieldPattern
                case eModuleID.BrightFieldPattern:
                    channelName = Enum.GetName(typeof(eModuleID), channelId);
                    break;

                //  Channels for EyeEdge / EdgeInspect
                case eModuleID.EdgeInspect:
                case eModuleID.EyeEdge:
                    if (IsNotEyeEdgeEdgeInspectChannelId(channelId))
                    {
                        throw new ApplicationException($"invalid channel ID: {channelId:D} for module ID: {moduleId}");
                    }
                    break;

                //  Channels for NanoTopography
                case eModuleID.NanoTopography:
                    channelName = "NanoTopography";
                    break;

                //  Channels for Lightspeed
                case eModuleID.Lightspeed:
                    if (IsNotLightspeedChannelId(channelId))
                    {
                        throw new ApplicationException($"invalid channel ID: {channelId:D} for module ID: {moduleId}");
                    }
                    break;

                // Channels for BrightField3D
                case eModuleID.BrightField3D:
                    if (IsNotBrightfield3DChannelId(channelId))
                    {
                        throw new ApplicationException($"invalid channel ID: {channelId:D} for module ID: {moduleId}");
                    }
                    break;
                default:
                    throw new ApplicationException($"unkown module ID: {moduleId}");
            }

            return channelName;
        }

        public static string GetImageName(eModuleID moduleId, eImageID imageId)
        {
            string imageName = Enum.GetName(typeof(eImageID), imageId);
            switch (moduleId)
            {
                case eModuleID.Topography:
                    if (IsNotTopographyImageId(imageId))
                    {

                    }
                    break;
                case eModuleID.Darkfield:
                    if (IsNotDarkFieldImageId(imageId))
                    {
                        throw new ApplicationException($"unkown image type: {imageId:D}");
                    }
                    break;
                default:
                    throw new ApplicationException($"invalid Module ID: {moduleId} for image");
            }

            return imageName;
        }

        private static bool IsNotTopographyChannelId(eChannelID eChannelID)
        {
            return !(eChannelID == eChannelID.DemeterBrightfield_Back || eChannelID == eChannelID.DemeterBrightfield_Front ||
                eChannelID == eChannelID.DemeterDarkImage_Back || eChannelID == eChannelID.DemeterDarkImage_Front ||
                eChannelID == eChannelID.DemeterDeflect_Back || eChannelID == eChannelID.DemeterDeflect_Front ||
                eChannelID == eChannelID.DemeterGlobalTopo_Back || eChannelID == eChannelID.DemeterGlobalTopo_Front ||
                eChannelID == eChannelID.DemeterObliqueLight_Back || eChannelID == eChannelID.DemeterObliqueLight_Front ||
                eChannelID == eChannelID.DemeterPhaseX_Back || eChannelID == eChannelID.DemeterPhaseX_Front ||
                eChannelID == eChannelID.DemeterPhaseY_Back || eChannelID == eChannelID.DemeterPhaseY_Front);
        }

        private static bool IsNotBrightfield2DChannelId(eChannelID eChannelID)
        {
            return !(eChannelID == eChannelID.BrightField2D ||
                eChannelID == eChannelID.BrightField2D_FullImage ||
                eChannelID == eChannelID.BrightField2D_LowRes);
        }

        private static bool IsNotEyeEdgeEdgeInspectChannelId(eChannelID eChannelID)
        {
            return !(eChannelID == eChannelID.EyeEdge_Up || eChannelID == eChannelID.EyeEdge_UpBevel || eChannelID == eChannelID.EyeEdge_Apex ||
                eChannelID == eChannelID.EyeEdge_BottomBevel || eChannelID == eChannelID.EyeEdge_Bottom);
        }

        private static bool IsNotLightspeedChannelId(eChannelID eChannelID)
        {
            return !(eChannelID == eChannelID.Vraissemblance_Backward || eChannelID == eChannelID.Vraissemblance_Forward ||
                    eChannelID == eChannelID.Visibility_Forward || eChannelID == eChannelID.Visibility_Backward ||
                    eChannelID == eChannelID.Amplitude_Forward || eChannelID == eChannelID.Amplitude_Backward ||
                    eChannelID == eChannelID.Haze_Forward || eChannelID == eChannelID.Haze_Backward ||
                    eChannelID == eChannelID.Saturation_Forward || eChannelID == eChannelID.Saturation_Backward ||
                    eChannelID == eChannelID.Haze_Tot);
        }

        private static bool IsNotBrightfield3DChannelId(eChannelID eChannelID)
        {
            return !(eChannelID == eChannelID.BrightField3D_HighRes ||
                eChannelID == eChannelID.BrightField3D_DieToDie ||
                eChannelID == eChannelID.BrightField3D_LowRes);
        }

        private static bool IsNotTopographyImageId(eImageID eImageID)
        {
            return !(eImageID == eImageID.CurvatureX || eImageID == eImageID.CurvatureY ||
                eImageID == eImageID.AmplitudeX || eImageID == eImageID.AmplitudeY);
        }

        private static bool IsNotDarkFieldImageId(eImageID eImageID)
        {
            return !(eImageID == eImageID.A0picture || eImageID == eImageID.A1picture ||
                eImageID == eImageID.R0picture || eImageID == eImageID.R1picture);
        }

    }
}

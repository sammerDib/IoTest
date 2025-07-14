using System;
using System.Collections.Generic;

using AcquisitionAdcExchange;

using AdcTools;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    public abstract class LayerBase
    {
        public string name;
        public int Index;           // Index de la layer (0..N), ne pas confondre avec les index d'images
        public int MaxDataIndex;    // Max de l'index des images

        public WaferBase Wafer;
        public MatrixBase Matrix;

        /// <summary> Liste des Contextes-machine </summary>
        public Dictionary<string, object> ContextMachineMap = new Dictionary<string, object>();

        public CustomExceptionDictionary<LayerMetaData, string> MetaData = new CustomExceptionDictionary<LayerMetaData, string>() { ExceptionKeyName = "metadata" };


        //=================================================================
        //
        //=================================================================
        public override string ToString()
        {
            return name;
        }

        ///=================================================================
        ///<summary>
        /// Crée la liste des Contextes Machine dans la Layer
        ///</summary>
        ///=================================================================
        public void SetContextMachineFromInput(List<ContextMachine> contextMachineList)
        {
            foreach (ContextMachine ctx in contextMachineList)
                ContextMachineMap.Add(ctx.Type, ctx);
        }

        public T GetContextMachine<T>(string sConfigurationTypeKey) where T : Serializable
        {
            object ctx;
            bool b = ContextMachineMap.TryGetValue(sConfigurationTypeKey, out ctx);
            if (!b)
                throw new ApplicationException(String.Format("No Configuration Type <{0}> in Context Machine", sConfigurationTypeKey));

            T config = (T)((ContextMachine)ctx).Configuration;
            if (config == null)
                throw new ApplicationException(String.Format("Null Configuration <{0}> in Context Machine", sConfigurationTypeKey));

            return config;
        }

#if UNUSED
		//=================================================================
		//
		//=================================================================
		public static Layer CreateLayer(eModuleID moduleID, eChannelID channelD)
		{
			Layer layer = null;

			switch (moduleID)
			{
				case eModuleID.BrightField2D:
					switch (channelD)
					{
						case eChannelID.BrightField2D:
							layer = new MosaicLayer();
							break;
						case eChannelID.BrightField2D_FullImage:
						case eChannelID.BrightField2D_LowRes:
							layer = new FullImageLayer();
							break;
					}
					break;
				//------------
				case eModuleID.BrightField3D:
					switch (channelD)
					{
						case eChannelID.BrightField3D:
							layer = new Layer3D();
							break;
						case eChannelID.BrightField3D_DieToDie:
							layer = new DieLayer();
							break;
					}
					break;
				//------------
				case eModuleID.BrightFieldPattern:
					layer = new DieLayer();
					break;
				//------------
				case eModuleID.Darkfield:
					layer = new FullImageLayer();
					break;
				//------------
				case eModuleID.EyeEdge:
				case eModuleID.EdgeInspect:
					layer = new MosaicLayer();
					break;
				//------------
				case eModuleID.Topography:
					{
						switch (channelD)
						{
							case eChannelID.DemeterDeflect_Front:
							case eChannelID.DemeterDeflect_Back:
							case eChannelID.DemeterBrightfield_Front:
							case eChannelID.DemeterBrightfield_Back:
							case eChannelID.DemeterDarkImage_Front:
							case eChannelID.DemeterDarkImage_Back:
							case eChannelID.DemeterObliqueLight_Front:
							case eChannelID.DemeterObliqueLight_Back:
								layer = new FullImageLayer();
								break;
							case eChannelID.DemeterDeflect_Die_Front:
							case eChannelID.DemeterDeflect_Die_Back:
							case eChannelID.DemeterReflect_Die_Front:
							case eChannelID.DemeterReflect_Die_Back:
								layer = new DieLayer();
								break;
							case eChannelID.TopoGlobalTopo_Front:
							case eChannelID.TopoGlobalTopo_Back:
								layer = new FullImageLayer();
								break;
						}
					}
					break;
			}

			if (layer == null)
				throw new ApplicationException("unknown module/channel id (" + (int)moduleID + "/" + (int)channelD + ")");

			layer.moduleID = moduleID;
			layer.channelD = channelD;

			return layer;
		}
#endif

    }

}

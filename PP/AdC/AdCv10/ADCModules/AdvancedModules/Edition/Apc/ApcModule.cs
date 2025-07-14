using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using AdcRobotExchange;

using BasicModules.DataLoader;

using UnitySC.Shared.Data.Enum;

namespace AdvancedModules.Edition.Apc
{
    public class ApcModule : ModuleBase
    {
        public readonly ApcParameter paramApc;

        private int blobCount;
        private double hazeValue = double.NaN;

        //=================================================================
        // Constructeur
        //=================================================================
        public ApcModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramApc = new ApcParameter(this, "VID");
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("cluster " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            if (obj is Cluster)
            {
                Cluster cluster = (Cluster)obj;
                Interlocked.Add(ref blobCount, cluster.blobList.Count);
            }

            ProcessChildren(obj);
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            Scheduler.StartSingleTask("Apc",
                () =>
                {
                    try
                    {
                        ProcessApc();
                    }
                    catch (Exception ex)
                    {
                        string msg = "apc failed: " + ex.Message;
                        HandleException(new ApplicationException(msg, ex));
                    }
                    finally
                    {
                        base.OnStopping(oldState);
                    }
                });
        }

        //=================================================================
        // 
        //=================================================================
        private void ProcessApc()
        {
            VidApc vid = new VidApc();
            vid.VidNumber = paramApc.VidNumber;
            vid.VidLabel = paramApc.VidLabel;

            //-------------------------------------------------------------
            // Lecture des fichiers APC
            //-------------------------------------------------------------
            var inputs = Recipe.InputInfoList.Where(i => i is InspectionInputInfoBase);
            foreach (InspectionInputInfoBase input in inputs)
            {
                // Plusieurs inputs peuvent correspondre au même ActorTypeId (c'est à dire qu'on a plusieurs channel comme dans le PSD)
                // donc on vérifie si on a déjà traité le ActorTypeId
                int actorTypeid = (int)(input.ResultType.GetActorType());
                if (vid.Modules.Any(m => m.ActorTypeId == actorTypeid))
                    continue;

                // Y a-t-il un fichier APC ?
                string apcfilename;
                bool hasapc = input.MetaData.TryGetValue(AcquisitionAdcExchange.LayerMetaData.apcfile, out apcfilename);
                if (!hasapc)
                    continue;

                // Lecture du fichier APC
                VidApcModule vidApcModule = new VidApcModule();
                vidApcModule.ActorTypeId = actorTypeid;
                vidApcModule.Dictionary = LoadApcFile(apcfilename);
                vid.Modules.Add(vidApcModule);
            }

            //-------------------------------------------------------------
            // Report
            //-------------------------------------------------------------
            if (vid.Modules.Count == 0)
                throw new ApplicationException("No APC data");

            List<VidBase> list = new List<VidBase>() { vid };
            ADC.Instance.TransferToRobotStub.TransferVids(Recipe.Toolname, $"{Recipe.Wafer.GetWaferInfo(eWaferInfo.JobID)}{Recipe.Wafer.Basename}", list);
        }

        //=================================================================
        // 
        //=================================================================
        private Dictionary<string, double> LoadApcFile(string filename)
        {
            Dictionary<string, double> dico = new Dictionary<string, double>();

            // Lecture du fichier
            //...................
            XmlDocument xmldoc = new XmlDocument();
            logDebug("loading " + filename);
            xmldoc.Load(filename);

            // On cherche le noeud racine
            //...........................
            XmlNode root = null;
            foreach (XmlNode node in xmldoc.ChildNodes)
            {
                string name = node.Name;
                if (name == "APCReport" || name == "APCDMTReport" || name == "APCEdgeReport" || name == "APC2DReport" || name == "APC3DReport")
                    root = node;
            }
            if (root == null)
                throw new ApplicationException($"missing APC report node in {filename}");

            // Lecture des valeurs
            //....................
            foreach (XmlNode node in root.ChildNodes)
            {
                double value;
                bool ok = double.TryParse(node.InnerText, out value);
                if (ok)
                    dico[node.Name] = value;
            }

            if (dico.Count == 0)
                throw new ApplicationException($"No APC data in {filename}");

            // Et une petite bidouille qui vient de la V8
            //...........................................
            if (root.Name == "APCEdgeReport")
            {
                XmlNode node = root.SelectSingleNode("SensorName");
                int sensor = 0;

                switch (node.InnerText)
                {
                    case "TOP_SURFACE": sensor = 0; break;
                    case "TOP_BEVEL": sensor = 1; break;
                    case "APEX": sensor = 2; break;
                    case "BOTTOM_BEVEL": sensor = 3; break;
                    case "BOTTOM_SURFACE": sensor = 4; break;
                    default: sensor = -1; break;
                }

                dico["SensorName"] = sensor;
            }

            // On ajoute en plus des valeurs, même si elles sont déjà reportées ailleurs
            //..........................................................................
            dico["TotalDefect"] = blobCount;
            dico["HazeValue"] = hazeValue;

            return dico;
        }

    }
}

/*!
------------------------------------------------------------------------------
Unity-sc Technical Software Department
------------------------------------------------------------------------------
Copyright (c) 2017, Unity-sc.
611, rue Aristide Bergès  Z.A. de Pré Millet 38320 Montbonnot-Saint-Martin (France)
All rights reserved.
This source program is the property of Unity-sc Company and may not be copied
in any form or by any means, whether in part or in whole, except under license
expressly granted by Unity-sc company
All copies of this program, whether in part or in whole, and
whether modified or not, must display this and all other
embedded copyright and ownership notices in full.
------------------------------------------------------------------------------
Project : ADCEditor
Module : ServiceModule 
@file
@brief 
 This module is the interface between Adc and AdcEngine
 
@date 
@remarks
@todo
------------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;

using ADC.ViewModel;

using ADCEngine;

using BasicModules.DataLoader;

using GraphModel;

namespace ADC.Model
{
    internal class ServiceRecipe
    {

        private static ServiceRecipe _instance = null;


        public delegate void RecipeChangedEventHandler(Recipe r, EventArgs e);
        public event RecipeChangedEventHandler RecipeChanged;

        private bool _lockBeSaved = false;
        private bool _mustBeSaved;

        public bool MustBeSaved
        {
            get { return (bool)_mustBeSaved; }
            set
            {
                if (value && RecipeChanged != null && RecipeCurrent != null)
                    RecipeChanged(RecipeCurrent, new EventArgs());

                _mustBeSaved = value;
                if (value == false)
                    _lockBeSaved = false;
            }
        }

        private Recipe _recipeCurrent = null;
        public Recipe RecipeCurrent
        {
            get { return _recipeCurrent; }
            set { _recipeCurrent = value; }
        }


        public event EventHandler RecipeExecutedEventHandler;
        public void RaiseRecipeExecutedEventHandler(object sender, EventArgs e)
        {
            if (RecipeExecutedEventHandler != null)
                RecipeExecutedEventHandler(this, new EventArgs());
        }

        public static ServiceRecipe Instance()
        {
            return _instance ?? (_instance = new ServiceRecipe());
        }

        public ServiceRecipe()
        {
            SubscribeModulesSave();
        }

        /// <summary>
        /// Souscrit à l'évènement MustBeSaved
        /// </summary>
        private void SubscribeModulesSave()
        {
            ModuleBase.ParametersChanged += (m, e) =>
            {
                if (m.Recipe == RecipeCurrent)
                {
                    MustBeSaved = true;
                    if (((ADCEngine.ParamReportEvent)e).persistant == true)
                    {
                        _lockBeSaved = true;
                    }
                }
            };
        }

        public void CreateRecipe()
        {
            _recipeCurrent = new Recipe();

            MustBeSaved = true;
            _recipeCurrent.recipeExecutedEvent += RaiseRecipeExecutedEventHandler;
        }

        /// <summary>
        /// Crée un Module fils de parentNode
        /// </summary>
        /// <returns></returns>
        public ModuleBase AddChild(ModuleBase parent, ModuleBase child)
        {
            if (parent != null)
                _recipeCurrent.AddModule(parent, child);
            else
                _recipeCurrent.AddModule(null, child);

            MustBeSaved = true;
            return child;
        }

        /// <summary>
        /// Merge graph in the Graph Adc
        /// </summary>
        /// <param name="graph"></param>
        public void MergeGraph(GraphViewModel graph, bool newId = true)
        {
            foreach (ModuleNodeViewModel node in graph.Nodes)
            {
                if (newId)
                    node.Module.Id = RecipeCurrent.GetNewId();

                AddChild(null, node.Module);
            }

            // Connect new node with childs
            foreach (ModuleNodeViewModel node in graph.Nodes)
            {
                List<NodeViewModel> listChilds = graph.GetChilds(node);
                // Connect new node with childs
                foreach (ModuleNodeViewModel childNode in listChilds)
                    ConnectModules(node.Module, childNode.Module);
            }
        }


        /// <summary>
        /// Supprime un module du graph Adc
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public void RemoveModule(ModuleBase module)
        {
            _recipeCurrent.DeleteModule(module);
            //Note FDE TODO pourquoi on n'enlève pas le module du CompatibilityManager ?
            MustBeSaved = true;
        }

        /// <summary>
        /// Connecte 2 modules
        /// </summary>
        public void ConnectModules(ModuleBase parentModule, ModuleBase childModule)
        {
            _recipeCurrent.ConnectModules(parentModule, childModule);

            CompatibilityManager compman = _recipeCurrent.CompatibilityManager;
            compman.AddTree(childModule);

            MustBeSaved = true;
        }

        /// <summary>
        /// Supprime la connexion entre 2 modules
        /// </summary>
        public void DisconnectModules(ModuleBase parentModule, ModuleBase childModule)
        {
            _recipeCurrent.DisconnectModules(parentModule, childModule);

            CompatibilityManager compman = _recipeCurrent.CompatibilityManager;
            compman.AddTree(childModule);

            MustBeSaved = true;
        }

        /// <summary>
        /// Charge une recette et génère le graphe correspondant
        /// </summary>
        /// <param name="xmldoc"></param>
        public Dictionary<int, Point> LoadRecipe(XmlDocument xmldoc)
        {
            _recipeCurrent = new Recipe();
            _recipeCurrent.Load(xmldoc);

            if (!_lockBeSaved)
                MustBeSaved = false;
            _recipeCurrent.recipeExecutedEvent += RaiseRecipeExecutedEventHandler;

            Dictionary<int, Point> posModulelist = LoadNodeslocation(xmldoc);
            return posModulelist;
        }

        private Dictionary<int, Point> LoadNodeslocation(XmlDocument xmldoc)
        {
            Dictionary<int, Point> posModulelist = new Dictionary<int, Point>();
            XmlNode nodes = xmldoc.SelectSingleNode(".//GraphView");

            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    int modId = int.Parse(node.Attributes["ModID"].Value);
                    Point nodePos = new Point();
                    nodePos.X = double.Parse(node.Attributes["X"].Value);
                    nodePos.Y = double.Parse(node.Attributes["Y"].Value);
                    posModulelist.Add(modId, nodePos);
                }
            }

            return posModulelist;
        }

        public void SaveRecipe(String fileName)
        {
            if (_recipeCurrent != null)
                _recipeCurrent.Save(fileName);
            MustBeSaved = false;
        }


        public void SaveMetaBlock(List<ModuleNodeViewModel> Metablock, String fileName)
        {
            List<int> listIdModules = new List<int>();

            foreach (ModuleNodeViewModel node in Metablock)
                listIdModules.Add(node.Module.Id);

            _recipeCurrent.SaveMetaBlock(listIdModules, fileName);

        }

        /// <summary>
        /// Charge les modules du méta bloc
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="metablock"></param>
        public void LoadMetaBlock(String filename, ref List<ModuleBase> metablock)
        {
            _recipeCurrent.LoadMetaBlock(filename, ref metablock);
        }


        public void CloseRecipe()
        {
            if (_recipeCurrent != null)
            {
                _recipeCurrent.recipeExecutedEvent -= RaiseRecipeExecutedEventHandler;
                _recipeCurrent.Dispose();
            }
            _recipeCurrent = null;
            MustBeSaved = false;
        }

        public bool ValidateInputData()
        {
            if (!_recipeCurrent.IsMerged)
                return true;

            // On verifie la présence de tous les fichiers définis dans InputDataList et du repertoire de sortie 
            return _recipeCurrent.InputInfoList.OfType<InspectionInputInfoBase>().SelectMany(i => i.InputDataList).All(d => File.Exists(d.Filename))
                && Directory.Exists(_recipeCurrent.OutputDir);
        }

    }
}

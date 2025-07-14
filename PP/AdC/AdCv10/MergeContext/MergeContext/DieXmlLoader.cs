using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

using AcquisitionAdcExchange;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace MergeContext
{
    ///////////////////////////////////////////////////////////////////////
    // Classe chargée de trouver les dies à partir du fichier die.xml
    ///////////////////////////////////////////////////////////////////////
    internal class DieXmlLoader
    {
        //=================================================================
        // Propriétés
        //=================================================================
        private ResultType _resulType; 
        private int _toolKey;
        private int _chamberKey;

        public string Basename = "CutUp_DieTo";    // nom de base des fichiers die
        public PathString diesxml;
        public PathString DieDirectory;

        // Dimension de la grille de die
        public Rectangle DieIndexes = new Rectangle(0, 0, 1, 1);

        // Liste des "images" de l'acquisition
        public List<AcquisitionDieImage> DieList { get; } = new List<AcquisitionDieImage>();

        // Calibration de l'acquisition
        public int TotalImageSizeX;
        public int TotalImageSizeY;
        public double PixelSizeX;
        public double PixelSizeY;
        public double WaferCenterX;
        public double WaferCenterY;
        public double Die00ToWaferCenterX;
        public double Die00ToWaferCenterY;
        public double CutStepX;
        public double CutStepY;
        public double DieSizeX;
        public double DieSizeY;

        //=================================================================
        // Constructeur
        //=================================================================
        public DieXmlLoader(ResultType restyp, int toolKey, int chamberKey)
        {
            _resulType = restyp;
            _toolKey = toolKey;
            _chamberKey = chamberKey;
        }

        //=================================================================
        // Chargement de la liste des dies à partir du fichier _dies.xml
        // @param diesxml: chemin du fichier _dies.xml généré par CutUp
        // @param basename: base du nom des fichiers die, en général "CutUp_DieTo"
        //=================================================================
        public void ReadDieList(PathString diesxml, string basename, string ext)
        {
            this.diesxml = diesxml;
            Basename = basename;
            DieDirectory = diesxml.Directory;
            DieList.Clear();

            //-------------------------------------------------------------
            // Lecture de la liste des dies dans le fichiers _dies.xml
            //-------------------------------------------------------------
            XmlDocument doc = new XmlDocument();
            doc.Load(diesxml);
            XmlNodeList nodes = doc.SelectNodes(".//Die");

            foreach (XmlNode node in nodes)
            {
                AcquisitionDieImage die = LoadDieInfo(node);
                DieList.Add(die);
            }

            // Sanity Check
            //.............
            if (DieList.Count() == 0)
                throw new ApplicationException("no die found in \"" + diesxml + "\"");

            // Lecture de la calibration de l'acquisition
            //...........................................
            TotalImageSizeX = XML.GetIntValue(doc, "TotalImageSizeX");
            TotalImageSizeY = XML.GetIntValue(doc, "TotalImageSizeY");
            PixelSizeX = XML.GetDoubleValue(doc, "PixelSizeX");
            PixelSizeY = XML.GetDoubleValue(doc, "PixelSizeY");
            WaferCenterX = XML.GetDoubleValue(doc, "WaferCenterX");
            WaferCenterY = XML.GetDoubleValue(doc, "WaferCenterY");

            Die00ToWaferCenterX = XML.GetDoubleValue(doc, "Die00ToWaferCenterX");
            Die00ToWaferCenterY = XML.GetDoubleValue(doc, "Die00ToWaferCenterY");
            CutStepX = XML.GetDoubleValue(doc, "CutStepX");
            CutStepY = XML.GetDoubleValue(doc, "CutStepY");
            DieSizeX = XML.GetDoubleValue(doc, "DieSizeX");
            DieSizeY = XML.GetDoubleValue(doc, "DieSizeY");

            //-------------------------------------------------------------
            // Lecture de la liste des images sur le disque
            //-------------------------------------------------------------
            EnumerateDieFiles(diesxml.Directory, basename, ext);
            foreach (AcquisitionDieImage die in DieList)
            {
                if (die.Filename == null)
                    throw new ApplicationException("cannot find image corresponding to die x=" + die.IndexX + " y=" + die.IndexY);
            }

            //-------------------------------------------------------------
            // Calcul des indexes min/max
            //-------------------------------------------------------------
            DieIndexes = ComputeDieIndexes();
        }

        //=================================================================
        // Crée la liste des images de die présentes dans le répertoire
        //=================================================================
        private void EnumerateDieFiles(string folder, string basename, string ext)
        {
            List<AcquisitionDieImage> list = new List<AcquisitionDieImage>();

            //-------------------------------------------------------------
            // Lecture de la liste des images sur le disque
            //-------------------------------------------------------------
            string template = "*" + ext;
            var diefilenamesTemp = Directory.EnumerateFiles(folder, template);

            //-------------------------------------------------------------
            // Info sur chaque image
            //-------------------------------------------------------------
            foreach (PathString diepath in diefilenamesTemp)
            {
                int x = 0, y = 0;

                bool is_die = true;
                string[] items = diepath.Basename.Split('_');
                is_die = is_die && (items.Length >= 2);
                is_die = is_die && int.TryParse(items[items.Length - 2], out x);
                is_die = is_die && int.TryParse(items.Last(), out y);

                if (is_die)
                {
                    AcquisitionDieImage die = DieList.Find(
                                    d => (d.IndexX == x && d.IndexY == y)
                                    );
                    if (die != null)
                        die.Filename = diepath;
                }
            }
        }

        //=================================================================
        // Lecture des infos d'un die dans le fichier _dies.xml
        //=================================================================
        private AcquisitionDieImage LoadDieInfo(XmlNode dieNode)
        {
            AcquisitionDieImage die = new AcquisitionDieImage();
            die.ResultType = _resulType;
            die.ToolKey = _toolKey;
            die.ChamberKey = _chamberKey;   

            die.IndexX = dieNode.GetIntValue("IndexX");
            die.IndexY = dieNode.GetIntValue("IndexY");

            die.X = dieNode.GetIntValue("X");
            die.Y = dieNode.GetIntValue("Y");
            die.Width = dieNode.GetIntValue("W");
            die.Height = dieNode.GetIntValue("H");

            return die;
        }

        //=================================================================
        // Cherche un die x,y dans la liste des fichiers présents sur le disque
        //=================================================================
        private string FindDieFilename(List<AcquisitionDieImage> diefiles, int x, int y)
        {
            AcquisitionData die = diefiles.First(d => (d.IndexX == x && d.IndexY == y));
            if (die == null)
                throw new ApplicationException("cannot find image file for die indexX=" + x + " indexY=" + y);

            return die.Filename;
        }

        //=================================================================
        // Calcul des indexes min/max des dies
        //=================================================================
        private Rectangle ComputeDieIndexes()
        {
            int minx = DieList.Min(d => d.IndexX);
            int maxx = DieList.Max(d => d.IndexX);
            int miny = DieList.Min(d => d.IndexY);
            int maxy = DieList.Max(d => d.IndexY);

            Rectangle dieIndexes = Rectangle.FromLTRB(minx, miny, maxx, maxy);
            return dieIndexes;
        }
    }
}

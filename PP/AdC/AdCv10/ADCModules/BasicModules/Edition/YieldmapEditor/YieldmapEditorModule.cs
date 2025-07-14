using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules.DataLoader;
using BasicModules.Edition.DataBase;

using UnitySC.Shared.Tools;

namespace BasicModules.YieldmapEditor
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    public class YieldmapEditorModule : DatabaseEditionModule
    {
        private enum eDieState
        {
            NotTested = -1,
            Good = 0,
            /// <summary> Le die a des défauts mais n'est pas mort </summary>
            HasDefects = 1,
            Dead = 2
        }

        private class DieDescriptor
        {
            public int IndexX { get; set; }
            public int IndexY { get; set; }
            public RectangleF MicronRect;

            public eDieState Status = eDieState.Good;
            public int DefectCount;
        }


        public PathString BaseFilename { get; set; }
        public readonly YieldEditorKillerDefectParameter paramDefectKillerStatus;

        private Dictionary<Tuple<int, int>, DieDescriptor> DieDescriptors = new Dictionary<Tuple<int, int>, DieDescriptor>();

        private float _FontSize = 10.0f;
        private bool _bDrawVertcal = false;

        //-----------------------------------------------------------------
        // Variables utilisées pendant le dessin de la Yield Map
        //-----------------------------------------------------------------
        private RectangularMatrix YldMapMatrix;


        //=================================================================
        // Constructeur
        //=================================================================
        public YieldmapEditorModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            BaseFilename = String.Empty;

            paramDefectKillerStatus = new YieldEditorKillerDefectParameter(this, "DefectKillerStatus");
        }

        //=================================================================
        /// <summary>
        /// Requested for Edition and registration matters
        /// </summary>
        //=================================================================
        protected override List<int> RegisteredResultTypes()
        {
            List<int> Rtypes = new List<int>(1);
            Rtypes.Add((int)ResultTypeFile.YieldMap_YLD);
            return Rtypes;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            BaseFilename = GetResultFullPathName(ResultTypeFile.YieldMap_YLD);
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            //-------------------------------------------------------------
            // Récuperation du cluster / Descripteur de Die
            //-------------------------------------------------------------
            Cluster cluster = (Cluster)obj;
            if (cluster == null || cluster.blobList == null || cluster.blobList.Count == 0)
                throw new ApplicationException("Empty Cluster");

            DieDescriptor die = GetOrCreateDieDescriptor(cluster.DieIndexX, cluster.DieIndexY);

            //-------------------------------------------------------------
            // Killer ou pas ?
            //-------------------------------------------------------------
            lock (die)
            {
                if (die.Status == eDieState.Dead)
                {
                    // nothing more to do, die die is already dead
                    return;
                }

                object val;
                if (cluster.characteristics.TryGetValue(Cluster3DCharacteristics.isFailure, out val))
                {
                    bool bIsFail = (bool)val;
                    die.Status = bIsFail ? eDieState.Dead : eDieState.Good;
                }
                else if (cluster.characteristics.TryGetValue(Cluster2DCharacteristics.isFailure, out val))
                {
                    bool bIsFail = (bool)val;
                    die.Status = bIsFail ? eDieState.Dead : eDieState.Good;
                }
                else
                {
                    die.DefectCount++;
                    int killerStatusNum = paramDefectKillerStatus.DefectKillerStatus[cluster.DefectClass].KillerStatusNum;
                    if (killerStatusNum > 0 && die.DefectCount >= killerStatusNum)
                        die.Status = eDieState.Dead;
                    else
                        die.Status = eDieState.HasDefects;
                }
            }

            Interlocked.Increment(ref nbObjectsOut);
        }

        //=================================================================
        // 
        //=================================================================
        private DieDescriptor GetOrCreateDieDescriptor(int indexX, int indexY)
        {
            Tuple<int, int> dieIndex = new Tuple<int, int>(indexX, indexY);
            lock (DieDescriptors)
            {
                DieDescriptor die;
                bool exits = DieDescriptors.TryGetValue(dieIndex, out die);

                if (!exits)
                {
                    die = new DieDescriptor();
                    DieDescriptors.Add(dieIndex, die);
                }

                return die;
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            Scheduler.StartSingleTask("ProcessYieldMap", () =>
            {
                try
                {
                    if (oldState == eModuleState.Running)
                    {
                        ProcessYieldMap();

                        ResultState resstate = ResultState.Ok; // TO DO -- check grading reject , rework if exist, or partial result
                        if (State == eModuleState.Aborting)
                            resstate = ResultState.Error;
                        RegisterResultInDatabase(ResultTypeFile.YieldMap_YLD, resstate);
                    }
                    else if (oldState == eModuleState.Aborting)
                        RegisterResultInDatabase(ResultTypeFile.YieldMap_YLD, ResultState.Error);

                }
                catch (Exception ex)
                {
                    RegisterResultInDatabase(ResultTypeFile.YieldMap_YLD, ResultState.Error);
                    string msg = "YLD generation failed: " + ex.Message;
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
        private void ProcessYieldMap()
        {
            // Finalisation de la YieldMap
            //............................
            CompleteDieList();

            // Ecriture des résultats
            //.......................
            PathString sYLDFullPathFile = BaseFilename;
            PathString sCSVFullPathFile = (sYLDFullPathFile.ToString()).Replace(Extentions[(int)ResultTypeFile.YieldMap_YLD], ".csv");

            // compute csv file and write on disk
            WriteCSV(sCSVFullPathFile);

            // DRAW IMAGE AND SAVE ON DISK !!
            WriteYLD(sYLDFullPathFile);
            log("YLD generated: " + sYLDFullPathFile);

        }

        //=================================================================
        // 
        //=================================================================
        private void CompleteDieList()
        {
            //-------------------------------------------------------------
            // Liste des dies de la Layer
            //-------------------------------------------------------------
            List<ModuleBase> AncestorDieLoadermodule = FindAncestors(mod => mod is DieDataLoaderBase);
            if (AncestorDieLoadermodule.Count == 0)
            {
                throw new ApplicationException("No Dieloader module has been set prior to this module");
            }

            DieDataLoaderBase DirectAncestor = AncestorDieLoadermodule[0] as DieDataLoaderBase;
            DieLayer mydielayer = ((DirectAncestor.Layer) as DieLayer);

            IEnumerable<DieImage> dieImageList = mydielayer.GetDieImageList();

            //-------------------------------------------------------------
            // Mise à jour de la liste des Descripteurs de Die
            //-------------------------------------------------------------
            foreach (DieImage dieImage in dieImageList)
            {
                // Récupération du descripteur
                //............................
                DieDescriptor die = GetOrCreateDieDescriptor(dieImage.DieIndexX, dieImage.DieIndexY);
                die.IndexX = dieImage.DieIndexX;
                die.IndexY = dieImage.DieIndexY;

                // On ne modifie pas le status car il a déjà été calculé lors du process

                // Calcul du rectangle du die
                //...........................
                // Les dies sont toujours inspectés à 0° donc on peut prendre le rectangle englobant
                QuadF micronQuad = mydielayer.Matrix.pixelToMicron(dieImage.imageRect);
                die.MicronRect = micronQuad.SurroundingRectangle;
            }
        }

        //=================================================================
        // 
        //=================================================================
        private void WriteCSV(PathString p_FilePath_CSV)
        {
            long lCountDie_Good = 0;
            long lCountDie_Death = 0;
            long lCountDie_Tested = DieDescriptors.Count;

            StringBuilder sb = new StringBuilder(1000000);
            foreach (DieDescriptor die in DieDescriptors.Values)
            {
                if (die.Status == eDieState.NotTested)
                {
                    lCountDie_Tested--;
                }
                else
                {
                    sb.Append(die.IndexX.ToString());
                    sb.Append("\t");
                    sb.Append(die.IndexY.ToString());
                    sb.Append("\t");

                    switch (die.Status)
                    {
                        //---
                        case eDieState.Good:
                            ++lCountDie_Good;
                            sb.AppendLine("OK");
                            break;
                        //---
                        case eDieState.HasDefects:
                            sb.AppendLine("Unclassified");
                            break;
                        //---
                        case eDieState.Dead:
                            ++lCountDie_Death;
                            sb.AppendLine("KO");
                            break;
                        default:
                            break;
                    }
                }
            }

            long lCountDie_Unclassified = lCountDie_Tested - lCountDie_Good - lCountDie_Death;

            double lfGoodDieInPerCent = ((double)lCountDie_Good / (double)lCountDie_Tested) * 100.0;
            double lfDeathDieInPerCent = ((double)lCountDie_Death / (double)lCountDie_Tested) * 100.0;
            double lfUnclassDieInPerCent = ((double)lCountDie_Unclassified / (double)lCountDie_Tested) * 100.0;

            //using (StreamWriter outStream = new StreamWriter(p_FilePath_CSV))
            //{
            //    outStream.WriteLine(String.Format("### Nb Die Tested / Nb Die Layout == {0} / {1} ", lCountDie_Tested, DieDescriptors.Count));

            //    outStream.WriteLine(String.Format("Nb Die OK = {0}", lCountDie_Good));
            //    outStream.WriteLine(String.Format("Nb Die KO = {0}", lCountDie_Death));
            //    outStream.WriteLine(String.Format("Nb Die unclassified = {0}", lCountDie_Unclassified));

            //    outStream.WriteLine(String.Format("% Die OK = {0:#0.0000}", lfGoodDieInPerCent));
            //    outStream.WriteLine(String.Format("% Die KO = {0:#0.0000}", lfDeathDieInPerCent));
            //    outStream.WriteLine(String.Format("% Die unclassified =  {0:#0.0000}", lfUnclassDieInPerCent));

            //    outStream.Write(sb.ToString());

            //    outStream.Close();
            //}
            sb = null;
        }

        //=================================================================
        // 
        //=================================================================
        private void WriteYLD(PathString filename)
        {
            Image image = AllocateImage();
            using (Graphics graphics = Graphics.FromImage(image))
            {
                // Fond
                graphics.FillRectangle(Brushes.Black, 0, 0, image.Width, image.Height);

                // Wafer
                if (Wafer is RectangularWafer)
                    DrawRectangularWafer(graphics);
                else if (Wafer is NotchWafer)
                    DrawNotchWafer(graphics);
                else if (Wafer is FlatWafer)
                    DrawFlatWafer(graphics);
                else
                    throw new ApplicationException("Unknown wafer type: " + Wafer.GetType());

                InitFontSize(graphics);

                // Dies
                DrawDies(graphics);
            }

            image.Save(filename, ImageFormat.Png);
        }


        //=================================================================
        // 
        //=================================================================
        private Image AllocateImage()
        {
            float yldPixelSize;
            int width;
            int height;

            //-------------------------------------------------------------
            // Calcul de la taille
            //-------------------------------------------------------------
            if (Wafer is RectangularWafer)
            {
                RectangularWafer rectWafer = Wafer as RectangularWafer;

                int margin = 50;
                yldPixelSize = 16.0f;
                width = (int)(rectWafer.Width / yldPixelSize) + 2 * margin;
                height = (int)(rectWafer.Height / yldPixelSize) + 2 * margin;
            }
            else
            {
                // on calcule le pixel size de display pour une taille d'image resultante fixée. (4000px parait suffisant pour les die tout petit vu en led max)
                int yldWaferSize = 4000;                    // note de RTI : doit on rendre ce paramètre ... parametrable ? si oui où ?
                int margin = 99;                            // note de RTI : doit on rendre ce paramètre ... parametrable ? si oui où ?
                width = height = yldWaferSize + 2 * margin; // on ajoute un incertitude de diametre plus grande sur notre diametre afin d'éviter que les die ne sorte visuellement de notre cercle wafer

                yldPixelSize = (float)Wafer.SurroundingRectangle.Width / yldWaferSize;
            }

            //-------------------------------------------------------------
            // Alloue une image
            //-------------------------------------------------------------
            System.Drawing.Image bitmap = new Bitmap(width, height);

            //-------------------------------------------------------------
            // Create a matrix to transfrom from microns to viewer pixels
            //-------------------------------------------------------------
            Point waferCenter = new Point(width / 2, height / 2);
            YldMapMatrix = new RectangularMatrix();
            YldMapMatrix.Init(waferCenter, new SizeF(yldPixelSize, yldPixelSize));

            return bitmap;
        }

        private void InitFontSize(Graphics graphics)
        {

            DieDescriptor firstdie = DieDescriptors.First().Value;
            Rectangle rect = YldMapMatrix.micronToPixel(firstdie.MicronRect);
            String sToMeasure = "-000/-000";
            float MinftSize = 6.0f;

            float apsectRatio = (float)rect.Height / (float)rect.Width;
            //   float dfontsize = 0.5f * Math.Min(rect.Width, rect.Height);
            if (apsectRatio >= 5.0f)
            {
                _bDrawVertcal = true;

                float dfontsize = 0.5f * rect.Width;
                dfontsize = (float)Math.Ceiling(dfontsize);
                SizeF sizeft = graphics.MeasureString(sToMeasure, new Font("Arial", dfontsize, FontStyle.Regular, GraphicsUnit.Point));
                while (sizeft.Height > rect.Width || sizeft.Width > rect.Height)
                {
                    dfontsize -= 0.5f;
                    if (dfontsize < MinftSize)
                    {
                        dfontsize = MinftSize;
                        break;
                    }
                    sizeft = graphics.MeasureString(sToMeasure, new Font("Arial", dfontsize, FontStyle.Regular, GraphicsUnit.Point));
                }
                _FontSize = dfontsize;
            }
            else
            {
                _bDrawVertcal = false;

                float dfontsize = 0.5f * rect.Height;
                dfontsize = (float)Math.Ceiling(dfontsize);
                SizeF sizeft = graphics.MeasureString(sToMeasure, new Font("Arial", dfontsize, FontStyle.Regular, GraphicsUnit.Point));
                while (sizeft.Width > rect.Width || sizeft.Height > rect.Height)
                {
                    dfontsize -= 0.5f;
                    if (dfontsize < MinftSize)
                    {
                        dfontsize = MinftSize;
                        break;
                    }
                    sizeft = graphics.MeasureString(sToMeasure, new Font("Arial", dfontsize, FontStyle.Regular, GraphicsUnit.Point));
                }
                _FontSize = dfontsize;
            }
        }


        //=================================================================
        // 
        //=================================================================
        private void DrawRectangularWafer(Graphics graphics)
        {
            RectangularWafer rctwfer = Wafer as RectangularWafer;

            Rectangle rect = YldMapMatrix.micronToPixel(rctwfer.SurroundingRectangle);
            Rectangle rectExt = rect;
            rectExt.Inflate(2, 2);

            // Pourtour du Wafer
            Brush brushLiteblk = new SolidBrush(Color.FromArgb(255, (byte)40, (byte)40, (byte)40));
            graphics.FillRectangle(brushLiteblk, rectExt);
            // Intérieur du Wafer
            graphics.FillRectangle(Brushes.Gray, rect);
        }

        //=================================================================
        // 
        //=================================================================
        private void DrawNotchWafer(Graphics graphics)
        {
            NotchWafer notchWafer = Wafer as NotchWafer;

            //-------------------------------------------------------------
            // Dessin du disque
            //-------------------------------------------------------------
            Rectangle rect = YldMapMatrix.micronToPixel(notchWafer.SurroundingRectangle);
            graphics.FillEllipse(Brushes.Gray, rect);

            //-------------------------------------------------------------
            // on dessine le pseudo notch -+- approx
            //-------------------------------------------------------------
            SizeF notchSize = new SizeF(5000, 5000);
            PointF notchCenter = Wafer.SurroundingRectangle.MidTop();
            float x = notchCenter.X - notchSize.Width / 2;
            float y = notchCenter.Y - notchSize.Height / 2;
            RectangleF notchRect = new RectangleF(x, y, notchSize.Width, notchSize.Height);
            rect = YldMapMatrix.micronToPixel(notchRect);
            graphics.FillEllipse(Brushes.Black, rect);
        }

        //=================================================================
        // 
        //=================================================================
        private void DrawFlatWafer(Graphics graphics)
        {
            FlatWafer flatWafer = Wafer as FlatWafer;

            //-------------------------------------------------------------
            // Dessin du disque du wafer
            //-------------------------------------------------------------
            RectangleF surroundingRectf = flatWafer.SurroundingRectangle;
            Rectangle rect = YldMapMatrix.micronToPixel(surroundingRectf);
            graphics.FillEllipse(Brushes.Gray, rect);

            //-------------------------------------------------------------
            // Flat Vertical
            //-------------------------------------------------------------
            bool hasFlatV = !(double.IsNaN(flatWafer.FlatVerticalX));
            if (hasFlatV)
            {
                RectangleF maskRectf;
                if (flatWafer.FlatVerticalX > 0.0)  // flat à droite
                {
                    float width = surroundingRectf.Right - (float)flatWafer.FlatVerticalX;
                    maskRectf = new RectangleF((float)flatWafer.FlatVerticalX, surroundingRectf.Y, width, surroundingRectf.Height);
                }
                else // flat à gauche
                {
                    float width = (float)flatWafer.FlatVerticalX - surroundingRectf.Left;
                    maskRectf = new RectangleF(surroundingRectf.X, surroundingRectf.Y, width, surroundingRectf.Height);
                }

                // On masque le flat
                rect = YldMapMatrix.micronToPixel(maskRectf);
                graphics.FillRectangle(Brushes.Black, rect);
            }

            //-------------------------------------------------------------
            // Flat Horizontal
            //-------------------------------------------------------------
            bool hasFlatH = !(double.IsNaN(flatWafer.FlatHorizontalY));
            if (hasFlatH)
            {
                RectangleF maskRectf;

                if (flatWafer.FlatHorizontalY > 0.0)    // flat en haut
                {
                    float height = surroundingRectf.Bottom - (float)flatWafer.FlatHorizontalY;
                    maskRectf = new RectangleF(surroundingRectf.X, (float)flatWafer.FlatHorizontalY, surroundingRectf.Width, height);
                }
                else   // flat en bas
                {
                    float height = (float)flatWafer.FlatHorizontalY - surroundingRectf.Top;
                    maskRectf = new RectangleF(surroundingRectf.X, surroundingRectf.Y, surroundingRectf.Width, height);
                }

                // On masque le flat
                rect = YldMapMatrix.micronToPixel(maskRectf);
                graphics.FillRectangle(Brushes.Black, rect);
            }
        }



        //=================================================================
        // 
        //=================================================================
        private void DrawDies(Graphics graphics)
        {
            if (DieDescriptors.Count == 0)
                return;

            //-------------------------------------------------------------
            // Brush
            //-------------------------------------------------------------
            Brush[] brushes = new Brush[4];
            brushes[0] = Brushes.Gray;  // non test
            brushes[1] = Brushes.Green; // good
            brushes[2] = Brushes.Orange; // unclassified
            brushes[3] = Brushes.Red; // KO

            //-------------------------------------------------------------
            // Font
            //-------------------------------------------------------------
            DieDescriptor firstdie = DieDescriptors.First().Value;
            Rectangle rect = YldMapMatrix.micronToPixel(firstdie.MicronRect);
            Font drawFont = new Font("Arial", _FontSize);

            //-------------------------------------------------------------
            // Dessin des dies
            //-------------------------------------------------------------
            foreach (DieDescriptor die in DieDescriptors.Values)
            {
                Interlocked.Increment(ref nbObjectsOut);

                int brushIdx = ((int)die.Status) + 1;
                rect = YldMapMatrix.micronToPixel(die.MicronRect);
                graphics.FillRectangle(brushes[brushIdx], rect);
                graphics.DrawRectangle(Pens.Black, rect);

                if (die.Status != eDieState.NotTested)
                {
                    string txt = die.IndexX.ToString() + "/" + die.IndexY.ToString();
                    DrawDieLabel(graphics, drawFont, txt, rect);
                }
            }
        }

        private void DrawDieLabel(Graphics graphics, Font drawFont, string txt, Rectangle DieRect)
        {
            int x = DieRect.X + DieRect.Width / 2;
            int y = DieRect.Y + DieRect.Height / 2;

            if (_bDrawVertcal)
            {
                float angle_dg = -90.0F;
                graphics.TranslateTransform(x, y); // Set rotation point
                graphics.RotateTransform(angle_dg); // Rotate text
                graphics.TranslateTransform(-x, -y); // Reset translate transform
            }
            //else
            //{
            //    SizeF sizeft = graphics.MeasureString(sTxt, new Font("Arial", _FontSize, FontStyle.Regular, GraphicsUnit.Point));
            //    float fStrPosX = DieRect.X + Math.Max(0, (DieRect.Width - sizeft.Width) * 0.5F);
            //    float fStrPosY = DieRect.Y + Math.Max(0, (DieRect.Height - sizeft.Height) * 0.5F);
            //    graphics.DrawString(sTxt, drawFont, Brushes.Black, fStrPosX, fStrPosY);
            //}

            SizeF sizeft = graphics.MeasureString(txt, drawFont); // Get size of rotated text (bounding box)
            float fStrPosX = DieRect.X + Math.Max(0, (DieRect.Width - sizeft.Width) * 0.5F);
            float fStrPosY = DieRect.Y + Math.Max(0, (DieRect.Height - sizeft.Height) * 0.5F);

            graphics.DrawString(txt, drawFont, Brushes.Black, new PointF(fStrPosX, fStrPosY)); // Draw string centered in x, y
            graphics.ResetTransform(); // Only needed if you reuse the Graphics object for multiple calls to DrawString
        }

    }
}

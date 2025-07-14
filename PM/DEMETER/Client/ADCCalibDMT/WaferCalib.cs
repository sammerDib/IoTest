using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Matrox.MatroxImagingLibrary;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Xml;
using System.Windows.Forms;
using System.Threading;
using UnitySC.PM.DMT.Service.DMTCalTransform;

namespace ADCCalibDMT
{
    public class WaferCalib : IDisposable
    {
        public delegate void GenericDelegate();
        public delegate void GenericDelegateString(String Str);
        public delegate void GenericDelegate_Rect(Rectangle rc, Color oColor);
        public delegate void GenericDelegate_Cross(double dx, double dy, string sLabel, Color oColor);
        public event GenericDelegate_Rect   OnDrawRect;
        public event GenericDelegate_Cross  OnDrawCross;
        public event GenericDelegate OnClearOverlay;
        public event GenericDelegate OnDrawGrid;
        public event GenericDelegate OnAbort;
        public event GenericDelegateString OnDisplayInfo;

        public MIL_ID m_MilSrcCalibImgID = MIL.M_NULL;

        public string m_sFileName;
        public float m_fWaferSize_um;
        public int m_nGridSizeX;            // Nb de pts de la grille en largeur
        public int m_nGridSizeY;            // Nb de pts de la grille en hauteur 
        public int m_nCenterGridIndexX;     // Index de l'indice du centre dans la grille -- Soit l'index X du Pts.nxx = 0
        public int m_nCenterGridIndexY;     // Index de l'indice du centre dans la grille -- Soit l'index Y du Pts.nyy = 0
     
        public List<String> m_MireName = new List<String>(); // liste des NOMS mires specifiques presentes dans le fichier xml decrivant le wafer de calibration
        public List<Pts> m_MirePts = new List<Pts>(); // liste des PTS mires specifiques presentes dans le fichier xml decrivant le wafer de calibration
        public List<AutoResetEvent> m_MireEvtClick = new List<AutoResetEvent>();  // liste des Events associé aux mires specifiques presentes dans le fichier xml decrivant le wafer de calibration
      
        protected bool m_bComputeCenterPts; //@ true if neede to determine  center from other specific mire points

        public float m_fStepX_um;
        public float m_fStepY_um;

        private bool m_bSearchGridDone = false;

        public int m_SearchSize_px = 5;  // Demi Size de la zone de recherche d'un Pts Mire standard
        private bool m_bCancelProcess = false; // @true abort le process de recherche des point
        public void CancelProcess() { m_bCancelProcess = true;}

        public const int SOUTH = 0;         // mire spécifique du bas en ligne avec le notch @0° (notch en bas de l'image)
        public const int NORTH = 1;         // mire spécifique du haut en ligne avec le notch @0°
        public const int WEST = 2;          // mire spécifique de gauche avec le notch @0°
        public const int EAST = 3;          // mire spécifique de droite avec le notch @0°
        public const int CENTER = 4;        // mire centrale du wafer
        public const int NB_MIRES = 5;      // Nombre total de mires spécifiques


        private const double g_cstTrendPixelSizeX = 70.0; // tendance pixel size X de notre caméra actuelle dans le cas particulier où le wafer calibration n'a pas de mire N,S,E,W
        private const double g_cstTrendPixelSizeY = 74.2; // tendance pixel size Y de notre caméra actuelle dans le cas particulier où le wafer calibration n'a pas de mire N,S,E,W
     
        public Pts[,] m_GridPts;    // grille contenant l'ensemble des point de la grille de calibration certain, peuvent être non utlisé car n'ayant pas de correspondance avec le monde physique
        public Pts[] m_SpecificPts; // tableau des mires specifique Center, North, South, East, West (selon le type de wafer calib elle peuvent être ou non présente /!\ == null)

        protected List<MIL_ID> m_oPatternedMireList;
        protected List<MIL_ID> m_oPatternedImgList;

        protected MIL_ID m_MilSystem;
        public string m_sLastSaveFile = "";

        public WaferCalib(string xmlfilepath, MIL_ID MilSystem)
        { 
            m_sFileName = Path.GetFileNameWithoutExtension(xmlfilepath);
            m_bComputeCenterPts = false;

            m_MilSystem = MilSystem;
            m_bCancelProcess = false;

            m_oPatternedMireList = new List<MIL_ID>();
            m_oPatternedImgList = new List<MIL_ID>();
           
            //---------------------------------------------------------
            // Parse XML
            //---------------------------------------------------------
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlfilepath);

            XmlNode SizeNode = doc.SelectSingleNode(".//WaferCalib//Sizeum");
            if(SizeNode != null)
                m_fWaferSize_um = Convert.ToSingle(SizeNode.InnerText);

            XmlNode GridSizeNode = doc.SelectSingleNode(".//WaferCalib//GridSize");
            if(GridSizeNode != null)
            {
                m_nGridSizeX = Convert.ToInt32(GridSizeNode.SelectSingleNode(".//NbX").InnerText);
                m_nGridSizeY = Convert.ToInt32(GridSizeNode.SelectSingleNode(".//NbY").InnerText);

                m_nCenterGridIndexX = Convert.ToInt32(GridSizeNode.SelectSingleNode(".//CenterIdx//X").InnerText);
                m_nCenterGridIndexY = Convert.ToInt32(GridSizeNode.SelectSingleNode(".//CenterIdx//Y").InnerText);

                m_fStepX_um = Convert.ToSingle(GridSizeNode.SelectSingleNode(".//Stepum//X").InnerText);
                m_fStepY_um = Convert.ToSingle(GridSizeNode.SelectSingleNode(".//Stepum//Y").InnerText);


                m_GridPts = new Pts[m_nGridSizeX,m_nGridSizeY];

                for (int i = 0; i < m_nGridSizeX; i++)
                {
                    for (int j = 0; j < m_nGridSizeY; j++)
                    {
                        m_GridPts[i, j] = new Pts();
                    }
                }
                m_SpecificPts = new Pts[NB_MIRES]; 
            }

              XmlNode PatternsListNode = doc.SelectSingleNode(".//WaferCalib//PatternsList");
              if (PatternsListNode != null)
              {
                  XmlNodeList PatNodelist = PatternsListNode.SelectNodes("Pat");
                  foreach (XmlNode node in PatNodelist)
                  {
                      int nId = Convert.ToInt32(node.SelectSingleNode(".//Id").InnerText);
                      XmlNode nodepatFile = node.SelectSingleNode(".//PatFile");
                      if (nodepatFile != null)
                      {
                          //init pattern par *.mmf file issue du model finder de MIL matrox
                          InitPatternFromMMF(nId, nodepatFile.InnerText, Path.GetDirectoryName(xmlfilepath));
                      }
                      else
                      {
                          XmlNode nodeimgfile = node.SelectSingleNode(".//ImgFile");
                          if (nodeimgfile != null)
                          {
                              InitPatternFromImage(nId, node,Path.GetDirectoryName(xmlfilepath));
                          }
                      }
                  }
              }

              XmlNode GridPtsNode = doc.SelectSingleNode(".//WaferCalib//GridPts");
              if (GridPtsNode != null)
              {
                  XmlNodeList PtsNodelist = GridPtsNode.SelectNodes("Pts");
                  foreach (XmlNode node in PtsNodelist)
                  {
                      int xx = Convert.ToInt32(node.SelectSingleNode(".//xx").InnerText);
                      int yy = Convert.ToInt32(node.SelectSingleNode(".//yy").InnerText);
                      double dPosX_um = Convert.ToDouble(node.SelectSingleNode(".//PosXum").InnerText);
                      double dPosY_um = Convert.ToDouble(node.SelectSingleNode(".//PosYum").InnerText);
                      int nPatid = Convert.ToInt32(node.SelectSingleNode(".//PatId").InnerText);

                      int nIdX = xx + m_nCenterGridIndexX;
                      int nIdY = yy + m_nCenterGridIndexY;

                      if (nIdX < 0 || nIdX >= m_nGridSizeX)
                          continue;

                      if (nIdY < 0 || nIdY >= m_nGridSizeY)
                          continue;

                      m_GridPts[nIdX, nIdY].bUsedGridPts = true;
                      m_GridPts[nIdX, nIdY].bFound = false;
                      m_GridPts[nIdX, nIdY].nxx = xx;
                      m_GridPts[nIdX, nIdY].nyy = yy;
                      m_GridPts[nIdX, nIdY].dPosX_um = dPosX_um;
                      m_GridPts[nIdX, nIdY].dPosY_um = dPosY_um;
                      m_GridPts[nIdX, nIdY].nPatternID = nPatid;
                  }
              }

                m_MireName.Clear();
                m_MireEvtClick.Clear();
                m_MirePts.Clear();
              XmlNode SpecificPtsNode = doc.SelectSingleNode(".//WaferCalib//SpecificPts");
              if (SpecificPtsNode != null)
              {
                  XmlNode SpecificMirePts = null;
                  m_bComputeCenterPts = true;

                  SpecificMirePts = SpecificPtsNode.SelectSingleNode("C");
                  if (SpecificMirePts != null)
                  {
                      // Si un Centre existe en tant que mire spécifique on ne déduit pas la search area des autre mire spécifique
                      m_bComputeCenterPts = false;

                      int xx = Convert.ToInt32(SpecificMirePts.SelectSingleNode(".//xx").InnerText);
                      int yy = Convert.ToInt32(SpecificMirePts.SelectSingleNode(".//yy").InnerText);
                      m_MireName.Add("Center");
                      m_MireEvtClick.Add(new AutoResetEvent(false));

                      m_MirePts.Add(m_GridPts[xx + m_nCenterGridIndexX, yy + m_nCenterGridIndexY]);
                      m_SpecificPts[CENTER] = m_GridPts[xx + m_nCenterGridIndexX, yy + m_nCenterGridIndexY];
                  }

                  SpecificMirePts = SpecificPtsNode.SelectSingleNode("S");
                  if(SpecificMirePts != null)
                  {
                      int xx = Convert.ToInt32(SpecificMirePts.SelectSingleNode(".//xx").InnerText);
                      int yy = Convert.ToInt32(SpecificMirePts.SelectSingleNode(".//yy").InnerText);
                      m_MireName.Add("South");
                      m_MireEvtClick.Add(new AutoResetEvent(false));
                      m_MirePts.Add(m_GridPts[xx + m_nCenterGridIndexX, yy + m_nCenterGridIndexY]);
                      m_SpecificPts[SOUTH] = m_GridPts[xx + m_nCenterGridIndexX, yy + m_nCenterGridIndexY];
                  }

                  SpecificMirePts = SpecificPtsNode.SelectSingleNode("N");
                  if (SpecificMirePts != null)
                  {
                      int xx = Convert.ToInt32(SpecificMirePts.SelectSingleNode(".//xx").InnerText);
                      int yy = Convert.ToInt32(SpecificMirePts.SelectSingleNode(".//yy").InnerText);
                      m_MireName.Add("North");
                      m_MireEvtClick.Add(new AutoResetEvent(false));
                      m_MirePts.Add(m_GridPts[xx + m_nCenterGridIndexX, yy + m_nCenterGridIndexY]);
                      m_SpecificPts[NORTH] = m_GridPts[xx + m_nCenterGridIndexX, yy + m_nCenterGridIndexY];
                  }

                  SpecificMirePts = SpecificPtsNode.SelectSingleNode("W");
                  if (SpecificMirePts != null)
                  {
                      int xx = Convert.ToInt32(SpecificMirePts.SelectSingleNode(".//xx").InnerText);
                      int yy = Convert.ToInt32(SpecificMirePts.SelectSingleNode(".//yy").InnerText);
                      m_MireName.Add("West");
                      m_MireEvtClick.Add(new AutoResetEvent(false));
                      m_MirePts.Add(m_GridPts[xx + m_nCenterGridIndexX, yy + m_nCenterGridIndexY]);
                      m_SpecificPts[WEST] = m_GridPts[xx + m_nCenterGridIndexX, yy + m_nCenterGridIndexY];
                  }

                  SpecificMirePts = SpecificPtsNode.SelectSingleNode("E");
                  if (SpecificMirePts != null)
                  {
                      int xx = Convert.ToInt32(SpecificMirePts.SelectSingleNode(".//xx").InnerText);
                      int yy = Convert.ToInt32(SpecificMirePts.SelectSingleNode(".//yy").InnerText);
                      m_MireName.Add("East");
                      m_MireEvtClick.Add(new AutoResetEvent(false));
                      m_MirePts.Add(m_GridPts[xx + m_nCenterGridIndexX, yy + m_nCenterGridIndexY]);
                      m_SpecificPts[EAST] = m_GridPts[xx + m_nCenterGridIndexX, yy + m_nCenterGridIndexY];
                  }

                  if (m_bComputeCenterPts)
                  {
                      // si on a pas de center on verifie que l'on a suffisamment de point pour calculer le centre
                      if (m_MirePts.Count < 4)
                      {
                          m_bComputeCenterPts = false;
                          MessageBox.Show("Search Points Xml file Error\n Some Specific Points are missing !");
                      }
                  }
              }
                
        }

        public void Dispose()
        {
            // Perform any object clean up here.
            foreach (MIL_ID oModFind in m_oPatternedMireList)
            {
                if (oModFind != MIL.M_NULL)
                    MIL.MmodFree(oModFind);
            }
            foreach (MIL_ID oModImg in m_oPatternedImgList)
            {
                if (oModImg != MIL.M_NULL)
                    MIL.MbufFree(oModImg);
            }
        }

        virtual public MIL_ID GetPatternImg(int nId)
        {
            if (nId >= 0 && nId < m_oPatternedMireList.Count)
            {
                return m_oPatternedImgList[nId];
            }
            return MIL.M_NULL;
        }

        virtual public void InitPatternFromImage(int nId, XmlNode oPatNode, string folder)
        {
            // -------
            // LOAD un model à partir d'une image, les paramètre par défaut sont appiliquer si non renseigné dans le XmlNode
            // -------
            try
            {
                // image d'un model que nous allons crées et paramètrer par défaut
                string imgRelativeFilePath = oPatNode.SelectSingleNode(".//ImgFile").InnerText;
                var imgfilepath = Path.Combine(folder, imgRelativeFilePath);
                XmlNode prmNode = null;
                if (!File.Exists(imgfilepath))
                {
                    //if not exist in "config" folder search in exe folder
                    imgfilepath = Path.GetFullPath(imgRelativeFilePath); // pour résoudre les pb de chemin en relatif..
                    if (!File.Exists(imgfilepath))
                    {
                        throw new Exception($"Model Pattern Image NOT found <{imgRelativeFilePath}>\n either in \n{folder}\n or in \n{Directory.GetCurrentDirectory()}");
                    }
                }


                // on load l'image
                MIL_INT imodSizeX = 0;
                
                MIL.MbufDiskInquire(imgfilepath, MIL.M_SIZE_X, ref imodSizeX);
                MIL_INT imodSizeY = 0;
                MIL.MbufDiskInquire(imgfilepath, MIL.M_SIZE_Y, ref imodSizeY);
                MIL_ID oModelImg = MIL.M_NULL;
                MIL.MbufAlloc2d(m_MilSystem, (MIL_INT)imodSizeX, (MIL_INT)imodSizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, ref oModelImg);
                MIL.MbufImport(imgfilepath, MIL.M_DEFAULT, MIL.M_LOAD, m_MilSystem, ref oModelImg);


                // Allocate a Geometric Model Finder context.
                //
                MIL_ID oModAlign = MIL.M_NULL;
                MIL.MmodAlloc(m_MilSystem, MIL.M_GEOMETRIC_CONTROLLED, MIL.M_DEFAULT, ref oModAlign);
                //Define the model.
                MIL.MmodDefine(oModAlign, MIL.M_IMAGE, oModelImg, 0, 0, imodSizeX, imodSizeY);

                // Here is the parameters check if exists in xmlnode then change it or let our default settings
                //
                prmNode = oPatNode.SelectSingleNode(".//Speed");
                if (prmNode != null)
                {
                    int nSpeed = Convert.ToInt32(prmNode.InnerText);
                    // LOW=1  MEDIUM=2(def)  HIGH=3 VERY_HIGHT=4
                    MIL.MmodControl(oModAlign, MIL.M_CONTEXT, MIL.M_SPEED, (MIL_INT)nSpeed);
                }
               
                prmNode = oPatNode.SelectSingleNode(".//Accuracy");
                if (prmNode != null)
                {
                    int nAccuracy = Convert.ToInt32(prmNode.InnerText);
                    // MEDIUM=2(def)  HIGH=3 
                    MIL.MmodControl(oModAlign, MIL.M_CONTEXT, MIL.M_ACCURACY, (MIL_INT)nAccuracy);
                }
              
                prmNode = oPatNode.SelectSingleNode(".//Acceptance");
                if (prmNode != null)
                {
                    double dAcceptance = Convert.ToDouble(prmNode.InnerText);
                    MIL.MmodControl(oModAlign, MIL.M_DEFAULT, MIL.M_ACCEPTANCE, dAcceptance);
                }

                prmNode = oPatNode.SelectSingleNode(".//Certainty");
                if (prmNode != null)
                {
                    double dCertainty = Convert.ToDouble(prmNode.InnerText);
                    MIL.MmodControl(oModAlign, MIL.M_DEFAULT, MIL.M_CERTAINTY, dCertainty);
                }

                prmNode = oPatNode.SelectSingleNode(".//Timeout");
                if (prmNode != null)
                {
                    double dTimeout_ms = Convert.ToDouble(prmNode.InnerText);
                    MIL.MmodControl(oModAlign, MIL.M_CONTEXT, MIL.M_TIMEOUT, dTimeout_ms);
                }

                prmNode = oPatNode.SelectSingleNode(".//ReferenceX");
                if (prmNode != null)
                {
                    double drefX = Convert.ToDouble(prmNode.InnerText);
                    MIL.MmodControl(oModAlign, MIL.M_DEFAULT, MIL.M_REFERENCE_X, drefX);
                }

                prmNode = oPatNode.SelectSingleNode(".//ReferenceY");
                if (prmNode != null)
                {
                    double drefY = Convert.ToDouble(prmNode.InnerText);
                    MIL.MmodControl(oModAlign, MIL.M_DEFAULT, MIL.M_REFERENCE_Y, drefY);
                }

                prmNode = oPatNode.SelectSingleNode(".//SearchAngleRange");
                if (prmNode != null)
                {
                    int nEnable = Convert.ToInt32(prmNode.InnerText);
                    if (nEnable != 0)
                        nEnable = MIL.M_ENABLE;
                    else
                        nEnable = MIL.M_DISABLE;
                    MIL.MmodControl(oModAlign, MIL.M_CONTEXT, MIL.M_SEARCH_ANGLE_RANGE, (MIL_INT)nEnable);
                }

                prmNode = oPatNode.SelectSingleNode(".//AngleDeltaNEG");
                if (prmNode != null)
                {
                    double dval = Convert.ToDouble(prmNode.InnerText);
                    MIL.MmodControl(oModAlign, MIL.M_DEFAULT, MIL.M_ANGLE_DELTA_NEG, dval);
                }

                prmNode = oPatNode.SelectSingleNode(".//AngleDeltaPOS");
                if (prmNode != null)
                {
                    double dval = Convert.ToDouble(prmNode.InnerText);
                    MIL.MmodControl(oModAlign, MIL.M_DEFAULT, MIL.M_ANGLE_DELTA_NEG, dval);
                }

                prmNode = oPatNode.SelectSingleNode(".//Polarity");
                if (prmNode != null)
                {
                    //M_SAME=-1(def) M_SAME_OR_REVERSE=5  M_REVERSE=4 M_ANY=285212672
                    int npolarity = Convert.ToInt32(prmNode.InnerText);
                    switch(npolarity)
                    {
                        case 0: npolarity = MIL.M_SAME ; break; // M_SAME
                        case 1: npolarity = MIL.M_SAME_OR_REVERSE ;break; // M_SAME_OR_REVERSE
                        case 2: npolarity = MIL.M_REVERSE ;break; // M_REVERSE
                        case 3: npolarity = MIL.M_ANY; break; // M_ANY
                        default:npolarity = MIL.M_SAME; break;
                    }
                    MIL.MmodControl(oModAlign, MIL.M_DEFAULT, MIL.M_POLARITY, (MIL_INT) npolarity);
                }

                prmNode = oPatNode.SelectSingleNode(".//Number");
                if (prmNode != null)
                {
                    int nNumber = Convert.ToInt32(prmNode.InnerText);
                    MIL.MmodControl(oModAlign, MIL.M_DEFAULT, MIL.M_NUMBER, (MIL_INT)nNumber);
                }

                // On doit préprocesser le model afin qu'il puisse être executer
                MIL.MmodPreprocess(oModAlign, MIL.M_DEFAULT);

                // On sauve notre pattern dans nos liste
                m_oPatternedMireList.Add(oModAlign);
                m_oPatternedImgList.Add(oModelImg);

                if(oModAlign == MIL.M_NULL)
                    MessageBox.Show("oModAlign == MIL.M_NULL init => " + imgfilepath);

            }
            catch (System.Exception ex)
            {
                String sMsg = "InitPatternFromImage Exception: \n #Message :" + ex.Message;
                MessageBox.Show(sMsg);
            }
        }

        virtual public void InitPatternFromMMF(int nId, string oModelFinderFilePath, string folder)
        {
            // ---------------------
            // LOAD un fichier MMF contenant model image + paramètre
            // -------------------
            // oModelFinderFilePath ==> *.mmf file path saved from MODEL FINDER tool from Matrox Imaging library
            try
            {
                MIL_ID oModImg = MIL.M_NULL;
                MIL_ID oModFind = MIL.M_NULL;

                // image d'un model que nous allons crées et paramètrer par défaut
                string imgRelativeFilePath = oModelFinderFilePath;
                var imgfilepath = Path.Combine(folder, imgRelativeFilePath);
                if (!File.Exists(imgfilepath))
                {
                    //if not exist in "config" folder search in exe folder
                    imgfilepath = Path.GetFullPath(imgRelativeFilePath); // pour résoudre les pb de chemin en relatif..
                    if (!File.Exists(imgfilepath))
                    {
                        throw new Exception($"Model Pattern MMF File NOT found <{imgRelativeFilePath}>\n either in \n{folder}\n or in \n{Directory.GetCurrentDirectory()}");
                    }
                }

                // On charge le model et ses paramètres dans oModFind
                MIL.MmodRestore(imgfilepath, m_MilSystem, MIL.M_DEFAULT, ref oModFind);

                // On doit préprocesser le model afin qu'il puisse être executer
                MIL.MmodPreprocess(oModFind, MIL.M_DEFAULT);

                // on recupere les elements de l'image du model afin d'en extraire une image et de la présenter dans l'ihm
                double dSizeX = 0.0;
                double dSizeY = 0;
                double dbitSize = 0.0;
                MIL_INT nModelINDEX = 0; // par défaut on considère que l'on a un seul et unique model de type IMAGE
                // Note de RTI : si on décide d'avoir autre chose qu'une image ou d'autre model c'est ici qu'il faut le gere, mais a t'on besoin de presenter tout les motifs du model ?
                MIL.MmodInquire(oModFind, nModelINDEX, MIL.M_ALLOC_SIZE_X, ref dSizeX);
                MIL.MmodInquire(oModFind, nModelINDEX, MIL.M_ALLOC_SIZE_Y, ref dSizeY);
                MIL.MmodInquire(oModFind, nModelINDEX, MIL.M_ALLOC_TYPE, ref dbitSize);
                // allocation et copy de l'image model 
                MIL.MbufAlloc2d(m_MilSystem, (MIL_INT)dSizeX, (MIL_INT)dSizeY, (MIL_INT)dbitSize, MIL.M_IMAGE + MIL.M_DISP, ref oModImg);
                MIL.MmodDraw(MIL.M_DEFAULT, oModFind, oModImg, MIL.M_DRAW_IMAGE, MIL.M_DEFAULT, MIL.M_DEFAULT);

                // On sauve notre pattern dans nos liste
                m_oPatternedMireList.Add(oModFind);
                m_oPatternedImgList.Add(oModImg);

                if (oModFind == MIL.M_NULL)
                    MessageBox.Show("oModFind == MIL.M_NULL MMF => " + oModelFinderFilePath);

                // on s'assure que l'id correspond bien à l'index ! 
                if (nId != m_oPatternedMireList.Count - 1)
                {
                    MessageBox.Show("InitPatternFromMMF Error in XML Patterns List Id : \nPattern ID order is not coherent !");
                }
            }
            catch (System.Exception ex)
            {
                String sMsg = "InitPatternFromMMF Exception: \n #Message :" + ex.Message;
                MessageBox.Show(sMsg);
            }
        }

        virtual public void SearchPoints()
        {
            if (OnDisplayInfo == null)
            {
                MessageBox.Show("Error ! Display info IHM is not LINKED to Process");
                if (OnAbort != null)
                    OnAbort();
                return;
            }

            // on reset la recherche
            m_bSearchGridDone = false;
            foreach(Pts oPts in m_GridPts)
            {
                oPts.bFound = false;
                oPts.dPosX_px = 0;
                oPts.dPosY_px = 0;
            }
          

            int nIdx = 0;
            foreach(Pts mire in m_MirePts)
            {
                String sMsg = String.Format("Click on image to Set Area Search of <{0}> Mire then valid it", m_MireName[nIdx]);
                OnDisplayInfo(sMsg);
                if (m_MireEvtClick[nIdx].WaitOne(10000 * 3) == false)
                {
                    OnDisplayInfo("TIMEOUT ! You took to long to choose Area. Search Mire Aborted");
                    if (OnAbort != null)
                        OnAbort();
                    return;
                }

                 if(OnDrawRect != null)
                     OnDrawRect(mire.SearchAreaInImage, Color.FromArgb(25, 128, 255));

                nIdx++;
            }

            if (m_bCancelProcess)
            {
                OnDisplayInfo("Search Mire Aborted by User");
                if (OnAbort != null)
                    OnAbort();
                return;
            }

            OnDisplayInfo("Processing all others search areas...");

            for (int i =0; i < NB_MIRES; i++)
            {
                if(m_SpecificPts[i] != null)
                {
                    SearchPts(m_SpecificPts[i], m_oPatternedMireList[m_SpecificPts[i].nPatternID]);
                    if (m_SpecificPts[i].bFound)
                    {
                        string schar = "";
                        Color oColor = Color.FromArgb(0, 255, 255);
                        switch (i)
                        {
                            case SOUTH: schar = "(S)"; break;
                            case NORTH: schar = "(N)"; break;
                            case EAST: schar = "(E)"; break;
                            case WEST: schar = "(W)"; break;
                            case CENTER: schar = "(C)"; oColor = Color.FromArgb(0, 0, 255); break;
                        }

                        String sLabel = String.Format("[{0},{1}] {2}", m_SpecificPts[i].nxx, m_SpecificPts[i].nyy, schar);
                        if (OnDrawCross != null)
                            OnDrawCross(m_SpecificPts[i].dPosX_px, m_SpecificPts[i].dPosY_px, sLabel, oColor);
                    }
                }

                if (m_bCancelProcess)
                {
                    OnDisplayInfo("Search Mire Aborted by User");
                    if (OnAbort != null)
                        OnAbort();
                    return;
                }
            }



            double dTrendPixelSizeX = g_cstTrendPixelSizeX; //70.0
            double dTrendPixelSizeY = g_cstTrendPixelSizeY; //74.2

            double Nx = m_SpecificPts[NORTH].dPosX_px; double Ny = m_SpecificPts[NORTH].dPosY_px;
            double Sx = m_SpecificPts[SOUTH].dPosX_px; double Sy = m_SpecificPts[SOUTH].dPosY_px;
            double Wx = m_SpecificPts[WEST].dPosX_px; double Wy = m_SpecificPts[WEST].dPosY_px;
            double Ex = m_SpecificPts[EAST].dPosX_px; double Ey = m_SpecificPts[EAST].dPosY_px;         
            if (m_bComputeCenterPts)
            {
                // on calcule le centre avec les mires

                //////////////////////////////////////////////////////////////////////////
                //  Let A,B,C,D be 2-space position vectors.  Then the directed line segments AB & CD are given by:
                //         AB=A+r(B-A), r in [0,1]
                //         CD=C+s(D-C), s in [0,1]
                //              If AB & CD intersect, then
                // 
                //         A+r(B-A)=C+s(D-C), or
                // 
                //         Ax+r(Bx-Ax)=Cx+s(Dx-Cx)
                //         Ay+r(By-Ay)=Cy+s(Dy-Cy)  for some r,s in [0,1]
                // 
                //               Solving the above for r and s yields
                // 
                //             (Ay-Cy)(Dx-Cx)-(Ax-Cx)(Dy-Cy)
                //         r = -----------------------------  (eqn 1)
                //             (Bx-Ax)(Dy-Cy)-(By-Ay)(Dx-Cx)
                // 
                //             (Ay-Cy)(Bx-Ax)-(Ax-Cx)(By-Ay)
                //         s = -----------------------------  (eqn 2)
                //             (Bx-Ax)(Dy-Cy)-(By-Ay)(Dx-Cx)
                // 
                //     Let P be the position vector of the intersection point, then
                // 
                //         P=A+r(B-A) or
                // 
                //         Px=Ax+r(Bx-Ax)
                //         Py=Ay+r(By-Ay)
                //
                //////////////////////////////////////////////////////////////////////////
                // Ici on Pose AB = NS et CD = WE

                double r = ((Ny - Wy) * (Ex - Wx) - (Nx - Wx) * (Ey - Wy)) / ((Sx - Nx) * (Ey - Wy) - (Sy - Ny) * (Ex - Wx));
                double Centerx = Nx + r * (Sx - Nx);
                double Centery = Ny + r * (Sy - Ny);

                m_SpecificPts[CENTER] = m_GridPts[m_nCenterGridIndexX,m_nCenterGridIndexY];
                m_SpecificPts[CENTER].SetSearchArea((int)Centerx, (int)Centery, m_SearchSize_px);
                SearchPts(m_SpecificPts[CENTER], m_oPatternedMireList[m_SpecificPts[CENTER].nPatternID]);
                if (m_SpecificPts[CENTER].bFound == false)
                {
                    OnDisplayInfo("ERROR ! Could not compute center. Search Mire Aborted");
                    if (OnAbort != null)
                        OnAbort();
                    return;
                }
//                 if (m_SpecificPts[CENTER].bFound)
//                 {
//                     string schar = "(C)";
//                     Color oColor = Color.FromArgb(0, 255, 0);
//                     String sLabel = String.Format("[{0},{1}] {2}", m_SpecificPts[CENTER].nxx, m_SpecificPts[CENTER].nyy, schar);
//                     if(OnDrawCross != null)
//                         OnDrawCross(m_SpecificPts[CENTER].dPosX_px, m_SpecificPts[CENTER].dPosY_px, sLabel, oColor);         
//                 }
            }
           
            // on ajuste le pixel size pour le sye^de recherche
            dTrendPixelSizeX = (m_SpecificPts[EAST].dPosX_um - m_SpecificPts[WEST].dPosX_um) / (Ex - Wx);
            dTrendPixelSizeY = (m_SpecificPts[SOUTH].dPosY_um - m_SpecificPts[NORTH].dPosY_um) / (Sy - Ny); 

            OnDisplayInfo("Wait Searching Grid Mire Points...");


            // on connait le centre et les tendances pixels size X et Y, on genere les search area pour tt les pts de la grid en partant du centre
            // puis on les cherche
            
            double dStepX_px = m_fStepX_um / dTrendPixelSizeX;
            double dStepY_px = m_fStepY_um / dTrendPixelSizeY;
  
            // Zone A droite du centre, on calcule l'area suivante basé sur les résultats de la zone situé sur sa gauche
            int j = m_nCenterGridIndexY;
            for (int i = m_nCenterGridIndexX + 1; i < m_nGridSizeX; i++)
            {
                if (m_GridPts[i, j].bUsedGridPts && m_GridPts[i, j].bFound == false)
                {
                    Pts oPrev = SearchPreviousPtsOnLeft(m_GridPts[i, j]);
                    double dDiffSteps = (double) (m_GridPts[i, j].nxx - oPrev.nxx); // pour les cas foireux où une detection s'est mal passé

                    m_GridPts[i, j].SetSearchArea((int)(oPrev.dPosX_px + dDiffSteps * dStepX_px), (int)(oPrev.dPosY_px), m_SearchSize_px);
                    SearchPts(m_GridPts[i, j], m_oPatternedMireList[m_GridPts[i, j].nPatternID]);
                }
            }
            // Zone A Gauche du centre, on calcule l'area suivante basé sur les résultats de la zone situé sur sa droite
            for (int i = m_nCenterGridIndexX - 1; i >= 0; i--)
            {
                if (m_GridPts[i, j].bUsedGridPts && m_GridPts[i, j].bFound == false)
                {
                    Pts oPrev = SearchPreviousPtsOnRight(m_GridPts[i, j]);
                    double dDiffSteps = (double) (oPrev.nxx - m_GridPts[i, j].nxx); // pour les cas foireux où une detection s'est mal passé

                    m_GridPts[i, j].SetSearchArea((int)(oPrev.dPosX_px - dDiffSteps * dStepX_px), (int)(oPrev.dPosY_px), m_SearchSize_px);
                    SearchPts(m_GridPts[i, j], m_oPatternedMireList[m_GridPts[i, j].nPatternID]);
                }
            }

            // on va chercher les elements du nord
            for (j = m_nCenterGridIndexY - 1; j >= 0; j--)
            {
                // on calcule la ligne central suivante du nord par rapport à la ligne central précédente situé plus bas
                if (m_GridPts[m_nCenterGridIndexX, j].bUsedGridPts && m_GridPts[m_nCenterGridIndexX, j].bFound == false)
                {
                    Pts oPrev = SearchPreviousPtsDownside(m_GridPts[m_nCenterGridIndexX, j]);
                    double dDiffSteps = (double)(oPrev.nyy - m_GridPts[m_nCenterGridIndexX, j].nyy);

                    m_GridPts[m_nCenterGridIndexX, j].SetSearchArea((int)(oPrev.dPosX_px), (int)(oPrev.dPosY_px - dDiffSteps * dStepY_px), m_SearchSize_px);
                    SearchPts(m_GridPts[m_nCenterGridIndexX, j], m_oPatternedMireList[m_GridPts[m_nCenterGridIndexX, j].nPatternID]);
                    if (m_GridPts[m_nCenterGridIndexX, j].bFound == false)
                    {
                        // si on ne trouve pas de mire sur la ligen centrale, il se peut que ça soit la fin du wafer ou de l'image on ne gere donc pas la ligne on passe à la suivante
                        // problème de confi du wafer.xml possible aussi
                        continue;
                    }
                }

                // Zone A droite du centre, on calcule l'area suivante basé sur les résultats de la zone situé sur sa gauche
                for (int i = m_nCenterGridIndexX + 1; i < m_nGridSizeX; i++)
                {
                    if (m_GridPts[i, j].bUsedGridPts && m_GridPts[i, j].bFound == false)
                    {     
                        Pts oPrev = SearchPreviousPtsOnLeft(m_GridPts[i, j]);
                        double dDiffSteps = (double)(m_GridPts[i, j].nxx - oPrev.nxx); // pour les cas foireux où une detection s'est mal passé

                        m_GridPts[i, j].SetSearchArea((int)(oPrev.dPosX_px + dDiffSteps * dStepX_px), (int)(oPrev.dPosY_px), m_SearchSize_px);
                        SearchPts(m_GridPts[i, j], m_oPatternedMireList[m_GridPts[i, j].nPatternID]); 
                    }
                }
                // Zone A Gauche du centre, on calcule l'area suivante basé sur les résultats de la zone situé sur sa droite
                for (int i = m_nCenterGridIndexX - 1; i >= 0; i--)
                {
                    if (m_GridPts[i, j].bUsedGridPts && m_GridPts[i, j].bFound == false)
                    {
                        Pts oPrev = SearchPreviousPtsOnRight(m_GridPts[i, j]);
                        double dDiffSteps = (double)(oPrev.nxx - m_GridPts[i, j].nxx); // pour les cas foireux où une detection s'est mal passé

                        m_GridPts[i, j].SetSearchArea((int)(oPrev.dPosX_px - dDiffSteps * dStepX_px), (int)(oPrev.dPosY_px), m_SearchSize_px);
                        SearchPts(m_GridPts[i, j], m_oPatternedMireList[m_GridPts[i, j].nPatternID]);
                    }
                }
            }

            // on va chercher les elements du sud
            for (j = m_nCenterGridIndexY + 1; j < m_nGridSizeY; j++)
            {
                // on calcule la ligne central suivante du sud par rapport à la ligne central précédente situé plus haut
                if (m_GridPts[m_nCenterGridIndexX, j].bUsedGridPts && m_GridPts[m_nCenterGridIndexX, j].bFound == false)
                {
                    Pts oPrev = SearchPreviousPtsUpside(m_GridPts[m_nCenterGridIndexX, j]);
                    double dDiffSteps = (double)(m_GridPts[m_nCenterGridIndexX, j].nyy - oPrev.nyy);

                    m_GridPts[m_nCenterGridIndexX, j].SetSearchArea((int)(oPrev.dPosX_px), (int)(oPrev.dPosY_px + dDiffSteps * dStepY_px), m_SearchSize_px);
                    SearchPts(m_GridPts[m_nCenterGridIndexX, j], m_oPatternedMireList[m_GridPts[m_nCenterGridIndexX, j].nPatternID]);
                    if (m_GridPts[m_nCenterGridIndexX, j].bFound == false)
                    {
                        // si on ne trouve pas de mire sur la ligen centrale, il se peut que ça soit la fin du wafer ou de l'image on ne gere donc pas la ligne on passe à la suivante
                        // problème de confi du wafer.xml possible aussi
                        continue;
                    }
                }

                // Zone A droite du centre, on calcule l'area suivante basé sur les résultats de la zone situé sur sa gauche
                for (int i = m_nCenterGridIndexX + 1; i < m_nGridSizeX; i++)
                {
                    if (m_GridPts[i, j].bUsedGridPts && m_GridPts[i, j].bFound == false)
                    {
                        Pts oPrev = SearchPreviousPtsOnLeft(m_GridPts[i, j]);
                        double dDiffSteps = (double)(m_GridPts[i, j].nxx - oPrev.nxx); // pour les cas foireux où une detection s'est mal passé

                        m_GridPts[i, j].SetSearchArea((int)(oPrev.dPosX_px + dDiffSteps * dStepX_px), (int)(oPrev.dPosY_px), m_SearchSize_px);
                        SearchPts(m_GridPts[i, j], m_oPatternedMireList[m_GridPts[i, j].nPatternID]);
                    }
                }

                // Zone A Gauche du centre, on calcule l'area suivante basé sur les résultats de la zone situé sur sa droite
                for (int i = m_nCenterGridIndexX - 1; i >= 0; i--)
                {
                    if (m_GridPts[i, j].bUsedGridPts && m_GridPts[i, j].bFound == false)
                    {
                        Pts oPrev = SearchPreviousPtsOnRight(m_GridPts[i, j]);
                        double dDiffSteps = (double)(oPrev.nxx - m_GridPts[i, j].nxx); // pour les cas foireux où une detection s'est mal passé

                        m_GridPts[i, j].SetSearchArea((int)(oPrev.dPosX_px - dDiffSteps * dStepX_px), (int)(oPrev.dPosY_px), m_SearchSize_px);
                        SearchPts(m_GridPts[i, j], m_oPatternedMireList[m_GridPts[i, j].nPatternID]);
                    }
                }

            }

            OnDisplayInfo("Search Finished !");

            m_bSearchGridDone = true;

            if (OnDrawGrid != null)
                 OnDrawGrid();

        }

        public void Calibration(MIL_ID p_oCalibImg, double p_dMarginTop, double p_dMarginBottom, double p_dMarginRight, double p_dMarginLeft, string destPSDFilePath, bool reportDebugImages )
        {

            Cursor.Current = Cursors.WaitCursor;

            if (m_bSearchGridDone)
            {
                m_sLastSaveFile = "";

                // on parcours l'ensemble des pts de la grilles associé avec une valeur physique
                // on aliment ainsi l'objet de calibration mil qui nous permettra d'effectuer le redressement

                // lance calibration
                MIL_INT nCalibrationStatus = 0;
                MIL_ID oMilCalibration = MIL.M_NULL;
                MIL.McalAlloc(m_MilSystem,
                    //MIL.M_LINEAR_INTERPOLATION, // use prior to 10/07/2024 with MIL9 and MIL10pp1
                    MIL.M_PERSPECTIVE_TRANSFORMATION, // use after 10/07/2024 With MILXsp6
                    MIL.M_DEFAULT, ref oMilCalibration);

                // on estime "grossièrement" le pixel size avec des données en milieu de grid
                Pts oN = null; Pts oS = null;  Pts oE = null;  Pts oW = null;
                int nGrididx = m_nGridSizeY / 4;
                while(oN == null &&  nGrididx < m_nCenterGridIndexY)
                {
                    if (m_GridPts[m_nCenterGridIndexX, nGrididx].IsGoodToCalibrate())
                        oN = m_GridPts[m_nCenterGridIndexX,nGrididx];
                    else
                        nGrididx++; //on se rapproche du centre vers le bas du wafer
                }

                nGrididx = 3 * m_nGridSizeY / 4;
                while (oS == null && nGrididx > m_nCenterGridIndexY)
                {
                    if (m_GridPts[m_nCenterGridIndexX, nGrididx].IsGoodToCalibrate())
                        oS = m_GridPts[m_nCenterGridIndexX, nGrididx];
                    else
                        nGrididx--; //on se rapproche du centre vers le haut du wafer
                }

                nGrididx = m_nGridSizeX / 4;
                while (oW == null && nGrididx < m_nCenterGridIndexX)
                {
                    if (m_GridPts[nGrididx, m_nCenterGridIndexY].IsGoodToCalibrate())
                        oW = m_GridPts[nGrididx, m_nCenterGridIndexY];
                    else
                        nGrididx++; //on se rapproche du centre vers la droite du wafer
                }

                nGrididx = 3 * m_nGridSizeX / 4;
                while (oE == null && nGrididx > m_nCenterGridIndexX)
                {
                    if (m_GridPts[nGrididx, m_nCenterGridIndexY].IsGoodToCalibrate())
                        oE = m_GridPts[nGrididx, m_nCenterGridIndexY];
                    else
                        nGrididx--; //on se rapproche du centre vers la gauche du wafer
                }
                double dTrendPixelSizeX = g_cstTrendPixelSizeX;
                double dTrendPixelSizeY = g_cstTrendPixelSizeY;
                if(oE != null && oW != null)
                    dTrendPixelSizeX = (oE.dPosX_um - oW.dPosX_um) / (oE.dPosX_px - oW.dPosX_px);
                if (oS != null && oN != null)
                    dTrendPixelSizeY = (oS.dPosY_um - oN.dPosY_um) / (oS.dPosY_px - oN.dPosY_px);

                int nNbPtsToCalibrate = 0;
                foreach (Pts oPts in m_GridPts)
                {
                    if(oPts.IsGoodToCalibrate())
                        nNbPtsToCalibrate++;
                }

                double[] dArrayX_px = new double[nNbPtsToCalibrate]; // position X dans l'image en PIXEL
                double[] dArrayY_px = new double[nNbPtsToCalibrate]; // position Y dans l'image en PIXEL
                double[] dArrayX_um = new double[nNbPtsToCalibrate]; // position X physique dans le repere wafer en µm
                double[] dArrayY_um = new double[nNbPtsToCalibrate]; // position Y physique dans le repere wafer en µm

                int nIdxPts = 0;
                double waferradius_um = 0.5 * (double)m_fWaferSize_um;
                foreach (Pts oPts in m_GridPts)
                {
                    if (oPts.IsGoodToCalibrate())
                    {
                        dArrayX_px[nIdxPts] = oPts.dPosX_px;
                        dArrayY_px[nIdxPts] = oPts.dPosY_px;
                        dArrayX_um[nIdxPts] = oPts.dPosX_um;
                        dArrayY_um[nIdxPts] = oPts.dPosY_um;

                        nIdxPts++;;
                    }
                }
                MIL.McalList(oMilCalibration, dArrayX_px, dArrayY_px, dArrayX_um, dArrayY_um, MIL.M_NULL, nNbPtsToCalibrate, MIL.M_FULL_CALIBRATION, MIL.M_DEFAULT);


                //Verify if the camera was well calibrate
                MIL.McalInquire(oMilCalibration, MIL.M_CALIBRATION_STATUS + MIL.M_TYPE_MIL_INT, ref nCalibrationStatus);
                if (nCalibrationStatus == MIL.M_CALIBRATED)
                {
                    MIL_INT iSrcSizeX = 0; MIL_INT iSizeX = 0;
                    MIL.MbufInquire(p_oCalibImg, MIL.M_SIZE_X, ref iSizeX);
                    iSrcSizeX = iSizeX;
                    MIL_INT iSrcSizeY = 0; MIL_INT iSizeY = 0;
                    MIL.MbufInquire(p_oCalibImg, MIL.M_SIZE_Y, ref iSizeY);
                    iSrcSizeY = iSizeY;

                    // old method only use pixel size trend from specific mire pts (version = 1.0).
                    // do not remove comment,  it is for memory to understand older version
                    //double dTargetedPixelSize = (dTrendPixelSizeX + dTrendPixelSizeY) / 2.0;

                    // new version take pixel size from calibration from McalList (version = 2.0)
                    double calibPixelSizeX = 0.0; double calibPixelSizeY = 0.0;
                    MIL.McalInquire(oMilCalibration, MIL.M_PIXEL_SIZE_X, ref calibPixelSizeX);
                    MIL.McalInquire(oMilCalibration, MIL.M_PIXEL_SIZE_Y, ref calibPixelSizeY);
                    double dTargetedPixelSize = (calibPixelSizeX + calibPixelSizeY) / 2.0;

                    iSizeX = (MIL_INT)((p_dMarginLeft + m_fWaferSize_um + p_dMarginRight) / dTargetedPixelSize);
                    iSizeY = (MIL_INT)((p_dMarginTop + m_fWaferSize_um + p_dMarginBottom) / dTargetedPixelSize);           
                   
                    MIL_ID oCalImgOUT = MIL.M_NULL;
                    MIL.MbufAlloc2d(m_MilSystem, (MIL_INT)iSizeX, (MIL_INT)iSizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref oCalImgOUT);

                    // // MIL9 - Calibration methode (deprecated since MIL10) (version = 1.0).
                    // do not remove those comments,  it is for memory to understand older version
                    // Mode USER DEFINE       
                    //MIL.McalControl(oMilCalibration, MIL.M_TRANSFORM_IMAGE_FILL_MODE, MIL.M_USER_DEFINED);
                    //MIL.McalRelativeOrigin(oMilCalibration, -(m_fWaferSize_um / 2.0), (m_fWaferSize_um / 2.0), 0, 0, MIL.M_DEFAULT);
                    //MIL.McalControl(oMilCalibration, MIL.M_TRANSFORM_IMAGE_WORLD_POS_X, -p_dMarginLeft);
                    //MIL.McalControl(oMilCalibration, MIL.M_TRANSFORM_IMAGE_WORLD_POS_Y, -(m_fWaferSize_um + p_dMarginTop));
                    //MIL.McalControl(oMilCalibration, MIL.M_TRANSFORM_IMAGE_PIXEL_SIZE_X, dTargetedPixelSize);
                    //MIL.McalControl(oMilCalibration, MIL.M_TRANSFORM_IMAGE_PIXEL_SIZE_Y, dTargetedPixelSize);

                    MIL.McalUniform(oCalImgOUT,
                        -p_dMarginLeft - waferradius_um,                    // match old deprecated McalControl with M_TRANSFORM_IMAGE_WORLD_POS_X
                        -(m_fWaferSize_um + p_dMarginTop) + waferradius_um, // match old deprecated McalControl with M_TRANSFORM_IMAGE_WORLD_POS_Y1
                        dTargetedPixelSize,                                 // match old deprecated McalControl with M_TRANSFORM_IMAGE_PIXEL_SIZE_X
                        dTargetedPixelSize,                                 // match old deprecated McalControl with M_TRANSFORM_IMAGE_PIXEL_SIZE_Y
                        0.0, MIL.M_DEFAULT);

                    // transform image
                    // with MILXsp6 : need to add M_USE_DESTINATION_CALIBRATION flag to M_WARP_IMAGE
                    MIL.McalTransformImage(p_oCalibImg, oCalImgOUT, oMilCalibration, MIL.M_BILINEAR/*M_BICUBIC*/ + MIL.M_OVERSCAN_CLEAR, MIL.M_FULL_CORRECTION, MIL.M_WARP_IMAGE + MIL.M_USE_DESTINATION_CALIBRATION);

                    if (reportDebugImages)
                    {
                        ReportDebugImage(p_oCalibImg, oMilCalibration, iSrcSizeX, iSizeX, iSrcSizeY, iSizeY, oCalImgOUT);
                    }

                    if (oCalImgOUT != MIL.M_NULL)
                    {
                        MIL.MbufFree(oCalImgOUT);
                        oCalImgOUT = MIL.M_NULL;
                    }

                    string destFile = destPSDFilePath;

                    // If the destPSDFilePath has not been provided by the comand line
                    if (string.IsNullOrEmpty(destFile))
                    {
                        SaveFileDialog oFileDlg = new SaveFileDialog();
                        oFileDlg.Title = "Save Calibration file";
                        oFileDlg.FileName = String.Format("PSD_{0:yyyyMMdd_hhmmss}.psd", DateTime.Now);
                        DialogResult ores = oFileDlg.ShowDialog();
                        if (ores == DialogResult.OK)
                        {
                            destFile = oFileDlg.FileName;
                        }
                        else
                            OnDisplayInfo("Calibration Done - User skip File saving !");
                    }

                    if (!string.IsNullOrEmpty(destFile))
                    { 
                        if (File.Exists(destFile) == true) // si le fichier existe dejà on le supprime
                        {
                            File.Delete(destFile);
                        }

                        DMTCalWriter ocalwriter = new DMTCalWriter(m_MilSystem, oMilCalibration, (int)iSrcSizeX, (int)iSrcSizeY, (int)iSizeX, (int)iSizeY, dTargetedPixelSize, p_dMarginTop, p_dMarginBottom, p_dMarginRight, p_dMarginLeft, (double) m_fWaferSize_um);
                        try
                        {
                            ocalwriter.Save(destFile);

                            if (reportDebugImages)
                            {
                                var destBaseName = Path.GetFileNameWithoutExtension(destFile);
                                var destRoot = new DirectoryInfo(destFile).Parent.FullName;
                                var srcDbgFile = Path.Combine(destRoot, $"{destBaseName}__DBG_SrcImage.png");
                                var outDbgFile = Path.Combine(destRoot, $"{destBaseName}__DBG_Transformed.png");
                                if (File.Exists(srcDbgFile) == true)
                                {
                                    File.Delete(srcDbgFile);
                                }
                                if (File.Exists(outDbgFile) == true)
                                {
                                    File.Delete(outDbgFile);
                                }

                                File.Move("DBG_SrcImage.png", srcDbgFile);
                                File.Move("DBG_Transformed.png", outDbgFile);
                            }


                            m_sLastSaveFile = destFile;
                            OnDisplayInfo("Calibration Done with Success.");
                        }
                        catch (Exception)
                        {
                            OnDisplayInfo("Calibration Done with Success. - Error while saving file");
                        }
                    }
                }
                else
                {
                    OnDisplayInfo("ERROR : the system cannot be calibrated !!! ");
                    OnAbort();
                }

                if (oMilCalibration != MIL.M_NULL)
                {
                    MIL.McalFree(oMilCalibration);
                }           
            }


            Cursor.Current = Cursors.Default;
        }

        private void ReportDebugImage(MIL_ID p_oCalibImg, MIL_ID oMilCalibration, MIL_INT iSrcSizeX, MIL_INT iSizeX, MIL_INT iSrcSizeY, MIL_INT iSizeY, MIL_ID oCalImgOUT)
        {
            MIL_ID oCalImgSRCColor = MIL.M_NULL;
            MIL_ID oCalImgOUTColor = MIL.M_NULL;
            try
            {
                const int milPNGCompressionLevelFlag = MIL.M_PNG_COMPRESSION_LEVEL5; //M_PNG_COMPRESSION_LEVELn where n is a value from 0 to 9.  //The highest compression rate (0 is lowest and 9 is highest) will result in the smallest file, but will take longer to save. 
#if DEBUG
                            // save output image
                            MIL.MbufExport("DBG_IMG_IN_Src.png", MIL.M_PNG + milPNGCompressionLevelFlag, p_oCalibImg);
                            MIL.MbufExport("DBG_IMG_OUT_Transformed.png", MIL.M_PNG + milPNGCompressionLevelFlag, oCalImgOUT);
#endif

                // SOURCE IMAGE 
                MIL.MbufAllocColor(m_MilSystem, 3, (MIL_INT)iSrcSizeX, (MIL_INT)iSrcSizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, ref oCalImgSRCColor);
                MIL.MimConvert(p_oCalibImg, oCalImgSRCColor, MIL.M_L_TO_RGB);
                MIL.MgraColor(MIL.M_DEFAULT, MIL.M_COLOR_RED);
                MIL.McalDraw(MIL.M_DEFAULT, oMilCalibration, oCalImgSRCColor, MIL.M_DRAW_ABSOLUTE_COORDINATE_SYSTEM + MIL.M_DRAW_AXES, MIL.M_DEFAULT, MIL.M_DEFAULT);
                MIL.MgraColor(MIL.M_DEFAULT, MIL.M_COLOR_YELLOW);
                MIL.McalDraw(MIL.M_DEFAULT, oMilCalibration, oCalImgSRCColor, MIL.M_DRAW_IMAGE_POINTS /*+ MIL.M_DRAW_CALIBRATION_ERROR*/, MIL.M_DEFAULT, MIL.M_DEFAULT);
                MIL.MbufExport("DBG_SrcImage.png", MIL.M_PNG + milPNGCompressionLevelFlag, oCalImgSRCColor);
                MIL.MbufFree(oCalImgSRCColor);
                oCalImgSRCColor = MIL.M_NULL;
                // OUTPUT TRANSFORMED IMAGE
                MIL.MbufAllocColor(m_MilSystem, 3, (MIL_INT)iSizeX, (MIL_INT)iSizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, ref oCalImgOUTColor);
                MIL.MimConvert(oCalImgOUT, oCalImgOUTColor, MIL.M_L_TO_RGB);

                //Draw the absolute coordinate system
                MIL.MgraColor(MIL.M_DEFAULT, MIL.M_COLOR_YELLOW);
                MIL.McalDraw(MIL.M_DEFAULT, oMilCalibration, oCalImgOUTColor, MIL.M_DRAW_ABSOLUTE_COORDINATE_SYSTEM + MIL.M_DRAW_ALL, MIL.M_DEFAULT, MIL.M_DEFAULT);
                //MIL.McalDraw(MIL.M_DEFAULT, MIL.M_NULL, oCalImgOUTColor, MIL.M_DRAW_PIXEL_COORDINATE_SYSTEM + MIL.M_DRAW_ALL, MIL.M_DEFAULT, MIL.M_DEFAULT);
                MIL.MgraColor(MIL.M_DEFAULT, MIL.M_COLOR_RED);
                MIL.McalDraw(MIL.M_DEFAULT, oMilCalibration, oCalImgOUTColor, MIL.M_DRAW_WORLD_POINTS /*+ MIL.M_DRAW_CALIBRATION_ERROR*/, MIL.M_DEFAULT, MIL.M_DEFAULT);
                MIL.MbufExport("DBG_Transformed.png", MIL.M_PNG + milPNGCompressionLevelFlag, oCalImgOUTColor);
                MIL.MbufFree(oCalImgOUTColor);
                oCalImgOUTColor = MIL.M_NULL;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Could Not Save All Debug Images, Skip to next part. - Error while saving file \n{e.Message}");
            }
            finally
            {
                if (oCalImgSRCColor != MIL.M_NULL)
                {
                    MIL.MbufFree(oCalImgSRCColor);
                    oCalImgSRCColor = MIL.M_NULL;
                }

                if (oCalImgOUTColor != MIL.M_NULL)
                {
                    MIL.MbufFree(oCalImgOUTColor);
                    oCalImgOUTColor = MIL.M_NULL;
                }
            }
        }

        virtual protected void SearchPts(Pts oPts, MIL_ID oPatternFinderID)
        {
            if (oPts.SearchAreaInImage.IsEmpty)
            {
                //MessageBox.Show("oPts.SearchAreaInImage.IsEmpty");
                return;
            }

            // on cree le child
            MIL_ID oChild = MIL.M_NULL;
            MIL.MbufChild2d(m_MilSrcCalibImgID, oPts.SearchAreaInImage.Left, oPts.SearchAreaInImage.Top, oPts.SearchAreaInImage.Width, oPts.SearchAreaInImage.Height, ref oChild);

            // on alloue les données resultats
            MIL_ID MilAlignResult = MIL.M_NULL;
            MIL.MmodAllocResult(m_MilSystem, MIL.M_DEFAULT, ref MilAlignResult);

            //on applique la recherche dans le child 
            MIL.MmodFind(oPatternFinderID, oChild, MilAlignResult);
            double dNbResults = 0.0;
            MIL.MmodGetResult(MilAlignResult, MIL.M_DEFAULT, MIL.M_NUMBER, ref dNbResults);

            if (oPatternFinderID == MIL.M_NULL)
                MessageBox.Show("(oPatternFinderID == MIL.M_NULL)");
            if (oChild == MIL.M_NULL)
                MessageBox.Show("(oChild == MIL.M_NULL)");
            if (oPatternFinderID == MIL.M_NULL)
                MessageBox.Show("(oPatternFinderID == MIL.M_NULL)");

            // si trouvé on update le pts
            if(dNbResults > 0.0)
            {
                double dX = 0.0;
                double dY = 0.0;
                double dScore = 0.0;
               // double dScale = 0.0;
               // double dAngle = 0.0;

                MIL.MmodGetResult(MilAlignResult, 0, MIL.M_SCORE, ref dScore);
                MIL.MmodGetResult(MilAlignResult, 0, MIL.M_POSITION_X, ref dX);
                MIL.MmodGetResult(MilAlignResult, 0, MIL.M_POSITION_Y, ref dY);

                oPts.dPosX_px = dX + oPts.SearchAreaInImage.Left;
                oPts.dPosY_px = dY + oPts.SearchAreaInImage.Top;
                oPts.bFound = true;

                //MIL.MmodGetResult(MilAlignResult, 0, MIL.M_SCALE, dScale);
                //MIL.MmodGetResult(MilAlignResult, 0, MIL.M_ANGLE, dAngle);
            }
            else
            {
             //   MessageBox.Show(String.Format("NO RESULTS !!!"));
            }
            MIL.MbufFree(oChild);
            MIL.MmodFree(MilAlignResult);

        }

        
        protected  Pts SearchPreviousPtsOnLeft(Pts oSrcPts)
        {
            int j = oSrcPts.nyy+m_nCenterGridIndexY;
            for(int i = oSrcPts.nxx+m_nCenterGridIndexX - 1; i>=0; i--)
            {
                if (m_GridPts[i,j].bFound)
                    return m_GridPts[i,j];
            }
            return m_GridPts[m_nCenterGridIndexX,m_nCenterGridIndexY]; 
        }

        protected  Pts SearchPreviousPtsOnRight(Pts oSrcPts)
        {
            int j = oSrcPts.nyy+m_nCenterGridIndexY;
            for (int i = oSrcPts.nxx + m_nCenterGridIndexX + 1; i <= m_nCenterGridIndexX; i++)
            {
                if (m_GridPts[i,j].bFound)
                    return m_GridPts[i,j];
            }
            return m_GridPts[m_nCenterGridIndexX,m_nCenterGridIndexY]; 
        }

        protected Pts SearchPreviousPtsDownside(Pts oSrcPts)
        {
            int i = oSrcPts.nxx + m_nCenterGridIndexX;
            for (int j = oSrcPts.nyy + m_nCenterGridIndexY + 1;  j < m_nGridSizeY; j++)
            {
                if (m_GridPts[i, j].bFound)
                    return m_GridPts[i, j];
            }
            return m_GridPts[m_nCenterGridIndexX, m_nCenterGridIndexY];
        }

        protected Pts SearchPreviousPtsUpside(Pts oSrcPts)
        {
            int i = oSrcPts.nxx + m_nCenterGridIndexX;
            for (int j = oSrcPts.nyy + m_nCenterGridIndexY - 1; j >= 0; j--)
            {
                if (m_GridPts[i, j].bFound)
                    return m_GridPts[i, j];
            }
            return m_GridPts[m_nCenterGridIndexX, m_nCenterGridIndexY];
        }


    }
}

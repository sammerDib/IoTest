using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using BasicModules.BorderRemoval;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.Data.Enum;

namespace GlobaltopoModule
{
    public class GTMeasurementModule : ModuleBase
    {

        public readonly GTParameters InPrm;

        /*
                //=================================================================
                // Paramètres du XML
                //=================================================================
                public readonly DoubleParameter paramEdgeExcusion_mm;


                // Region 
                // INPUTS
                //                ^
                //                | 
                //                Y2 
                //                | 
                //                |
                //   ------ X1----+------X2---->     X1< WaferCenter < X2
                //                |
                //                |
                //               Y1
                //                |
                //       Y1 < WaferCenter < Y2
                //
                public readonly DoubleParameter paramX1_mm; // en mm par rapport centre wafer (repere wafer) - default -25
                public readonly DoubleParameter paramX2_mm; // en mm par rapport centre wafer (repere wafer) - default +25
                public readonly DoubleParameter paramY1_mm; // en mm par rapport centre wafer (repere wafer) - default -25
                public readonly DoubleParameter paramY2_mm; // en mm par rapport centre wafer (repere wafer) - default +25

                public readonly DoubleParameter paramRadiusCenterBow;   // en mm
                public readonly EnumParameter<PrmLowPassType.lpType> paramLowPassKernelType;//0: none; 1:Smooth; 2: uniform; 3:Gaussian - default 0 
                public readonly IntParameter paramLowPassKernelSize;    // 3<=K<=21 //doit etre impaire !! - default 3
                public readonly DoubleParameter paramLowPassGaussianSigma; // only for Low pass gaussian type =>  sigma used for gaussian - Default 1.0f

                // exclusion boxes in wafer 
                 // parameters input are centers of box X|Y in mm and Its Width and Height also in mm. those are express in wafer coordinates reférential
                 // Param List RectangleF /!\ could be empty
        */
        //=================================================================
        // Constructeur
        //=================================================================
        public GTMeasurementModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            InPrm = new GTParameters(this, "InPrm");
        }

        protected override void OnInit()
        {
            base.OnInit();
        }


        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            ImageBase image = (ImageBase)obj;
            MIL_ID MilGraphContext = MIL.M_NULL;
            MIL.MgraAlloc(Mil.Instance.HostSystem, ref MilGraphContext);
            MIL_ID MilStatContextId = MIL.M_NULL;
            MIL_ID MilStatResult = MIL.M_NULL;
            MIL.MimAlloc(Mil.Instance.HostSystem, MIL.M_STATISTICS_CONTEXT, MIL.M_DEFAULT, ref MilStatContextId);
            MIL.MimAllocResult(Mil.Instance.HostSystem, MIL.M_DEFAULT, MIL.M_STATISTICS_RESULT, ref MilStatResult);

            try
            {
                ImageLayerBase imgLayer = (ImageLayerBase)image.Layer;
                if (imgLayer.ResultType.GetActorType() != ActorType.DEMETER)
                {
                    throw new ApplicationException("Bad Layer actor (expecting : DEMETER) : " + imgLayer.ResultType.GetActorType());
                }

#pragma warning disable CS0618 // Obsolete // to remove when the foolowing thrw will be removed
#pragma warning disable CS0162 // Unreachable code detected // to remove when the foolowing thrw will be removed
                // rti need to use tjose moduel as helios module , lightspeed should not be integrated in UPS for the moment
                throw new ApplicationException("RESULT TYPE OF THIS KIND IS NOT YET HANDLED for GlobalTopo measurmenent");

                var restyp = imgLayer.ResultType;
             //   if (!(restyp == ResultType.DMT_TopoPhaseNZ_Front || restyp == ResultType.DMT_TopoPhaseNZ_Back 
             //      || restyp == ResultType.DMT_TopoBackLight_Front || restyp == ResultType.DMT_TopoBackLight_Back))
                if (true)
                {
                    throw new ApplicationException($"Wrong Result Type (expecting : TopoPhaseNZ or TopoBackLight) : {restyp}");
                }

                if (MilGraphContext == MIL.M_NULL)
                    throw new ApplicationException("Unable to create Graph context : Mil Memory failure)");

                if (MilStatContextId == MIL.M_NULL)
                    throw new ApplicationException("Unable to create StatContextId : Mil Memory failure)");

                if (MilStatResult == MIL.M_NULL)
                    throw new ApplicationException("Unable to create StatResult : Mil Memory failure)");

                //
                // Inputs Paramters
                //

                float fEdgeExclusion_um = InPrm.InputPrmClass.EdgeExcusion_mm * 1000.0f;
                List<Rectangle> PrmBoxesExclusion_px = new List<Rectangle>();
                foreach (RectangleF rcf in InPrm.InputPrmClass.ExcludeAreasList)
                {
                    float fX, fY, fW, fH; // en mm au départ referential Wafer coordinates, X Y rectangle center
                    fX = rcf.X;
                    fY = rcf.Y;
                    fW = rcf.Width;
                    fH = rcf.Height;
                    // calcul origine car fx et fy sont les coord du centre de la box d'exclusion
                    fX -= fW * 0.5f;
                    fY -= fH * 0.5f;
                    // passage mm en µm
                    fX *= 1000.0f;
                    fY *= 1000.0f;
                    fW *= 1000.0f;
                    fH *= 1000.0f;
                    // convertion micron to pixel
                    RectangleF rcBoxMicrons = new RectangleF(fX, fY, fW, fH);
                    PrmBoxesExclusion_px.Add(imgLayer.Matrix.micronToPixel(rcBoxMicrons));
                }

                using (GTMeasure Gtmes = new GTMeasure())
                {
                    Gtmes.Data.m_fEdgeExclusion_um = fEdgeExclusion_um;
                    Gtmes.Data.m_BoxesExclusion = InPrm.InputPrmClass.ExcludeAreasList;
                    Gtmes.Data.m_fRadiusCenterBow_um = InPrm.InputPrmClass.RadiusCenterBow * 1000.0f;
                    Gtmes.Data.m_nLowPassKernelType = (int)InPrm.InputPrmClass.LowPassKernelType;
                    Gtmes.Data.m_nLowPassKernelSize = InPrm.InputPrmClass.LowPassKernelSize;
                    Gtmes.Data.m_fLowPassKernelGaussianSigma = InPrm.InputPrmClass.LowPassGaussianSigma;

                    Gtmes.Data.m_fX1_um = InPrm.InputPrmClass.X1_mm * 1000.0f;
                    Gtmes.Data.m_fX2_um = InPrm.InputPrmClass.X2_mm * 1000.0f;
                    Gtmes.Data.m_fY1_um = InPrm.InputPrmClass.Y1_mm * 1000.0f;
                    Gtmes.Data.m_fY2_um = InPrm.InputPrmClass.Y2_mm * 1000.0f;

                    float lfPixelSizeX_um = 1.0f;
                    float lfPixelSizeY_um = 1.0f;
                    int iCenterX_px = 0;
                    int iCenterY_px = 0;

                    WaferBase wafer = image.Layer.Wafer;
                    Gtmes.Data.m_nWaferSize_mm = (int)(wafer.SurroundingRectangle.Width / 1000.0f);

                    if (imgLayer.Matrix is RectangularMatrix)
                    {
                        RectangularMatrix mat = imgLayer.Matrix as RectangularMatrix;
                        lfPixelSizeX_um = mat.PixelWidth;
                        lfPixelSizeY_um = mat.PixelHeight;

                        iCenterX_px = (int)Math.Round(mat.WaferCenterX);
                        iCenterY_px = (int)Math.Round(mat.WaferCenterY);
                    }
                    Gtmes.Data.m_dPixelSizeX = lfPixelSizeX_um;
                    Gtmes.Data.m_dPixelSizeY = lfPixelSizeY_um;

                    MilImage imgIN_float = image.ResultProcessingImage.GetMilImage();
                    int nSize_X = imgIN_float.SizeX;
                    int nSize_Y = imgIN_float.SizeY;
                    int nDepth = imgIN_float.SizeBit; // normally should be 32 float
                    float fExludingVal = -666.0666f;

                    using (MilImage milFltMaskNan = new MilImage())
                    using (MilImage milBinMaskImage = new MilImage())
                    {

                        milFltMaskNan.Alloc2dCompatibleWith(imgIN_float);
                        milBinMaskImage.Alloc2d(nSize_X, nSize_Y, 1 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
                        milBinMaskImage.Clear(1);

                        //---------------------------------
                        // MASK : edge remove pour mettre à jour le mask
                        //---------------------------------

                        using (ImageBase mskimage = (ImageBase)image.DeepClone())
                        {
                            MilImage milmsk = mskimage.ResultProcessingImage.GetMilImage();
                            // on passe en 8 bits
                            MIL.MbufFree(milmsk.DetachMilId());
                            milmsk.Alloc2d(nSize_X, nSize_Y, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
                            MIL.MimBinarize(imgIN_float.MilId, milmsk.MilId, MIL.M_IN_RANGE, -float.MaxValue + 1.0f, float.MaxValue - 1.0f); // on s'affranchi des NaNs

                            MIL.MbufCopy(milmsk.MilId, milFltMaskNan.MilId);

                            EdgeRemoveAlgorithm edgeRemoveAlgo = new EdgeRemoveAlgorithm();
                            edgeRemoveAlgo.Margin = fEdgeExclusion_um;
                            edgeRemoveAlgo.PerformRemoval(mskimage);

                            milmsk = mskimage.ResultProcessingImage.GetMilImage();
                            // MIL.MimBinarize(milmsk.MilId, milmsk.MilId, MIL.M_GREATER_OR_EQUAL, 1, MIL.M_NULL);
                            milmsk.Arith(255.0, MIL.M_DIV_CONST);
                            milFltMaskNan.Arith(255.0, MIL.M_DIV_CONST);

                            // On applique le mask à l'image
                            MilImage.Arith(milBinMaskImage, milmsk, milBinMaskImage, MIL.M_MULT); // and ?
                        }

                        //-------------------
                        // MASK : Mise à jour des box d'exclusion
                        //-------------------
                        if (PrmBoxesExclusion_px != null && PrmBoxesExclusion_px.Count > 0)
                        {
                            MIL.MgraColor(MilGraphContext, 0.0);
                            foreach (Rectangle rc in PrmBoxesExclusion_px)
                            {
                                MIL.MgraRectFill(MilGraphContext, milBinMaskImage.MilId, rc.Left, rc.Top, rc.Right, rc.Bottom);
                            }
                        }

                        // on verifie que l'on ne fera pas la gestion des Nan pour rien
                        double dNumbernan = 0.0;
                        MIL.MimControl(MilStatContextId, MIL.M_STAT_NUMBER, MIL.M_ENABLE);
                        MIL.MimControl(MilStatContextId, MIL.M_CONDITION, MIL.M_EQUAL);
                        MIL.MimControl(MilStatContextId, MIL.M_COND_LOW, 0.0);

                        MIL.MimStatCalculate(MilStatContextId, milFltMaskNan.MilId, MilStatResult, MIL.M_DEFAULT);

                        MIL.MimGetResult(MilStatResult, MIL.M_STAT_NUMBER, ref dNumbernan);
                        bool bIsMaskHaveNaNs = (dNumbernan > 0.0);

                        //-------------------
                        // Low Pass filter
                        //-------------------

                        switch (InPrm.InputPrmClass.LowPassKernelType)
                        {

                            case lpType.Smooth: //1-Smooth
                                {
                                    imgIN_float.Convolve(MIL.M_SMOOTH + MIL.M_OVERSCAN_DISABLE);
                                }
                                break;
                            case lpType.Uniform: //2- Uniform - Average
                                {
                                    // A cause de la convolution ici les Nan foutent la merde on les virent donc !
                                    //
                                    // New Problem le Zeros peut être une valeur trop eloigne de la réel valeur du bord - ce qui crée des effet aux bord du wafer suite a la convolution
                                    // Plusieur solution:
                                    // * soit un se fait la convolution à la mano : (lent à trés lent)
                                    //          - dés q'un Nan est dans le Kernel on passe au pixel suivant la valeur suivante (on laisse la valeur courante)
                                    // * Soit on rectifie le tir des pixel que l'on sait qui vont déborder et on les remplace aprés process par leur valeur initiale
                                    //    => Erode de la taille kernel / 2  du maskNan (crée MaskDilateNan)
                                    //    => Intersection  du maskNan et MaskDilateNan (MaskRenew)
                                    //    => ImgRenew = MaskRenew x Img3DAvantCovol
                                    //    => Convol
                                    //    => Remise des valeur initiale dans le buffer final a l'aide de ImgRenew
                                    //
                                    // => On mets en place la 2eme manière


                                    using (MilImage milBinMaskNanErode = new MilImage())
                                    using (MilImage milFltSaveBorder = new MilImage())
                                    {

                                        int nLPKernelSize = InPrm.InputPrmClass.LowPassKernelSize;
                                        if (bIsMaskHaveNaNs)
                                        {
                                            MilImage.Arith(imgIN_float, milFltMaskNan, imgIN_float, MIL.M_MULT + MIL.M_SATURATION); //On force la saturation pour virer les nan - qui seront remplacé par une valeur FloatMax
                                            MilImage.Arith(imgIN_float, milFltMaskNan, imgIN_float, MIL.M_MULT); //La 2eme fois on doit passer les floatmax à Zeros

                                            // les Nan sont mis à zéros pour le calcul de convolution
                                            // gestion des limites aux bords
                                            milBinMaskNanErode.Alloc2d(nSize_X, nSize_Y, 1 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
                                            milFltSaveBorder.Alloc2dCompatibleWith(imgIN_float);

                                            MilImage.Erode(milFltMaskNan, milBinMaskNanErode, (nLPKernelSize / 2), MIL.M_BINARY);
                                            MilImage.Arith(milBinMaskNanErode, milFltMaskNan, milBinMaskNanErode, MIL.M_XOR);
                                            milFltSaveBorder.Clear(0.0);
                                            MilImage.Arith(imgIN_float, milBinMaskNanErode, milFltSaveBorder, MIL.M_MULT);
                                        }

                                        MIL_ID Kernel = MIL.M_NULL;
                                        MIL.MbufAlloc2d(Mil.Instance.HostSystem, nLPKernelSize, nLPKernelSize, 32 + MIL.M_FLOAT, MIL.M_KERNEL, ref Kernel);
                                        MIL.MbufClear(Kernel, 1.0f / (float)(nLPKernelSize * nLPKernelSize));
                                        MIL.MbufControlNeighborhood(Kernel, MIL.M_OFFSET_CENTER_X, (nLPKernelSize / 2));
                                        MIL.MbufControlNeighborhood(Kernel, MIL.M_OFFSET_CENTER_Y, (nLPKernelSize / 2));
                                        imgIN_float.Convolve(Kernel);
                                        MIL.MbufFree(Kernel); Kernel = MIL.M_NULL;

                                        if (bIsMaskHaveNaNs)
                                        {
                                            MilImage.Arith(imgIN_float, milFltMaskNan, imgIN_float, MIL.M_MULT); // /!\ pas de saturation sinon on perd le Nan !
                                                                                                                 // les Nan sont mis à zéros pour la suite;       
                                                                                                                 // On remplace les limites par les valeurs initiales
                                            milBinMaskNanErode.Arith(MIL.M_NOT);// on inverse le maskNanDilate pour avoir des Zéro là où sont les valeurs de bordures
                                            MilImage.Arith(imgIN_float, milBinMaskNanErode, imgIN_float, MIL.M_MULT); // mise à zero des valeurs bordures
                                            MilImage.Arith(imgIN_float, milFltSaveBorder, imgIN_float, MIL.M_ADD); // on remplace les valeurs bordure par les valeur initiales précédemmetn sauvées
                                                                                                                   // libération des buffer utilisés
                                        }
                                    }
                                }
                                break;
                            case lpType.Gaussian: // 3-Gaussian
                                {
                                    // A cause de la convolution ici les Nan foutent la merde on les virent donc !
                                    //
                                    // on a ajoute aussi la gestion des limites en bord de Nan comme pour le Average ci-dessus

                                    // compute Gaussan coef
                                    float sigma = InPrm.InputPrmClass.LowPassGaussianSigma;
                                    int nLPKernelSize = InPrm.InputPrmClass.LowPassKernelSize;
                                    int W = nLPKernelSize;
                                    float[] fDatakernel = new float[W * W];
                                    float sum = 0.0f; // For accumulating the fDatakernel values
                                    double dOffestPosCenter = Math.Floor((double)nLPKernelSize / 2.0);

                                    // avec Xi et Yi indicé de manière à ce que le [0,0] soit au centre du kernel
                                    //
                                    //                 1                      Xi² + Yi²
                                    // G(Xi,Yi) = --------------- * Exp( -1 * ---------- )
                                    //           2 * pi * Sig²               2 * Sig²

                                    // soit dSig2 == Sig²
                                    // soit dKdpi == 1/(2*pi*dSig2)
                                    double dSig2 = (double)sigma * (double)sigma;
                                    System.Diagnostics.Debug.Assert(dSig2 > 0.0);
                                    double dKdpi = (2.0 * Math.PI * dSig2);
                                    dKdpi = 1.0 / dKdpi;
                                    for (int x = 0; x < W; ++x)
                                    {
                                        double dXi = (double)x - dOffestPosCenter;
                                        for (int y = 0; y < W; ++y)
                                        {
                                            double dYi = (double)y - dOffestPosCenter;
                                            double Gxy = dKdpi * Math.Exp(-(dXi * dXi + dYi * dYi) / (2.0 * dSig2));
                                            fDatakernel[x + y * W] = (float)Gxy;
                                            // Accumulate the fDatakernel values 
                                            sum += fDatakernel[x + y * W];
                                        }
                                    }

                                    using (MilImage milBinMaskNanErode = new MilImage())
                                    using (MilImage milFltSaveBorder = new MilImage())
                                    {
                                        if (bIsMaskHaveNaNs)
                                        {
                                            MilImage.Arith(imgIN_float, milFltMaskNan, imgIN_float, MIL.M_MULT + MIL.M_SATURATION); //On force la saturation pour virer les nan - qui seront remplacé par une valeur FloatMax
                                            MilImage.Arith(imgIN_float, milFltMaskNan, imgIN_float, MIL.M_MULT); //La 2eme fois on doit passer les floatmax à Zeros

                                            // les Nan sont mis à zéros pour le calcul de convolution
                                            // gestion des limites aux bords
                                            milBinMaskNanErode.Alloc2d(nSize_X, nSize_Y, 1 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
                                            milFltSaveBorder.Alloc2dCompatibleWith(imgIN_float);

                                            MilImage.Erode(milFltMaskNan, milBinMaskNanErode, (nLPKernelSize / 2), MIL.M_BINARY);
                                            MilImage.Arith(milBinMaskNanErode, milFltMaskNan, milBinMaskNanErode, MIL.M_XOR);
                                            milFltSaveBorder.Clear(0.0);
                                            MilImage.Arith(imgIN_float, milBinMaskNanErode, milFltSaveBorder, MIL.M_MULT);
                                        }

                                        MIL_ID GaussKernel = MIL.M_NULL;
                                        MIL.MbufAlloc2d(Mil.Instance.HostSystem, W, W, 32 + MIL.M_FLOAT, MIL.M_KERNEL, ref GaussKernel);
                                        MIL.MbufPut(GaussKernel, fDatakernel);
                                        // Normalize the kernel
                                        MIL.MimArith(GaussKernel, 1.0f / sum, GaussKernel, MIL.M_MULT_CONST);
                                        MIL.MbufControlNeighborhood(GaussKernel, MIL.M_OFFSET_CENTER_X, (W / 2));
                                        MIL.MbufControlNeighborhood(GaussKernel, MIL.M_OFFSET_CENTER_Y, (W / 2));

                                        MIL.MbufControlNeighborhood(GaussKernel, MIL.M_OVERSCAN, MIL.M_DISABLE);

                                        imgIN_float.Convolve(GaussKernel);
                                        MIL.MbufFree(GaussKernel); GaussKernel = MIL.M_NULL;

                                        if (bIsMaskHaveNaNs)
                                        {
                                            MilImage.Arith(imgIN_float, milFltMaskNan, imgIN_float, MIL.M_MULT); // /!\ pas de saturation sinon on perd le Nan !
                                                                                                                 // les Nan sont mis à zéros pour la suite;       
                                                                                                                 // On remplace les limites par les valeurs initiales
                                            milBinMaskNanErode.Arith(MIL.M_NOT);// on inverse le maskNanDilate pour avoir des Zéro là où sont les valeurs de bordures
                                            MilImage.Arith(imgIN_float, milBinMaskNanErode, imgIN_float, MIL.M_MULT); // mise à zero des valeurs bordures
                                            MilImage.Arith(imgIN_float, milFltSaveBorder, imgIN_float, MIL.M_ADD); // on remplace les valeurs bordure par les valeur initiales précédemmetn sauvées
                                                                                                                   // libération des buffer utilisés
                                        }
                                    }
                                }
                                break;
                            case 0: // None
                            default:
                                break;
                        }

                        // On applique le mask d'une certaine manière à notre image ainsi les zone à exclure auront une hauteur de fExludingVal
                        MilImage.Arith(imgIN_float, milBinMaskImage, imgIN_float, MIL.M_MULT); // /!\ pas de saturation sinon on perd le Nan !

                        using (MilImage milFltExcludeMsk = new MilImage())
                        {
                            milFltExcludeMsk.Alloc2dCompatibleWith(imgIN_float);
                            milFltExcludeMsk.Clear(fExludingVal);
                            milBinMaskImage.Arith(MIL.M_NOT);

                            MilImage.Arith(milFltExcludeMsk, milBinMaskImage, milFltExcludeMsk, MIL.M_MULT); // /!\ pas de saturation sinon on perd le Nan !
                            MilImage.Arith(imgIN_float, milFltExcludeMsk, imgIN_float, MIL.M_ADD); // /!\ pas de saturation sinon on perd le Nan !
                        }
                    } // end using milFltMaskNan & milBinMaskImage

                    float[] pDataFloat = new float[nSize_X * nSize_Y]; // Note de RTI : ici pour Demeter - en memoire ça passe - attention au future evolution  pour la gestion des images
                    imgIN_float.Get(pDataFloat);

                    //
                    // Calcul équation du plan Moyen par moindre carrés
                    //

                    //Soit z=a.x+b.y+c l'équation du plan .
                    //la solution est donné par la résolution du système : 
                    //(Sxx).a+(Sxy).b+(Sx).c = (Sxz)
                    //(Sxy).a+(Syy).b+(Sy).c = (Syz)
                    //(Sx).a+(Sy).b+n.c = (Sz)
                    // Equivalence :
                    //  a = (Sxz - (Sx * Sz / n) - ((Sxy - Sy * Sx / n) * (Syz - Sy * Sz / n)) / (Syy - Sy * Sy / n))
                    //    / (Sxx - (Sx * Sx / n) - ((Sxy - Sy * Sx / n) * (Sxy - Sx * Sy / n)) / (Syy - Sy * Sy / n));
                    //  b = (Syz - Sy * Sz / n) / (Syy - Sy * Sy / n) - (Sxy - Sx * Sy / n) / (Syy - Sy * Sy / n) * a;
                    //  c = (Sz - Sx * a - Sy * b) / n;
                    //Les coefficients étant calculés préalablement par :
                    //Sxx = Somme des (xk)² pour k=1 à k=n
                    //Sxy = Somme des (xk)(yk)
                    //Sx = Somme des (xk)
                    //Sxz = Somme de (xk)(zk)
                    //Syy = Somme de (yk)²
                    //Syz = Somme de (yk)(zk)
                    //Szz = Somme de (zk)²
                    //Sz = Somme de (zk) 
                    //Sy = Somme de (yk) 

                    double Sxx = 0.0;
                    double Sxy = 0.0;
                    double Sxz = 0.0;
                    double Syy = 0.0;
                    double Syz = 0.0;
                    double Szz = 0.0;
                    double Sx = 0.0;
                    double Sy = 0.0;
                    double Sz = 0.0;
                    double n = 0.0;

                    // sampling pour pour une meilleur perf on prendra 1 point sur p_nSamples en X et en Y
                    double Z = 0.0f;
                    int nBestFitNbSample = InPrm.InputPrmClass.NbSamples;
                    if (nBestFitNbSample <= 0)
                        nBestFitNbSample = 1;
                    Gtmes.Data.m_nNSamples = nBestFitNbSample;
                    for (int x = 0; x < nSize_X; x = x + nBestFitNbSample)
                    {
                        for (int y = 0; y < nSize_Y; y = y + nBestFitNbSample)
                        {
                            if (float.IsNaN(pDataFloat[x + y * nSize_X]))
                                pDataFloat[x + y * nSize_X] = fExludingVal; // valeur non calculé on la place en zone exclusion
                            else
                            {
                                Z = (double)pDataFloat[x + y * nSize_X];
                                if (Z != fExludingVal)/*Si Zij n'est pas dans un zone d'exclusion*/
                                {
                                    ++n;
                                    Sxx += x * x;
                                    Sxy += x * y;
                                    Sxz += x * Z;
                                    Syy += y * y;
                                    Syz += y * Z;
                                    Szz += Z * Z;
                                    Sx += x;
                                    Sy += y;
                                    Sz += Z;
                                }
                            }

                        }
                    }

                    if (n == 0.0 || (Sxx - Sx * Sx / n) == 0 || (Syy - Sy * Sy / n) == 0)
                    {
                        // impossible de calculer un plan moyen
                        throw new ApplicationException("GlobalTopoRowWarp : Exit No Best Fit Plane possible.)");
                    }

                    //  z=a.x+b.y+c l'équation du plan (best fit plane)
                    double a = 0, b = 0, c = 0;
                    a = (Sxz - (Sx * Sz / n) - ((Sxy - Sy * Sx / n) * (Syz - Sy * Sz / n)) / (Syy - Sy * Sy / n)) / (Sxx - (Sx * Sx / n) - ((Sxy - Sy * Sx / n) * (Sxy - Sx * Sy / n)) / (Syy - Sy * Sy / n));
                    b = (Syz - Sy * Sz / n) / (Syy - Sy * Sy / n) - (Sxy - Sx * Sy / n) / (Syy - Sy * Sy / n) * a;
                    c = (Sz - Sx * a - Sy * b) / n;

                    //
                    // Calcul des mesures
                    //

                    float fMax = -float.MaxValue;
                    float fMin = float.MaxValue;
                    int dbg_nXMax = 0; int dbg_nYMax = 0;
                    int dbg_nXMin = 0; int dbg_nYMin = 0;

                    // Local Warp Map
                    for (int x = 0; x < nSize_X; x++)
                    {
                        for (int y = 0; y < nSize_Y; y++)
                        {
                            Z = (double)pDataFloat[x + y * nSize_X];
                            if (Z != fExludingVal) /*Si Zij n'est pas dans une zone d'exclusion*/
                            {
                                double dZBestFit = a * ((double)x) + b * ((double)y) + c;
                                Z -= dZBestFit;
                                pDataFloat[x + y * nSize_X] = (float)Z;
                                if (fMax < Z)
                                {
                                    fMax = (float)Z;

                                    dbg_nXMax = x;
                                    dbg_nYMax = y;
                                }
                                if (fMin > Z)
                                {
                                    fMin = (float)Z;

                                    dbg_nXMin = x;
                                    dbg_nYMin = y;
                                }
                            }
                            else
                                pDataFloat[x + y * nSize_X] = float.NaN;
                        }
                    }
                    // MessageBox.Show(String.Format("After bestFit\n Max = {0} @[{1};{2}]\nMin = {3} @[{4};{5}]", fMax, dbg_nXMax, dbg_nYMax, fMin, dbg_nXMin, dbg_nYMin));




                    //  Max Pos Warp (max Local Warp)
                    Gtmes.Data.m_fOut_MaxPosWarp = fMax;
                    //  Max Neg Warp (min Local Warp)
                    Gtmes.Data.m_fOut_MinNegWarp = fMin;
                    //  Total Warp (max - min)
                    Gtmes.Data.m_fOut_TotalWarp = fMax - fMin;

                    //  Bow BF (Bestfit @center)
                    bool noBowBF = (float.IsNaN(pDataFloat[iCenterX_px + iCenterY_px * nSize_X]));
                    if (!noBowBF)
                        Gtmes.Data.m_fOut_BowBF = pDataFloat[iCenterX_px + iCenterY_px * nSize_X];
                    else
                    {
                        Gtmes.Data.m_fOut_BowBF = 0.0f;
                        // pWafer.WriteInfoLog_loop("GlobalTopoRowWarp : Wafer center in Zone Exclusion : No Bow measures.");
                    }

                    //  Bow X
                    if (!noBowBF)
                    {

                        Point Pt1 = imgLayer.Matrix.micronToPixel(new PointF(Gtmes.Data.m_fX1_um, 0.0f));
                        int iX1x_px = Pt1.X;
                        int iX1y_px = Pt1.Y;
                        Point Pt2 = imgLayer.Matrix.micronToPixel(new PointF(Gtmes.Data.m_fX2_um, 0.0f));
                        int iX2x_px = Pt2.X;
                        int iX2y_px = Pt2.Y;

                        if (float.IsNaN(pDataFloat[iX1x_px + iX1y_px * nSize_X]) || float.IsNaN(pDataFloat[iX2x_px + iX2y_px * nSize_X]))
                        {
                            Gtmes.Data.m_fOut_BowX = 0.0f;
                            //pWafer.WriteInfoLog_loop("GlobalTopoRowWarp : X1 or X2 in Zone Exclusion : No BowX measure.");
                        }
                        else
                        {
                            Gtmes.Data.m_fOut_BowX = Gtmes.Data.m_fOut_BowBF - (pDataFloat[iX1x_px + iX1y_px * nSize_X] + pDataFloat[iX2x_px + iX2y_px * nSize_X]) / 2.0f;
                        }
                    }
                    else
                    {
                        Gtmes.Data.m_fOut_BowX = 0.0f;
                    }

                    //  Bow Y
                    if (!noBowBF)
                    {
                        Point Pt1 = imgLayer.Matrix.micronToPixel(new PointF(0.0f, Gtmes.Data.m_fY1_um));
                        int iY1x_px = Pt1.X;
                        int iY1y_px = Pt1.Y;
                        Point Pt2 = imgLayer.Matrix.micronToPixel(new PointF(0.0f, Gtmes.Data.m_fY2_um));
                        int iY2x_px = Pt2.X;
                        int iY2y_px = Pt2.Y;
                        if (float.IsNaN(pDataFloat[iY1x_px + iY1y_px * nSize_X]) || float.IsNaN(pDataFloat[iY2x_px + iY2y_px * nSize_X]))
                        {
                            Gtmes.Data.m_fOut_BowY = 0.0f;
                            //pWafer.WriteInfoLog_loop("GlobalTopoRowWarp : Y1 or Y2 in Zone Exclusion : No BowX measure.");
                        }
                        else
                        {
                            Gtmes.Data.m_fOut_BowY = Gtmes.Data.m_fOut_BowBF - (pDataFloat[iY1x_px + iY1y_px * nSize_X] + pDataFloat[iY2x_px + iY2y_px * nSize_X]) / 2.0f;
                        }
                    }
                    else
                    {
                        Gtmes.Data.m_fOut_BowY = 0.0f;
                    }

                    //  Bow XY (max (|BowX|,|BowY|))
                    if (Math.Abs(Gtmes.Data.m_fOut_BowX) > Math.Abs(Gtmes.Data.m_fOut_BowY))
                        Gtmes.Data.m_fOut_BowXY = Gtmes.Data.m_fOut_BowX;
                    else
                        Gtmes.Data.m_fOut_BowXY = Gtmes.Data.m_fOut_BowY;

                    //  Center Bow
                    //  application du Low filter ou non sur chaque points autour du radius // TODO ?
                    if (!noBowBF)
                    {
                        float fSumAngl = 0.0f;
                        int nbAngleTaken = 0;
                        double dCenterBowradius_um = (double)Gtmes.Data.m_fRadiusCenterBow_um;
                        for (int angle_dg = 1; angle_dg <= 360; angle_dg++)
                        {
                            double dAngle_rd = (Math.PI * ((double)angle_dg) / 180.0);
                            double dAx_um = dCenterBowradius_um * Math.Cos(dAngle_rd);
                            double dAy_um = dCenterBowradius_um * Math.Sin(dAngle_rd);
                            if (angle_dg == 31)
                            { int nn0 = 0; nn0++; }
                            Point PtA = imgLayer.Matrix.micronToPixel(new PointF((float)dAx_um, (float)dAy_um));
                            int nAx_px = PtA.X;
                            int nAy_px = PtA.Y;
                            if (float.IsNaN(pDataFloat[nAx_px + nAy_px * nSize_X]) == false) //si on n'est pas dans une zone exclusions
                            {
                                fSumAngl += pDataFloat[nAx_px + nAy_px * nSize_X];
                                nbAngleTaken++;
                            }
                        }
                        if (nbAngleTaken > 0)
                            Gtmes.Data.m_fOut_CenterBow = (fSumAngl / (float)nbAngleTaken) - Gtmes.Data.m_fOut_BowBF;
                    }
                    else
                    {
                        Gtmes.Data.m_fOut_CenterBow = 0.0f;
                    }

                    Gtmes.Data.m_Out3DData = pDataFloat;
                    Gtmes.Data.m_Out3DData_SizeX = nSize_X;
                    Gtmes.Data.m_Out3DData_SizeY = nSize_Y;

                    // -------------------------- 
                    // Send Measure to childrens
                    // ---------------------------
                    ProcessChildren(Gtmes);

                }

            }
            catch
            {
                throw;
            }
            finally
            {

                if (MilStatContextId != MIL.M_NULL)
                {
                    MIL.MimFree(MilStatContextId);
                    MilStatContextId = MIL.M_NULL;
                }

                if (MilStatResult != MIL.M_NULL)
                {
                    MIL.MimFree(MilStatResult);
                    MilStatResult = MIL.M_NULL;
                }

                if (MilGraphContext != MIL.M_NULL)
                {
                    MIL.MgraFree(MilGraphContext);
                    MilGraphContext = MIL.M_NULL;
                }
            }
        }
    }
}

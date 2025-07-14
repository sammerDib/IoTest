using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace LibProcessing
{
    public partial class ProcessingClassMil : ProcessingClass
    {
        //=================================================================
        public override void RemoveLittleBlob(ProcessingImage processImage, double NoisePercent)
        {
            MilImage milImage = processImage.GetMilImage();

            using (MilImage binImage = new MilImage())
            using (MilBlobFeatureList blobFeatureList = new MilBlobFeatureList())
            using (MilBlobResult blobResult = new MilBlobResult())
            {
                //--------------------------------------------------------------
                // Binarisation de l'image
                //--------------------------------------------------------------
                binImage.Alloc2d(milImage.SizeX, milImage.SizeY, 1 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
                MilImage.Binarize(milImage, binImage, MIL.M_GREATER_OR_EQUAL, 1, MIL.M_NULL);

                //--------------------------------------------------------------
                // Calcul des blobs
                //--------------------------------------------------------------
                // Enable area feature calculation.
                blobFeatureList.Alloc();
                blobFeatureList.SelectFeature(MIL.M_AREA);

                // Calculate selected bin features for each blob.
                blobResult.Alloc();
                blobResult.Calculate(binImage, milImage, blobFeatureList);

                //--------------------------------------------------------------
                // Filtrage des blobs
                //--------------------------------------------------------------
                MIL_INT nbBlobs = blobResult.Number;
                if (nbBlobs == 0)
                    return;

                // Get blob areas
                double[] lfDataBlob_Area = blobResult.GetResult(MIL.M_AREA);

                // Max area
                double lfMaxArea = lfDataBlob_Area.Max();
                double lfRemoveArea = lfMaxArea * NoisePercent / 100.0;

                // Remove small blobs
                blobResult.Select(MIL.M_DELETE, MIL.M_AREA, MIL.M_LESS, lfRemoveArea, MIL.M_NULL);
                blobResult.Control(MIL.M_SAVE_RUNS, MIL.M_ENABLE);

                //--------------------------------------------------------------
                // Copie dans l'image d'origine
                //--------------------------------------------------------------
                milImage.Clear();
                blobResult.Fill(milImage, MIL.M_INCLUDED_BLOBS, 255);
            }
        }

        public void MilBlobFiltering(ProcessingImage ProcessImage, List<BlobFilteringParameters> characteriticList, enTypeOfCondition typeOfCondition)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            using (MilImage milBinImage = new MilImage())                              // Binary image buffer identifier.
            using (MilBlobFeatureList milblobFeatureList = new MilBlobFeatureList())   // Feature list identifier. 
            using (MilBlobResult milBlobResult = new MilBlobResult())              // Blob result buffer identifier.
            {
                MIL_INT TotalBlobs = 0;                                 // Total number of blobs.
                MIL_INT BlobsWithHoles = 0;                             // Number of blobs with holes.
                MIL_INT BlobsWithRoughHoles = 0;                        // Number of blobs with rough holes.
                //--------------------------------------------------------------
                // Binarisation de l'image
                //--------------------------------------------------------------
                // Allocate a binary image buffer for fast processing.
                milBinImage.Alloc2d(milImage.SizeX, milImage.SizeY, 1 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
                // Binarize image.
                MilImage.Binarize(milImage, milBinImage, MIL.M_GREATER_OR_EQUAL, 1, MIL.M_NULL);
                // Allocate a feature list.
                milblobFeatureList.Alloc();
                // Enable all feature calculation.
                milblobFeatureList.SelectFeature(MIL.M_ALL_FEATURES);
                // Allocate a blob bin result buffer.
                milBlobResult.Alloc();
                // Calculate selected grey features for each blob.
                milBlobResult.Calculate(milBinImage, milImage, milblobFeatureList);
                // Exclude blobs whose area is too small.
                TotalBlobs = milBlobResult.Number;

                // Read all feature and set it in blob.
                double[] lfDataBlob_Breadth = milBlobResult.GetResult(MIL.M_BREADTH);
                double[] lfDataBlob_Compactness = milBlobResult.GetResult(MIL.M_COMPACTNESS);
                double[] lfDataBlob_Feret_Elongation = milBlobResult.GetResult(MIL.M_FERET_ELONGATION);
                double[] lfDataBlob_Perimeter = milBlobResult.GetResult(MIL.M_PERIMETER);
                double[] lfDataBlob_Lenght = milBlobResult.GetResult(MIL.M_LENGTH);
                double[] lfDataBlob_Area = milBlobResult.GetResult(MIL.M_AREA);
                double[] lfDataBlob_Angle = milBlobResult.GetResult(MIL.M_AXIS_PRINCIPAL_ANGLE);
                double[] lfDataBlob_Roughness = milBlobResult.GetResult(MIL.M_ROUGHNESS);
                double[] lfDataBlob_CX = milBlobResult.GetResult(MIL.M_CENTER_OF_GRAVITY_X);
                double[] lfDataBlob_CY = milBlobResult.GetResult(MIL.M_CENTER_OF_GRAVITY_Y);

                MIL_INT condition = MIL.M_GREATER;
                switch (typeOfCondition)
                {
                    case enTypeOfCondition.en_Equal: condition = MIL.M_EQUAL; break;
                    case enTypeOfCondition.en_Greater: condition = MIL.M_GREATER; break;
                    case enTypeOfCondition.en_GreaterOrEqual: condition = MIL.M_GREATER_OR_EQUAL; break;
                    case enTypeOfCondition.en_less: condition = MIL.M_LESS; break;
                    case enTypeOfCondition.en_LessOrEqual: condition = MIL.M_LESS_OR_EQUAL; break;
                    case enTypeOfCondition.en_NotEqual: condition = MIL.M_NOT_EQUAL; break;
                }

                foreach (BlobFilteringParameters characParameters in characteriticList)
                {
                    MIL_INT operation;
                    double lfValue = 0.0;
                    bool bUsed = false;

                    lfValue = characParameters.getValue;
                    bUsed = characParameters.getUsed;
                    operation = characParameters.getOperation;

                    // Select blob with good caracteristic
                    if (bUsed)
                        milBlobResult.Select(MIL.M_DELETE, operation, condition, lfValue, MIL.M_NULL);
                }

                milBlobResult.Control(MIL.M_SAVE_RUNS, MIL.M_ENABLE);
                milImage.Clear();
                milBlobResult.Fill(milImage, MIL.M_INCLUDED_BLOBS, 255);
            }
        }

        //======================================================================================
        public override void MilBlobFilling(ProcessingImage ProcessImage)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            MIL.MblobReconstruct(milImage, MIL.M_NULL, milImage, MIL.M_FILL_HOLES, MIL.M_BINARY);
        }

        //======================================================================================
        public override void MilBlobExtractHoles(ProcessingImage ProcessImage, enKindOfPicture kindOfPicture)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            switch (kindOfPicture)
            {
                case enKindOfPicture.Binary:
                    MIL.MblobReconstruct(milImage, MIL.M_NULL, milImage, MIL.M_EXTRACT_HOLES, MIL.M_BINARY);
                    break;
                case enKindOfPicture.GreyLevel:
                    MIL.MblobReconstruct(milImage, MIL.M_NULL, milImage, MIL.M_EXTRACT_HOLES, MIL.M_GRAYSCALE);
                    break;
            }
        }

        //======================================================================================
        public override void MilBorderBlobRemove(ProcessingImage ProcessImage, enKindOfPicture kindOfPicture)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            switch (kindOfPicture)
            {
                case enKindOfPicture.Binary:
                    MIL.MblobReconstruct(milImage, MIL.M_NULL, milImage, MIL.M_ERASE_BORDER_BLOBS, MIL.M_BINARY);
                    break;
                case enKindOfPicture.GreyLevel:
                    MIL.MblobReconstruct(milImage, MIL.M_NULL, milImage, MIL.M_ERASE_BORDER_BLOBS, MIL.M_GRAYSCALE);
                    break;
            }
        }
    }
}

#include "CppUnitTest.h"
#include "olovia.h"


#include <filesystem>
#include <iostream>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace AlgosLiseHFNativeTests
{
	TEST_CLASS(AlgosLiseHFNativeTests)
	{
	public:

        std::string TEST_DATA_PATH = "\\..\\..\\Data\\";
		
		TEST_METHOD(SimpleLayer51um_LiseHFRun)
		{
            double* pWavelength_nm = NULL, * pDark_spectrum = NULL, * pReference_spectrum = NULL, * pSpectrum = NULL;


            double* pOptical_thicknesses = NULL, * pThickness_Tolerance = NULL, * pRef_index_re = NULL, * pRef_index_im = NULL; //refractive index at LISE-HF center wavelength (0.56 µm) 
            bool bFT_done, bSignal_analysis_done;
            double dAsymptStdErr, dResidual; //estimated TSV and layer thicknesses, asymptotic standard error for it, sum of remaining squares
            double dSignalQuality; //quality of the signal: amplitude of highest peak in window / dResidual of least square fit; the larger the better; 
            double dMaxPeakInWindow = 0.0; // max peak FTT amplitude in search window

            //1. input parameters  
            double dTSV_diam = 5.0;
            int iNo_layers = 1;
            int iNo_pixels = 4094; //number of spectrometer pixels 
            double dThreshold_valid_signal = 0.01;  //results are invalid when the highest peak of the FT to be analyzed is smaller than dThreshold_valid_signal * peak height at z = 0
            double dThreshold_peak = 0.95; //a local maximum of the FT is considered as meaningful peak, when it has at least 
            //the amplitude dThreshold_peak * amplitude of highest peak in considered window (z-region)
            int iDebugFileNo = 1; //The Debug files should be numbered sequentially; the debug file name contains the current number 
            double dz_Resolution = 0.0; //desired remaining z-resolution after smoothening of FT-data 
            int iOperational_Mode = GRID_SEARCH_MODE;  //FFT_MODE, LISE_ED_MODE or GRID_SEARCH_MODE as mode of signal processing    
            int iFT_dim;
            double* pModulus_of_FT = NULL, * pz = NULL;
            int i, iNo_detected_peaks; //number of analyzed peaks is equal to number of material interfaces at the top of the TSV; no layer <=> 1 interface (Si-air)
            double* pComputed_optical_thicknesses = NULL, * pAmplitudes_of_peaks = NULL, * pComputed_geometrical_thicknesses = NULL;
           
            const int iNo_char = 512;  char szAdditional_message[iNo_char] = "";
            stringstream ssMessage;


            //3. prepare input: 
            pOptical_thicknesses = new double[iNo_layers], pThickness_Tolerance = new  double[iNo_layers];
            pRef_index_re = new double[iNo_layers], pRef_index_im = new double[iNo_layers];

            pOptical_thicknesses[0] = 51.5;    pThickness_Tolerance[0] = 2.0;
            pRef_index_re[0] = 1.0;   pRef_index_im[0] = 0.0; //refractive index of sillicon


            std::filesystem::path cwd = std::filesystem::current_path();

            char szFileInputSummarized[128] = "50x obj 4ms 50av TSV centered_2010165U1.TXT";
            std::string FileInputSummarized = cwd.string().c_str() + TEST_DATA_PATH + "50x obj 4ms 50av TSV centered_2010165U1.TXT";


            pWavelength_nm = new double[iNo_pixels], pDark_spectrum = new double[iNo_pixels], pReference_spectrum = new double[iNo_pixels], pSpectrum = new double[iNo_pixels];
            pComputed_optical_thicknesses = new double[iNo_layers], pAmplitudes_of_peaks = new double[iNo_layers], pComputed_geometrical_thicknesses = new double[iNo_layers];

            Assert::IsTrue(bReadAvantesFile(FileInputSummarized.c_str(), iNo_pixels, pWavelength_nm, pDark_spectrum, pReference_spectrum, pSpectrum), L"Spectrums summary File could not be correctly read");

            //5. Analyze LISE-HF data:  
            double dTotalThickness = 0.0;
            for (i = 0; i < iNo_layers; i++)
                dTotalThickness += pOptical_thicknesses[i] + pThickness_Tolerance[i];
            if (dTotalThickness < 25.0)
                iFT_dim = 2048;
            else if (dTotalThickness < 50.0)
                iFT_dim = 4096;
            else
                iFT_dim = 8192;


            pModulus_of_FT = new double[iFT_dim];
            pz = new double[iFT_dim];

            bool bPeakDetectionOnRight = false;
            bool bNewPeakDetection = false;
            vLISE_HF_main(dTSV_diam, iNo_layers, pOptical_thicknesses, pThickness_Tolerance, pRef_index_re, pRef_index_im,
                iNo_pixels, pWavelength_nm, pDark_spectrum, pReference_spectrum, pSpectrum, NULL, iFT_dim, iOperational_Mode,
                bPeakDetectionOnRight, bNewPeakDetection,
                dThreshold_valid_signal, dThreshold_peak, dz_Resolution, false, iDebugFileNo++,
                dAsymptStdErr, dResidual, dSignalQuality, 
                &ssMessage, //szAdditional_message, iNo_char,
                pModulus_of_FT, pz, iNo_detected_peaks, pComputed_optical_thicknesses, pAmplitudes_of_peaks, pComputed_geometrical_thicknesses, bFT_done, bSignal_analysis_done, dMaxPeakInWindow);

            Assert::IsTrue(bFT_done, L"FFT has not been calculated");
            Assert::IsTrue(bSignal_analysis_done, L"Signal FT has not been analyzed");
            
            // for 50x obj 4ms 50av TSV centered_2010165U1
            Assert::AreEqual(1, iNo_detected_peaks, L"wrong peak detection");
            Assert::AreEqual(45.947, pAmplitudes_of_peaks[0], 0.001, L"incorrect peak amplitude");
            Assert::AreEqual(51.5204, pComputed_optical_thicknesses[0], 0.001, L"incorrect optical depth ");
            Assert::AreEqual(51.3421, pComputed_geometrical_thicknesses[0], 0.001, L"incorrect corrected geometrical depth ");
            Assert::AreEqual(41.2358, dMaxPeakInWindow, 0.001, L"incorrect Max peak amplitude in window");


            delete[] pOptical_thicknesses, delete[] pThickness_Tolerance, delete[] pRef_index_re, delete[] pRef_index_im;
            delete[] pWavelength_nm, delete[] pDark_spectrum, delete[] pReference_spectrum, delete[] pSpectrum;
            delete[] pModulus_of_FT, delete[] pz;
            delete[] pComputed_optical_thicknesses, delete[] pAmplitudes_of_peaks, delete[] pComputed_geometrical_thicknesses;
		}


        TEST_METHOD(SimpleLayer30um_LiseHFRun)
        {
            double* pWavelength_nm = NULL, * pDark_spectrum = NULL, * pReference_spectrum = NULL, * pSpectrum = NULL;


            double* pOptical_thicknesses = NULL, * pThickness_Tolerance = NULL, * pRef_index_re = NULL, * pRef_index_im = NULL; //refractive index at LISE-HF center wavelength (0.56 µm) 
            bool bFT_done, bSignal_analysis_done;
            double dAsymptStdErr, dResidual; //estimated TSV and layer thicknesses, asymptotic standard error for it, sum of remaining squares
            double dSignalQuality; //quality of the signal: amplitude of highest peak in window / dResidual of least square fit; the larger the better; 
            double dMaxPeakInWindow = 0.0; // max peak FTT amplitude in search window

            //1. input parameters  
            double dTSV_diam = 5.0;
            int iNo_layers = 1;
            int iNo_pixels = 4094; //number of spectrometer pixels 
            double dThreshold_valid_signal = 0.01;  //results are invalid when the highest peak of the FT to be analyzed is smaller than dThreshold_valid_signal * peak height at z = 0
            double dThreshold_peak = 0.95; //a local maximum of the FT is considered as meaningful peak, when it has at least 
            //the amplitude dThreshold_peak * amplitude of highest peak in considered window (z-region)
            int iDebugFileNo = 1; //The Debug files should be numbered sequentially; the debug file name contains the current number 
            double dz_Resolution = 0.0; //desired remaining z-resolution after smoothening of FT-data 
            int iOperational_Mode = GRID_SEARCH_MODE;  //FFT_MODE, LISE_ED_MODE or GRID_SEARCH_MODE as mode of signal processing    
            int iFT_dim;
            double* pModulus_of_FT = NULL, * pz = NULL;
            int i, iNo_detected_peaks; //number of analyzed peaks is equal to number of material interfaces at the top of the TSV; no layer <=> 1 interface (Si-air)
            double* pComputed_optical_thicknesses = NULL, * pAmplitudes_of_peaks = NULL, * pComputed_geometrical_thicknesses = NULL;

            const int iNo_char = 512;  char szAdditional_message[iNo_char] = "";
            stringstream ssMessage;

            //3. prepare input: 
            pOptical_thicknesses = new double[iNo_layers], pThickness_Tolerance = new  double[iNo_layers];
            pRef_index_re = new double[iNo_layers], pRef_index_im = new double[iNo_layers];

            pOptical_thicknesses[0] = 30.0;    pThickness_Tolerance[0] = 2.0;
            pRef_index_re[0] = 1.0;   pRef_index_im[0] = 0.0; //refractive index of sillicon

            std::filesystem::path cwd = std::filesystem::current_path();

            char szFileInputSummarized[128] = "Wafer center TSV center2 18-2-23 8ms 300av.TXT";
            std::string FileInputSummarized = cwd.string().c_str() + TEST_DATA_PATH + "Wafer center TSV center2 18-2-23 8ms 300av.TXT";


            pWavelength_nm = new double[iNo_pixels], pDark_spectrum = new double[iNo_pixels], pReference_spectrum = new double[iNo_pixels], pSpectrum = new double[iNo_pixels];
            pComputed_optical_thicknesses = new double[iNo_layers], pAmplitudes_of_peaks = new double[iNo_layers], pComputed_geometrical_thicknesses = new double[iNo_layers];

            Assert::IsTrue(bReadAvantesFile(FileInputSummarized.c_str(), iNo_pixels, pWavelength_nm, pDark_spectrum, pReference_spectrum, pSpectrum), L"Spectrums summary File could not be correctly read");

            //5. Analyze LISE-HF data:  
            double dTotalThickness = 0.0;
            for (i = 0; i < iNo_layers; i++)
                dTotalThickness += pOptical_thicknesses[i] + pThickness_Tolerance[i];
            if (dTotalThickness < 25.0)
                iFT_dim = 2048;
            else if (dTotalThickness < 50.0)
                iFT_dim = 4096;
            else
                iFT_dim = 8192;


            pModulus_of_FT = new double[iFT_dim];
            pz = new double[iFT_dim];

     /*       vLISE_HF_main(false, dTSV_diam, iNo_layers, pReal_thicknesses, pThickness_Tolerance, pRef_index_re, pRef_index_im,
                iNo_pixels, pWavelength_nm, pDark_spectrum, pReference_spectrum, pSpectrum, NULL, iFT_dim,
                iOperational_Mode, dThreshold_valid_signal, dThreshold_peak, dz_Resolution, false, iDebugFileNo++,
                dAsymptStdErr, dResidual, dSignalQuality, szAdditional_message, iNo_char, pModulus_of_FT, pz,
                iNo_detected_peaks, pComputed_optical_thicknesses, pAmplitudes_of_peaks, pComputed_geometrical_thicknesses, bFT_done, bSignal_analysis_done, dMaxPeakInWindow);*/

            bool bPeakDetectionOnRight = false;
            bool bNewPeakDetection = false;
            vLISE_HF_main(dTSV_diam, iNo_layers, pOptical_thicknesses, pThickness_Tolerance, pRef_index_re, pRef_index_im,
                iNo_pixels, pWavelength_nm, pDark_spectrum, pReference_spectrum, pSpectrum, NULL, iFT_dim, iOperational_Mode,
                bPeakDetectionOnRight, bNewPeakDetection,
                dThreshold_valid_signal, dThreshold_peak, dz_Resolution, false, iDebugFileNo++,
                dAsymptStdErr, dResidual, dSignalQuality,
                &ssMessage, //szAdditional_message, iNo_char,
                pModulus_of_FT, pz, iNo_detected_peaks, pComputed_optical_thicknesses, pAmplitudes_of_peaks, pComputed_geometrical_thicknesses, bFT_done, bSignal_analysis_done, dMaxPeakInWindow);


            Assert::IsTrue(bFT_done, L"FFT has not been calculated");
            Assert::IsTrue(bSignal_analysis_done, L"Signal FT has not been analyzed");

            // Wafer center TSV center2 18-2-23 8ms 300av.TXT
            Assert::AreEqual(1, iNo_detected_peaks, L"wrong peak detection");
            Assert::AreEqual(103.008, pAmplitudes_of_peaks[0], 0.001, L"incorrect peak amplitude");
            Assert::AreEqual(29.815468, pComputed_optical_thicknesses[0], 0.001, L"incorrect optical depth ");
            Assert::AreEqual(29.712324, pComputed_geometrical_thicknesses[0], 0.001, L"incorrect corrected geometrical depth ");
            Assert::AreEqual(94.8846, dMaxPeakInWindow, 0.001, L"incorrect Max peak amplitude in window");

            delete[] pOptical_thicknesses, delete[] pThickness_Tolerance, delete[] pRef_index_re, delete[] pRef_index_im;
            delete[] pWavelength_nm, delete[] pDark_spectrum, delete[] pReference_spectrum, delete[] pSpectrum;
            delete[] pModulus_of_FT, delete[] pz;
            delete[] pComputed_optical_thicknesses, delete[] pAmplitudes_of_peaks, delete[] pComputed_geometrical_thicknesses;
        }
        
    
        TEST_METHOD(SimpleLayer30um_LiseHFRun_BadThreshold)
        {
            double* pWavelength_nm = NULL, * pDark_spectrum = NULL, * pReference_spectrum = NULL, * pSpectrum = NULL;


            double* pOptical_thicknesses = NULL, * pThickness_Tolerance = NULL, * pRef_index_re = NULL, * pRef_index_im = NULL; //refractive index at LISE-HF center wavelength (0.56 µm) 
            bool bFT_done, bSignal_analysis_done;
            double dAsymptStdErr, dResidual; //estimated TSV and layer thicknesses, asymptotic standard error for it, sum of remaining squares
            double dSignalQuality; //quality of the signal: amplitude of highest peak in window / dResidual of least square fit; the larger the better; 
            double dMaxPeakInWindow = 0.0; // max peak FTT amplitude in search window

            //1. input parameters  
            double dTSV_diam = 5.0;
            int iNo_layers = 1;
            int iNo_pixels = 4094; //number of spectrometer pixels 
            double dBADThreshold_valid_signal = 0.03;  //results are invalid when the highest peak of the FT to be analyzed is smaller than dThreshold_valid_signal * peak height at z = 0
            double dThreshold_peak = 0.95; //a local maximum of the FT is considered as meaningful peak, when it has at least 
            //the amplitude dThreshold_peak * amplitude of highest peak in considered window (z-region)
            int iDebugFileNo = 1; //The Debug files should be numbered sequentially; the debug file name contains the current number 
            double dz_Resolution = 0.0; //desired remaining z-resolution after smoothening of FT-data 
            int iOperational_Mode = GRID_SEARCH_MODE;  //FFT_MODE, LISE_ED_MODE or GRID_SEARCH_MODE as mode of signal processing    
            int iFT_dim;
            double* pModulus_of_FT = NULL, * pz = NULL;
            int i, iNo_detected_peaks; //number of analyzed peaks is equal to number of material interfaces at the top of the TSV; no layer <=> 1 interface (Si-air)
            double* pComputed_optical_thicknesses = NULL, * pAmplitudes_of_peaks = NULL, * pComputed_geometrical_thicknesses = NULL;

            const int iNo_char = 512;  char szAdditional_message[iNo_char] = "";
            stringstream ssMessage;

            //3. prepare input: 
            pOptical_thicknesses = new double[iNo_layers], pThickness_Tolerance = new  double[iNo_layers];
            pRef_index_re = new double[iNo_layers], pRef_index_im = new double[iNo_layers];
  
            pOptical_thicknesses[0] = 30.0;    pThickness_Tolerance[0] = 2.0;
            pRef_index_re[0] = 1.0;   pRef_index_im[0] = 0.0; //refractive index of sillicon

            std::filesystem::path cwd = std::filesystem::current_path();

            char szFileInputSummarized[128] = "Wafer center TSV center2 18-2-23 8ms 300av.TXT";
            std::string FileInputSummarized = cwd.string().c_str() + TEST_DATA_PATH + "Wafer center TSV center2 18-2-23 8ms 300av.TXT";


            pWavelength_nm = new double[iNo_pixels], pDark_spectrum = new double[iNo_pixels], pReference_spectrum = new double[iNo_pixels], pSpectrum = new double[iNo_pixels];
            pComputed_optical_thicknesses = new double[iNo_layers], pAmplitudes_of_peaks = new double[iNo_layers], pComputed_geometrical_thicknesses = new double[iNo_layers];

            Assert::IsTrue(bReadAvantesFile(FileInputSummarized.c_str(), iNo_pixels, pWavelength_nm, pDark_spectrum, pReference_spectrum, pSpectrum), L"Spectrums summary File could not be correctly read");

            //5. Analyze LISE-HF data:  
            double dTotalThickness = 0.0;
            for (i = 0; i < iNo_layers; i++)
                dTotalThickness += pOptical_thicknesses[i] + pThickness_Tolerance[i];
            if (dTotalThickness < 25.0)
                iFT_dim = 2048;
            else if (dTotalThickness < 50.0)
                iFT_dim = 4096;
            else
                iFT_dim = 8192;


            pModulus_of_FT = new double[iFT_dim];
            pz = new double[iFT_dim];


           /* vLISE_HF_main(false, dTSV_diam, iNo_layers, pOptical_thicknesses, pThickness_Tolerance, pRef_index_re, pRef_index_im,
                iNo_pixels, pWavelength_nm, pDark_spectrum, pReference_spectrum, pSpectrum, NULL, iFT_dim,
                iOperational_Mode, dBADThreshold_valid_signal, dThreshold_peak, dz_Resolution, false, iDebugFileNo++,
                dAsymptStdErr, dResidual, dSignalQuality, szAdditional_message, iNo_char, pModulus_of_FT, pz,
                iNo_detected_peaks, pComputed_optical_thicknesses, pAmplitudes_of_peaks, pComputed_geometrical_thicknesses, bFT_done, bSignal_analysis_done, dMaxPeakInWindow);*/
          
            bool bPeakDetectionOnRight = false;
            bool bNewPeakDetection = false;
            vLISE_HF_main(dTSV_diam, iNo_layers, pOptical_thicknesses, pThickness_Tolerance, pRef_index_re, pRef_index_im,
                iNo_pixels, pWavelength_nm, pDark_spectrum, pReference_spectrum, pSpectrum, NULL, iFT_dim, iOperational_Mode,
                bPeakDetectionOnRight, bNewPeakDetection,
                dBADThreshold_valid_signal, dThreshold_peak, dz_Resolution, false, iDebugFileNo++,
                dAsymptStdErr, dResidual, dSignalQuality,
                &ssMessage, //szAdditional_message, iNo_char,
                pModulus_of_FT, pz, iNo_detected_peaks, pComputed_optical_thicknesses, pAmplitudes_of_peaks, pComputed_geometrical_thicknesses, bFT_done, bSignal_analysis_done, dMaxPeakInWindow);


            Assert::IsTrue(bFT_done, L"FFT has not been calculated");
            Assert::IsFalse(bSignal_analysis_done, L"Signal FT has been analyzed with a tool low signal");

            Assert::AreEqual(0.0, dSignalQuality, 0.0001, L"wrong signal quality");
            stringstream ssExpectedMessage("Too small signal: FT[0]: 4916.89, signal threshold: 0.03, highest peak: 94.8846\n");
            Assert::AreEqual(ssExpectedMessage.str().c_str(), ssMessage.str().c_str(), L"Bad message returned");

            delete[] pOptical_thicknesses, delete[] pThickness_Tolerance, delete[] pRef_index_re, delete[] pRef_index_im;
            delete[] pWavelength_nm, delete[] pDark_spectrum, delete[] pReference_spectrum, delete[] pSpectrum;
            delete[] pModulus_of_FT, delete[] pz;
            delete[] pComputed_optical_thicknesses, delete[] pAmplitudes_of_peaks, delete[] pComputed_geometrical_thicknesses;
        }
	};
}

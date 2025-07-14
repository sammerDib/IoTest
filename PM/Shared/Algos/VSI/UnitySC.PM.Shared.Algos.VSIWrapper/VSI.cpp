#include "VSI.h"
#include "VSI_StackImages_IF.h"

using namespace System::Runtime::InteropServices;

namespace UnitySCPMSharedAlgosVSIWrapper {

	VSIOutput^ VSI::ComputeTopography(array<array<System::Byte>^>^ images, int width, int height, double ruleStep, double lambdaCenter, double fwhmLambda, double noiseLevel, double maskThreshold)
	{
        VSIOutput^ result = gcnew VSIOutput();
        
        VSI_StackImages_Type imageStack;
		imageStack.NumberOfImages = images->Length;
		imageStack.Width = width;
		imageStack.Height = height;

		unsigned char** imageStackArray = new unsigned char* [images->Length];

		for (int i = 0; i < images->Length; i++)
		{
			pin_ptr<System::Byte> imgPinPtr = &images[i][0];
			unsigned char* ptrUChar = imgPinPtr;
			imageStackArray[i] = ptrUChar;
		}

		imageStack.ImageArray = imageStackArray;

		VSI_StackImages_Setting_Type settings;
		
		settings.RuleStep = ruleStep;
		settings.LambdaCenter = lambdaCenter;// wavelength [m]
		settings.FWHMLambda = fwhmLambda;    // spectral bandwidth [m]
		settings.NoiseLevel = noiseLevel;    // noise [LSB]
		
		VSI_StackImages_opt_Type options;
		
		ErrorFlagType optionsFlag = VSI_DefaultOptions(&options);

		options.MaskThreshold = maskThreshold;

		VSI_Output_Type output;
        // Bruno Version
        //ErrorFlagType flag2= VSI_StackImages(&imageStack, &settings, &options, &output);
        // 
        // Memory Optimized RTi Version (Work in Progress)
        ErrorFlagType flag = VSI_RTI_StackImages(&imageStack, &settings, &options, &output);

		delete[] imageStackArray;
		
        if (flag == VSI_OK_FLAG)
        {
            result->Status = StatusCode::OK;
            
            SINGLE_RM_Image* topo = output.TopoFlatten_Image;

            int arraySize = topo->nx * topo->ny;

            array<float>^ topoArray = gcnew array<float>(arraySize);
            Marshal::Copy((IntPtr)topo->Data, topoArray, 0, arraySize);
            result->ResultArray = topoArray;
        }
        else
        {
            result->Status = (StatusCode)flag;
        }

		return result;

	}

}
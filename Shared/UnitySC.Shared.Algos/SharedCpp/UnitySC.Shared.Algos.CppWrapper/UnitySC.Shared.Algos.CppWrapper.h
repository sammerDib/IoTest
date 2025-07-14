#pragma once

#pragma managed
using namespace System;
namespace UnitySCSharedAlgosCppWrapper {
	public ref class Utils 
	{
        public:
            double static testopenmp(const int nbpoint, bool bUseopenMP);
            void static DisableOpenMP();
	};
}

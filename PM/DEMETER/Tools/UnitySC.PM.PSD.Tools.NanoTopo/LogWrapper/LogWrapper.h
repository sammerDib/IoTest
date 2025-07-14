using namespace System;
using namespace LogWorker;

namespace Wrapper
{
	public ref class LogWrapper
	{
	public :
		LogWrapper(void)
		{
			m_oLogWorkerObj = gcnew LogObsWorker();
		}

	private :
		LogObsWorker ^ m_oLogWorkerObj;

	public:	
		static LogWrapper ^ Instance = gcnew LogWrapper();

		LogObsWorker ^ GetLogObsWorker()
		{
			return m_oLogWorkerObj;
		}
	
	
	};
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Common
{
    public class CCommandBase<TFunctionCode>
    {
        private List<Object> m_ParamsList;        
        private TFunctionCode m_fct;

        public CCommandBase()
        { }
        public CCommandBase(TFunctionCode fct, Object pParam1, Object pParam2, Object pParam3, Object pParam4, Object pParam5, Object pParam6)
        {
            m_fct = fct;
            m_ParamsList = new List<object>();
            if(pParam1!=null)
                m_ParamsList.Add(pParam1);
            if (pParam2 != null)
                m_ParamsList.Add(pParam2);            
            if (pParam3 != null)
                m_ParamsList.Add(pParam3);
            if (pParam4 != null)
                m_ParamsList.Add(pParam4);
            if (pParam5 != null)
                m_ParamsList.Add(pParam5);
            if (pParam6 != null)
                m_ParamsList.Add(pParam6); 
        }                

        public TFunctionCode Fct
        {
          get { return m_fct; }
            set { m_fct = value; }
        }
        public List<Object> ParamsList
        {
            get { return m_ParamsList; }
            set { m_ParamsList = value; }
        }
    }

    public class ExceptionExit : Exception
    {
        public ExceptionExit()
        { }
    }

    public delegate void EvtOnCallCommand<TFunctionCode>(TFunctionCode pFct);

    public class CTaskThreaded<TFunctionCode, TCommandObject> : IDisposable 
        where TCommandObject : CCommandBase<TFunctionCode>, new()
    {        
        Thread m_ThreadTask;
        bool m_bExitThread;

        public bool bCompleted = false;
        bool m_bCommamdRunning = false;

        AutoResetEvent m_EvtCommandReceived = new AutoResetEvent(false);
        List<TCommandObject> m_CommandsList = new List<TCommandObject>();
        Object m_SynchroCommandList = new Object();
        EvtOnCallCommand<TFunctionCode> m_EvtOnCallCommand;
        

        public CTaskThreaded(String pName)
        {
            m_ThreadTask = new Thread(new ParameterizedThreadStart(TaskThread_Execute));
            m_ThreadTask.Name = pName;
            m_ThreadTask.Start(null);
        }

        public TCommandObject CreateNewCommandBase(TFunctionCode pfct, Object pParam1, Object pParam2, Object pParam3, Object pParam4, Object pParam5, Object pParam6)
        {
            TCommandObject NewCommand = new TCommandObject();            
            List<Object> lList = new List<object>();                       
            if (pParam1 != null)
                lList.Add(pParam1);
            if (pParam1 != null)
                lList.Add(pParam2);
            if (pParam1 != null)
                lList.Add(pParam3);
            if (pParam1 != null)
                lList.Add(pParam4);
            if (pParam1 != null)
                lList.Add(pParam5);
            if (pParam1 != null) 
                lList.Add(pParam6);

            NewCommand.Fct = pfct;
            NewCommand.ParamsList = lList;
            return (TCommandObject)NewCommand;
        }

        public EvtOnCallCommand<TFunctionCode> EvtOnCallCommand
        {
            get { return m_EvtOnCallCommand; }
            set { m_EvtOnCallCommand = value; }
        }

        public bool bExitThread
        {
            get { return m_bExitThread; }             
        }

        public void StopThread()
        {
            m_bExitThread = true;
            m_EvtCommandReceived.Set();

        }

        public void TaskThread_Execute(Object Data)
        {
            try
            {
                // Cast entry parameter in CCommandObject            
                TCommandObject CurrentCommand = null;
                //Init Thread
                m_bExitThread = false;
                int iNbCommand = 1;
                // Infinity loop
                while (!m_bExitThread)
                {
                    // Waiting Event received function call
                    m_EvtCommandReceived.WaitOne(-1, true);
                    if (m_bExitThread)
                        break;

                    iNbCommand = 1; // Pour rentrer dans la boucle, remise a jour avec bonne valeur apres etre entre
                    while (iNbCommand > 0)
                    {
                        // Protected Commands Object data
                        lock (m_SynchroCommandList)
                        {
                            iNbCommand = m_CommandsList.Count;
                            if (iNbCommand > 0)
                            {
                                CurrentCommand = m_CommandsList[0];
                            }
                            else
                                break;
                        }

                        // Call function according to command received 
                        try
                        {
                            m_bCommamdRunning = true;
                            EvtOnCallCommand(CurrentCommand.Fct);
                        }
                        catch (ExceptionExit Ex) { String Err = Ex.Message; }
                        catch { }
                        m_bCommamdRunning = false;
                        lock (m_SynchroCommandList)
                        { m_CommandsList.RemoveAt(0); }
                    }
                }
            }
            finally
            {
                bCompleted = true;
            }
        }

        public void AddCommand(TFunctionCode pFct, Object pParam1)
        {
            AddCommand(pFct, pParam1, null, null, null, null, null);
        }
        public void AddCommand(TFunctionCode pFct, Object pParam1, Object pParam2)
        {
            AddCommand(pFct, pParam1, pParam2, null, null, null, null);
        }
        public void AddCommand(TFunctionCode pFct, Object pParam1, Object pParam2, Object pParam3)
        {
            AddCommand(pFct, pParam1, pParam2, pParam3, null, null, null);
        }
        public void AddCommand(TFunctionCode pFct, Object pParam1, Object pParam2, Object pParam3, Object pParam4)
        {
            AddCommand(pFct, pParam1, pParam2, pParam3, pParam4, null, null);
        }
        public void AddCommand(TFunctionCode pFct, Object pParam1, Object pParam2, Object pParam3, Object pParam4, Object pParam5)
        {
            AddCommand(pFct, pParam1, pParam2, pParam3, pParam4, pParam5, null);
        }
        public void AddCommand(TFunctionCode pFct, Object pParam1, Object pParam2, Object pParam3, Object pParam4, Object pParam5, Object pParam6)
        {
            TCommandObject NewCommand = CreateNewCommandBase(pFct, pParam1, pParam2, pParam3, pParam4, pParam5, pParam6);
            lock (m_SynchroCommandList)
            {
                m_CommandsList.Add(NewCommand);
            }
            // Signal l'arrivee d'une commande
            m_EvtCommandReceived.Set();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (m_ThreadTask != null)
            { 
                if (m_bCommamdRunning) m_ThreadTask.Interrupt();
                m_bExitThread = true;
                if(m_EvtCommandReceived!=null) m_EvtCommandReceived.Set();
                Thread.Sleep(100);
                m_EvtOnCallCommand = null;
            }
            m_ThreadTask = null;
    }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Common.EException;

namespace Common
{
    public class CNotifyObject<T> where T : class
    {
        //Membres 
        #region Protected Member Variables
        private System.Object m_CallbackSynchronizationObject = new System.Object();

        protected LinkedList<T> m_CallbackList = new LinkedList<T>();
        #endregion

        //Properties Callback
        public LinkedList<T> CallbackList
        {
            get { return m_CallbackList; }
        }

        public Object CallbackSynchronizationObject
        {
            get { return m_CallbackSynchronizationObject; }
            set { m_CallbackSynchronizationObject = value; }
        }

        protected T AddCallback
        {
            set
            {
                lock (m_CallbackSynchronizationObject)
                {
                    m_CallbackList.AddLast(value);
                }
            }
        }
        protected T RemoveCallback
        {
            set
            {
                lock (m_CallbackSynchronizationObject)
                {
                    m_CallbackList.Remove(value);
                }
            }
        }

        public void UnregisterCallback(T pCB)
        {
            if (pCB == null)
                throw new CxCOMInvalidArgumentException();
           
            if (!CallbackList.Contains(pCB as T))
                throw new CxCOMAccessDeniedException();

            RemoveCallback = pCB as T;
        }

        public void RegisterCallback(T pCB)
        {
            if (pCB == null)
                throw new CxCOMInvalidArgumentException();

            if (CallbackList.Contains(pCB as T))
                throw new CxCOMAccessDeniedException();

            AddCallback = pCB as T;
        }
        
    }
}

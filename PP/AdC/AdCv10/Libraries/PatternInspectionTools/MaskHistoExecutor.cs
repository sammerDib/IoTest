using System;
using System.Collections.Generic;

using AdcTools.Collection;

using Matrox.MatroxImagingLibrary;

namespace PatternInspectionTools
{
    public class MaskHistoExecutor : ITakeable, IDisposable, ICloneable
    {
        private bool m_bIsCloned = false;
        private MIL_ID m_MilSys = MIL.M_NULL;
        private int _nMAXREGION = 255;

        private MIL_ID _RunMilHisto = MIL.M_NULL;
        private MIL_INT[] _Histo = null;

        public MaskHistoExecutor()
        {
            m_bIsCloned = false;
            m_MilSys = MIL.M_NULL;
        }

        public MaskHistoExecutor(MIL_ID p_MilSys, int nbMaskRegion)
        {
            m_bIsCloned = false;
            m_MilSys = p_MilSys;
            _nMAXREGION = nbMaskRegion;
            InitRunData();
        }

        private void InitRunData()
        {
            MIL.MimAllocResult(m_MilSys, _nMAXREGION + 1, MIL.M_HIST_LIST, ref _RunMilHisto);
            _Histo = new MIL_INT[_nMAXREGION + 1];
        }

        public List<int> FindROI_Ids(MIL_ID p_BinaryMilId, MIL_ID p_MaskROIMilId)
        {
            List<int> Ids = new List<int>();

            int SizeX = (int)MIL.MbufInquire(p_BinaryMilId, MIL.M_SIZE_X);
            int SizeY = (int)MIL.MbufInquire(p_BinaryMilId, MIL.M_SIZE_Y);
            int Type = (int)MIL.MbufInquire(p_BinaryMilId, MIL.M_TYPE);
            int Attribute = (int)MIL.MbufInquire(p_BinaryMilId, MIL.M_ATTRIBUTE);

            MIL_ID milworkimg = MIL.M_NULL;
            MIL.MbufAlloc2d(m_MilSys, SizeX, SizeY, Type, MIL.M_IMAGE + MIL.M_PROC, ref milworkimg);
            if (milworkimg == MIL.M_NULL)
                throw new Exception(String.Format("FindROI_Ids : No more MIL memory to allocate Histogram"));

            MIL.MimArith(p_BinaryMilId, 255, milworkimg, MIL.M_DIV_CONST);
            MIL.MimArith(milworkimg, p_MaskROIMilId, milworkimg, MIL.M_MULT);

            MIL.MimHistogram(milworkimg, _RunMilHisto);
            MIL.MimGetResult(_RunMilHisto, MIL.M_VALUE, _Histo);
            for (int nRoiID = 1; nRoiID < _nMAXREGION + 1; nRoiID++)
            {
                if (_Histo[nRoiID] != 0)
                {
                    Ids.Add(nRoiID);
                }
            }

            MIL.MbufFree(milworkimg);
            milworkimg = MIL.M_NULL;

            return Ids;
        }

        #region IDisposable Members and Methods
        private bool m_hasDisposed = false;
        //Finalizer
        ~MaskHistoExecutor()
        {
            // The object went out of scope and finalized is called. Lets call dispose in to release unmanaged resources 
            // the managed resources will anyways be released when GC runs the next time.
            Dispose(false);
        }
        public void Dispose()
        {
            // If this function is being called the user wants to release the resources. lets call the Dispose which will do this for us.
            Dispose(true);

            // Now since we have done the cleanup already there is nothing left for the Finalizer to do. So lets tell the GC not to call it later.
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (m_hasDisposed == false)
            {
                if (disposing == true)
                {
                    //someone want the deterministic release of all resources Let us release all the managed resources
                    ReleaseManagedResources();
                }
                else
                {
                    // Do nothing, no one asked a dispose, the object went out of scope and finalized is called so lets next round of GC 
                    // release these resources
                }

                // Release the unmanaged resource in any case as they will not be released by GC
                ReleaseUnmanagedResources();

                m_hasDisposed = true;
            }
            else
            {
                // Object already been disposed - avoid MS exception
            }
        }

        private void ReleaseManagedResources()
        {
            _Histo = null;
        }

        private void ReleaseRunData()
        {
            //free memory
            if (_RunMilHisto != MIL.M_NULL)
            {
                MIL.MimFree(_RunMilHisto);
                _RunMilHisto = MIL.M_NULL;
            }
        }

        private void ReleaseUnmanagedResources()
        {
            ReleaseRunData();

            if (!m_bIsCloned)
            {
                // ces données là sont partagée entre les threads
            }
        }
        #endregion

        #region ICloneable Members and methods

        protected void CloneData(ref MaskHistoExecutor cloned)
        {
            cloned.InitRunData();
        }

        protected object DeepCopy()
        {
            MaskHistoExecutor cloned = MemberwiseClone() as MaskHistoExecutor;
            // ici on clone les instances nécéssaire au run (elles sont différentes et utlisé pour chaque run)
            CloneData(ref cloned);
            cloned.m_bIsCloned = true;
            if (!cloned.IsFree)
                cloned.Return();
            return cloned;
        }

        public virtual object Clone()
        {
            return DeepCopy();
        }

        #endregion
    }
}

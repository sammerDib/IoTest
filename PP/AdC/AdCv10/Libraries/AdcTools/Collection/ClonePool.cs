using System;
using System.Collections.Generic;
using System.Threading;

namespace AdcTools.Collection
{
    public class ITakeable
    {
        private bool Token { get; set; }
        public ITakeable()
        {
            Token = false;
        }
        public void Take() { Token = true; }
        public void Return() { Token = false; }
        public bool IsFree { get { return !Token; } }


        private CancellationTokenSource _cts = null;
        public void SetCancelToken(CancellationTokenSource cts) { _cts = cts; }
        public bool IsCancellationRequested { get { return _cts.IsCancellationRequested; } }
    }

    public class ClonePool<T> : IDisposable where T : ITakeable, ICloneable, IDisposable
    {
        protected object oLockFor = new object();
        protected List<T> m_oPool;
        protected SemaphoreSlim _sem = null;
        protected CancellationTokenSource _cts = null;
        protected bool bAbort = false;
        protected int _nMaxClone = 0;

        public ClonePool(T MyPoolElt, int nNbCloneMax)
        {
            _nMaxClone = nNbCloneMax;

            _sem = new SemaphoreSlim(nNbCloneMax);
            _cts = new CancellationTokenSource();

            InitPool(MyPoolElt, nNbCloneMax);
        }

        protected virtual void InitPool(T MyPoolElt, int nNbCloneMax)
        {
            m_oPool = new List<T>(nNbCloneMax);
            MyPoolElt.SetCancelToken(_cts);
            m_oPool.Add(MyPoolElt);

            for (int i = 1; i < nNbCloneMax; i++)
            {
                T clone = (T)MyPoolElt.Clone();
                clone.SetCancelToken(_cts);
                m_oPool.Add(clone);
            }

        }

        public virtual T GetFirstAvailable()
        {
            if (!bAbort)
            {
                try
                {
                    _sem.Wait(_cts.Token); // ON ATTEND ICI Si la ressource n'est pas libre
                }
                catch (OperationCanceledException)
                {
                    //c'est normal que l'on fasse rien sur cette exception ici c'est un phenomène attendu.
                }
            }

            T firstElt = null;
            lock (oLockFor)
            {
                if (!bAbort)
                {
                    foreach (T elt in m_oPool)
                    {
                        if (elt.IsFree && !bAbort)
                        {
                            firstElt = elt;
                            firstElt.Take();
                            break;
                        }
                    }
                }
            }
            return firstElt;
        }

        public void Release(T TakenElt)
        {
            TakenElt.Return();
            _sem.Release(); // On rend la main, la ressource ets de nouveau selectionnable
        }

        public void Abort()
        {
            bAbort = true;
            if (_sem != null)
            {
                _cts.Cancel();
                while (_sem.CurrentCount < _nMaxClone)
                {
                    _sem.Release(_nMaxClone - _sem.CurrentCount);
                    //Thread.Sleep(25);
                }
            }
        }

        #region IDisposable Members and Methods
        private bool m_hasDisposed = false;
        public void Dispose()
        {
            // If this function is being called the user wants to release the resources. lets call the Dispose which will do this for us.
            DisposePool();

            // Now since we have done the cleanup already there is nothing left for the Finalizer to do. So lets tell the GC not to call it later.
            GC.SuppressFinalize(this);
        }
        protected virtual void DisposePool()
        {
            if (m_hasDisposed == false)
            {
                //Console.WriteLine("Releasing Managed Resources");
                lock (oLockFor)
                {
                    for (int i = 0; i < m_oPool.Count; i++)
                    {
                        if (m_oPool[i] != null)
                        {
                            if (!m_oPool[i].IsFree)
                            {
                                Release(m_oPool[i]);
                            }
                            m_oPool[i].Dispose();
                            m_oPool[i] = null;
                        }
                    }
                    m_oPool.Clear();
                    m_oPool = null;
                }
                _sem.Dispose();
                _sem = null;

                m_hasDisposed = true;
            }
            else
            {
                // Object already been disposed - avoid MS exception
            }
        }
        #endregion
    }

    public class CloneDynamicPool<T> : ClonePool<T>
        where T : ITakeable, ICloneable, IDisposable
    {

        public CloneDynamicPool(T MyPoolElt, int nNbCloneMax)
            : base(MyPoolElt, nNbCloneMax)
        {

        }

        protected override void InitPool(T MyPoolElt, int nNbCloneMax)
        {
            m_oPool = new List<T>(nNbCloneMax);
            MyPoolElt.SetCancelToken(_cts);
            m_oPool.Add(MyPoolElt);
        }

        public override T GetFirstAvailable()
        {
            // en dynamic si on a pas d'executor dispo on en clone un, jsuqu'au max toléré
            T firstElt = base.GetFirstAvailable();
            if (firstElt == null && !bAbort)
            {
                lock (oLockFor)
                {
                    T clone = (T)m_oPool[0].Clone();
                    clone.SetCancelToken(_cts);
                    m_oPool.Add(clone);

                    firstElt = clone;
                    firstElt.Take();
                }
            }
            return firstElt;
        }
    }
}

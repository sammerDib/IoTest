//#define USE_GC
// En cas de problème, mettre le flag suivant et regarder UnitySC.Shared.Tools.DisposableObject.ObjList
//#define DEBUG_OBJECT_LEAK
#if DEBUG_OBJECT_LEAK
#warning *** DEBUG_OBJECT_LEAK is activated ***
#endif

using System;
using System.Threading;

namespace UnitySC.Shared.Tools
{
    ///////////////////////////////////////////////////////////////////////
    ///<summary>
    /// Implémentation de IDisposable avec un compteur de référence.
    ///</summary>
    ///////////////////////////////////////////////////////////////////////
    public abstract class DisposableObject : IDisposable
    {
#if USE_GC
        public const bool UseGC = true;
#else
        public const bool UseGC = false;
#endif

        //=================================================================
        // Méthodes virtuelles
        //=================================================================
        protected virtual void Dispose(bool disposing)
        { }

        //=================================================================
        //
        //=================================================================
        private static int s_nbObjects;			// Nombre d'objets existants

        private static int s_nbObjectsCreated;   // Nombre total d'object créés
        private int _refCount;					// Compteur de référence de l'object

        public static int NbObjects
        { get { return s_nbObjects; } }

#if DEBUG_OBJECT_LEAK
        public static System.Collections.Generic.HashSet<DisposableObject> ObjList = new System.Collections.Generic.HashSet<DisposableObject>();  // Liste des objets existants
        private int _objectIndex;                       // Index de l'object
        private static readonly int s_breakpointObjectIndex = -1;	// Déclenche un breakpoint quand on alloue l'objet avec cet index.
        private System.Diagnostics.StackFrame[] _stackFrames;
#endif

        //=================================================================
        // Constructeur
        //=================================================================
        public DisposableObject()
        {
            InitRefCounter();
        }

        //=================================================================
        //
        //=================================================================
        private void InitRefCounter()
        {
            _refCount = 1;
            Interlocked.Increment(ref s_nbObjects);
            int objectIndex = Interlocked.Increment(ref s_nbObjectsCreated);

#if DEBUG_OBJECT_LEAK
            _objectIndex = objectIndex;
            if (_objectIndex == s_breakpointObjectIndex)
                System.Diagnostics.Debugger.Break();
            lock (ObjList)
                ObjList.Add(this);

            var stackTrace = new System.Diagnostics.StackTrace(fNeedFileInfo: true);           // get call stack
            _stackFrames = stackTrace.GetFrames();
#endif
        }

        //=================================================================
        // Destructeur
        //=================================================================
        ~DisposableObject()
        {
#if !USE_GC
#if DEBUG            
            // GVA : TODO à revoir
            /*if (_refCount != 0)
                throw new ApplicationException("object not disposed");*/
#endif
#endif
            try
            {
                Dispose(false);
            }
#if DEBUG
            catch (Exception e)
#else
            catch (Exception)
#endif
            {
#if DEBUG
                Console.WriteLine($"Finalizer : {e.Message}");
                Console.WriteLine($"Finalizer : {e.StackTrace}");
#endif
                System.Diagnostics.Debugger.Break();
            }
        }

        //=================================================================
        // AddRef / DelRef
        //=================================================================
        public void AddRef()
        {
            if (_refCount == 0)
                throw new ApplicationException("referencing an already disposed object");
#if DEBUG_OBJECT_LEAK
            if (_objectIndex == s_breakpointObjectIndex)
                System.Diagnostics.Debugger.Break();
#endif
            Interlocked.Increment(ref _refCount);
        }

        public void DelRef()
        {
#if DEBUG_OBJECT_LEAK
            if (_objectIndex == s_breakpointObjectIndex)
                System.Diagnostics.Debugger.Break();
#endif

            try
            {
                int count = Interlocked.Decrement(ref _refCount);
                if (count < 0)
                    throw new ApplicationException("too much DelRef()");
                if (count == 0)
                {
                    Interlocked.Decrement(ref s_nbObjects);
                    if (s_nbObjects < 0)
                        throw new ApplicationException("freeing too much objects");

                    Dispose(true);
                    GC.SuppressFinalize(this);

#if DEBUG_OBJECT_LEAK
                lock (ObjList)
                    ObjList.Remove(this);
#endif
                }
            }
#if DEBUG
            catch (Exception e)
#else
            catch (Exception)
#endif
            {
#if DEBUG
                Console.WriteLine($"DelRef : {e.Message}");
                Console.WriteLine($"DelRef : {e.StackTrace}");
#endif

                System.Diagnostics.Debugger.Break();
            }
        }

        //=================================================================
        // Dispose
        //=================================================================
        public virtual void Dispose()
        {
            DelRef();
        }

        //=================================================================
        // Clonage
        //=================================================================
        protected virtual void CloneTo(DisposableObject obj)
        { }

        public DisposableObject DeepClone()
        {
            var clone = (DisposableObject)MemberwiseClone();
            clone.InitRefCounter();
            CloneTo(clone);
            return clone;
        }
    }
}

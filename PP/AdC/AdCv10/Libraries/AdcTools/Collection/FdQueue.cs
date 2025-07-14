using System.Threading;

using UnitySC.Shared.Tools;

namespace AdcTools
{
    ///////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////
    // A Queue that is thread Safe and blocks when it is empty
    public class FdQueue<T> : System.Collections.Concurrent.ConcurrentQueue<T>
    {
        // Indicate that no more messages can be put in the queue.
        protected int _closed = 0;

        // The Semaphore object requires a maximum value, but I don't have one.
        // So I put a huge value, that will never be reached.
        // Also note that half this value will never be reached, so it is safe
        // to Release(MAX_SEM / 2) to unblock all threads.
        // OK, I known it is not so clean, but I didn't find a better solution.
        protected const int MAX_SEM = 1234567890;

        protected Semaphore _sem = new Semaphore(0, MAX_SEM);

        public WaitHandle WaitHandle
        {
            get { return _sem; }
            private set { }
        }

        public int MaxCount { get; protected set; }
        public int _NbInsertions = 0;
        public int NbInsertions
        {
            get { return _NbInsertions; }
        }


        //=================================================================
        // Overload the Enqueue() method
        // @return false if the queue is already closed and the item is not
        // queued. true otherwise.
        //=================================================================
        public new bool Enqueue(T item)
        {
            if (_closed != 0)
                return false;

            DisposableObject obj = item as DisposableObject;
            if (obj != null)
                obj.AddRef();

            base.Enqueue(item);

            Interlocked.Add(ref _NbInsertions, 1);

            int current = _sem.Release();
            if (current > MaxCount)  //TODO locking ?
                MaxCount = current;

            return true;
        }

        //=================================================================
        // Make TryQueue() a private function, calling
        // it from outside wouldn't make sense
        //=================================================================
        private new bool TryDequeue(out T result)
        {
            return base.TryDequeue(out result);
        }


        //=================================================================
        // Unlike TryDequeue(), Dequeue() blocks if the queue is empty.
        // It may return false if the queue has been closed (cf CloseQueue() ).
        //=================================================================
        public bool Dequeue(out T result)
        {
            _sem.WaitOne();
            bool b = base.TryDequeue(out result);
            return b;
        }

        public T Dequeue()
        {
            T result;
            bool bOk = Dequeue(out result);
            if (bOk)
                return result;
            else
                return default(T);
        }

        //=================================================================
        // Indicates that the last item has been put in the queue.
        // It unblocks all waiting threads. 
        //=================================================================
        public void CloseQueue()
        {
            int alreadyClosed = Interlocked.Exchange(ref _closed, 1);
            if (alreadyClosed != 0)
                return;

            _sem.Release(MAX_SEM / 2);    // see comment of MAX_SEM to understand why /2
        }

        //=================================================================
        // Remove all items from the queue.
        // It unblocks all waiting threads. 
        //=================================================================
        public virtual void AbortQueue()
        {
            int alreadyClosed = Interlocked.Exchange(ref _closed, 1);

            // Remove all items from the queue
            T item;
            while (base.TryDequeue(out item))
            {
                DisposableObject obj = item as DisposableObject;
                if (obj != null)
                    obj.DelRef();
            }

            if (alreadyClosed == 0)
                _sem.Release(MAX_SEM / 2);    // see comment of MAX_SEM to understand why /2
        }

        //=================================================================
        // Wait for several queues. 
        //=================================================================
        public static bool WaitAny(System.Collections.Generic.List<FdQueue<T>> queues, out T result)
        {
            result = default(T);

            while (queues.Count > 0)
            {
                WaitHandle[] wait_handles = new WaitHandle[queues.Count];
                for (int i = 0; i < queues.Count; i++)
                    wait_handles[i] = queues[i].WaitHandle;

                int idx = WaitHandle.WaitAny(wait_handles);

                bool b = queues[idx].TryDequeue(out result);
                if (b)
                {
                    return true;
                }
                else
                {
                    queues.RemoveAt(idx);
                }
            }
            return false;
        }
    }

    ///////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////
#if OBJ_QUEUE
    //TODO cette classe ne fonctionne pas comme il faut:
    // la fonction Dequeue() debloque meme quand la queue est vide
    internal class FdObjectQueue<T> : FdQueue<T> where T : ObjectBase
    {
        //=================================================================
        // Overloaded:
        // The item reference counter is incremented before placing it in the queue
        //=================================================================
        public new bool Enqueue(T item)
        {
            if (_closed != 0)
                return false;

            item.AddRef();
            base.Enqueue(item);

            Interlocked.Add(ref _NbInsertions, 1);

            int current = _sem.Release();
            if (current > MaxCount)  //TODO locking ?
                MaxCount = current;

            return true;
        }

        //=================================================================
        // Unlike Enqueue(), Dequeue() is not overloaded:
        // The caller is responsible for decrementing the reference counter
        // when he's done with the item.
        //=================================================================

        //=================================================================
        // Overloaded:
        // The items reference counts are decremented.
        //=================================================================
        public override void AbortQueue()
        {
            int alreadyClosed = Interlocked.Exchange(ref _closed, 1);

            // Remove all items from the queue
            T item;
            while (base.TryDequeue(out item))
            {
                item.DelRef();
            }

            if (alreadyClosed == 0)
                _sem.Release(MAX_SEM / 2);    // see comment of MAX_SEM to understand why /2
        }

    }
#endif

}


using System;
using System.Threading.Tasks;

using AdcTools;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe de base des modules avec une queue de sortie 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    ///////////////////////////////////////////////////////////////////////
    public abstract class QueueModuleBase<T> : ModuleBase where T : ObjectBase
    {
        //=================================================================
        // Membres et propriétés
        //=================================================================
        protected FdQueue<T> outputQueue = new FdQueue<T>();
        private Task _dequeueTask;

        //=================================================================
        // Constructeur
        //=================================================================
        public QueueModuleBase(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // 
        //=================================================================
        protected void InitOutputQueue()
        {
            outputQueue = new FdQueue<T>();
            _dequeueTask = Scheduler.StartTaskPool(Name, () =>
                {
                    try
                    {
                        ProcessQueue();
                    }
                    catch (Exception e)
                    {
                        HandleException(e);
                    }
                });
        }

        //=================================================================
        // 
        //=================================================================
        public override void Abort()
        {
            outputQueue.AbortQueue();
            base.Abort();
        }

        //=================================================================
        // 
        //=================================================================
        private void ProcessQueue()
        {
            T element;
            do
            {
                using (element = outputQueue.Dequeue())
                {
                    if (element == null)    // Queue has been closed/aborted
                        break;
                    ProcessQueueElement(element);
                }
            } while (true);

            logDebug("end");
        }

        //=================================================================
        // 
        //=================================================================
        protected virtual void ProcessQueueElement(T element)
        {
            ProcessChildren(element);
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            logDebug("closing queue");
            outputQueue.CloseQueue();

            Scheduler.StartSingleTask(Name + "-dequeue", () =>
              {
                  try
                  {
                      if (_dequeueTask != null)
                          _dequeueTask.Wait();
                  }
                  catch
                  {
                      // le Wait() produit des exceptions inexpliquées, on les ignore...
                  }
                  finally
                  {
                      base.OnStopping(oldState);
                  }
              });
        }

        //=================================================================
        // 
        //=================================================================
        public override void GetStats(out int nbIn, out int nbOut)
        {
            nbIn = nbObjectsIn;
            nbOut = outputQueue.NbInsertions;
        }


    }
}

namespace UnitySC.PM.ANA.Hardware.Probe
{
    public class Singleton<T> where T : class, new()
    {
        private static T instance = null;
        private static readonly object padlock = new object();

        internal Singleton()
        {
        }

        public static T Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }
                    return instance;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using SimpleInjector;

using Container = SimpleInjector.Container; // to avoid ambiguity with System.ComponentModel

namespace UnitySC.Shared.Tools
{
    public sealed class ClassLocator
    {
        private static ClassLocator s_instance = null;
        private static readonly object s_padlock = new object();
        private readonly Container _container;

        private ClassLocator()
        {
            _container = new Container();
            _container.Options.SuppressLifestyleMismatchVerification = true;
            // Since nuget upgrade adapt new nuget with new Simple Injector Behavior
            _container.Options.EnableAutoVerification = false;  // Warning, if you call manually Verify it would have teh same effect taht use True (if true => at this time provoke some invalid operation in service invoker so let it @false while concrete is still used)
            _container.Options.UseStrictLifestyleMismatchBehavior = true;
            // Reverting to the pre-v5 behavior
            _container.Options.ResolveUnregisteredConcreteTypes = true; // Important stay @true : need a global survey to correctly register container with interface or concrete call. it is recommended to set at false to be rigorous
#if DEBUG
            // to list all UnregisteredServiceType if we need to work on a real and rigourous lsit of types (and set ResolveUnregisteredConcreteTypes=false as recommended)
            _container.ResolveUnregisteredType += (s, e) =>
            {
                if (!e.Handled && !e.UnregisteredServiceType.IsAbstract)
                {
                    Debug.WriteLine($"ClassLocator.CONTAINER UnregisteredServiceType : {e.UnregisteredServiceType}");
                }
            };
#endif

        }

        private ClassLocator(Container container)
        {
            _container = container;
            _container.Options.SuppressLifestyleMismatchVerification = true;
            // Since nuget upgrade adapt new nuget with new Simple Injector Behavior
            _container.Options.EnableAutoVerification = false;  // Warning, if you call manually Verify it would have teh same effect taht use True (if true => at this time provoke some invalid operation in service invoker so let it @false while concrete is still used)
            _container.Options.UseStrictLifestyleMismatchBehavior = true;
            // Reverting to the pre-v5 behavior
            _container.Options.ResolveUnregisteredConcreteTypes = true;
#if DEBUG
            // to list all UnregisteredServiceType if we need to work on a real and rigourous lsit of types (and set ResolveUnregisteredConcreteTypes=false as recommended)
            _container.ResolveUnregisteredType += (s, e) =>
            {
                if (!e.Handled && !e.UnregisteredServiceType.IsAbstract)
                {
                    Debug.WriteLine($"ClassLocator.CONTAINER Copy Constructor UnregisteredServiceType : {e.UnregisteredServiceType}");
                }
            };
#endif
        }

        public static ClassLocator Default
        {
            get
            {
                lock (s_padlock)
                {
                    if (s_instance == null)
                    {
                        s_instance = new ClassLocator();
                    }
                    return s_instance;
                }
            }
        }

        /// <summary>
        /// Init class locator wit external container
        /// </summary>
        /// <param name="container"></param>
        public static void ExternalInit(Container container, bool force = false)
        {
            lock (s_padlock)
            {
                if (s_instance == null || force)
                {
                    s_instance = new ClassLocator(container);
                }
                else
                {
                    throw new InvalidOperationException("Classlocator already created");
                }
            }
        }

        public TClass GetInstance<TClass>()
              where TClass : class => _container.GetInstance<TClass>();

        public object GetInstance(Type type) => _container.GetInstance(type);

        public void Register(Type serviceType, Func<object> instanceCreator, Lifestyle lifestyle)
        {
            _container.Register(serviceType, instanceCreator, lifestyle);
        }

        public void Register<TClass>(bool singleton = false) where TClass : class
        {
            _container.Register<TClass>(singleton ? Lifestyle.Singleton : Lifestyle.Transient);
        }

        public void Register<TInterface, TClass>(bool singleton = false)
            where TInterface : class
            where TClass : class, TInterface
        {
            _container.Register<TInterface, TClass>(singleton ? Lifestyle.Singleton : Lifestyle.Transient);
        }

        public void Register(Type type, Type implementationType, bool singleton = false)
        {
            _container.Register(type, implementationType, singleton ? Lifestyle.Singleton : Lifestyle.Transient);
        }

        public void Register<TService>(Func<TService> instanceCreator, bool singleton = false) where TService : class
        {
            _container.Register(instanceCreator, singleton ? Lifestyle.Singleton : Lifestyle.Transient);
        }

        public bool IsRegistered<T>()
        {
            return _container.GetCurrentRegistrations().Any(producer => producer.ServiceType == typeof(T) || producer.ImplementationType == typeof(T));
        }

        public bool IsRegistered(Type type)
        {
            return _container.GetCurrentRegistrations().Any(producer => producer.ServiceType == type || producer.ImplementationType == type);
        }

        public void AppendToCollection<TService>(Func<TService> instanceCreator, Lifestyle lifestyle) where TService : class
        {
            _container.Collection.Append<TService>(instanceCreator, lifestyle);
        }

        public IEnumerable<object> GetCollection<TService>()
        {
            return _container.GetAllInstances(typeof(TService));
        }

        public bool IsInit => _container.GetCurrentRegistrations().Any();
    }
}

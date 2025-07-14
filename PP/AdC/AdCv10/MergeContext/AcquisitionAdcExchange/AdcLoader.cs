using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AcquisitionAdcExchange
{
    public class AdcLoader
    {
        private static IAdcAcquisition adcExecutor;

        //=================================================================
        // Fonctions publiques
        //=================================================================
        public static bool IsAdcEmbeddedInAcquisition()
        {
            string str = System.Configuration.ConfigurationManager.AppSettings.Get("Acquisition.ADC.Embedded");
            if (str == null)
                return false;
            bool b = Convert.ToBoolean(str);
            return b;
        }

        public static IAdcAcquisition GetAdcExecutor()
        {
            if (adcExecutor == null)
                CreateAdcExecutor();
            return adcExecutor;
        }

        //=================================================================
        // Fonctions privées
        //=================================================================
        private static void CreateAdcExecutor()
        {
            // Ajoute le répertoire ADC dans le %PATH%
            //........................................
            string AcquisitionAdcPath = ConfigurationManager.AppSettings["Acquisition.ADC.Path"];

            string path = Environment.GetEnvironmentVariable("path");
            path += ";" + AcquisitionAdcPath;
            Environment.SetEnvironmentVariable("path", path);

            // Load library
            //.............
            Console.WriteLine("Loading MergeContext");
            string mergectx = Path.Combine(AcquisitionAdcPath, "MergeContext.dll");
            Assembly assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(mergectx));

            // Cherche la classe concrète
            //...........................
            Type type = assembly.GetTypes().FirstOrDefault(
                t => !t.IsAbstract && t.GetInterface("IAdcAcquisition") != null
                );
            if (type == null)
                throw new ApplicationException("AdcExecutor (IAdcAcquisition) class not found in " + path);

            // Récupération de l'instance
            //...........................
            adcExecutor = (IAdcAcquisition)Activator.CreateInstance(type);
        }
    }
}

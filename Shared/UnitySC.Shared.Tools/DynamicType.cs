using System;
using System.Xml;

namespace UnitySC.Shared.Tools
{
    public class DynamicType
    {
        /// <summary>
        /// Helper routine that looks up a type name and tries to retrieve the
        /// full type reference in the actively executing assemblies.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        /// https://weblog.west-wind.com/posts/2013/Nov/12/Dynamically-loading-Assemblies-to-reduce-Runtime-Dependencies
        public static Type GetType(string typeName)
        {

            // Let default name binding find it
            var type = Type.GetType(typeName, false);
            if (type != null)
                return type;

            // look through assembly list
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // try to find manually
            foreach (var asm in assemblies)
            {
                type = asm.GetType(typeName, false);

                if (type != null)
                    break;
            }
            return type;
        }

        public static object LoadXmlObject(XmlNode node, params object[] args)
        {
            string className = node.Attributes["Type"].Value;

            var type = DynamicType.GetType(className);
            object o = Activator.CreateInstance(type, args);

            return o;
        }
    }
}
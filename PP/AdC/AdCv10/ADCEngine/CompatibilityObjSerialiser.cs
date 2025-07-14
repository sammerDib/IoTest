using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ADCEngine
{
    public class CompatibilityObjSerialiser
    {
        [Serializable]
        public class Family
        {
            public Family() { }
            public Family(String name) { Name = name; } // pour jeu de test

            [XmlAttribute]
            public String Name;
        }

        [Serializable]
        public class ModuleName
        {
            [XmlAttribute]
            public String Name;
            public ModuleName() { }
            public ModuleName(String name) { Name = name; }
        }

        [Serializable]
        public class Module
        {
            [XmlAttribute]
            public String Name;
            public List<Family> Requires;
            public List<Family> IncompatibleWith;
            public List<Family> Cleared;
            public List<Family> Provides;
            [XmlArrayItem("Module")]
            public List<ModuleName> Suggestion { get; set; }

            public Module()
            {
                Requires = new List<Family>();
                IncompatibleWith = new List<Family>();
                Cleared = new List<Family>();
                Provides = new List<Family>();
                Suggestion = new List<ModuleName>();
            }

            public Module(String name) // pour jeu de test
            {
                Name = name;

                Requires = new List<Family>();
                IncompatibleWith = new List<Family>();
                Cleared = new List<Family>();
                Provides = new List<Family>();
                Suggestion = new List<ModuleName>();
            }
            //-----
            public void AddIncompatibility(String Familyname)
            {
                Family familly = new Family(Familyname);
                IncompatibleWith.Add(familly);
            }
            public void AddRequieres(String Familyname)
            {
                Family familly = new Family(Familyname);
                Requires.Add(familly);
            }
            //-----
            public void AddCleared(String Familyname)
            {
                Family familly = new Family(Familyname);
                Cleared.Add(familly);
            }
            //-----
            public void AddProvides(String Familyname)
            {
                Family familly = new Family(Familyname);
                Provides.Add(familly);
            }
            //-----
            public void AddSuggestion(String moduleName)
            {
                Suggestion.Add(new ModuleName(moduleName));
            }
            //-----
            public void RemoveSuggestion(String moduleName)
            {
                foreach (ModuleName modname in Suggestion)
                {
                    if (modname.Name == moduleName)
                    {
                        Suggestion.Remove(modname);
                        break;
                    }
                }
            }
            //-----
            public void RemoveIncompatibility(String Familyname)
            {
                foreach (Family eFamily in IncompatibleWith)
                {
                    if (eFamily.Name == Familyname)
                    {
                        IncompatibleWith.Remove(eFamily);
                        break;
                    }
                }
            }
            //-----
            public void RemoveRequieres(String Familyname)
            {
                foreach (Family eFamily in Requires)
                {
                    if (eFamily.Name == Familyname)
                    {
                        Requires.Remove(eFamily);
                        break;
                    }
                }
            }
            //-----
            public void RemoveCleared(String Familyname)
            {
                foreach (Family eFamily in Cleared)
                {
                    if (eFamily.Name == Familyname)
                    {
                        Cleared.Remove(eFamily);
                        break;
                    }
                }
            }
            //-----
            public void RemoveProvides(String Familyname)
            {
                foreach (Family eFamily in Provides)
                {
                    if (eFamily.Name == Familyname)
                    {
                        Provides.Remove(eFamily);
                        break;
                    }
                }
            }
        }

        [Serializable]
        public class CompatibilityRules
        {
            public List<Family> Families;
            public List<Module> Modules;

            public CompatibilityRules()
            {
                Families = new List<Family>();
                Modules = new List<Module>();
            }

            public Module GetModuleByName(String name)
            {
                int ifound = -1;
                for (int i = 0; i < Modules.Count; i++)
                {
                    if (Modules[i].Name == name)
                        ifound = i;
                }

                if (ifound != -1)
                    return Modules[ifound];
                else
                {
                    return new Module(name);
                }
            }

            public Family GetFamilyByName(String name)
            {
                int ifound = -1;
                for (int i = 0; i < Families.Count; i++)
                {
                    if (Families[i].Name == name)
                        ifound = i;
                }

                if (ifound != -1)
                    return Families[ifound];
                else
                {
                    return new Family(name);
                }
            }
        }

        //---------------------------------------------
        public static void Serialize(string filename, CompatibilityRules _CompatibilityRules)
        {
            try
            {
                using (StreamWriter SW = new StreamWriter(filename, false))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CompatibilityRules));
                    serializer.Serialize(SW, _CompatibilityRules);
                }
            }
            catch
            {
                throw new ArgumentNullException("Unable to write compatibility file : " + filename);
            }
        }

        //---------------------------------------------
        public static CompatibilityRules DeSerialize(string filename)
        {
            try
            {
                using (StreamReader SR = new StreamReader(filename))
                {
                    CompatibilityRules _CompatibilityRules;

                    XmlSerializer Serializer = new XmlSerializer(typeof(CompatibilityRules));
                    _CompatibilityRules = Serializer.Deserialize(SR) as CompatibilityRules;
                    return _CompatibilityRules;
                }
            }
            catch
            {
                throw new ArgumentNullException("Unable to read compatibility file : " + filename);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnitySC.Shared.Tools
{


    public class AssemblyLoader
    {
        public static AssemblyLoader Instance { get; } = new AssemblyLoader();

        private AssemblyLoader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;


            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var an = asm.GetName();
                Console.WriteLine($"Loaded Assembly : {an.Name} {an.Version}");
            }

        }

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Console.WriteLine($"AssemblyLoad : {args.LoadedAssembly.FullName} - {(!args.LoadedAssembly.IsDynamic ? args.LoadedAssembly.Location : "-")}");
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // args.


            //if (args.RequestingAssembly == null) return null;

            AssemblyName an = new AssemblyName(args.Name);


            //AssemblyName an = args.RequestingAssembly.GetName();

            Console.WriteLine($"AssemblyResolve : {an.Name} {an.Version}");

            return Instance.Get(an.Name, an.Version);
        }

        private SortedDictionary<string, AssemblyInfoVersion> _assemblyInfoVersions = new SortedDictionary<string, AssemblyInfoVersion>();


        public void RegisterPath(string path)
        {
            var files = System.IO.Directory.EnumerateFiles(path, "*.dll", System.IO.SearchOption.AllDirectories);

            foreach (string f in files)
            {
                try
                {
                    Assembly asm = Assembly.ReflectionOnlyLoadFrom(f);
                    AssemblyName an = asm.GetName();
                    AssemblyInfo ai = new AssemblyInfo(an.Name, f, an.Version);

                    if (!_assemblyInfoVersions.TryGetValue(an.Name, out AssemblyInfoVersion aiv))
                    {
                        aiv = new AssemblyInfoVersion(an.Name);

                        _assemblyInfoVersions.Add(an.Name, aiv);

                        Console.WriteLine($"Assembly Register : {an.Name} {an.Version}");


                    }

                    aiv.Add(ai);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Assembly Register : {f} {e.Message}");

                }
            }

        }


        public Assembly Get(string name, Version version)
        {
            if (_assemblyInfoVersions.TryGetValue(name, out AssemblyInfoVersion aiv))
            {
                return aiv.Get(version);
            }

            return null;
        }
    }


    public class AssemblyInfoVersion
    {
        public string Name { get; }

        private SortedDictionary<Version, AssemblyInfo> _assemblyInfos = new SortedDictionary<Version, AssemblyInfo>();

        public AssemblyInfoVersion(string name)
        {
            Name = name;
        }

        private AssemblyInfo AssemblyInfoMostRecent { get; set; } = null;

        public void Add(AssemblyInfo assemblyInfo)
        {
            if (!_assemblyInfos.TryGetValue(assemblyInfo.Version, out AssemblyInfo ai))
            {
                _assemblyInfos.Add(assemblyInfo.Version, assemblyInfo);

                Version vMR = _assemblyInfos.Values.Max(e => e.Version);
                AssemblyInfoMostRecent = _assemblyInfos[vMR];
            }
        }

        public Assembly Get(Version version)
        {
            if (!_assemblyInfos.TryGetValue(version, out AssemblyInfo ai))
            {
                ai = AssemblyInfoMostRecent;
            }

            if (ai != null)
            {
                if (!ai.Loaded) ai.Load();
                return ai.Assembly;
            }

            return null;
        }
    }



    public class AssemblyInfo
    {
        public Assembly Assembly { get; private set; }
        public string Name { get; }
        public string Path { get; }
        public Version Version { get; }


        public bool Loaded { get { return Assembly != null; } }
        public bool IsLoading { get; private set; }

        public AssemblyInfo(string name, string path, Version version)
        {
            Name = name;
            Path = path;
            Version = version;
        }

        public void Load()
        {
            if (IsLoading) return;
            if (Loaded) return;

            try
            {
                IsLoading = true;
                Assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(Path));
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}

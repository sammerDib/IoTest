using System;
using System.IO;
using System.Management; // for ManagementScope
using System.Net;        // for IPAddress
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace UnitySC.Shared.Tools
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe pour manipuler les noms de fichiers/répetoires.
    /// La concaténation se fait avec le /
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [Serializable]
    public class PathString
    {
        [XmlAttribute] public string path;    // Dans Public, tout est public (et aussi dans la serialization)

        //=================================================================
        // Constructeurs
        //=================================================================
        public PathString()
        {
            path = ".";
        }

        public PathString(string path)
        {
            this.path = path;
            TrimEndSlash();
        }

        public PathString(PathString pathString)
        {
            path = pathString.path;
        }

        //=================================================================
        // Opérateurs
        //=================================================================
        public static implicit operator PathString(string s)
        {
            var ps = new PathString(s);
            return ps;
        }

        public static implicit operator string(PathString ps)
        {
            if (ps == null)
                return null;
            return ps.path;
        }

        public static PathString operator /(PathString ps1, PathString ps2)
        {
            var ps = new PathString
            {
                path = System.IO.Path.Combine(ps1.path, ps2.path)
            };
            return ps;
        }

        public static PathString operator /(PathString ps1, string s2)
        {
            var ps = new PathString
            {
                path = System.IO.Path.Combine(ps1.path, s2)
            };
            ps.TrimEndSlash();
            return ps;
        }

        public static PathString operator /(string s1, PathString ps2)
        {
            var ps = new PathString
            {
                path = System.IO.Path.Combine(s1, ps2.path)
            };
            return ps;
        }

        //=================================================================
        //
        //=================================================================
        public override string ToString()
        {
            return path;
        }

        //=================================================================
        //
        //=================================================================
        [XmlIgnore] public PathString Drive { get { return Path.GetPathRoot(path); } }

        [XmlIgnore] public PathString Directory { get { return Path.GetDirectoryName(path); } }
        [XmlIgnore] public PathString FullPath { get { return Path.GetFullPath(path); } }
        [XmlIgnore] public string Filename { get { return Path.GetFileName(path); } }
        [XmlIgnore] public string Basename { get { return Path.GetFileNameWithoutExtension(path); } }
        [XmlIgnore] public string Extension { get { return Path.GetExtension(path); } }
        [XmlIgnore] public bool IsPathRooted { get { return Path.IsPathRooted(path); } }
        //[XmlIgnore] public bool IsPathFullyQualified { get { return Path.IsPathFullyQualified(path); } }

        ///=================================================================<summary>
        /// Retourne le chemin moins le répertoire de base
        ///</summary>=================================================================
        public PathString GetSubPath(string basedir)
        {
            if (!path.StartsWith(basedir))
                throw new ApplicationException("invalid directory: " + path + " expected: " + basedir);

            int start = basedir.Length + 1; // +1 pour le séparateur '\'
            int length = path.Length - start;
            string relative_path = path.Substring(start, length);

            return new PathString(relative_path);
        }

        //=================================================================
        //
        //=================================================================
        private void TrimEndSlash()
        {
            if (path == null)
                return;
            path = path.TrimEnd('\\');
            //path = Path.GetFullPath(path);
        }

        //=================================================================
        //
        //=================================================================
        public PathString RemoveInvalidFilePathCharacters(string replaceChar = "-", bool removeSpaces = true)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            if (removeSpaces)
                regexSearch += " \t";
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            string s = r.Replace(path, replaceChar);
            return s;
        }

        ///=================================================================
        /// <summary>
        /// Retourne un nouveaut PathString dont l'extension a été changée.
        /// </summary>
        /// <param name="ext"> Nouvelle extension, doit commencer par '.' </param>
        ///=================================================================
        public PathString ChangeExtension(string ext)
        {
            return Path.ChangeExtension(path, ext);
        }

        public bool OptimNetworkPath(out string sResError)
        {
            bool bSuccess = false;
            string sNewPath = OptimNetworkPath(path, out sResError);
            if (string.IsNullOrEmpty(sResError))
            {
                bSuccess = true;
                path = sNewPath;
                TrimEndSlash();
            }
            return bSuccess;
        }

        //=================================================================
        // Static
        //=================================================================
        public static PathString GetExecutingAssemblyPath()
        {
            var uri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            return uri.LocalPath;
        }

        public static PathString GetExeFullPath()
        {
            return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        }

        public static PathString GetAppBaseDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static PathString GetCurrentDirectory()
        {
            return System.IO.Directory.GetCurrentDirectory();
        }

        public static PathString GetTempPath()
        {
            return Path.GetTempPath();
        }

        //------------------------------------------------------------------------
        [DllImport("shlwapi.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PathIsNetworkPath(string pszPath);

        //------------------------------------------------------------------------
        public static string OptimNetworkPath(string sUNCPath, out string sResError)
        {
            // Converti un chemin réseau "sUNCPath" par un chemin local à la machine (pour eviter un temps de copies via réseau)
            // si ce chemin est bien sur la machine en question et qu'il est partagé avec les bons droits, alors ce chemin réseau est transformé en chemin LOCAL
            // -- si le chemin Réseau n'est pas un vrai chemin réseau ou encore mal ou non configuré le chemin réseau est renvoyé... (une erreur surviendra plus tard dans les methodes suivantes)
            sResError = "";
            if (PathIsNetworkPath(sUNCPath) == false)
                return sUNCPath; // il ne s'agit pas d'un chemin réseau mais déjà d'un chemin local -- on l'utilise donc tel quel

            string uncPath = sUNCPath;
            string sRes;
            try
            {
                // on enlève le "\\" du UNC path et on le split
                //---------------------------------------------------------------------
                // -- rappel CONVENTION UNC = Universal Naming Convention (UNC) : UNC =  \\<hostname>\<sharename>[\<objectname>]*
                // <hostname>: Represents the host name of a server or the domain name of a domain hosting resource; the string MUST be a NetBIOS name as specified in [MS-NBTE]
                // <sharename>: Represents the name of a share or a resource to be accessed. The format of this name depends on the actual file server protocol that is used to access the share.
                // <objectname>: Represents the name of an object; this name depends on the actual resource accessed.The notation "[\<objectname>]*" indicates that zero or more object names may exist in the path, and each <objectname> is separated from the immediately preceding <objectname> with a backslash path separator.
                uncPath = uncPath.Replace(@"\\", "");
                string[] uncParts = uncPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (uncParts.Length < 2)
                {
                    sResError = "OptimNetworkPath : [UNRESOLVED UNC PATH: " + uncPath + "]"; //chemin réseau mal ecrit on le retourne quand même il 'sagit d'une erreur de config sans doute
                    //WriteErrorLog(enTypeError_Wafer.en_FileVariableError, sResError);
                    return sUNCPath;
                }

                // on teste si il s'agit de la machine locale
                if (string.Compare(System.Environment.MachineName, uncParts[0], StringComparison.OrdinalIgnoreCase) != 0)
                {
                    // il se peut que le nom soit exprimè en adresse IP on gere ce cas
                    var localIPs = Dns.GetHostAddresses(System.Environment.MachineName);
                    bool findIp = false;
                    foreach (var ipadd in localIPs)
                    {
                        string ip = ipadd.ToString();
                        if (ip == uncParts[0])
                        {
                            findIp = true; // l'ip est identique c'est la même machine
                            break;
                        }
                    }
                    if (findIp == false)
                        return sUNCPath; //Pas de correspondance Nom ou IP, il ne s'agit pas de la même machine --> donc in retourne le chemin réseau
                }

                // ON THE LOCAL MACHINE
                //---------
                // Get a connection to the server as found in the UNC path -- without identification (use user rights);
                var scope = new ManagementScope(@"\\" + uncParts[0] + @"\root\cimv2");

                // Query the server for the share name
                var query = new SelectQuery("Select * From Win32_Share Where Name = '" + uncParts[1] + "'");
                var searcher = new ManagementObjectSearcher(scope, query);

                // Get the path
                var queryCollection = searcher.Get();
                string path = string.Empty;
                foreach (ManagementObject obj in queryCollection)
                {
                    path = obj["path"].ToString();
                }
                if (string.IsNullOrEmpty(path))
                    return sUNCPath; // this path is not shared on this machine

                // Append any additional folders to the local path name
                if (uncParts.Length > 2)
                {
                    for (int i = 2; i < uncParts.Length; i++)
                        path = path.EndsWith(@"\") ? path + uncParts[i] : path + @"\" + uncParts[i];
                }

                sRes = path;
            }
            catch (Exception ex)
            {
                // probablement une machine distante (pas d'accés, ou pas de droit -- on retourne donc le network path d'origine)
                sResError = "OptimNetworkPath : [ERROR RESOLVING UNC PATH: " + uncPath + ": " + ex.Message + "]";
                //WriteErrorLog(enTypeError_Wafer.en_FileVariableError, sResError);
                return sUNCPath;
            }
            return sRes;
        }
    }

    //=================================================================
    // String extension
    //=================================================================
    public static class PathStringExtension
    {
        public static PathString ToPathString(this string str)
        {
            return new PathString(str);
        }
    }
}

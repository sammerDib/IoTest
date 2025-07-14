using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Xml;

using Agileo.Common.Tracing;
using Agileo.GUI.Configuration;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.SystemInformation
{
    /// <summary>
    /// Get information for all loaded assembly by the executable.
    /// Model part that can be used twice (system information panel, export data)
    /// </summary>
    public class SystemInformation
    {
        private AssemblyInformation _mainAssembly;

        #region Properties

        public string NetVersion { get; private set; }
        public OperatingSystem OperatingSystem { get; private set; }
        public string Process { get; private set; }
        public ObservableCollection<AssemblyInformation> AssemblyInformations { get; private set; }

        public string OperatingSystemName => OsVersionHelper.GetWindowsVersion().ToString();

        #endregion Properties

        #region Constructor

        public SystemInformation()
        {
            Extract();
        }

        private void Extract()
        {
            //Update global information
            OperatingSystem = Environment.OSVersion;
            NetVersion = Environment.Version.ToString();
            Process = Environment.Is64BitProcess ? "x64" : "x86";

            // Get main assembly information
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
                assembly = Assembly.GetExecutingAssembly();

            _mainAssembly = new AssemblyInformation(assembly);

            // Get assembly information. Cannot get FileVersionInfo on Dynamic dlls.
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !(a is AssemblyBuilder) && !a.IsDynamic)
                .Select(a => new AssemblyInformation(a))
                .OrderBy(a => a.Name);

            AssemblyInformations = new ObservableCollection<AssemblyInformation>(assemblies);
        }

        #endregion Constructor

        public void SaveAsText(string fileName)
        {
            using (var writer = new StreamWriter(fileName, false))
            {
                WriteAsText(writer);
            }
        }

        public void WriteAsText(StreamWriter writer)
        {
            writer.AutoFlush = true;
            writer.WriteLine(_mainAssembly.ProductName + " - " +
                             DateTime.Now.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine();
            writer.WriteLine("Application Information");
            writer.WriteLine("===============================");
            writer.WriteLine("Application Full Name: " + _mainAssembly.OriginalFilename);
            writer.WriteLine("Application Version: " + _mainAssembly.ProductVersion);
            writer.WriteLine();
            writer.WriteLine("System Information");
            writer.WriteLine("******************");
            writer.WriteLine("Machine: " + Environment.MachineName);
            writer.WriteLine("Executing Application User: " + Environment.UserName);
            writer.WriteLine("Current User Domain: " + Environment.UserDomainName);
            writer.WriteLine("Operating System: " + OperatingSystemName);
            writer.WriteLine(".Net CLR Version: " + NetVersion);
            writer.WriteLine();
            writer.WriteLine("Assemblies Information");
            writer.WriteLine("===============================");

            if (AssemblyInformations.Count > 0)
            {
                foreach (var item in AssemblyInformations)
                {
                    writer.WriteLine(item.ToString());
                    writer.WriteLine();
                }
            }
            else
            {
                writer.WriteLine("No external assemblies have been loaded.");
            }
        }

        public void SaveAsXml(string fileName)
        {
            using (var writer = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                WriteAsXml(writer);
            }
        }

        public void WriteAsXml(XmlTextWriter writer)
        {
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("ApplicationInformation");
            writer.WriteAttributeString("Application", _mainAssembly.ProductName);
            writer.WriteAttributeString("Date", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            writer.WriteStartElement("GeneralInformation");
            writer.WriteElementString("ApplicationFullName", _mainAssembly.OriginalFilename);
            writer.WriteElementString("ApplicationVersion", _mainAssembly.ProductVersion);
            writer.WriteEndElement();
            writer.WriteStartElement("SystemInformation");
            writer.WriteElementString("Machine", Environment.MachineName);
            writer.WriteElementString("ExecutingApplicationUser", Environment.UserName);
            writer.WriteElementString("CurrentUserDomain", Environment.UserDomainName);
            writer.WriteElementString("OperatingSystem", OperatingSystemName);
            writer.WriteElementString("NetCLRVersion", NetVersion);
            writer.WriteEndElement();
            writer.WriteStartElement("ModulesInformation");

            try
            {
                foreach (var item in AssemblyInformations)
                {
                    item.ToXML(writer);
                }
            }
            catch (Exception ex)
            {
                TraceManager.Instance().Trace(Constants.TracerName, TraceLevelType.Error,
                    "{0} - An error occurred while saving information as XML. Message={1}",
                    new TraceParam(ex.ToString()), GetType().Name, ex.Message);
            }
            finally
            {
                try
                {
                    while (writer.WriteState == WriteState.Element)
                    {
                        writer.WriteEndElement();
                    }
                }
                catch
                {
                    // ignored
                }
            }

            writer.WriteEndDocument();
        }
    }
}
